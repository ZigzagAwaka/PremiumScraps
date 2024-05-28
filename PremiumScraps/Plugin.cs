using BepInEx;
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


    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "zigzag.premiumscraps";
        const string NAME = "PremiumScraps";
        const string VERSION = "1.5.0";

        public static Plugin instance;

        void loadBehaviour(Item item, int behaviourId)
        {
            switch (behaviourId)
            {
                case 1:
                    {
                        Explosion script = item.spawnPrefab.AddComponent<Explosion>();
                        script.grabbable = true;
                        script.grabbableToEnemies = true;
                        script.itemProperties = item;
                        break;
                    }
                default: return;
            }
        }

        void Awake()
        {
            instance = this;

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "premiumscraps");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);

            string directory = "Assets/Data/";

            var scraps = new List<Scrap> {
                new Scrap("Frieren/FrierenItem.asset", 10),
                new Scrap("Chocobo/ChocoboItem.asset", 10),
                new Scrap("AinzOoalGown/AinzOoalGownItem.asset", 5),
                new Scrap("HelmDomination/HelmDominationItem.asset", 11),
                new Scrap("TheKing/TheKingItem.asset", 13),
                new Scrap("HarryMason/HarryMasonItem.asset", 10),
                new Scrap("Cristal/CristalItem.asset", 7),
                new Scrap("PuppyShark/PuppySharkItem.asset", 10),
                new Scrap("Rupee/RupeeItem.asset", 15),
                new Scrap("EaNasir/EaNasirItem.asset", 7),
                new Scrap("HScard/HSCardItem.asset", 8),
                new Scrap("SODA/SODAItem.asset", 10),
                new Scrap("Spoon/SpoonItem.asset", 11),
                new Scrap("Crouton/CroutonItem.asset", 5),
                new Scrap("AirHornCustom/AirHornCustomItem.asset", 7, 1),
                new Scrap("Balan/BalanItem.asset", 10)
            };

            foreach (Scrap scrap in scraps)
            {
                Item item = bundle.LoadAsset<Item>(directory + scrap.asset);
                if (scrap.behaviourId != 0) loadBehaviour(item, scrap.behaviourId);
                NetworkPrefabs.RegisterNetworkPrefab(item.spawnPrefab);
                Utilities.FixMixerGroups(item.spawnPrefab);
                Items.RegisterScrap(item, scrap.rarity, Levels.LevelTypes.All);

                //// TEST
                TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
                node.clearPreviousText = true;
                node.displayText = "test";
                Items.RegisterShopItem(item, null, null, node, 0);
            }

            Logger.LogInfo("PremiumScraps is loaded !");
        }
    }
}
