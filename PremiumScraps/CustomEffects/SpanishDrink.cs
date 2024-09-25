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
        public int usageBeforeDrunk = 3;
        private readonly int usageBeforeDrunkMin = 0;
        private readonly int usageBeforeDrunkMax = 6;
        public LethalClientMessage<ulong> network;
        public LethalClientMessage<PosId> networkAudio;
        public LethalClientMessage<PosId> networkAudio2;

        public SpanishDrink()
        {
            useCooldown = 4;
            customGrabTooltip = "Coger : [E]";
            network = new LethalClientMessage<ulong>(identifier: "premiumscrapsSpanishID");
            networkAudio = new LethalClientMessage<PosId>(identifier: "premiumscrapsSpanishAudioID");
            networkAudio2 = new LethalClientMessage<PosId>(identifier: "premiumscrapsSpanishAudio2ID");
            network.OnReceivedFromClient += HealPlayerNetwork;
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
            networkAudio2.OnReceivedFromClient += InvokeAudioNetwork2;
        }

        private void HealPlayerNetwork(ulong playerID, ulong clientId)
        {
            Effects.Heal(playerID, 100);
        }

        private void InvokeAudioNetwork(PosId posId, ulong clientId)
        {
            Effects.Audio(posId.Id, posId.position, 3f);
        }

        private void InvokeAudioNetwork2(PosId posId, ulong clientId)
        {
            Effects.Audio(posId.Id, posId.position, 5f);
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
                Effects.Audio(7, 1f);  // drink audio local player
                networkAudio.SendAllClients(new PosId(7, playerHeldBy.transform.position), false);  // sync drink audio
                if (IsOwner)
                {
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
            yield return new WaitForSeconds(3);
            if (IsOwner)
            {
                player.playerBodyAnimator.SetBool("useTZPItem", false);  // stop drink animation
                player.activatingItem = false;
                Effects.Audio(6, 1f);  // spanish audio local player
                networkAudio.SendAllClients(new PosId(6, player.transform.position), false);  // sync spanish audio
                yield return new WaitForSeconds(0.2f);
                if (usage >= usageBeforeDrunk)
                {
                    if (player.IsHost)
                        StartCoroutine(Effects.DamageHost(player, 10, CauseOfDeath.Suffocation, (int)Effects.DeathAnimation.CutInHalf));  // damage or death (host)
                    else
                        Effects.Damage(player, 10, CauseOfDeath.Suffocation, (int)Effects.DeathAnimation.CutInHalf);  // damage or death
                }
                if (!isDrunk && usage >= usageBeforeDrunk)
                {
                    StartCoroutine(SpanishDrunk(player)); // drunk effect
                    isDrunk = true;
                }
                else if (!isDrunk)
                {
                    network.SendAllClients(StartOfRound.Instance.localPlayerController.playerClientId);  // heal
                }
            }
        }

        private IEnumerator SpanishDrunk(PlayerControllerB player)
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                if (player.isPlayerDead)
                {
                    isDrunk = false;
                    usage = 0;
                    SelectUsageBeforeDrunk();
                    break;
                }
                player.drunkness = 1;
                player.drunknessInertia = 1;
                player.drunknessSpeed = 1;
            }
        }
    }
}
