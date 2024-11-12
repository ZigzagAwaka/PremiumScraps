using BepInEx.Configuration;
using PremiumScraps.Utils;
using System.Collections.Generic;

namespace PremiumScraps
{
    class Config
    {
        public readonly ConfigEntry<bool> diceEvents;
        public readonly List<ConfigEntry<int>> entries = new List<ConfigEntry<int>>();

        public Config(ConfigFile cfg, List<Scrap> scraps)
        {
            cfg.SaveOnConfigSet = false;
            diceEvents = cfg.Bind("General", "Dice events", true, "Adds some custom dice rolls to Emergency Dice items. Requires 'Emergency Dice Updated' 1.6.1+ to work, or else it will be automatically false.");
            foreach (Scrap scrap in scraps)
            {
                entries.Add(cfg.Bind("Spawn chance", scrap.asset.Split("/")[0], scrap.rarity));
            }
            cfg.Save();
            cfg.SaveOnConfigSet = true;
        }
    }
}
