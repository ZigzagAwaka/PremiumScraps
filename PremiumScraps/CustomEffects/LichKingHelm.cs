using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class LichKingHelm : PhysicsProp
    {
        public AudioSource? weAreOneAudio;

        public LichKingHelm() { }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            weAreOneAudio = transform.GetComponent<AudioSource>();
        }

        public override void GrabItem()
        {
            base.GrabItem();
            if (Random.Range(0, 100) >= 80)
                EffectServerRpc(20, 0.8f, playerHeldBy != null && Effects.IsUnlucky(playerHeldBy.playerSteamId) && Random.Range(0, 10) >= 2);
        }

        [ServerRpc(RequireOwnership = false)]
        private void EffectServerRpc(int audioID, float volume, bool unlucky = false)
        {
            EffectClientRpc(audioID, volume, unlucky);
        }

        [ClientRpc]
        private void EffectClientRpc(int audioID, float volume, bool unlucky)
        {
            weAreOneAudio?.PlayOneShot(Plugin.audioClips[audioID], volume);
            if (true && !StartOfRound.Instance.inShipPhase && !StartOfRound.Instance.shipIsLeaving)
                StartCoroutine(DoBoom());
        }

        private IEnumerator DoBoom()
        {
            yield return new WaitForSeconds(4);
            if (!StartOfRound.Instance.inShipPhase && !StartOfRound.Instance.shipIsLeaving)
                Effects.Explosion(transform.position, 1.5f);
        }
    }
}
