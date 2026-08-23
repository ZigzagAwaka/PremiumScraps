/*
 Modified from https://github.com/Theronguard/Emergency-Dice/blob/main/Patches/GetEnemies.cs
*/

using HarmonyLib;

namespace PremiumScraps.Utils
{
    [HarmonyPatch(typeof(Terminal))]
    internal class GetEnemies
    {
        public static SpawnableEnemyWithRarity Masked, HoardingBug, SnareFlea, Jester, Bracken, Thumper, CoilHead,
                                               CircuitBees, EarthLeviathan, BunkerSpider, ForestKeeper, GhostGirl,
                                               TulipSnake, EyelessDog, Maneater, Nutcracker, Barber, Butler, OldBird,
                                               ShyGuy, RedwoodTitan, RedwoodGiant, Locker, Bruce, SparkTower, BaboonHawk,
                                               Tourist, Kiwi, Gunfish, Feiopar, CadaverGrowths;
        public static UnityEngine.GameObject Landmine, Turret, SpikeTrap, Seamine, BigBertha;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void GetEnemy(Terminal __instance)
        {
            foreach (SelectableLevel level in __instance.moonsCatalogueList)
            {
                foreach (var enemy in level.Enemies)
                {
                    if (enemy.enemyType.enemyName == "Masked" && Masked == null)
                        Masked = enemy;
                    else if (enemy.enemyType.enemyName == "Hoarding bug" && HoardingBug == null)
                        HoardingBug = enemy;
                    else if (enemy.enemyType.enemyName == "Centipede" && SnareFlea == null)
                        SnareFlea = enemy;
                    else if (enemy.enemyType.enemyName == "Jester" && Jester == null)
                        Jester = enemy;
                    else if (enemy.enemyType.enemyName == "Flowerman" && Bracken == null)
                        Bracken = enemy;
                    else if (enemy.enemyType.enemyName == "Crawler" && Thumper == null)
                        Thumper = enemy;
                    else if (enemy.enemyType.enemyName == "Spring" && CoilHead == null)
                        CoilHead = enemy;
                    else if (enemy.enemyType.enemyName == "Bunker Spider" && BunkerSpider == null)
                        BunkerSpider = enemy;
                    else if (enemy.enemyType.enemyName == "Girl" && GhostGirl == null)
                        GhostGirl = enemy;
                    else if (enemy.enemyType.enemyName == "Maneater" && Maneater == null)
                        Maneater = enemy;
                    else if (enemy.enemyType.enemyName == "Nutcracker" && Nutcracker == null)
                        Nutcracker = enemy;
                    else if (enemy.enemyType.enemyName == "Clay Surgeon" && Barber == null)
                        Barber = enemy;
                    else if (enemy.enemyType.enemyName == "Butler" && Butler == null)
                        Butler = enemy;
                    else if (enemy.enemyType.enemyName == "Shy guy" && ShyGuy == null)
                        ShyGuy = enemy;
                    else if (enemy.enemyType.enemyName == "Locker" && Locker == null)
                        Locker = enemy;
                    else if (enemy.enemyType.enemyName == "Stingray" && Gunfish == null)
                        Gunfish = enemy;
                    else if (enemy.enemyType.enemyName == "Cadaver Growths" && CadaverGrowths == null)
                        CadaverGrowths = enemy;
                }

                foreach (var enemy in level.DaytimeEnemies)
                {
                    if (enemy.enemyType.enemyName == "Red Locust Bees" && CircuitBees == null)
                        CircuitBees = enemy;
                    else if (enemy.enemyType.enemyName == "Tulip Snake" && TulipSnake == null)
                        TulipSnake = enemy;
                    else if (enemy.enemyType.enemyName == "GiantKiwi" && Kiwi == null)
                        Kiwi = enemy;
                }

                foreach (var enemy in level.OutsideEnemies)
                {
                    if (enemy.enemyType.enemyName == "Earth Leviathan" && EarthLeviathan == null)
                        EarthLeviathan = enemy;
                    else if (enemy.enemyType.enemyName == "ForestGiant" && ForestKeeper == null)
                        ForestKeeper = enemy;
                    else if (enemy.enemyType.enemyName == "MouthDog" && EyelessDog == null)
                        EyelessDog = enemy;
                    else if (enemy.enemyType.enemyName == "Baboon hawk" && BaboonHawk == null)
                        BaboonHawk = enemy;
                    else if (enemy.enemyType.enemyName == "RadMech" && OldBird == null)
                        OldBird = enemy;
                    else if (enemy.enemyType.enemyName == "Redwood Titan" && RedwoodTitan == null)
                        RedwoodTitan = enemy;
                    else if (enemy.enemyType.enemyName == "RedWoodGiant" && RedwoodGiant == null)
                        RedwoodGiant = enemy;
                    else if (enemy.enemyType.enemyName == "Bruce" && Bruce == null)
                        Bruce = enemy;
                    else if (enemy.enemyType.enemyName == "SparkTower" && SparkTower == null)
                        SparkTower = enemy;
                    else if (enemy.enemyType.enemyName == "Tourist" && Tourist == null)
                        Tourist = enemy;
                    else if (enemy.enemyType.enemyName == "Feiopar" && Feiopar == null)
                        Feiopar = enemy;
                }

                foreach (var trap in level.indoorMapHazards)
                {
                    if (trap.hazardType.prefabToSpawn.name == "Landmine" && Landmine == null)
                        Landmine = trap.hazardType.prefabToSpawn;
                    else if (trap.hazardType.prefabToSpawn.name == "TurretContainer" && Turret == null)
                        Turret = trap.hazardType.prefabToSpawn;
                    else if (trap.hazardType.prefabToSpawn.name == "SpikeRoofTrapHazard" && SpikeTrap == null)
                        SpikeTrap = trap.hazardType.prefabToSpawn;
                    else if (trap.hazardType.prefabToSpawn.name == "Seamine" && Seamine == null)
                        Seamine = trap.hazardType.prefabToSpawn;
                    else if (trap.hazardType.prefabToSpawn.name == "Bertha" && BigBertha == null)
                        BigBertha = trap.hazardType.prefabToSpawn;
                }
            }
        }
    }
}