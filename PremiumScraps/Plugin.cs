using BepInEx;
using HarmonyLib;
using LethalLib.Modules;
using PremiumScraps.CustomEffects;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace PremiumScraps
{
    public class Scrap
    {
        public string asset;
        public int rarity;
        public int behaviourId;
        public Scrap(string asset, int rarity) : this(asset, rarity, 0) { }
        public Scrap(string asset, int rarity, int behaviourId)
        {
            this.asset = asset;
            this.rarity = rarity;
            this.behaviourId = behaviourId;
        }
    }


    [BepInDependency("LethalNetworkAPI")]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "zigzag.premiumscraps";
        const string NAME = "PremiumScraps";
        const string VERSION = "1.7.0";

        public static Plugin instance;
        public static List<AudioClip> sounds;
        private readonly Harmony harmony = new Harmony(GUID);
        internal static Config config { get; private set; } = null!;

        void LoadItemBehaviour(Item item, int behaviourId)
        {
            PhysicsProp script;
            switch (behaviourId)
            {
                case 1: script = item.spawnPrefab.AddComponent<SoundExplosion>(); break;
                case 2: script = item.spawnPrefab.AddComponent<Troll>(); break;
                case 3: script = item.spawnPrefab.AddComponent<Teleportation>(); break;
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

            sounds = new List<AudioClip> {
                bundle.LoadAsset<AudioClip>(directory + "_audio/AirHorn1.ogg"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/friendship_ends_here.wav"),
                bundle.LoadAsset<AudioClip>(directory + "_audio/scroll_tp.wav")
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
                new Scrap("AirHornCustom/AirHornCustomItem.asset", 8, 1),
                new Scrap("Balan/BalanItem.asset", 10),
                new Scrap("CustomFace/CustomFaceItem.asset", 8, 2),
                new Scrap("Scroll/ScrollItem.asset", 8, 3)
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
                TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
                node.clearPreviousText = true;
                node.displayText = "test";
                Items.RegisterShopItem(item, null, null, node, 0);
            }

            harmony.PatchAll();
            Logger.LogInfo("PremiumScraps is loaded !");
        }
    }
}
