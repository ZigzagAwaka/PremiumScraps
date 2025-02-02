using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class FakeAirhorn : NoisemakerProp
    {
        public Animator? warningAnimator;

        public FakeAirhorn() { }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            warningAnimator = transform.GetChild(0).GetComponent<Animator>();
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (buttonDown && playerHeldBy != null)
            {
                var unlucky = Effects.IsUnlucky(playerHeldBy.playerSteamId);
                if (StartOfRound.Instance.inShipPhase || (unlucky && Random.Range(0, 10) <= 1) || (!unlucky && Random.Range(0, 10) >= 3))  // 70%, or 20% if unlucky
                {
                    BaseItemActivateServerRpc(used, buttonDown);  // airhorn audio
                }
                else  // 30%, or 80% if unlucky
                {
                    var player = playerHeldBy;
                    StartCoroutine(BadEffect(player, unlucky, used, buttonDown));
                }
            }
        }

        private IEnumerator BadEffect(PlayerControllerB player, bool unlucky, bool used, bool buttonDown = true)
        {
            bool explosion = Random.Range(0, 2) == 0 || unlucky;
            BaseItemActivateServerRpc(used, buttonDown, true, explosion);  // airhorn warning audio
            yield return new WaitForSeconds(explosion ? 0.7f : 0.5f);
            if (player.isPlayerDead)
            {
                RestorePitchServerRpc();
                yield break;
            }
            if (!explosion)  // 50%
            {
                for (int i = 0; i < 3; i++)
                {
                    LightningServerRpc(player.transform.position, player.isInsideFactory);
                    yield return new WaitForSeconds(0.1f);
                }
                Effects.Damage(player, 100, CauseOfDeath.Burning, (int)Effects.DeathAnimation.Fire);  // death
            }
            else  // 50%, or 100% if unlucky
            {
                ExplosionServerRpc(player.transform.position);  // explosion
            }
            yield return new WaitForSeconds(explosion ? 1.25f : 1f);
            RestorePitchServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void BaseItemActivateServerRpc(bool used, bool buttonDown, bool warning = false, bool explosion = false)
        {
            BaseItemActivateClientRpc(used, buttonDown, warning, explosion);
        }

        [ClientRpc]
        private void BaseItemActivateClientRpc(bool used, bool buttonDown, bool warning, bool explosion)
        {
            if (!warning)
                base.ItemActivate(used, buttonDown);
            else
            {
                warningAnimator?.SetTrigger(explosion ? "Explosion" : "Death");
                noiseAudio.pitch = explosion ? 0.5f : 1.5f;
                noiseAudio.PlayOneShot(noiseSFX[0], 1f);
            }
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

        [ServerRpc(RequireOwnership = false)]
        private void LightningServerRpc(Vector3 position, bool inside)
        {
            LightningClientRpc(position, inside);
        }

        [ClientRpc]
        private void LightningClientRpc(Vector3 position, bool inside)
        {
            Effects.SpawnLightningBolt(position, false, false);
            if (inside)
                Effects.Audio3D(30, position, 1f);
        }
    }
}
