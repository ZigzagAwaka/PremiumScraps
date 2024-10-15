using System.Collections;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class BadStatue : PhysicsProp
    {
        public BadStatue() { }

        public override void GrabItem()
        {
            base.GrabItem();
            GetComponent<MeshRenderer>().sharedMaterials[1] = itemProperties.materialVariants[3];
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            //StartCoroutine(FixMaterial());
        }

        private IEnumerator FixMaterial()
        {
            if (!StartOfRound.Instance.shipHasLanded)
                yield return new WaitUntil(() => StartOfRound.Instance.shipHasLanded == true);
            yield return new WaitForSeconds(2f);
            GetComponent<MeshRenderer>().sharedMaterials[1] = itemProperties.materialVariants[3];
        }
    }
}
