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
            Effects.Explosion(position, 4f, 50, 2);
        }

        private void InvokeAudioNetwork(Vector3 position, ulong clientId)
        {
            Effects.Audio(0, position, 5f);
            RoundManager.Instance.PlayAudibleNoise(position, 60, 0.9f);
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
                        if (playerHeldBy.IsHost)
                            StartCoroutine(Effects.DamageHost(playerHeldBy, 100, CauseOfDeath.Burning, (int)Effects.DeathAnimation.Fire));  // death (host)
                        else
                            Effects.Damage(playerHeldBy, 100, CauseOfDeath.Burning, (int)Effects.DeathAnimation.Fire);  // death
                    }
                    else  // 50%
                    {
                        var playerTmp = playerHeldBy;
                        if (playerHeldBy.IsHost)
                        {
                            StartCoroutine(Effects.ExplosionHostDeath(playerHeldBy.transform.position, 4f, 50, 10));  // explosion (host)
                            network.SendAllClients(playerTmp.transform.position, false);  // explosion for other players
                        }
                        else
                            network.SendAllClients(playerTmp.transform.position);  // explosion
                    }
                }
            }
        }
    }
}
