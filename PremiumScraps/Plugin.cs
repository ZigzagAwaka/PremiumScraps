using BepInEx;
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
    [BepInDependency("LethalNetworkAPI")]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "zigzag.premiumscraps";
        const string NAME = "PremiumScraps";
        const string VERSION = "1.7.3";

        public static Plugin instance;
        public static List<AudioClip> audioClips;
        private readonly Harmony harmony = new Harmony(GUID);
        internal static Config config { get; private set; } = null!;

        void LoadItemBehaviour(Item item, int behaviourId)
        {
            PhysicsProp script;
            switch (behaviourId)
            {
                case 1: script = item.spawnPrefab.AddComponent<FakeAirhorn>(); break;
                case 2: script = item.spawnPrefab.AddComponent<TrollFace>(); break;
                case 3: script = item.spawnPrefab.AddComponent<ScrollTP>(); break;
                case 4: script = item.spawnPrefab.AddComponent<LegendaryStick>(); break;
                case 5: script = item.spawnPrefab.AddComponent<StupidBook>(); break;
                default: return;
            }
            script.grabbable = true;
            script.grabbableToEnemies = true;
            script.itemProperties = item;
        }

        void Awake()
        {
            instance = this;

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "premiumscraps");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);

            string directory = "Assets/Data/";

            audioClips = new List<AudioClip> {
                bundle.LoadAsset<AudioClip>(directory + "_audio/AirHorn1.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/friendship_ends_here.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/scroll_tp.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/ShovelReelUp.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/ShovelSwing.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/wooden-staff-hit.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/MineTrigger.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/book_page.wav")
            };

            var scraps = new List<Scrap> {
                new Scrap("Frieren/FrierenItem.asset", 10),
                new Scrap("Chocobo/ChocoboItem.asset", 10),
                new Scrap("AinzOoalGown/AinzOoalGownItem.asset", 5),
                new Scrap("HelmDomination/HelmDominationItem.asset", 11),
                new Scrap("TheKing/TheKingItem.asset", 13),
                new Scrap("HarryMason/HarryMasonItem.asset", 10),
                new Scrap("Cristal/CristalItem.asset", 9),
                new Scrap("PuppyShark/PuppySharkItem.asset", 10),
                new Scrap("Rupee/RupeeItem.asset", 15),
                new Scrap("EaNasir/EaNasirItem.asset", 8),
                new Scrap("HScard/HSCardItem.asset", 10),
                new Scrap("SODA/SODAItem.asset", 8),
                new Scrap("Spoon/SpoonItem.asset", 12),
                new Scrap("Crouton/CroutonItem.asset", 6),
                new Scrap("AirHornCustom/AirHornCustomItem.asset", 9, 1),
                new Scrap("Balan/BalanItem.asset", 10),
                new Scrap("CustomFace/CustomFaceItem.asset", 8, 2),
                new Scrap("Scroll/ScrollItem.asset", 6, 3),
                new Scrap("Stick/StickItem.asset", 10, 4),
                new Scrap("BookCustom/BookCustomItem.asset", 11, 5)
            };

            int i = 0; config = new Config(base.Config, scraps);

            foreach (Scrap scrap in scraps)
            {
                Item item = bundle.LoadAsset<Item>(directory + scrap.asset);
                if (scrap.behaviourId != 0) LoadItemBehaviour(item, scrap.behaviourId);
                NetworkPrefabs.RegisterNetworkPrefab(item.spawnPrefab);
                Utilities.FixMixerGroups(item.spawnPrefab);
                Items.RegisterScrap(item, config.entries[i++].Value, Levels.LevelTypes.All);

                //// TEST
                /*                TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
                                node.clearPreviousText = true;
                                node.displayText = "test";
                                Items.RegisterShopItem(item, itemInfo: node, price: 0);*/
            }

            harmony.PatchAll();
            Logger.LogInfo("PremiumScraps is loaded !");
        }
    }
}
