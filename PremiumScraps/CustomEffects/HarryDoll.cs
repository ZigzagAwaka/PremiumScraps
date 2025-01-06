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
            if (!StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded && playerHeldBy != null)
            {
                if (Random.Range(0, 100) >= 92 || (Effects.IsUnlucky(playerHeldBy.playerSteamId) && Random.Range(0, 10) < 9))
                    SpawnEnemyServerRpc(playerHeldBy.transform.position, playerHeldBy.isInsideFactory);  // spawn girl 8%, or 90% if unlucky
            }
            base.DiscardItem();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnEnemyServerRpc(Vector3 position, bool insideFactory)
        {
            var spawnPosition = RoundManager.Instance.GetNavMeshPosition(position, sampleRadius: 3f);
            if (!RoundManager.Instance.GotNavMeshPositionResult)
                spawnPosition = Effects.GetClosestAINodePosition(insideFactory ? RoundManager.Instance.insideAINodes : RoundManager.Instance.outsideAINodes, position);
            Effects.Spawn(GetEnemies.GhostGirl, spawnPosition);
        }
    }
}
