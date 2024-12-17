using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
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
                    var player = playerHeldBy;
                    StartCoroutine(BadEffect(player, used, buttonDown));
                }
            }
        }

        private IEnumerator BadEffect(PlayerControllerB player, bool used, bool buttonDown = true)
        {
            bool explosion = false;
            if (Random.Range(0, 2) == 0)
                explosion = true;
            BaseItemActivateServerRpc(used, buttonDown, true, explosion ? 0.5f : 1.5f);  // airhorn warning audio
            yield return new WaitForSeconds(0.7f);
            if (player.isPlayerDead)
            {
                RestorePitchServerRpc();
                yield break;
            }
            if (!explosion)  // 50%
            {
                AudioServerRpc(6, player.transform.position, 1.5f);  // landmine audio
                Effects.Damage(player, 100, CauseOfDeath.Burning, (int)Effects.DeathAnimation.Fire);  // death
            }
            else  // 50%
            {
                ExplosionServerRpc(player.transform.position);  // explosion
            }
            yield return new WaitForSeconds(1.2f);
            RestorePitchServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void BaseItemActivateServerRpc(bool used, bool buttonDown, bool warning = false, float warningPitch = -1)
        {
            BaseItemActivateClientRpc(used, buttonDown, warning, warningPitch);
        }

        [ClientRpc]
        private void BaseItemActivateClientRpc(bool used, bool buttonDown, bool warning, float warningPitch)
        {
            if (!warning)
                base.ItemActivate(used, buttonDown);
            else
            {
                noiseAudio.pitch = warningPitch;
                noiseAudio.PlayOneShot(noiseSFX[0], 1f);
            }
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
        private void RestorePitchServerRpc()
        {
            RestorePitchClientRpc();
        }

        [ClientRpc]
        private void RestorePitchClientRpc()
        {
            noiseAudio.pitch = noisemakerRandom.Next((int)(minPitch * 100f), (int)(maxPitch * 100f)) / 100f;
        }

        [ServerRpc(RequireOwnership = false)]
        private void ExplosionServerRpc(Vector3 position)
        {
            ExplosionClientRpc(position);
        }

        [ClientRpc]
        private void ExplosionClientRpc(Vector3 position)
        {
            Effects.Explosion(position, 4.5f, 40, 2);
        }
    }
}
