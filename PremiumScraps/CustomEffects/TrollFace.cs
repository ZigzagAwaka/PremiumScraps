using LethalNetworkAPI;
using PremiumScraps.Utils;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class TrollFace : PhysicsProp
    {
        public LethalClientMessage<Vector3> network, networkAudio;
        public TrollFace()
        {
            useCooldown = 3;
            customGrabTooltip = "Friendship ends here : [E]";
            network = new LethalClientMessage<Vector3>(identifier: "premiumscrapsTrollFaceID");
            networkAudio = new LethalClientMessage<Vector3>(identifier: "premiumscrapsTrollFaceAudioID");
            network.OnReceivedFromClient += SpawnEnemyNetwork;
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
        }

        private void SpawnEnemyNetwork(Vector3 position, ulong clientId)
        {
            Effects.Spawn(GetEnemies.Giant, position);
        }

        private void InvokeAudioNetwork(Vector3 position, ulong clientId)
        {
            Effects.Audio(1, position, 2.5f);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded)
            {
                networkAudio.SendAllClients(playerHeldBy.transform.position);
                if (playerHeldBy.health > 90)
                {
                    Effects.Damage(playerHeldBy, 10);
                    Effects.Message("Don't do this bro", "Don't listen to the voices in your head.", true);
                }
                else if (playerHeldBy.health > 70 && playerHeldBy.health <= 90)
                {
                    Effects.Damage(playerHeldBy, 20);
                    Effects.Message("We warned you", "You know there's no turning back from what you're about to do, right?", true);
                }
                else if (playerHeldBy.health > 20 && playerHeldBy.health <= 70)
                {
                    Effects.Damage(playerHeldBy, playerHeldBy.health - 10);
                    Effects.Message("W̴ͪ̅e̤̲̞ ḏ͆ȍ̢̥ a̵̿͘ l̙ͭ͠ittle b̈́͠it of troll͢i̗̍͜n͙̆͠g", "", true);
                }
                else
                {
                    var playerTmp = playerHeldBy;
                    Effects.Damage(playerHeldBy, 100, 1);
                    if (playerTmp.IsHost)  // Multiple spawn, feature for host, bug for clients
                        for (int i = 0; i < 4; i++)
                            SpawnEnemyNetwork(playerTmp.transform.position, 0);
                    else
                        network.SendAllClients(playerTmp.transform.position, false);
                }
            }
        }
    }
}
