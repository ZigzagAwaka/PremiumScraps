using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.Utils
{
    internal class Effects
    {
        public static List<PlayerControllerB> GetPlayers(bool includeDead = false)
        {
            var allPlayers = new List<PlayerControllerB>();
            foreach (GameObject playerObj in StartOfRound.Instance.allPlayerObjects)
            {
                PlayerControllerB player = playerObj.GetComponent<PlayerControllerB>();
                if (player.isActiveAndEnabled && player.IsSpawned && player.isPlayerControlled && (includeDead || !player.isPlayerDead))
                    allPlayers.Add(player);
            }
            return allPlayers;
        }

        public static void Damage(PlayerControllerB player, int damageNb, int animation = 0, bool criticalBlood = true)
        {
            if (criticalBlood && player.health - damageNb <= 20)
                player.bleedingHeavily = true;
            player.DamagePlayer(damageNb, deathAnimation: animation);
        }

        // modified from https://github.com/Cedeli/PushCompany/blob/master/Assets/Scripts/PushComponent.cs
        public static IEnumerator Knockback(PlayerControllerB player, Vector3 direction, float force)
        {
            Vector3 knockback = direction * force * Time.fixedDeltaTime;
            float smoothTime = knockback.magnitude / (force / 12.5f);

            Vector3 targetPosition = player.thisController.transform.position + knockback;
            Vector3 targetDirection = (targetPosition - player.thisController.transform.position).normalized;
            float distance = Vector3.Distance(player.thisController.transform.position, targetPosition);

            for (float currentTime = 0; currentTime < smoothTime; currentTime += Time.fixedDeltaTime)
            {
                float currentDistance = distance * Mathf.Min(currentTime, smoothTime) / smoothTime;
                player.thisController.Move(targetDirection * currentDistance);
                yield return null;
            }
        }

        public static void Teleportation(PlayerControllerB player, Vector3 position)
        {
            if (position == StartOfRound.Instance.middleOfShipNode.position)
            {
                player.isInElevator = true;
                player.isInHangarShipRoom = true;
                player.isInsideFactory = false;
            }
            player.averageVelocity = 0f;
            player.velocityLastFrame = Vector3.zero;
            player.TeleportPlayer(position, true);
            player.beamOutParticle.Play();
            HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
        }

        public static IEnumerator Explosion(Vector3 position, int range, float waitTime = 0f)
        {
            yield return new WaitForSeconds(waitTime);
            Landmine.SpawnExplosion(position, true, range, range * 2, 50, 1);
        }

        public static void DropItem(bool destroy = false)
        {
            GameNetworkManager.Instance.localPlayerController.DiscardHeldObject(true, placePosition: destroy ? RoundManager.Instance.insideAINodes[^1].transform.position - (Vector3.up * 100) : default);
        }

        public static void Audio(int audioID, Vector3 position, float volume, bool adjust = true)
        {
            var finalPosition = position;
            if (adjust)
                finalPosition += (Vector3.up * 2);
            AudioSource.PlayClipAtPoint(Plugin.audioClips[audioID], finalPosition, volume);
        }

        public static void Message(string title, string bottom, bool warning = false)
        {
            HUDManager.Instance.DisplayTip(title, bottom, warning);
        }

        public static void Spawn(SpawnableEnemyWithRarity enemy, Vector3 position)
        {
            GameObject gameObject = Object.Instantiate(enemy.enemyType.enemyPrefab, position, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
            RoundManager.Instance.SpawnedEnemies.Add(gameObject.GetComponent<EnemyAI>());
        }
    }
}
