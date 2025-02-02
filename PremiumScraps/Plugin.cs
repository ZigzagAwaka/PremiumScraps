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
    [BepInDependency("AudioKnight.StarlancerAIFix", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("ShipInventory", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("mrov.WeatherRegistry", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(LethalThings.Plugin.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Theronguard.EmergencyDice", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "zigzag.premiumscraps";
        const string NAME = "PremiumScraps";
        const string VERSION = "2.2.1";

        public static Plugin instance;
        public static List<AudioClip> audioClips = new List<AudioClip>();
        public static List<GameObject> gameObjects = new List<GameObject>();
        private readonly Harmony harmony = new Harmony(GUID);
        internal static Config config { get; private set; } = null!;

        void HarmonyPatchAll()
        {
            PremiumScrapsMonoModPatches.Load();  // IL code patches
            harmony.CreateClassProcessor(typeof(GetEnemies), true).Patch();
            harmony.CreateClassProcessor(typeof(ControllerTerminalPatch), true).Patch();
            harmony.CreateClassProcessor(typeof(ControllerHUDManagerPatch), true).Patch();
            harmony.CreateClassProcessor(typeof(ControllerPlayerControllerBPatch), true).Patch();
            harmony.CreateClassProcessor(typeof(ControllerVehicleControllerPatch), true).Patch();
            if (Chainloader.PluginInfos.ContainsKey(LethalThings.Plugin.ModGUID))
                harmony.CreateClassProcessor(typeof(LethalThingsBombItemChargerPatch), true).Patch();  // bombitem charger with lethalthings
            else
                harmony.CreateClassProcessor(typeof(BombItemChargerPatch), true).Patch();  // bombitem charger
            if (Chainloader.PluginInfos.ContainsKey("mattymatty.MattyFixes"))
                harmony.CreateClassProcessor(typeof(MattyFixesAirhornPositionPatch), true).Patch();  // fake airhorn position fix with matty fixes
            if (Chainloader.PluginInfos.ContainsKey("ShipInventory"))
                ShipInventoryConditions.Setup(Chainloader.PluginInfos.GetValueOrDefault("ShipInventory").Metadata);  // setup conditions for shipinventory
            if (config.diceEvents.Value && Chainloader.PluginInfos.ContainsKey("Theronguard.EmergencyDice"))
                DiceEvents.RegisterDiceEvents(Logger, Chainloader.PluginInfos.GetValueOrDefault("Theronguard.EmergencyDice").Metadata);  // register custom events for emergency dice mod
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
                case 12: script = item.spawnPrefab.AddComponent<Controller>(); break;
                case 13: script = item.spawnPrefab.AddComponent<SteelBar>(); SetupScript.Copy((Shovel)script, item); break;
                default: return;
            }
            script.grabbable = true;
            script.isInFactory = true;
            script.grabbableToEnemies = true;
            script.itemProperties = item;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        void Awake()
        {
            instance = this;

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "premiumscraps");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);

            string directory = "Assets/Data/";

            var prefabs = new string[] { "Controller/ControlledAntena.prefab", "Controller/ControlledUI.prefab" };

            var audios = new string[] { "AirHorn1.ogg", "friendship_ends_here.wav", "scroll_tp.wav", "ShovelReelUp.ogg",
                "ShovelSwing.ogg", "wooden-staff-hit.wav", "MineTrigger.ogg", "book_page.wav", "CVuse1.wav", "CVuse2.wav",
                "CVuse3.wav", "CVuse4.wav", "TerminalAlarm.ogg", "Breathing.wav", "huh.wav", "book_use_redesign.wav",
                "uwu.wav", "uwu-rot.wav", "drink.wav", "spanishsound.wav", "arthas.wav", "glass-grab.wav", "glass-drop.wav",
                "beam.wav", "ControlModeStart.wav", "ControlModeStop.wav", "FlashlightOutOfBatteries.ogg", "ControlledOn.wav",
                "ControlledOff.wav", "controller-alert.wav", "LightningStrike2.ogg"
            };

            foreach (string prefab in prefabs)
            {
                gameObjects.Add(bundle.LoadAsset<GameObject>(directory + prefab));
            }

            foreach (string sfx in audios)
            {
                audioClips.Add(bundle.LoadAsset<AudioClip>(directory + "_audio/" + sfx));
            }

            var scraps = new List<Scrap> {
                new Scrap("Frieren/FrierenItem.asset", 10),
                new Scrap("Chocobo/ChocoboItem.asset", 10),
                new Scrap("AinzOoalGown/AinzOoalGownItem.asset", 5),
                new Scrap("HelmDomination/HelmDominationItem.asset", 11, 11),
                new Scrap("TheKing/TheKingItem.asset", 13),
                new Scrap("HarryMason/HarryMasonItem.asset", 10, 9),
                new Scrap("Cristal/CristalItem.asset", 10),
                new Scrap("PuppyShark/PuppySharkItem.asset", 10),
                new Scrap("Rupee/RupeeItem.asset", 15),
                new Scrap("EaNasir/EaNasirItem.asset", 9),
                new Scrap("HScard/HSCardItem.asset", 9),
                new Scrap("SODA/SODAItem.asset", 8),
                new Scrap("Spoon/SpoonItem.asset", 13),
                new Scrap("Crouton/CroutonItem.asset", 6),
                new Scrap("AirHornCustom/AirHornCustomItem.asset", 11, 1),
                new Scrap("Balan/BalanItem.asset", 10),
                new Scrap("CustomFace/CustomFaceItem.asset", 8, 2),
                new Scrap("Scroll/ScrollItem.asset", 7, 3),
                new Scrap("Stick/StickItem.asset", 9, 4),
                new Scrap("BookCustom/BookCustomItem.asset", 11, 5),
                new Scrap("SquareSteel/SquareSteelItem.asset", 7, 13),
                new Scrap("DarkJobApplication/JobApplicationItem.asset", 8, 6),
                new Scrap("Moogle/MoogleItem.asset", 10),
                new Scrap("Gazpacho/GazpachoItem.asset", 9, 7),
                new Scrap("Abi/AbiItem.asset", 4, 8),
                new Scrap("Bomb/BombItem.asset", 12, 10),
                new Scrap("Controller/ControllerItem.asset", 8, 12)
            };

            int i = 0; config = new Config(base.Config, scraps);
            config.SetupCustomConfigs();
            SetupScript.Network();

            foreach (Scrap scrap in scraps)
            {
                Item item = bundle.LoadAsset<Item>(directory + scrap.asset);
                if (config.scrapValues[i].Item1 != -1) { item.minValue = config.scrapValues[i].Item1; item.maxValue = config.scrapValues[i].Item2; }
                if (scrap.behaviourId != 0) LoadItemBehaviour(item, scrap.behaviourId);
                SpecialEvent.LoadSpecialEvent(item.spawnPrefab);
                NetworkPrefabs.RegisterNetworkPrefab(item.spawnPrefab);
                Utilities.FixMixerGroups(item.spawnPrefab);
                Items.RegisterScrap(item, config.entries[i++].Value, Levels.LevelTypes.All);
            }

            HarmonyPatchAll();
            Logger.LogInfo("PremiumScraps is loaded !");
        }
    }
}
