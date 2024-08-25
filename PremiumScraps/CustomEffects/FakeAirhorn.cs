using LethalNetworkAPI;
using PremiumScraps.Utils;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class FakeAirhorn : PhysicsProp
    {
        public LethalClientMessage<Vector3> network, networkAudio, networkAudio2;
        public FakeAirhorn()
        {
            useCooldown = 2;
            network = new LethalClientMessage<Vector3>(identifier: "premiumscrapsFakeAirhornID");
            networkAudio = new LethalClientMessage<Vector3>(identifier: "premiumscrapsFakeAirhornAudioID");
            networkAudio2 = new LethalClientMessage<Vector3>(identifier: "premiumscrapsFakeAirhornAudio2ID");
            network.OnReceivedFromClient += SpawnExplosionNetwork;
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
            networkAudio2.OnReceivedFromClient += InvokeAudioNetwork2;
        }

        private void SpawnExplosionNetwork(Vector3 position, ulong clientId)
        {
            Effects.ExplosionDirect(position, 4);
        }

        private void InvokeAudioNetwork(Vector3 position, ulong clientId)
        {
            Effects.Audio(0, position, 5f);
        }

        private void InvokeAudioNetwork2(Vector3 position, ulong clientId)
        {
            Effects.Audio(6, position, 4f);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                if (StartOfRound.Instance.inShipPhase || Random.Range(1, 11) >= 4)  // 70%
                    networkAudio.SendAllClients(playerHeldBy.transform.position);  // airhorn audio
                else  // 30%
                {
                    if (Random.Range(1, 11) >= 6)  // 50%
                    {
                        networkAudio2.SendAllClients(playerHeldBy.transform.position);  // landmine audio
                        Effects.Damage(playerHeldBy, 100);  // death
                    }
                    else  // 50%
                    {
                        network.SendAllClients(playerHeldBy.transform.position);  // explosion
                    }
                }
            }
        }
    }
}
