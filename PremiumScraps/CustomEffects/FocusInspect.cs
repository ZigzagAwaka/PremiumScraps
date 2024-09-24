using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    // special ABIBABOU inspect item code
    internal class FocusInspect : SoccerBallProp
    {
        public float originalSpeed = 0.3f;
        public bool getOriginalSpeed = false;
        public Vector3? originalPosition = null;

        public FocusInspect() { }

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