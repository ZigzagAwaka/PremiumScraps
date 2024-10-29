using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using LethalLib.Modules;
using PremiumScraps.CustomEffects;
using PremiumScraps.Utils;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace PremiumScraps
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(LethalThings.Plugin.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "zigzag.premiumscraps";
        const string NAME = "PremiumScraps";
        const string VERSION = "2.0.3";

        public static Plugin instance;
        public static List<AudioClip> audioClips;
        //public static List<GameObject> gameObjects;
        private readonly Harmony harmony = new Harmony(GUID);
        internal static Config config { get; private set; } = null!;

        void HarmonyPatchAll()
        {
            harmony.CreateClassProcessor(typeof(GetEnemies), true).Patch();  // getenemies patch
            if (Chainloader.PluginInfos.ContainsKey(LethalThings.Plugin.ModGUID))
                harmony.CreateClassProcessor(typeof(LethalThingsBombItemChargerPatch), true).Patch();  // bombitem charger with lethalthings
            else
                harmony.CreateClassProcessor(typeof(BombItemChargerPatch), true).Patch();  // bombitem charger
            if (Chainloader.PluginInfos.ContainsKey("mattymatty.MattyFixes"))
                harmony.CreateClassProcessor(typeof(MattyFixesAirhornPositionPatch), true).Patch();  // fake airhorn position fix with matty fixes
        }

        void LoadItemBehaviour(Item item, int behaviourId)
        {
            GrabbableObject script;
            switch (behaviourId)
            {
                case 1: script = item.spawnPrefab.AddComponent<FakeAirhorn>(); SetupScript.Copy((NoisemakerProp)script, item); break;
                case 2: script = item.spawnPrefab.AddComponent<TrollFace>(); break;
                case 3: script = item.spawnPrefab.AddComponent<ScrollTP>(); break;
                case 4: script = item.spawnPrefab.AddComponent<LegendaryStick>(); break;
                case 5: script = item.spawnPrefab.AddComponent<StupidBook>(); break;
                case 6: script = item.spawnPrefab.AddComponent<JobDark>(); break;
                case 7: script = item.spawnPrefab.AddComponent<SpanishDrink>(); break;
                case 8: script = item.spawnPrefab.AddComponent<TalkingBall>(); SetupScript.Copy((SoccerBallProp)script, item); break;
                case 9: script = item.spawnPrefab.AddComponent<HarryDoll>(); break;
                case 10: script = item.spawnPrefab.AddComponent<Bomb>(); SetupScript.Copy((ThrowableItem)script, item); break;
                case 11: script = item.spawnPrefab.AddComponent<LichKingHelm>(); break;
                default: return;
            }
            script.grabbable = true;
            script.isInFactory = true;
            script.grabbableToEnemies = true;
            script.itemProperties = item;
        }

        void Awake()
        {
            instance = this;

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "premiumscraps");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);

            string directory = "Assets/Data/";

            /*gameObjects = new List<GameObject> {
                bundle.LoadAsset<GameObject>(directory + "DeathNote/DeathNoteCanvas.prefab")
            };*/

            audioClips = new List<AudioClip> {
                bundle.LoadAsset<AudioClip>(directory + "_audio/AirHorn1.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/friendship_ends_here.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/scroll_tp.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/ShovelReelUp.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/ShovelSwing.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/wooden-staff-hit.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/MineTrigger.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/book_page.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/CVuse1.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/CVuse2.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/CVuse3.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/CVuse4.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/TerminalAlarm.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/Breathing.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/huh.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/book_use_redesign.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/uwu.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/uwu-rot.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/drink.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/spanishsound.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/arthas.wav")
            };

            var scraps = new List<Scrap> {
                new Scrap("Frieren/FrierenItem.asset", 10),
                new Scrap("Chocobo/ChocoboItem.asset", 10),
                new Scrap("AinzOoalGown/AinzOoalGownItem.asset", 5),
                new Scrap("HelmDomination/HelmDominationItem.asset", 11, 11),
                new Scrap("TheKing/TheKingItem.asset", 13),
                new Scrap("HarryMason/HarryMasonItem.asset", 10, 9),
                new Scrap("Cristal/CristalItem.asset", 9),
                new Scrap("PuppyShark/PuppySharkItem.asset", 10),
                new Scrap("Rupee/RupeeItem.asset", 15),
                new Scrap("EaNasir/EaNasirItem.asset", 9),
                new Scrap("HScard/HSCardItem.asset", 10),
                new Scrap("SODA/SODAItem.asset", 8),
                new Scrap("Spoon/SpoonItem.asset", 12),
                new Scrap("Crouton/CroutonItem.asset", 6),
                new Scrap("AirHornCustom/AirHornCustomItem.asset", 11, 1),
                new Scrap("Balan/BalanItem.asset", 10),
                new Scrap("CustomFace/CustomFaceItem.asset", 8, 2),
                new Scrap("Scroll/ScrollItem.asset", 7, 3),
                new Scrap("Stick/StickItem.asset", 9, 4),
                new Scrap("BookCustom/BookCustomItem.asset", 11, 5),
                new Scrap("SquareSteel/SquareSteelItem.asset", 10),
                new Scrap("DarkJobApplication/JobApplicationItem.asset", 8, 6),
                new Scrap("Moogle/MoogleItem.asset", 10),
                new Scrap("Gazpacho/GazpachoItem.asset", 9, 7),
                new Scrap("Abi/AbiItem.asset", 4, 8),
                new Scrap("Bomb/BombItem.asset", 10, 10)
            };

            int i = 0; config = new Config(base.Config, scraps);
            SetupScript.Network();

            foreach (Scrap scrap in scraps)
            {
                Item item = bundle.LoadAsset<Item>(directory + scrap.asset);
                if (scrap.behaviourId != 0) LoadItemBehaviour(item, scrap.behaviourId);
                NetworkPrefabs.RegisterNetworkPrefab(item.spawnPrefab);
                Utilities.FixMixerGroups(item.spawnPrefab);
                Items.RegisterScrap(item, config.entries[i++].Value, Levels.LevelTypes.All);
            }

            HarmonyPatchAll();
            Logger.LogInfo("PremiumScraps is loaded !");
        }
    }
}
