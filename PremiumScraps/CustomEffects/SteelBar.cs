using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class SteelBar : Shovel
    {
        public AudioSource? audio;
        public MeshRenderer? renderer;
        private Color steelColor;

        public SteelBar() { }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            audio = transform.GetComponent<AudioSource>();
            renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            steelColor = renderer.materials[0].color;
            if (!IsHost && !IsServer && Plugin.config.squareSteelWeapon.Value)
                SyncColorServerRpc();
        }

        private bool IsUnlucky(bool instantCondition)
        {
            var player = GameNetworkManager.Instance.localPlayerController;
            if (player != null)
                return Effects.IsUnlucky(player.playerSteamId) && (instantCondition || Random.Range(0, 10) <= 3);  // 40% unlucky
            return false;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (!Plugin.config.squareSteelWeapon.Value)
                return;
            base.ItemActivate(used, buttonDown);
        }

        public override void GrabItem()
        {
            base.GrabItem();
            if (IsUnlucky(false) && Plugin.config.squareSteelWeapon.Value && shovelHitForce == 1)
            {
                StartCoroutine(UnluckySteel(playerHeldBy));
            }
        }

        public override void DiscardItem()
        {
            if (IsUnlucky(true) && Plugin.config.squareSteelWeapon.Value && shovelHitForce == 99)
            {
                shovelHitForce = 1;
                SetControlTips();
                ChargeSteelServerRpc(false);
            }
            base.DiscardItem();
        }

        public override void OnNetworkDespawn()
        {
            if (IsUnlucky(true) && Plugin.config.squareSteelWeapon.Value && shovelHitForce == 99)
            {
                shovelHitForce = 1;
                SetControlTips();
                ChargeSteelServerRpc(false);
            }
            base.OnNetworkDespawn();
        }

        private IEnumerator UnluckySteel(PlayerControllerB player)
        {
            yield return new WaitForEndOfFrame();
            while (player.isGrabbingObjectAnimation)
            {
                yield return new WaitForEndOfFrame();
            }
            Effects.Damage(player, player.health - 10, CauseOfDeath.Inertia, criticalBlood: false);
            if (player.health <= 0 || player.isPlayerDead)
                yield break;
            ChargeSteelServerRpc(true);
            shovelHitForce = 99;
            SetControlTips();
        }

        public override void EquipItem()
        {
            SetControlTips();
            EnableItemMeshes(enable: true);
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
            if (!Plugin.config.squareSteelWeapon.Value)
                allLines = new string[1] { "" };
            else
                allLines = (shovelHitForce == 1 ? new string[1] { "Steel bonk : [RMB]" } : new string[1] { "ULTIMATE bonk : [RMB]" });
            if (IsOwner)
            {
                HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.ChangeControlTipMultiple(allLines, holdingItem: true, itemProperties);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChargeSteelServerRpc(bool chargeOK)
        {
            ChargeSteelClientRpc(chargeOK);
        }

        [ClientRpc]
        private void ChargeSteelClientRpc(bool chargeOK)
        {
            if (chargeOK && itemProperties.dropSFX != null && audio != null)
            {
                audio.PlayOneShot(itemProperties.dropSFX);
            }
            if (chargeOK && renderer != null)
            {
                renderer.materials[0].color = Color.red;
            }
            if (!chargeOK && renderer != null)
            {
                renderer.materials[0].color = steelColor;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncColorServerRpc()
        {
            if (renderer != null && renderer.materials[0].color != steelColor)
            {
                SyncColorClientRpc(renderer.materials[0].color);
            }
        }

        [ClientRpc]
        private void SyncColorClientRpc(Color col)
        {
            if (renderer != null)
            {
                renderer.materials[0].color = col;
            }
        }
    }
}
