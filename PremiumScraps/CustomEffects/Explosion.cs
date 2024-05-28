using Unity.Netcode;

namespace PremiumScraps.CustomEffects
{
    internal class Explosion : PhysicsProp
    {
        [ServerRpc(RequireOwnership = false)]
        public static void SpawnExplosionServerRpc(UnityEngine.Vector3 position)
        {
            SpawnExplosionClientRpc(position);
        }

        [ClientRpc]
        public static void SpawnExplosionClientRpc(UnityEngine.Vector3 position)
        {
            Landmine.SpawnExplosion(position, true);
            // UnityEngine.Vector3.up;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {
                if (playerHeldBy != null) SpawnExplosionServerRpc(playerHeldBy.oldPlayerPosition);
            }
        }
    }
}
