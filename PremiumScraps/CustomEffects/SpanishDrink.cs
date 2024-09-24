using GameNetcodeStuff;
using LethalNetworkAPI;
using PremiumScraps.Utils;
using System.Collections;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class SpanishDrink : PhysicsProp
    {
        public int usage = 0;
        public bool isDrunk = false;
        public LethalClientMessage<ulong> network;
        public LethalClientMessage<PosId> networkAudio;
        public LethalClientMessage<PosId> networkAudio2;

        public SpanishDrink()
        {
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

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (playerHeldBy != null)
            {
                usage++;
                Effects.Audio(7, 1f);
                networkAudio.SendAllClients(new PosId(7, playerHeldBy.transform.position), false);
                if (IsOwner)
                {
                    playerHeldBy.activatingItem = buttonDown;
                    playerHeldBy.playerBodyAnimator.SetBool("useTZPItem", buttonDown);
                }
                var player = playerHeldBy;
                StartCoroutine(SpanishEffect(player));
            }
        }

        private IEnumerator SpanishEffect(PlayerControllerB player)
        {
            yield return new WaitForSeconds(3);
            if (IsOwner)
            {
                player.playerBodyAnimator.SetBool("useTZPItem", false);
                player.activatingItem = false;
                Effects.Audio(6, 1f);
                networkAudio.SendAllClients(new PosId(6, player.transform.position), false);
                yield return new WaitForSeconds(0.2f);
                network.SendAllClients(StartOfRound.Instance.localPlayerController.playerClientId);
                if (usage >= 3 && !isDrunk)
                {
                    StartCoroutine(SpanishDrunk(player));
                    isDrunk = true;
                }
            }
        }

        private IEnumerator SpanishDrunk(PlayerControllerB player)
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                if (player.health <= 0)
                {
                    isDrunk = false;
                    break;
                }
                player.drunkness = 1;
                player.drunknessInertia = 1;
                player.drunknessSpeed = 1;
            }
            // StartOfRound.Instance.gameStats.daysSpent = 0;

        }
    }
}
