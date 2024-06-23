using BepInEx.Configuration;
using PremiumScraps.Utils;
using System.Collections.Generic;

namespace PremiumScraps
{
    class Config
    {
        public readonly List<ConfigEntry<int>> entries = new List<ConfigEntry<int>>();

        public Config(ConfigFile cfg, List<Scrap> scraps)
        {
            cfg.SaveOnConfigSet = false;
            foreach (Scrap scrap in scraps)
            {
                entries.Add(cfg.Bind("Spawn chance", scrap.asset.Split("/")[0], scrap.rarity));
            }
            cfg.Save();
            cfg.SaveOnConfigSet = true;
        }
    }
}
