using BepInEx.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PremiumScraps.Utils
{
    internal static class Lang
    {
        public static string ACTUAL_LANG = "en";
        private static readonly Dictionary<string, string> langValues = new Dictionary<string, string>();

        public static string Get(string id) => langValues.GetValueOrDefault(id, id);

        public static void Load(ManualLogSource logger, SystemLanguage systemLanguage, bool changeLanguageMode)
        {
            if ((systemLanguage == SystemLanguage.French && !changeLanguageMode) || (systemLanguage != SystemLanguage.French && changeLanguageMode))
            {
                ACTUAL_LANG = "fr";
            }
            string langDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"Lang/lang-{ACTUAL_LANG}.json");
            if (!File.Exists(langDir))
            {
                logger.LogError("Languade file (" + langDir + ") was not found. Please re-download the mod properly.");
                return;
            }
            var jobj = JObject.Parse(File.ReadAllText(langDir));
            langValues.Clear();
            foreach (var (key, value) in jobj)
                langValues[key] = value?.ToString() ?? key;
        }
    }
}
