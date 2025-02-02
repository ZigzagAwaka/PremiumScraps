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
        public readonly List<ulong> unluckyPlayersID = new List<ulong>();
        public readonly List<(int, int)> scrapValues = new List<(int, int)>();
        public readonly ConfigEntry<bool> diceEvents;
        public readonly ConfigEntry<string> unluckyPlayersStr;
        public readonly ConfigEntry<bool> squareSteelWeapon;
        public readonly List<ConfigEntry<int>> entries = new List<ConfigEntry<int>>();
        public readonly List<ConfigEntry<string>> values = new List<ConfigEntry<string>>();

        public Config(ConfigFile cfg, List<Scrap> scraps)
        {
            cfg.SaveOnConfigSet = false;
            diceEvents = cfg.Bind("General", "Dice events", true, "Adds some custom dice rolls to Emergency Dice items. Requires 'Emergency Dice Updated' 1.7.4+ to work, or else it will be automatically false.");
            unluckyPlayersStr = cfg.Bind("General", "Unlucky players", "76561198984467725,76561199094139351,76561198198881967,76561198002410826", "Comma separated list of players Steam ID that you want them to be unlucky. Bad things will happen to unlucky players, use this config to take a sweet revenge on your friends...");
            squareSteelWeapon = cfg.Bind("Items", "Square Steel weapon", true, "Turns Galvanized Square Steel into a usable weapon.");
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
