using LethalNetworkAPI;
using PremiumScraps.Utils;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class TalkingBall : SoccerBallProp
    {
        public bool isSpeaking = true;
        public float originalSpeed = 0.3f;
        public bool getOriginalSpeed = false;
        public Vector3? originalPosition = null;
        public LethalClientMessage<PosId> networkAudio;

        public TalkingBall()
        {
            networkAudio = new LethalClientMessage<PosId>(identifier: "premiumscrapsAbiAudioID");
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
        }

        private void InvokeAudioNetwork(PosId posId, ulong clientId)
        {
            Effects.Audio(posId.Id, posId.position, 3f);
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
                        Effects.Audio(audioID, 1.5f);  // uwu audio
                        networkAudio.SendAllClients(new PosId(audioID, playerHeldBy.transform.position), false);  // sync uwu audio
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
    }
}