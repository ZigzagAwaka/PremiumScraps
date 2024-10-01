using GameNetcodeStuff;
using LethalNetworkAPI;
using PremiumScraps.Utils;
using System.Collections;
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
        public LethalClientMessage<ulong> network;
        public LethalClientMessage<PosId> networkAudio;
        public LethalClientMessage<PosId> networkPosition;

        public SpanishDrink()
        {
            useCooldown = 4;
            customGrabTooltip = "Coger : [E]";
            network = new LethalClientMessage<ulong>(identifier: "premiumscrapsSpanishID");
            networkAudio = new LethalClientMessage<PosId>(identifier: "premiumscrapsSpanishAudioID");
            networkPosition = new LethalClientMessage<PosId>(identifier: "premiumscrapsSpanishPositionID");
            network.OnReceivedFromClient += HealPlayerNetwork;
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
            networkPosition.OnReceivedFromClient += InvokePositionNetwork;
        }

        private void HealPlayerNetwork(ulong playerID, ulong clientId)
        {
            Effects.Heal(playerID, 100);
        }

        private void InvokeAudioNetwork(PosId posId, ulong clientId)
        {
            Effects.Audio(posId.Id, posId.position, 1.8f);
        }

        private void InvokePositionNetwork(PosId posId, ulong clientId)
        {
            if (posId.Id == 0)
                itemProperties.positionOffset = posId.position;
            else
                itemProperties.rotationOffset = posId.position;
        }

        private void SelectUsageBeforeDrunk()
        {
            usageBeforeDrunk = Random.Range(usageBeforeDrunkMin, usageBeforeDrunkMax + 1);
        }

        public override void Start()
        {
            base.Start();
            SelectUsageBeforeDrunk();
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
                    networkPosition.SendAllClients(new PosId(0, new Vector3(0.05f, 0.1f, 0.05f)));
                    networkPosition.SendAllClients(new PosId(1, new Vector3(-50, 10, 0)));
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
                Effects.Audio(18, 1f);  // drink audio local player
                networkAudio.SendAllClients(new PosId(18, playerHeldBy.transform.position), false);  // sync drink audio
                yield return new WaitForSeconds(1.8f);
                networkPosition.SendAllClients(new PosId(0, originalPosition != null ? originalPosition.Value : default));
                networkPosition.SendAllClients(new PosId(1, originalRotation != null ? originalRotation.Value : default));
                player.playerBodyAnimator.SetBool("useTZPItem", false);  // stop drink animation
                player.activatingItem = false;
                yield return new WaitForSeconds(0.2f);
                if (!player.isPlayerDead)
                {
                    if (usage >= usageBeforeDrunk)
                    {
                        Effects.Audio(19, 1f);  // spanish audio local player
                        networkAudio.SendAllClients(new PosId(19, player.transform.position), false);  // sync spanish audio
                        if (player.IsHost)
                            StartCoroutine(Effects.DamageHost(player, 20, CauseOfDeath.Suffocation, (int)Effects.DeathAnimation.CutInHalf));  // damage or death (host)
                        else
                            Effects.Damage(player, 20, CauseOfDeath.Suffocation, (int)Effects.DeathAnimation.CutInHalf);  // damage or death
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
                        network.SendAllClients(StartOfRound.Instance.localPlayerController.playerClientId);  // heal
                        HUDManager.Instance.UpdateHealthUI(100, false);
                    }
                }
            }
        }

        private IEnumerator SpanishDrunk(PlayerControllerB player)
        {
            for (int n = 0; n < 240; n++)  // 2 minutes
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
    }
}
