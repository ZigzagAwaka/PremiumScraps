using PremiumScraps.Utils;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class TalkingBall : SoccerBallProp
    {
        public bool isSpeaking = true;
        public float originalSpeed = 0.3f;
        public bool getOriginalSpeed = false;
        public Vector3? originalPosition = null;

        public TalkingBall() { }

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
            if (itemProperties.canBeInspected && IsOwner && playerHeldBy != null)
            {
                base.InspectItem();
                if (playerHeldBy.IsInspectingItem)
                {
                    originalPosition = itemProperties.positionOffset;
                    itemProperties.positionOffset = new Vector3(originalPosition.Value.x, originalPosition.Value.y * 2.0f, originalPosition.Value.z);
                    if (!getOriginalSpeed)
                    {
                        getOriginalSpeed = true;
                        originalSpeed = itemProperties.spawnPrefab.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.GetFloat("_Speed");
                    }
                    itemProperties.spawnPrefab.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Speed", 0f);
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
            itemProperties.spawnPrefab.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Speed", originalSpeed);
            if (originalPosition != null)
                itemProperties.positionOffset = originalPosition.Value;
            if (fixHUD)
                HUDManager.Instance.HideHUD(false);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AudioServerRpc(int audioID, Vector3 clientPosition, float localVolume, float clientVolume = default)
        {
            AudioClientRpc(audioID, clientPosition, localVolume, clientVolume == default ? localVolume : clientVolume);
        }

        [ClientRpc]
        private void AudioClientRpc(int audioID, Vector3 clientPosition, float localVolume, float clientVolume)
        {
            Effects.Audio(audioID, clientPosition, localVolume, clientVolume, playerHeldBy);
        }
    }
}