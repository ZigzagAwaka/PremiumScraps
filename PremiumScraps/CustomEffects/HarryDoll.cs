using PremiumScraps.Utils;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class HarryDoll : PhysicsProp
    {
        public HarryDoll() { }

        public override void DiscardItem()
        {
            if (!StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded)
            {
                if (Random.Range(0, 100) >= 92)
                    SpawnEnemyServerRpc(playerHeldBy.transform.position);
            }
            base.DiscardItem();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnEnemyServerRpc(Vector3 position)
        {
            Effects.Spawn(GetEnemies.GhostGirl, position);
        }
    }
}
