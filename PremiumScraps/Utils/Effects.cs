using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.Utils
{
    internal class Effects
    {
        public static void Explosion(Vector3 position, int range)
        {
            Landmine.SpawnExplosion(position, true, range, range * 2, 50, 1);
        }

        public static void Audio(int audioID, Vector3 position, float volume)
        {
            AudioSource.PlayClipAtPoint(Plugin.sounds[audioID], position + (Vector3.up * 2), volume);
        }

        public static void Message(string title, string bottom, bool warning)
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
