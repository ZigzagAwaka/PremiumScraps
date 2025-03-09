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
            itemProperties.toolTips[0] = Lang.Get("KING_USAGE");
            itemProperties.toolTips[1] = Lang.Get("KING_USAGE2");
            if (Lang.ACTUAL_LANG == "fr")
            {
                itemProperties.grabSFX = Plugin.audioClips[41];
                itemProperties.dropSFX = Plugin.audioClips[42];
                noiseSFX[0] = Plugin.audioClips[43];
                noiseSFX[1] = Plugin.audioClips[44];
                noiseSFX[2] = Plugin.audioClips[45];
                noiseSFX[3] = Plugin.audioClips[46];
                noiseSFX[4] = Plugin.audioClips[47];
            }
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (buttonDown && playerHeldBy != null && !noiseAudio.isPlaying)
            {
                var unlucky = (Effects.IsUnlucky(playerHeldBy.playerSteamId) && Random.Range(0, 10) <= 4) || Random.Range(0, 100) <= 1;  // unlucky 50%, or 2%
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
