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

        private void SpawnExplosionNetwork(UnityEngine.Vector3 position, ulong clientId)
        {
            Landmine.SpawnExplosion(position, true, 4, 8, 50, 1);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {
                if (playerHeldBy != null) network.SendAllClients(playerHeldBy.transform.position);
            }
        }
    }
}
