using GameNetcodeStuff;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.Utils
{
    internal class Effects
    {
        public static void Damage(PlayerControllerB player, int damageNb, CauseOfDeath cause = 0, int animation = 0, bool criticalBlood = true)
        {
            if (criticalBlood && player.health - damageNb <= 20)
                player.bleedingHeavily = true;
            player.DamagePlayer(damageNb, causeOfDeath: cause, deathAnimation: animation);
        }

        public static IEnumerator DamageHost(PlayerControllerB player, int damageNb, CauseOfDeath cause = 0, int animation = 0, bool criticalBlood = true)
        {
            yield return new WaitForEndOfFrame();
            Damage(player, damageNb, cause, animation, criticalBlood);
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

        public static void Explosion(Vector3 position, float range)
        {
            Landmine.SpawnExplosion(position, true, range, range * 2.5f, 50, 1);
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

        public static void Audio(int audioID, Vector3 position, float volume, float pitch, bool adjust = true)
        {
            var clip = Plugin.audioClips[audioID];
            var finalPosition = position;
            if (adjust)
                finalPosition += (Vector3.up * 2);
            GameObject gameObject = new GameObject("One shot audio");
            gameObject.transform.position = finalPosition;
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();
            Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
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

        public static void Spawn(string scrapName, Vector3 position)
        {
            var scrap = RoundManager.Instance.currentLevel.spawnableScrap.FirstOrDefault(i => i.spawnableItem.name.Equals(scrapName));
            GameObject gameObject = Object.Instantiate(scrap.spawnableItem.spawnPrefab, position, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);
            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
            component.transform.rotation = Quaternion.Euler(component.itemProperties.restingRotation);
            component.fallTime = 0f;
            component.scrapValue = (int)(Random.Range(scrap.spawnableItem.minValue, scrap.spawnableItem.maxValue) * RoundManager.Instance.scrapValueMultiplier);
            NetworkObject network = gameObject.GetComponent<NetworkObject>();
            network.Spawn();
            component.FallToGround(true);
        }
    }
}
