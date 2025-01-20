using DigitalRuby.ThunderAndLightning;
using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace PremiumScraps.CustomEffects
{
    internal class Controller : PhysicsProp
    {
        public MeshRenderer? renderer;
        public ParticleSystem? chargingParticle;
        public AudioSource? chargingAudio;
        public ParticleSystem? controlModeParticle;
        public AudioSource? controlModeAudio;
        public Transform? chargingTransform;
        public readonly float zapRange = 9f;
        public bool screenIsReady = false;
        public bool targetIsValid = false;
        public bool serverDataValid = false;
        public ulong targetPlayerId;
        public ulong targetClientId;
        public PlayerControllerB? targetPlayer;
        public bool isInControlMode = false;
        public bool isBeingControlled = false;
        public bool cameraReady = false;
        public GameObject? nightVision;
        public ControllerRender? controllerRender;
        public Camera? camera;
        public readonly int cameraTextureWidth = 860;
        public readonly int cameraTextureHeight = 520;
        public RenderTexture cameraTexture;
        public readonly GameObject controlledAntenaPrefab;
        public readonly GameObject controlledUIPrefab;
        public GameObject? controlledAntena;
        public GameObject? controlledUI;
        public Animator? controlledUIAnimator;
        public Coroutine? controlledUITextCoroutine;

        public Controller()
        {
            useCooldown = 1.5f;
            cameraTexture = new RenderTexture(cameraTextureWidth, cameraTextureHeight, 24);
            controlledAntenaPrefab = Plugin.gameObjects[0];
            controlledUIPrefab = Plugin.gameObjects[1];
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            chargingParticle = transform.GetChild(2).GetComponent<ParticleSystem>();
            chargingAudio = transform.GetChild(2).GetComponent<AudioSource>();
            controlModeParticle = transform.GetChild(3).GetComponent<ParticleSystem>();
            controlModeAudio = transform.GetChild(3).GetComponent<AudioSource>();
            chargingTransform = transform.GetChild(2).transform;
            if (insertedBattery != null)
                insertedBattery.charge = 1;
            if (!IsHost && !IsServer)
                SyncStateServerRpc(true, targetPlayerId, targetClientId);
        }

        public override void EquipItem()
        {
            EnableItemMeshes(enable: true);
            var batteryOK = insertedBattery != null && !insertedBattery.empty;
            PrepareScreen(batteryOK, true, batteryOK);
            SetControlTips();
            isPocketed = false;
            if (!hasBeenHeld)
            {
                hasBeenHeld = true;
                if (!isInShipRoom && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.currentLevel.spawnEnemiesAndScrap)
                {
                    RoundManager.Instance.valueOfFoundScrapItems += scrapValue;
                }
            }
        }

        public override void SetControlTipsForItem()
        {
            SetControlTips();
        }

        private void SetControlTips()
        {
            string[] allLines;
            if (targetIsValid)
            {
                string targetName;
                if (targetPlayer != null)
                    targetName = targetPlayer.playerUsername;
                else
                    targetName = StartOfRound.Instance.allPlayerObjects[targetPlayerId].GetComponent<PlayerControllerB>().playerUsername;
                allLines = new string[3] { "Activate : [RMB]", isInControlMode ? "Stop : [Q]" : "Start controlling : [Q]", "Target : " + targetName };
            }
            else
                allLines = new string[3] { "Activate : [RMB]", "Start controlling : [Q]", "" };
            if (IsOwner)
            {
                HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.ChangeControlTipMultiple(allLines, holdingItem: true, itemProperties);
            }
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (playerHeldBy == null || insertedBattery == null || insertedBattery.empty || isInControlMode || isBeingControlled)
                return;
            var player = playerHeldBy;
            StartCoroutine(SearchForTarget(player));
        }

        public override void ItemInteractLeftRight(bool right)
        {
            base.ItemInteractLeftRight(right);
            if (!right && playerHeldBy != null && insertedBattery != null && !insertedBattery.empty && screenIsReady && targetPlayer != null
                && !targetPlayer.disconnectedMidGame && targetPlayer.IsSpawned && targetPlayer.isPlayerControlled && !targetPlayer.isPlayerDead
                && cameraReady && camera != null)
            {
                SetControlModeServerRpc(!isInControlMode, targetClientId);
                isInControlMode = !isInControlMode;
                isBeingUsed = isInControlMode;
                ControlModeAnimationServerRpc(isInControlMode);
                SetControlTips();
            }
        }

        public override void Update()
        {
            base.Update();
            if (playerHeldBy == null || GameNetworkManager.Instance.localPlayerController.playerClientId != playerHeldBy.playerClientId || !screenIsReady || targetPlayer == null)
                return;
            if (targetPlayer.disconnectedMidGame || !targetPlayer.IsSpawned || !targetPlayer.isPlayerControlled || targetPlayer.isPlayerDead)
            {
                targetPlayer = null;
                targetIsValid = false;
                isInControlMode = false;
                isBeingUsed = false;
                ControlModeAnimationServerRpc(false);
                SyncStateServerRpc(false, 0, 0);
                PrepareScreen(false, false, false);
                SetControlTips();
                return;
            }
            if (cameraReady && camera != null)
            {
                renderer?.materials[3].SetTexture("_ScreenTexture", cameraTexture);
            }
            else
            {
                CreateCamera();
            }
        }

        private IEnumerator SearchForTarget(PlayerControllerB player)
        {
            ChargingAnimationServerRpc();
            yield return new WaitForSeconds(0.9f);
            if (!isHeld || player.isPlayerDead || chargingTransform == null)
                yield break;
            var objectsInRange = Physics.RaycastAll(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward, zapRange, 11012424, QueryTriggerInteraction.Collide);
            var objectsInRangeList = objectsInRange.OrderBy((x) => x.distance).ToList();
            var startPosition = player.gameplayCamera.transform.position;
            for (int i = 0; i < objectsInRangeList.Count; i++)
            {
                var obj = objectsInRangeList[i];
                if (obj.transform.gameObject.layer == 8 || obj.transform.gameObject.layer == 11)
                    break;
                if (obj.transform.TryGetComponent(out IHittable component) && obj.transform != player.transform
                    && (obj.point == Vector3.zero || !Physics.Linecast(startPosition, obj.point, out _, StartOfRound.Instance.collidersAndRoomMaskAndDefault)))
                {
                    if (component is PlayerControllerB target && target.IsSpawned && target.isPlayerControlled && !target.isPlayerDead)
                    {
                        var oldTarget = targetPlayerId;
                        var oldValid = targetIsValid;
                        targetPlayerId = target.playerClientId;
                        targetClientId = target.OwnerClientId;
                        targetIsValid = true;
                        isInControlMode = false;
                        isBeingUsed = false;
                        SyncStateServerRpc(true, target.playerClientId, target.OwnerClientId, true);
                        ZapAnimationServerRpc(chargingTransform.position, target.transform.position + (Vector3.up * 2));
                        yield return new WaitForSeconds(0.1f);
                        if (!isHeld || player.isPlayerDead)
                            yield break;
                        if (oldValid && oldTarget != targetPlayerId)  // switching from a valid target to a new valid one requires a new camera
                        {
                            screenIsReady = false;
                            DestroyCamera();
                        }
                        PrepareScreen(true, false, false);
                        SetControlTips();
                        break;
                    }
                }
            }
        }

        public void PrepareScreen(bool isReady, bool updateFlagQE, bool updateEmissive)
        {
            if (targetIsValid && GameNetworkManager.Instance.localPlayerController.playerClientId == targetPlayerId)
                targetIsValid = false;
            if (isReady)
            {
                if (updateFlagQE && playerHeldBy != null)
                    playerHeldBy.equippedUsableItemQE = true;
                if (updateEmissive && renderer != null)
                {
                    renderer.materials[3].SetFloat("_EmissivePower", 1.5f);
                    renderer.materials[3].SetColor("_EmissiveColor", new Color(0.62f, 1f, 0.58f));
                }
            }
            else
            {
                DestroyCamera();
                if (updateFlagQE && playerHeldBy != null)
                    playerHeldBy.equippedUsableItemQE = false;
                if (updateEmissive && renderer != null)
                {
                    renderer.materials[3].SetFloat("_EmissivePower", 0);
                    renderer.materials[3].SetColor("_EmissiveColor", Color.black);
                }
            }
            isInControlMode = false;
            isBeingUsed = false;
            if (isBeingControlled)
                SetControlMode(false);
            if (isReady && targetIsValid)
                targetPlayer = StartOfRound.Instance.allPlayerObjects[targetPlayerId].GetComponent<PlayerControllerB>();
            screenIsReady = isReady && targetIsValid;
        }

        public override void DiscardItem()
        {
            if (playerHeldBy != null && playerHeldBy.IsInspectingItem)
                HUDManager.Instance.HideHUD(false);
            DiscardSyncServerRpc();  // bug fix, vanilla synced discard is done too late
        }

        [ServerRpc(RequireOwnership = false)]
        private void DiscardSyncServerRpc()
        {
            DiscardSyncClientRpc();
        }

        [ClientRpc]
        private void DiscardSyncClientRpc()
        {
            ControlModeAnimationClientRpc(false);
            PrepareScreen(false, true, false);
            base.DiscardItem();
        }

        public override void PocketItem()
        {
            ControlModeAnimationClientRpc(false);
            PrepareScreen(false, true, true);
            base.PocketItem();
        }

        public override void OnNetworkDespawn()
        {
            targetIsValid = false;
            PrepareScreen(false, true, true);
            base.OnNetworkDespawn();
        }

        public override void UseUpBatteries()
        {
            targetIsValid = false;
            ControlModeAnimationClientRpc(false, true);
            PrepareScreen(false, false, true);
            if (isHeld && IsOwner && GameNetworkManager.Instance.localPlayerController.playerClientId == playerHeldBy.playerClientId)
                SetControlTips();
            base.UseUpBatteries();
            SyncBatteryClientRpc(0);  // bug fix, vanilla battery is not synced at this moment
        }

        public override void ChargeBatteries()
        {
            base.ChargeBatteries();
            if (playerHeldBy != null && insertedBattery != null && insertedBattery.charge == 1f)
                PrepareScreen(true, false, true);
        }

        public void CreateCamera()
        {
            if (targetPlayer == null)
                return;
            var cameraObj = new GameObject("Controller Camera");
            cameraObj.transform.SetParent(targetPlayer.gameplayCamera.transform, false);
            cameraObj.transform.position = targetPlayer.gameplayCamera.transform.position + (targetPlayer.gameplayCamera.transform.forward * 0.4f);
            cameraObj.transform.rotation = targetPlayer.gameplayCamera.transform.rotation;
            camera = cameraObj.AddComponent<Camera>();
            camera.targetTexture = cameraTexture;
            camera.nearClipPlane = 0.01f;
            camera.farClipPlane = 100f;
            camera.cullingMask = playerHeldBy.gameplayCamera.cullingMask & ~LayerMask.GetMask("Ignore Raycast", "UI", "HelmetVisor");
            var cameraData = cameraObj.AddComponent<HDAdditionalCameraData>();
            cameraData.volumeLayerMask = 1;
            cameraData.customRenderingSettings = true;
            var frameSettings = cameraData.renderingPathCustomFrameSettings;
            var frameMask = cameraData.renderingPathCustomFrameSettingsOverrideMask;
            frameSettings.SetEnabled(FrameSettingsField.Tonemapping, false);
            frameMask.mask[(uint)FrameSettingsField.Tonemapping] = true;
            frameSettings.SetEnabled(FrameSettingsField.ColorGrading, false);
            frameMask.mask[(uint)FrameSettingsField.ColorGrading] = true;
            cameraData.renderingPathCustomFrameSettings = frameSettings;
            cameraData.renderingPathCustomFrameSettingsOverrideMask = frameMask;
            cameraData.hasPersistentHistory = true;
            StartCustomRender();
            cameraReady = true;
        }

        public void DestroyCamera()
        {
            cameraReady = false;
            StopCustomRender();
            if (camera != null)
            {
                Destroy(camera.gameObject);
                camera = null;
            }
            renderer?.materials[3].SetTexture("_ScreenTexture", null);
        }

        public void StartCustomRender()
        {
            if (targetPlayer == null || camera == null)
                return;
            nightVision = Instantiate(targetPlayer.nightVision.gameObject);
            nightVision.transform.SetParent(targetPlayer.gameplayCamera.transform, false);
            nightVision.transform.position = targetPlayer.gameplayCamera.transform.position;
            nightVision.transform.rotation = targetPlayer.gameplayCamera.transform.rotation;
            nightVision.SetActive(true);
            var nightVisionLight = nightVision.GetComponent<Light>();
            nightVisionLight.intensity = 10000;
            nightVisionLight.range = 50;
            nightVisionLight.enabled = false;
            controllerRender = camera.gameObject.AddComponent<ControllerRender>();
            controllerRender.Setup(camera, playerHeldBy, targetPlayer, nightVisionLight);
        }

        public void StopCustomRender()
        {
            if (controllerRender != null)
            {
                controllerRender.Free();
                Destroy(controllerRender);
            }
            if (nightVision != null && nightVision.gameObject != null)
            {
                Destroy(nightVision.gameObject);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChargingAnimationServerRpc()
        {
            ChargingAnimationClientRpc(Random.Range(0.9f, 1f));
        }

        [ClientRpc]
        private void ChargingAnimationClientRpc(float chargePitch)
        {
            if (chargingParticle != null && chargingAudio != null)
            {
                chargingParticle.Play();
                chargingAudio.pitch = chargePitch;
                chargingAudio.Play();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ControlModeAnimationServerRpc(bool start, bool playSpStopAudio = false)
        {
            ControlModeAnimationClientRpc(start, playSpStopAudio);
        }

        [ClientRpc]
        private void ControlModeAnimationClientRpc(bool start, bool playSpStopAudio = false)
        {
            if (controlModeParticle != null && controlModeAudio != null)
            {
                if (start && !controlModeParticle.isPlaying)
                {
                    controlModeParticle.Play();
                    controlModeAudio.PlayOneShot(Plugin.audioClips[24], 1f);
                }
                else if (!start && controlModeParticle.isPlaying)
                {
                    controlModeParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    controlModeAudio.PlayOneShot(Plugin.audioClips[playSpStopAudio ? 26 : 25], 1f);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ZapAnimationServerRpc(Vector3 source, Vector3 destination)
        {
            ZapAnimationClientRpc(source, destination);
        }

        [ClientRpc]
        private void ZapAnimationClientRpc(Vector3 source, Vector3 destination)
        {
            LightningBoltPrefabScript zap = Instantiate(FindObjectOfType<StormyWeather>(true).targetedThunder);
            zap.enabled = true;
            zap.Camera = GameNetworkManager.Instance.localPlayerController.gameplayCamera;
            zap.AutomaticModeSeconds = 0.2f;
            zap.LightningTintColor = Color.magenta;
            zap.GlowTintColor = Color.green;
            zap.LightParameters.LightColor = Color.green;
            zap.CountRange = new RangeOfIntegers { Minimum = 2, Maximum = 3 };
            zap.TrunkWidthRange = new RangeOfFloats { Minimum = 0.01f, Maximum = 0.02f };
            zap.Generations = 1;
            zap.GlowIntensity = 4;
            zap.Intensity = 10;
            zap.LightParameters.LightIntensity = 0.2f;
            zap.Source.transform.position = source;
            zap.Destination.transform.position = destination;
            zap.CreateLightningBoltsNow();
            Effects.Audio3D(23, destination + Vector3.up * 0.5f, 1.3f, 40);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetControlModeServerRpc(bool start, ulong clientId)
        {
            var clientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { clientId } } };
            SetControlModeClientRpc(start, clientRpcParams);
        }

        [ClientRpc]
        private void SetControlModeClientRpc(bool start, ClientRpcParams clientRpcParams = default)
        {
            SetControlMode(start);
        }

        private void SetControlMode(bool start)
        {
            var player = GameNetworkManager.Instance.localPlayerController;
            isBeingControlled = start;
            player.disableMoveInput = start;
            player.disableLookInput = start;
            player.isTypingChat = start;
            ControlledEffectServerRpc(start, player.playerClientId);
            StartCoroutine(SetupUI(start));
            if (!start || controlledUITextCoroutine != null)
                StopCoroutine(controlledUITextCoroutine);
            if (start && playerHeldBy != null)
                controlledUITextCoroutine = StartCoroutine(Effects.Status("Controlled by\n" + playerHeldBy.playerUsername));
        }

        private IEnumerator SetupUI(bool start)
        {
            if (controlledUI != null)
            {
                if (!start)
                    Effects.Audio(28, 1.1f);
                controlledUIAnimator?.SetTrigger("EndUI");
                yield return new WaitForSeconds(0.7f);
                Destroy(controlledUI);
                controlledUI = null;
            }
            if (start)
            {
                controlledUI = Instantiate(controlledUIPrefab);
                controlledUIAnimator = controlledUI.GetComponent<Animator>();
                Effects.Audio(27, 1.1f);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ControlledEffectServerRpc(bool start, ulong playerId)
        {
            ControlledEffectClientRpc(start, playerId);
        }

        [ClientRpc]
        private void ControlledEffectClientRpc(bool start, ulong playerId)
        {
            StartCoroutine(ControlledEffect(start, playerId));
        }

        private IEnumerator ControlledEffect(bool start, ulong playerId)
        {
            var player = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
            if (controlledAntena != null)
            {
                yield return Effects.FadeOutAudio(controlledAntena.GetComponent<AudioSource>(), 0.1f);
                Destroy(controlledAntena);
                controlledAntena = null;
            }
            if (start && player != null)
            {
                var headTransform = player.transform.Find("ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/spine.004/spine.004_end");
                if (headTransform != null)
                {
                    var pos = headTransform.position;
                    if (GameNetworkManager.Instance.localPlayerController.playerClientId == playerId)
                        pos += (headTransform.up * 0.12f) + (headTransform.forward * -0.1f);
                    controlledAntena = Instantiate(controlledAntenaPrefab, pos, headTransform.rotation, headTransform);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SendActionControlModeServerRpc(int actionId, ulong clientId, Vector2 input = default, float data = default)
        {
            if (input != default && input.x == 0 && input.y == 0)
                return;
            var clientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { clientId } } };
            SendActionControlModeClientRpc(actionId, input, data, clientRpcParams);
        }

        [ClientRpc]
        private void SendActionControlModeClientRpc(int actionId, Vector2 input, float data, ClientRpcParams clientRpcParams = default)
        {
            var player = GameNetworkManager.Instance.localPlayerController;
            if (!isBeingControlled || player.isPlayerDead)
                return;
            ControllerMovement.PerformAction(player, (ControllerMovement.ControllerActions)actionId, input, data);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncStateServerRpc(bool valid, ulong playerId, ulong clientId, bool serverValidOverride = false)
        {
            if (valid)
            {
                if (serverValidOverride)
                    serverDataValid = serverValidOverride;
                if (serverDataValid)
                    SyncStateClientRpc(valid, playerId, clientId);
            }
            else
            {
                serverDataValid = false;
                SyncStateClientRpc(valid, playerId, clientId);
            }
        }

        [ClientRpc]
        private void SyncStateClientRpc(bool valid, ulong playerId, ulong clientId)
        {
            isInControlMode = false;
            isBeingUsed = false;
            ControlModeAnimationClientRpc(false);
            if (isBeingControlled)
                SetControlMode(false);
            targetIsValid = valid;
            targetPlayerId = playerId;
            targetClientId = clientId;
            if (valid && GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null &&
                GameNetworkManager.Instance.localPlayerController.playerClientId == playerId)
                targetIsValid = false;
        }
    }
}
