using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class SpanishDrink : PhysicsProp
    {
        public bool isDrunk = false;
        public int usage = 0;
        public int usageBeforeDrunk = 1;
        private readonly int usageBeforeDrunkMin = 0;
        private readonly int usageBeforeDrunkMax = 6;
        public Vector3? originalPosition = null;
        public Vector3? originalRotation = null;

        public SpanishDrink()
        {
            useCooldown = 4;
            customGrabTooltip = "Coger : [E]";
        }

        private void SelectUsageBeforeDrunk()
        {
            usageBeforeDrunk = Random.Range(usageBeforeDrunkMin, usageBeforeDrunkMax + 1);
        }

        public override void Start()
        {
            base.Start();
            SelectUsageBeforeDrunk();
            /*if (!Plugin.config.gazpachoMemeSfx.Value)
            {
                itemProperties.grabSFX = Plugin.audioClips[21];
                itemProperties.dropSFX = Plugin.audioClips[22];
            }*/
            itemProperties.grabSFX = Plugin.audioClips[21];
            itemProperties.dropSFX = Plugin.audioClips[22];
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (playerHeldBy != null)
            {
                if (IsOwner)
                {
                    originalPosition = itemProperties.positionOffset;
                    originalRotation = itemProperties.rotationOffset;
                    UpdatePosRotServerRpc(new Vector3(0.05f, 0.1f, 0.05f), new Vector3(-50, 10, 0));
                    playerHeldBy.activatingItem = buttonDown;
                    playerHeldBy.playerBodyAnimator.SetBool("useTZPItem", buttonDown);  // start drink animation
                }
                usage++;
                var player = playerHeldBy;
                StartCoroutine(SpanishEffect(player));
            }
        }

        private IEnumerator SpanishEffect(PlayerControllerB player)
        {
            if (IsOwner)
            {
                yield return new WaitForSeconds(0.8f);
                AudioServerRpc(18, playerHeldBy.transform.position, 1f, 0.7f);  // drink audio
                yield return new WaitForSeconds(1.8f);
                UpdatePosRotServerRpc(originalPosition != null ? originalPosition.Value : default, originalRotation != null ? originalRotation.Value : default);
                player.playerBodyAnimator.SetBool("useTZPItem", false);  // stop drink animation
                player.activatingItem = false;
                yield return new WaitForSeconds(0.2f);
                if (!player.isPlayerDead)
                {
                    if (usage >= usageBeforeDrunk)
                    {
                        AudioServerRpc(19, player.transform.position, 1f, 0.85f, player.health - 19 <= 0);  // spanish audio
                        StartCoroutine(SpanishBoom(player));  // damage and things
                    }
                    if (!isDrunk && usage >= usageBeforeDrunk)
                    {
                        Effects.Message("You were poisoned !", "");
                        yield return new WaitForSeconds(2f);
                        StartCoroutine(SpanishDrunk(player));  // drunk effect
                        isDrunk = true;
                    }
                    else if (!isDrunk)
                    {
                        HealPlayerServerRpc(StartOfRound.Instance.localPlayerController.playerClientId, 100);  // heal
                    }
                }
            }
        }

        private IEnumerator SpanishBoom(PlayerControllerB player)
        {
            yield return new WaitForEndOfFrame();
            if (player == null || player.isPlayerDead)
                yield break;
            if (Effects.IsUnlucky(player.playerSteamId) && Random.Range(0, 10) < 7)  // unlucky 70%
            {
                yield return new WaitForSeconds(1.9f);
                if (player == null || player.isPlayerDead)
                    yield break;
                ExplosionServerRpc(player.transform.position);
            }
            else  // normal
            {
                Effects.Damage(player, 20, CauseOfDeath.Suffocation, (int)Effects.DeathAnimation.CutInHalf);
            }
        }

        private IEnumerator SpanishDrunk(PlayerControllerB player)
        {
            for (int n = 0; n < 120; n++)  // 1 minute
            {
                yield return new WaitForSeconds(0.5f);
                if (player == null || player.isPlayerDead)
                    break;
                player.drunkness = 1;
                player.drunknessInertia = 1;
                player.drunknessSpeed = 1;
            }
            isDrunk = false;
        }

        [ServerRpc(RequireOwnership = false)]
        private void AudioServerRpc(int audioID, Vector3 clientPosition, float localVolume, float clientVolume = default, bool is3D = false)
        {
            AudioClientRpc(audioID, clientPosition, localVolume, clientVolume == default ? localVolume : clientVolume, is3D);
        }

        [ClientRpc]
        private void AudioClientRpc(int audioID, Vector3 clientPosition, float localVolume, float clientVolume, bool is3D)
        {
            if (!is3D)
                Effects.Audio(audioID, clientPosition, localVolume, clientVolume, playerHeldBy);
            else
                Effects.Audio3D(audioID, clientPosition, clientVolume);
        }

        [ServerRpc(RequireOwnership = false)]
        private void HealPlayerServerRpc(ulong playerID, int health)
        {
            HealPlayerClientRpc(playerID, health);
        }

        [ClientRpc]
        private void HealPlayerClientRpc(ulong playerID, int health)
        {
            Effects.Heal(playerID, health);
            if (playerHeldBy != null && GameNetworkManager.Instance.localPlayerController.playerClientId == playerHeldBy.playerClientId)
                HUDManager.Instance.UpdateHealthUI(health, false);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdatePosRotServerRpc(Vector3 newPos, Vector3 newRot)
        {
            UpdatePosRotClientRpc(newPos, newRot);
        }

        [ClientRpc]
        private void UpdatePosRotClientRpc(Vector3 newPos, Vector3 newRot)
        {
            itemProperties.positionOffset = newPos;
            itemProperties.rotationOffset = newRot;
        }

        [ServerRpc(RequireOwnership = false)]
        private void ExplosionServerRpc(Vector3 position)
        {
            ExplosionClientRpc(position);
        }

        [ClientRpc]
        private void ExplosionClientRpc(Vector3 position)
        {
            Effects.Explosion(position, 2f, 20);
        }
    }
}
