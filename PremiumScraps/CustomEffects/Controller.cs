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
        public Transform? chargingTransform;
        public Light? screenLight;
        public readonly float zapRange = 8f;
        public bool screenIsReady = false;
        public bool targetIsValid = false;
        public bool serverDataValid = false;
        public ulong targetPlayerId;
        public ulong targetClientId;
        public PlayerControllerB? targetPlayer;
        public bool isInControlMode = false;
        public bool isBeingControlled = false;
        public bool cameraReady = false;
        public Camera? camera;
        public readonly int cameraTextureWidth = 860;
        public readonly int cameraTextureHeight = 520;
        public RenderTexture cameraTexture;

        public Controller()
        {
            useCooldown = 1.5f;
            cameraTexture = new RenderTexture(cameraTextureWidth, cameraTextureHeight, 24);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            chargingParticle = transform.GetChild(2).GetComponent<ParticleSystem>();
            chargingAudio = transform.GetChild(2).GetComponent<AudioSource>();
            chargingTransform = transform.GetChild(2).transform;
            screenLight = transform.GetChild(3).GetComponent<Light>();
            if (insertedBattery != null)
                insertedBattery.charge = 1;
            if (!IsHost && !IsServer)
                SyncStateServerRpc(isBeingUsed, true, targetPlayerId, targetClientId);
            itemProperties.batteryUsage = 50;
            screenLight.enabled = false;
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
                allLines = new string[4] { "Activate : [RMB]", isInControlMode ? "Stop : [Q]" : "Start controlling : [Q]", "Inspect: [Z]", "Target : " + targetName };
            }
            else
                allLines = new string[4] { "Activate : [RMB]", "Start controlling : [Q]", "Inspect: [Z]", "" };
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
                SyncStateServerRpc(false, false, 0, 0);
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
                        targetPlayerId = target.playerClientId;
                        targetClientId = target.OwnerClientId;
                        targetIsValid = true;
                        isInControlMode = false;
                        SyncStateServerRpc(false, true, target.playerClientId, target.OwnerClientId, true);
                        ZapAnimationServerRpc(chargingTransform.position, target.transform.position + (Vector3.up * 2));
                        yield return new WaitForSeconds(0.1f);
                        if (!isHeld || player.isPlayerDead)
                            yield break;
                        PrepareScreen(true, false, false);
                        SetControlTips();
                        break;
                    }
                }
            }
        }

        public void PrepareScreen(bool isReady, bool updateFlagQE, bool updateLight)
        {
            if (targetIsValid && GameNetworkManager.Instance.localPlayerController.playerClientId == targetPlayerId)
                targetIsValid = false;
            if (isReady)
            {
                if (updateFlagQE && playerHeldBy != null)
                    playerHeldBy.equippedUsableItemQE = true;
                if (updateLight && screenLight != null)
                    ;//screenLight.enabled = true;
            }
            else
            {
                DestroyCamera();
                if (updateFlagQE && playerHeldBy != null)
                    playerHeldBy.equippedUsableItemQE = false;
                if (updateLight && screenLight != null)
                    screenLight.enabled = false;
            }
            isInControlMode = false;
            if (isBeingControlled)
                SetControlModeClientRpc(false);
            if (isReady && targetIsValid)
                targetPlayer = StartOfRound.Instance.allPlayerObjects[targetPlayerId].GetComponent<PlayerControllerB>();
            screenIsReady = isReady && targetIsValid;
        }

        public override void DiscardItem()
        {
            PrepareScreen(false, true, false);
            base.DiscardItem();
        }

        public override void PocketItem()
        {
            Debug.LogError("pocket");
            PrepareScreen(false, true, true);
            base.PocketItem();
        }

        public override void OnNetworkDespawn()
        {
            PrepareScreen(false, true, true);
            base.OnNetworkDespawn();
        }

        public override void UseUpBatteries()
        {
            PrepareScreen(false, false, true);
            base.UseUpBatteries();
            SyncBatteryClientRpc(0);  // vanilla bug fix, battery is not synced at this moment
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
            cameraObj.transform.position = targetPlayer.gameplayCamera.transform.position + (targetPlayer.gameplayCamera.transform.forward * 0.7f);
            cameraObj.transform.rotation = targetPlayer.gameplayCamera.transform.rotation;
            camera = cameraObj.AddComponent<Camera>();
            camera.targetTexture = cameraTexture;
            camera.nearClipPlane = 0.01f;
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
            frameSettings.maximumLODLevel = 1;
            frameMask.mask[(uint)FrameSettingsField.MaximumLODLevel] = true;
            frameSettings.maximumLODLevelMode = MaximumLODLevelMode.OverrideQualitySettings;
            frameMask.mask[(uint)FrameSettingsField.MaximumLODLevelMode] = true;
            cameraData.renderingPathCustomFrameSettings = frameSettings;
            cameraData.renderingPathCustomFrameSettingsOverrideMask = frameMask;
            cameraData.hasPersistentHistory = true;
            cameraReady = true;
        }

        public void DestroyCamera()
        {
            cameraReady = false;
            if (camera != null)
            {
                Destroy(camera.gameObject);
                camera = null;
            }
            renderer?.materials[3].SetTexture("_ScreenTexture", null);
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
            isBeingControlled = start;
            GameNetworkManager.Instance.localPlayerController.disableMoveInput = start;
            GameNetworkManager.Instance.localPlayerController.disableLookInput = start;
            GameNetworkManager.Instance.localPlayerController.isTypingChat = start;
            Debug.LogError("controlled: " + start);
            // controlled overlay
        }

        [ServerRpc(RequireOwnership = false)]
        public void SendActionControlModeServerRpc(int actionId, ulong clientId)
        {
            var clientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { clientId } } };
            SendActionControlModeClientRpc(actionId, clientRpcParams);
        }

        [ClientRpc]
        private void SendActionControlModeClientRpc(int actionId, ClientRpcParams clientRpcParams = default)
        {
            var player = GameNetworkManager.Instance.localPlayerController;
            if (!isBeingControlled || player.isPlayerDead)
                return;
            switch ((ControllerMovement.ControllerActions)actionId)
            {
                case ControllerMovement.ControllerActions.Jump: ControllerMovement.Jump(player); break;
                case ControllerMovement.ControllerActions.Crouch: ControllerMovement.Crouch(player); break;
                case ControllerMovement.ControllerActions.Interact: ControllerMovement.Interact(player); break;
                default: break;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncStateServerRpc(bool inUse, bool valid, ulong playerId, ulong clientId, bool serverValidOverride = false)
        {
            if (valid)
            {
                if (serverValidOverride)
                    serverDataValid = serverValidOverride;
                if (serverDataValid)
                    SyncStateClientRpc(inUse, valid, playerId, clientId);
            }
            else
            {
                serverDataValid = false;
                SyncStateClientRpc(inUse, valid, playerId, clientId);
            }
        }

        [ClientRpc]
        private void SyncStateClientRpc(bool inUse, bool valid, ulong playerId, ulong clientId)
        {
            isInControlMode = false;
            if (isBeingControlled)
                SetControlModeClientRpc(false);
            isBeingUsed = inUse;
            targetIsValid = valid;
            targetPlayerId = playerId;
            targetClientId = clientId;
            if (valid && GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null &&
                GameNetworkManager.Instance.localPlayerController.playerClientId == playerId)
                targetIsValid = false;
        }
    }
}
