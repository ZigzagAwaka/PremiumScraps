using LethalNetworkAPI;
using PremiumScraps.Utils;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class ScrollTP : PhysicsProp
    {
        public LethalClientMessage<Vector3> networkAudio;
        public ScrollTP()
        {
            networkAudio = new LethalClientMessage<Vector3>(identifier: "premiumscrapsScrollTPAudioID");
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
        }

        private void InvokeAudioNetwork(Vector3 position, ulong clientId)
        {
            Effects.Audio(2, position, 2.5f);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded)
            {
                var player = playerHeldBy;
                Effects.DropItem(); // to rework
                grabbable = false;
                Effects.Teleportation(player, StartOfRound.Instance.middleOfShipNode.position);
                networkAudio.SendAllClients(player.transform.position);
            }
        }
    }
}
