using DigitalRuby.ThunderAndLightning;
using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class Controller : PhysicsProp
    {
        public MeshRenderer? renderer;
        public ParticleSystem? chargingParticle;
        public AudioSource? chargingAudio;
        public Transform? chargingTransform;
        public Light? screenLight;
        public float zapRange = 8f;
        public bool screenIsReady = false;
        public bool targetIsValid = false;
        public ulong targetPlayerId;
        public ulong targetClientId;
        public PlayerControllerB? targetPlayer;
        public float timePassed = 0;
        public bool isInControlMode = false;
        public bool readyToDisplay = true;
        public readonly float screenUpdateRate = 1;

        public Controller()
        {
            useCooldown = 2;
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
            if (!IsHost)
                SyncStateServerRpc();
            screenLight.enabled = false;
        }

        public override void EquipItem()
        {
            EnableItemMeshes(enable: true);
            PrepareScreen(true, insertedBattery != null && !insertedBattery.empty, true);
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
                HUDManager.Instance.ChangeControlTipMultiple(allLines, holdingItem: true, itemProperties);
            }
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (playerHeldBy == null || insertedBattery == null || insertedBattery.empty)
                return;
            var player = playerHeldBy;
            StartCoroutine(SearchForTarget(player));
        }

        public override void Update()
        {
            base.Update();
            if (playerHeldBy == null || GameNetworkManager.Instance.localPlayerController.playerClientId != playerHeldBy.playerClientId || !screenIsReady || targetPlayer == null)
                return;
            timePassed += Time.deltaTime;
            if (timePassed >= screenUpdateRate)
            {
                if (readyToDisplay)
                {
                    readyToDisplay = false;
                    UpdateScreenRequestServerRpc(GameNetworkManager.Instance.localPlayerController.OwnerClientId);
                }
                timePassed = 0;
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
                        targetIsValid = true;
                        targetPlayerId = target.playerClientId;
                        targetClientId = target.OwnerClientId;
                        SyncStateServerRpc(true, target.playerClientId, target.OwnerClientId);
                        ZapAnimationServerRpc(chargingTransform.position, target.transform.position + (Vector3.up * 2));
                        PrepareScreen(true);
                        SetControlTips();
                        break;
                    }
                }
            }
        }

        public void PrepareScreen(bool isReady, bool updateLight = false, bool updateFlagQE = false)
        {
            if (targetIsValid && GameNetworkManager.Instance.localPlayerController.playerClientId == targetPlayerId)
                targetIsValid = false;
            if (isReady)
            {
                if (updateLight && screenLight != null)
                    /*screenLight.enabled = true*/
                    ;
                if (updateFlagQE && playerHeldBy != null)
                    playerHeldBy.equippedUsableItemQE = true;
            }
            else
            {
                renderer?.materials[3].SetTexture("_ScreenTexture", null);
                if (updateLight && screenLight != null)
                    screenLight.enabled = false;
                if (updateFlagQE && playerHeldBy != null)
                    playerHeldBy.equippedUsableItemQE = false;
            }
            if (isReady && targetIsValid)
                targetPlayer = StartOfRound.Instance.allPlayerObjects[targetPlayerId].GetComponent<PlayerControllerB>();
            screenIsReady = isReady && targetIsValid;
        }

        public override void DiscardItem()
        {
            PrepareScreen(false, updateFlagQE: true);
            base.DiscardItem();
        }

        public override void PocketItem()
        {
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
            PrepareScreen(false, true);
            base.UseUpBatteries();
        }

        public override void ChargeBatteries()
        {
            base.ChargeBatteries();
            if (playerHeldBy != null && insertedBattery != null && insertedBattery.charge == 1f)
                PrepareScreen(true, true);
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
            isBeingUsed = true;
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
        private void UpdateScreenRequestServerRpc(ulong playerAskingClientId)
        {
            if (!targetIsValid)
                return;
            var clientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { targetClientId } } };
            UpdateScreenRequestClientRpc(playerAskingClientId, clientRpcParams);
        }

        [ClientRpc]
        private void UpdateScreenRequestClientRpc(ulong playerAskingClientId, ClientRpcParams clientRpcParams = default)
        {
            StartCoroutine(ScreenRequest(playerAskingClientId));
        }

        private IEnumerator ScreenRequest(ulong playerAskingClientId)
        {
            yield return null;
            var texture = GameNetworkManager.Instance.localPlayerController.gameplayCamera.targetTexture;
            byte[] data = ControllerData.SerializeObject(ControllerData.Encode(ControllerData.GetPixels(texture)));
            UpdateScreenResultServerRpc(playerAskingClientId, data, ControllerData.dataWidth, ControllerData.dataHeight);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateScreenResultServerRpc(ulong playerAskingClientId, byte[] data, int width, int height)
        {
            var clientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { playerAskingClientId } } };
            UpdateScreenResultClientRpc(data, width, height, clientRpcParams);
        }

        [ClientRpc]
        private void UpdateScreenResultClientRpc(byte[] data, int width, int height, ClientRpcParams clientRpcParams = default)
        {
            StartCoroutine(ScreenResult(data, width, height));
        }

        private IEnumerator ScreenResult(byte[] data, int width, int height)
        {
            yield return null;
            var colors = ControllerData.Decode(ControllerData.DeserializeObject<ControllerData.SerializableColor[]>(data));
            var texture = new Texture2D(width, height);
            texture.SetPixels(colors);
            texture.Apply();
            renderer?.materials[3].SetTexture("_ScreenTexture", texture);
            readyToDisplay = true;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncStateServerRpc()
        {
            if (!targetIsValid)
                return;
            SyncStateClientRpc(isBeingUsed, targetIsValid, targetPlayerId, targetClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncStateServerRpc(bool inUse, ulong playerId, ulong clientId)
        {
            SyncStateClientRpc(inUse, true, playerId, clientId);
        }

        [ClientRpc]
        private void SyncStateClientRpc(bool inUse, bool valid, ulong playerId, ulong clientId)
        {
            isBeingUsed = inUse;
            targetIsValid = valid;
            targetPlayerId = playerId;
            targetClientId = clientId;
        }
    }
}
