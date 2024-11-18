using BepInEx.Configuration;
using PremiumScraps.Utils;
using System.Collections.Generic;
using System.Linq;

namespace PremiumScraps
{
    class Config
    {
        public readonly List<ulong> unluckyPlayersID = new List<ulong>();
        public readonly ConfigEntry<bool> diceEvents;
        public readonly ConfigEntry<string> unluckyPlayersStr;
        public readonly List<ConfigEntry<int>> entries = new List<ConfigEntry<int>>();

        public Config(ConfigFile cfg, List<Scrap> scraps)
        {
            cfg.SaveOnConfigSet = false;
            diceEvents = cfg.Bind("General", "Dice events", true, "Adds some custom dice rolls to Emergency Dice items. Requires 'Emergency Dice Updated' 1.6.1+ to work, or else it will be automatically false.");
            unluckyPlayersStr = cfg.Bind("General", "Unlucky players", "76561198984467725,76561199094139351,76561198198881967", "Comma separated list of players Steam ID that you want them to be unlucky. Bad things will happen to unlucky players, use this config to take a sweet revenge on your friends...");
            foreach (Scrap scrap in scraps)
            {
                entries.Add(cfg.Bind("Spawn chance", scrap.asset.Split("/")[0], scrap.rarity));
            }
            cfg.Save();
            cfg.SaveOnConfigSet = true;
        }

        public void SetupCustomConfigs()
        {
            foreach (string playerId in unluckyPlayersStr.Value.Split(',').Select(s => s.Trim()))
            {
                ulong id;
                if (!ulong.TryParse(playerId, out id))
                    continue;
                unluckyPlayersID.Add(id);
            }
        }
    }
}
