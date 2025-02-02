using PremiumScraps.Utils;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class HarryDoll : PhysicsProp
    {
        public bool unluckyFirstTimeEffect = false;

        public HarryDoll() { }

        public override void DiscardItem()
        {
            if (!StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded && playerHeldBy != null)
            {
                var unlucky = Effects.IsUnlucky(playerHeldBy.playerSteamId);
                if (Random.Range(0, 100) >= 92 || (unlucky && Random.Range(0, 10) < 9))
                    SpawnEnemyServerRpc(playerHeldBy.transform.position, playerHeldBy.isInsideFactory, unlucky);  // spawn girl 8%, or 90% if unlucky
            }
            base.DiscardItem();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnEnemyServerRpc(Vector3 position, bool insideFactory, bool unlucky)
        {
            var spawnPosition = RoundManager.Instance.GetNavMeshPosition(position, sampleRadius: 3f);
            if (!RoundManager.Instance.GotNavMeshPositionResult)
                spawnPosition = Effects.GetClosestAINodePosition(insideFactory ? RoundManager.Instance.insideAINodes : RoundManager.Instance.outsideAINodes, position);
            var nb = 1;
            if (unlucky && !unluckyFirstTimeEffect)
            {
                nb = 10;
                unluckyFirstTimeEffect = true;
            }
            for (var i = 0; i < nb; i++)
                Effects.Spawn(GetEnemies.GhostGirl, spawnPosition);
        }
    }
}
