using LethalNetworkAPI;
using PremiumScraps.Utils;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class Troll : PhysicsProp
    {
        public LethalClientMessage<Vector3> network;
        public Troll()
        {
            useCooldown = 2;
            customGrabTooltip = "Friendship ends here : [E]";
            network = new LethalClientMessage<Vector3>(identifier: "premiumscrapsTrollID");
            network.OnReceivedFromClient += SpawnEnemyNetwork;
        }

        private void SpawnEnemyNetwork(Vector3 position, ulong clientId)
        {
            GameObject gameObject = Instantiate(GetEnemies.Giant.enemyType.enemyPrefab, position, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
            RoundManager.Instance.SpawnedEnemies.Add(gameObject.GetComponent<EnemyAI>());
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded)
            {
                AudioSource.PlayClipAtPoint(Plugin.sounds[1], playerHeldBy.transform.position, 3f);
                if (Random.Range(1, 11) < 8)
                    HUDManager.Instance.DisplayTip("Don't do this bro", "Don't listen to the voices in your head.", true);
                else
                {
                    var playerTmp = playerHeldBy;
                    playerHeldBy.DamagePlayer(100, deathAnimation: 1);
                    if (playerTmp.IsHost)
                        SpawnEnemyNetwork(playerTmp.transform.position, 0);
                    else
                        network.SendAllClients(playerTmp.transform.position, false);
                }
            }
        }
    }
}
