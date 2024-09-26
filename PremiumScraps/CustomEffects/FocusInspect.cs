using LethalNetworkAPI;
using PremiumScraps.Utils;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    // special ABIBABOU inspect item code
    internal class FocusInspect : SoccerBallProp
    {
        public float originalSpeed = 0.3f;
        public bool getOriginalSpeed = false;
        public Vector3? originalPosition = null;
        public LethalClientMessage<PosId> networkAudio;

        public FocusInspect()
        {
            networkAudio = new LethalClientMessage<PosId>(identifier: "premiumscrapsAbiAudioID");
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
        }

        private void InvokeAudioNetwork(PosId posId, ulong clientId)
        {
            Effects.Audio(posId.Id, posId.position, 3f);
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
                    if (Random.Range(0, 10) >= 3)  // 70%
                        Effects.Audio(14, 1.5f);  // huh audio
                    else  // 30%
                    {
                        Effects.Audio(16, 1.5f);  // uwu audio
                        networkAudio.SendAllClients(new PosId(16, playerHeldBy.transform.position), false);  // sync uwu audio
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