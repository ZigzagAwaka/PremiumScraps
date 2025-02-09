using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class CDI_King : NoisemakerProp
    {
        public Animator? kingAnimator;

        public CDI_King() { }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            kingAnimator = transform.GetChild(0).GetComponent<Animator>();
            if (insertedBattery != null)
                insertedBattery.charge = 1;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (buttonDown && playerHeldBy != null && !noiseAudio.isPlaying)
            {
                var unlucky = (Effects.IsUnlucky(playerHeldBy.playerSteamId) && Random.Range(0, 10) <= 3) || Random.Range(0, 100) <= 2;  // unlucky 40%, or 3%
                if (!unlucky || StartOfRound.Instance.inShipPhase || !StartOfRound.Instance.shipHasLanded || StartOfRound.Instance.shipIsLeaving)
                    KingAudioServerRpc();
                else
                    KingBadEffectServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void KingAudioServerRpc()
        {
            KingAudioClientRpc(Random.Range(0, noiseSFX.Length));
        }

        [ClientRpc]
        private void KingAudioClientRpc(int selectedNoise)
        {
            if (GameNetworkManager.Instance.localPlayerController == null)
                return;
            float volume = noisemakerRandom.Next((int)(minLoudness * 100f), (int)(maxLoudness * 100f)) / 100f;
            noiseAudio.PlayOneShot(noiseSFX[selectedNoise], volume);
            kingAnimator?.SetTrigger("Noise" + (selectedNoise + 1));
            WalkieTalkie.TransmitOneShotAudio(noiseAudio, noiseSFX[selectedNoise], volume);
            RoundManager.Instance.PlayAudibleNoise(transform.position, noiseRange, volume, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
            if (minLoudness >= 0.6f && playerHeldBy != null)
            {
                playerHeldBy.timeSinceMakingLoudNoise = 0f;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void KingBadEffectServerRpc()
        {
            KingBadEffectClientRpc();
        }

        [ClientRpc]
        private void KingBadEffectClientRpc()
        {
            if (GameNetworkManager.Instance.localPlayerController == null)
                return;
            StartCoroutine(BadEffect());
        }

        private IEnumerator BadEffect()
        {
            noiseAudio.PlayOneShot(Plugin.audioClips[31], 1.2f);
            kingAnimator?.SetTrigger("BadEffect");
            yield return new WaitForSeconds(1.4f);
            Effects.Explosion(transform.position, 3f, 50, 20);
        }
    }
}
