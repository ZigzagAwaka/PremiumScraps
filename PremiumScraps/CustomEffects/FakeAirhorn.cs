using PremiumScraps.Utils;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class FakeAirhorn : NoisemakerProp
    {
        public FakeAirhorn() { }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (buttonDown && playerHeldBy != null)
            {
                if (StartOfRound.Instance.inShipPhase || Random.Range(1, 11) >= 4)  // 70%
                {
                    BaseItemActivateServerRpc(used, buttonDown);  // airhorn audio
                }
                else  // 30%
                {
                    if (Random.Range(1, 11) >= 6)  // 50%
                    {
                        AudioServerRpc(6, playerHeldBy.transform.position, 1.5f);  // landmine audio
                        if (playerHeldBy.IsHost)
                            StartCoroutine(Effects.DamageHost(playerHeldBy, 100, CauseOfDeath.Burning, (int)Effects.DeathAnimation.Fire));  // death (host)
                        else
                            Effects.Damage(playerHeldBy, 100, CauseOfDeath.Burning, (int)Effects.DeathAnimation.Fire);  // death
                    }
                    else  // 50%
                    {
                        ExplosionServerRpc(playerHeldBy.transform.position);  // explosion
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void BaseItemActivateServerRpc(bool used, bool buttonDown)
        {
            BaseItemActivateClientRpc(used, buttonDown);
        }

        [ClientRpc]
        private void BaseItemActivateClientRpc(bool used, bool buttonDown)
        {
            base.ItemActivate(used, buttonDown);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AudioServerRpc(int audioID, Vector3 clientPosition, float clientVolume = default)
        {
            AudioClientRpc(audioID, clientPosition, clientVolume);
        }

        [ClientRpc]
        private void AudioClientRpc(int audioID, Vector3 clientPosition, float clientVolume)
        {
            Effects.Audio3D(audioID, clientPosition, clientVolume);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ExplosionServerRpc(Vector3 position)
        {
            ExplosionClientRpc(position);
        }

        [ClientRpc]
        private void ExplosionClientRpc(Vector3 position)
        {
            if (IsHost)
                StartCoroutine(Effects.ExplosionHostDeath(position, 4f, 50, 2));
            else
                Effects.Explosion(position, 4f, 50, 2);
        }
    }
}
