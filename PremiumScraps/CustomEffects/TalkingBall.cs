using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class TalkingBall : SoccerBallProp
    {
        public bool isSpeaking = false;
        public bool playingSpecialAudio = false;
        public float originalSpeed = 0.3f;
        public bool getOriginalSpeed = false;
        public Vector3? originalPosition = null;
        public AudioSource? classicAudio;
        public AudioSource? specialAudio;
        public MeshRenderer? renderer;

        public TalkingBall() { }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            classicAudio = GetComponent<AudioSource>();
            specialAudio = transform.GetChild(3).GetComponent<AudioSource>();
            renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            StartCoroutine(StartIdleAudio());
        }

        private IEnumerator StartIdleAudio()
        {
            yield return new WaitForSeconds(0.5f);
            if (!isHeld)
            {
                classicAudio?.Play();
                isSpeaking = true;
            }
        }

        public override void GrabItem()
        {
            base.GrabItem();
            if (isSpeaking && isHeld)
            {
                isSpeaking = false;
                var audio = GetComponent<AudioSource>();
                audio.Pause();
                audio.loop = false;
                audio.clip = null;
                audio.UnPause();
            }
        }

        public override void InspectItem()
        {
            if (itemProperties.canBeInspected && IsOwner && playerHeldBy != null && renderer != null)
            {
                base.InspectItem();
                if (playerHeldBy.IsInspectingItem)
                {
                    originalPosition = itemProperties.positionOffset;
                    itemProperties.positionOffset = new Vector3(originalPosition.Value.x, originalPosition.Value.y * 2.0f, originalPosition.Value.z);
                    if (!getOriginalSpeed)
                    {
                        getOriginalSpeed = true;
                        originalSpeed = renderer.materials[0].GetFloat("_Speed");
                    }
                    renderer.materials[0].SetFloat("_Speed", 0f);
                    if (!playingSpecialAudio && Effects.IsUnlucky(playerHeldBy.playerSteamId) && Random.Range(0, 10) < 6)
                    {
                        playingSpecialAudio = true;
                        AudioServerRpc(-1, Vector3.zero, 1, 1, true);
                    }
                    else
                    {
                        if (Random.Range(0, 10) >= 1)  // 90%
                            Effects.Audio(14, 1.5f);  // huh audio
                        else  // 10%
                        {
                            int audioID = 16;
                            if (Random.Range(0, 10) <= 1)  // 20% rare uwu
                                audioID = 17;
                            AudioServerRpc(audioID, playerHeldBy.transform.position, 1.1f, 0.75f);  // uwu audio
                        }
                    }
                }
                else
                {
                    StopInspect();
                }
            }
        }

        public override void PocketItem()
        {
            base.PocketItem();
            StopInspect(true);
        }

        public override void DiscardItem()
        {
            base.DiscardItem();
            StopInspect(true);
        }

        private void StopInspect(bool fixHUD = false)
        {
            renderer?.materials[0].SetFloat("_Speed", originalSpeed);
            if (originalPosition != null)
                itemProperties.positionOffset = originalPosition.Value;
            if (fixHUD)
                HUDManager.Instance.HideHUD(false);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AudioServerRpc(int audioID, Vector3 clientPosition, float localVolume, float clientVolume = default, bool spAudio = false)
        {
            AudioClientRpc(audioID, clientPosition, localVolume, clientVolume == default ? localVolume : clientVolume, spAudio);
        }

        [ClientRpc]
        private void AudioClientRpc(int audioID, Vector3 clientPosition, float localVolume, float clientVolume, bool spAudio)
        {
            if (!spAudio)
                Effects.Audio(audioID, clientPosition, localVolume, clientVolume, playerHeldBy);
            else
                StartCoroutine(SpecialAudio());
        }

        private IEnumerator SpecialAudio()
        {
            playingSpecialAudio = true;
            specialAudio?.Play();
            yield return new WaitForSeconds(9f);
            playingSpecialAudio = false;
        }
    }
}