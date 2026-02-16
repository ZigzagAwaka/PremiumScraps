using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class OldStatue : PhysicsProp
    {
        public AudioSource? itemAudio;
        public Coroutine? audioCoroutine;

        public OldStatue()
        {
            customGrabTooltip = "iltam sumra rashupti elatim : [E]";
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            itemAudio = transform.GetComponent<AudioSource>();
        }

        public override void DiscardItem()
        {
            if (playerHeldBy != null && Effects.IsUnlucky(playerHeldBy.playerSteamId) && Random.Range(0, 10) <= 4)  // 50% unlucky
            {
                AudioServerRpc();
            }
            base.DiscardItem();
        }

        [ServerRpc(RequireOwnership = false)]
        private void AudioServerRpc()
        {
            AudioClientRpc();
        }

        [ClientRpc]
        private void AudioClientRpc()
        {
            audioCoroutine ??= StartCoroutine(PlaySpecialAudio());
        }

        private IEnumerator PlaySpecialAudio()
        {
            yield return new WaitForSeconds(7f);
            if (!isHeld && !isHeldByEnemy && itemAudio != null)
            {
                itemAudio.PlayOneShot(Plugin.audioClips[48], 1.0f);
                WalkieTalkie.TransmitOneShotAudio(itemAudio, Plugin.audioClips[48]);
                RoundManager.Instance.PlayAudibleNoise(transform.position, 20, 0.5f, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
            }
            audioCoroutine = null;
        }
    }
}
