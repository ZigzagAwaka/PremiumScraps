using LethalNetworkAPI;

namespace PremiumScraps.CustomEffects
{
    internal class Explosion : PhysicsProp
    {
        public LethalClientMessage<UnityEngine.Vector3> network;
        Explosion()
        {
            network = new LethalClientMessage<UnityEngine.Vector3>(identifier: "premiumscrapsExplosionID");
            network.OnReceivedFromClient += SpawnExplosionNetwork;
        }

        /*        [ServerRpc(RequireOwnership = false)]
                public static void SpawnExplosionServerRpc(UnityEngine.Vector3 position)
                {
                    SpawnExplosionClientRpc(position);
                }

                [ClientRpc]
                public static void SpawnExplosionClientRpc(UnityEngine.Vector3 position)
                {
                    Landmine.SpawnExplosion(position, true);
                    // UnityEngine.Vector3.up;
                }*/

        private void SpawnExplosionNetwork(UnityEngine.Vector3 position, ulong clientId)
        {
            Landmine.SpawnExplosion(position, true);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {
                if (playerHeldBy != null) network.SendAllClients(playerHeldBy.oldPlayerPosition);
                //SpawnExplosionServerRpc(playerHeldBy.oldPlayerPosition);
            }
        }
    }
}
