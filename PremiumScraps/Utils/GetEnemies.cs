/*
 Modified from https://github.com/Theronguard/Emergency-Dice/blob/main/Patches/GetEnemies.cs
*/

using HarmonyLib;

namespace PremiumScraps.Utils
{
    [HarmonyPatch(typeof(Terminal))]
    internal class GetEnemies
    {
        public static SpawnableEnemyWithRarity Masked, HoardingBug, Centipede, Jester, Bracken, Stomper,
                                                Coilhead, Beehive, Sandworm, Spider, Giant;
        public static SpawnableMapObject Landmine, Turret;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void GetEnemy(Terminal __instance)
        {
            foreach (SelectableLevel level in __instance.moonsCatalogueList)
            {
                foreach (var enemy in level.Enemies)
                {
                    if (enemy.enemyType.enemyName == "Masked" && Masked == null)
                        Masked = enemy;
                    else if (enemy.enemyType.enemyName == "Hoarding bug" && HoardingBug == null)
                        HoardingBug = enemy;
                    else if (enemy.enemyType.enemyName == "Centipede" && Centipede == null)
                        Centipede = enemy;
                    else if (enemy.enemyType.enemyName == "Jester" && Jester == null)
                        Jester = enemy;
                    else if (enemy.enemyType.enemyName == "Flowerman" && Bracken == null)
                        Bracken = enemy;
                    else if (enemy.enemyType.enemyName == "Crawler" && Stomper == null)
                        Stomper = enemy;
                    else if (enemy.enemyType.enemyName == "Spring" && Coilhead == null)
                        Coilhead = enemy;
                    else if (enemy.enemyType.enemyName == "Bunker Spider" && Spider == null)
                        Spider = enemy;
                }

                foreach (var enemy in level.DaytimeEnemies)
                {
                    if (enemy.enemyType.enemyName == "Red Locust Bees" && Beehive == null)
                        Beehive = enemy;
                }

                foreach (var enemy in level.OutsideEnemies)
                {
                    if (enemy.enemyType.enemyName == "Earth Leviathan" && Sandworm == null)
                        Sandworm = enemy;
                    else if (enemy.enemyType.enemyName == "ForestGiant" && Giant == null)
                        Giant = enemy;
                }

                foreach (var trap in level.spawnableMapObjects)
                {
                    if (trap.prefabToSpawn.name == "Landmine" && Landmine == null)
                        Landmine = trap;
                    if (trap.prefabToSpawn.name == "TurretContainer" && Turret == null)
                        Turret = trap;
                }

                if (Masked != null && HoardingBug != null && Centipede != null && Jester != null
                    && Bracken != null && Stomper != null && Coilhead != null && Beehive != null
                    && Sandworm != null && Spider != null && Giant != null && Landmine != null
                    && Turret != null)
                    break;
            }
        }
    }
}