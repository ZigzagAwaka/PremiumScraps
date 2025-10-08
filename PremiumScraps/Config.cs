using BepInEx.Configuration;
using PremiumScraps.Utils;
using System.Collections.Generic;
using System.Linq;

namespace PremiumScraps
{
    class Config
    {
        public bool StarlancerAIFix = false;
        public bool WeatherRegistery = false;
        public bool OpenBodyCams = false;
        public readonly List<ulong> unluckyPlayersID = new List<ulong>();
        public readonly List<(int, int)> scrapValues = new List<(int, int)>();
        public readonly ConfigEntry<bool> diceEvents;
        public readonly ConfigEntry<string> languageMode;
        public readonly ConfigEntry<string> unluckyPlayersStr;
        public readonly ConfigEntry<bool> gazpachoMemeSfx;
        public readonly ConfigEntry<bool> squareSteelWeapon;
        public readonly ConfigEntry<bool> controllerBodyCams;
        public readonly ConfigEntry<float> ronkaVolumeMultiplier;
        public readonly List<ConfigEntry<int>> entries = new List<ConfigEntry<int>>();
        public readonly List<ConfigEntry<string>> values = new List<ConfigEntry<string>>();

        public Config(ConfigFile cfg, List<Scrap> scraps)
        {
            cfg.SaveOnConfigSet = false;
            diceEvents = cfg.Bind("General", "Dice events", true, "Adds some custom dice rolls to Emergency Dice items.\nRequires 'Emergency Dice Updated' 1.9.25+ to work, or else it will be automatically false.");
            languageMode = cfg.Bind("General", "Language mode", "default", new ConfigDescription("Change the language of this mod (translate text, tool tips and change some sounds). The 'default' value is automatically assigned to your system language.", new AcceptableValueList<string>("default", "english", "french")));
            unluckyPlayersStr = cfg.Bind("General", "Unlucky players", "76561198984467725,76561198198881967,76561198195911589", "Comma separated list of players Steam ID that you want them to be unlucky.\nBad things will happen to unlucky players, use this config to take a sweet revenge on your friends...");
            gazpachoMemeSfx = cfg.Bind("Items", "Gazpacho meme sfx", true, "Turns El Gazpacho's grab and drop sfx to memes sounds.\nWill be automatically false if the chosen language is not french.");
            squareSteelWeapon = cfg.Bind("Items", "Square Steel weapon", true, "Turns Galvanized Square Steel into a usable weapon.");
            controllerBodyCams = cfg.Bind("Items", "Controller Body Cams", true, "Upgrade the Controller screen with a camera from OpenBodyCams mod.\nWill be automatically false if OpenBodyCams is not installed.");
            ronkaVolumeMultiplier = cfg.Bind("Items", "Ronka volume multiplier", 1f, new ConfigDescription("Change the volume of Ronka's 'scree' sound effects.", new AcceptableValueRange<float>(0f, 1f)));
            foreach (Scrap scrap in scraps)
            {
                entries.Add(cfg.Bind("Spawn chance", scrap.asset.Split("/")[0], scrap.rarity, "Rarity of the item."));
                values.Add(cfg.Bind("Values", scrap.asset.Split("/")[0], "", "Min,max value of the item, follow the format 200,300 or empty for default.\nIn-game value will be randomized between these numbers and divided by 2.5."));
            }
            cfg.Save();
            cfg.SaveOnConfigSet = true;
        }

        public void SetupCustomConfigs()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("AudioKnight.StarlancerAIFix"))
            {
                StarlancerAIFix = true;
            }
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("mrov.WeatherRegistry"))
            {
                WeatherRegistery = true;
            }
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Zaggy1024.OpenBodyCams"))
            {
                OpenBodyCams = true;
            }
            foreach (string playerId in unluckyPlayersStr.Value.Split(',').Select(s => s.Trim()))
            {
                if (!ulong.TryParse(playerId, out var id))
                    continue;
                unluckyPlayersID.Add(id);
            }
            foreach (var value in values)
            {
                if (value.Value == "")
                { scrapValues.Add((-1, -1)); continue; }
                var valueTab = value.Value.Split(',').Select(s => s.Trim()).ToArray();
                if (valueTab.Count() != 2)
                { scrapValues.Add((-1, -1)); continue; }
                if (!int.TryParse(valueTab[0], out var minV) || !int.TryParse(valueTab[1], out var maxV))
                { scrapValues.Add((-1, -1)); continue; }
                if (minV > maxV)
                { scrapValues.Add((-1, -1)); continue; }
                scrapValues.Add((minV, maxV));
            }
        }
    }
}
