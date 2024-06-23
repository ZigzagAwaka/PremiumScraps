using LethalNetworkAPI;
using PremiumScraps.Utils;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class FakeAirhorn : PhysicsProp
    {
        public LethalClientMessage<Vector3> network, networkAudio;
        public FakeAirhorn()
        {
            useCooldown = 2;
            network = new LethalClientMessage<Vector3>(identifier: "premiumscrapsFakeAirhornID");
            networkAudio = new LethalClientMessage<Vector3>(identifier: "premiumscrapsFakeAirhornAudioID");
            network.OnReceivedFromClient += SpawnExplosionNetwork;
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
        }

        private void SpawnExplosionNetwork(Vector3 position, ulong clientId)
        {
            Effects.Audio(6, position, 4f);
            StartCoroutine(Effects.Explosion(position, 5, 0.5f));
        }

        private void InvokeAudioNetwork(Vector3 position, ulong clientId)
        {
            Effects.Audio(0, position, 5f);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                if (StartOfRound.Instance.inShipPhase || Random.Range(1, 11) >= 4)  // 30% explosion
                    networkAudio.SendAllClients(playerHeldBy.transform.position);
                else
                    network.SendAllClients(playerHeldBy.transform.position);
            }
        }
    }
}
