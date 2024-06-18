using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.Utils
{
    internal class Effects
    {
        public static void Damage(PlayerControllerB player, int damageNb, int animation = 0, bool criticalBlood = true)
        {
            if (criticalBlood && player.health - damageNb <= 20)
                player.bleedingHeavily = true;
            player.DamagePlayer(damageNb, deathAnimation: animation);
        }

        public static void Knockback(PlayerControllerB player, Vector3 direction, int force)
        {
            new PhysicsKnockbackOnHit().Hit(force, direction, player);
        }

        public static void Teleportation(PlayerControllerB player, Vector3 position, bool toShip)
        {
            if (toShip)
            {
                player.isInElevator = true;
                player.isInHangarShipRoom = true;
                player.isInsideFactory = false;
            }
            player.averageVelocity = 0f;
            player.velocityLastFrame = Vector3.zero;
            player.TeleportPlayer(position, true);
            player.beamUpParticle.Play();
            HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
        }

        public static void Explosion(Vector3 position, int range)
        {
            Landmine.SpawnExplosion(position, true, range, range * 2, 50, 1);
        }

        public static void Audio(int audioID, Vector3 position, float volume)
        {
            AudioSource.PlayClipAtPoint(Plugin.sounds[audioID], position + (Vector3.up * 2), volume);
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
