using LethalNetworkAPI;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class SoundExplosion : PhysicsProp
    {
        public LethalClientMessage<Vector3> network, networkAudio;
        public SoundExplosion()
        {
            useCooldown = 2;
            network = new LethalClientMessage<Vector3>(identifier: "premiumscrapsSoundExplosionID");
            networkAudio = new LethalClientMessage<Vector3>(identifier: "premiumscrapsSoundExplosionAudioID");
            network.OnReceivedFromClient += SpawnExplosionNetwork;
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
        }

        private void SpawnExplosionNetwork(Vector3 position, ulong clientId)
        {
            Landmine.SpawnExplosion(position, true, 4, 8, 50, 1);
        }

        private void InvokeAudioNetwork(Vector3 position, ulong clientId)
        {
            AudioSource.PlayClipAtPoint(Plugin.sounds[0], position + (Vector3.up * 2), 5f);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                networkAudio.SendAllClients(playerHeldBy.transform.position);
                if (Random.Range(1, 11) < 4 && !StartOfRound.Instance.inShipPhase)
                {
                    network.SendAllClients(playerHeldBy.transform.position);
                }
            }
        }
    }
}
