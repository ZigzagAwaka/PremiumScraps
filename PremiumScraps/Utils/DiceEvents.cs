using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using MysteryDice;
using MysteryDice.Effects;
using System.Collections;
using UnityEngine;

namespace PremiumScraps.Utils
{
    internal class DiceEvents
    {
        public static void RegisterDiceEvents(ManualLogSource logger, BepInPlugin diceMetadata)
        {
            if (diceMetadata.Name == "Emergency Dice Updated" && new System.Version("1.6.4").CompareTo(diceMetadata.Version) <= 0)
            {
                MysteryDice.MysteryDice.RegisterNewEffect(new Premium(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Haunted(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Death(), false);
            }
            else
                logger.LogWarning("Compatibility with 'Emergency Dice Updated' was enabled but you are not using the targeted 1.6.5+ version. Custom events will not be loaded.");
        }
    }

    public static class NetworkerExtensions
    {
        public static IEnumerator StartHallucination(this Networker networker, ulong playerId, int hallucinationID)
        {
            var player = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (StartOfRound.Instance.shipIsLeaving || StartOfRound.Instance.inShipPhase || player == null || player.isPlayerDead)
                    break;
                switch (hallucinationID)
                {
                    case 2: yield return networker.HauntedEffect(player); break;
                    case 3: yield return networker.DeathEffect(player); break;
                    default: break;
                }
            }
        }

        public static IEnumerator HauntedEffect(this Networker networker, PlayerControllerB player)
        {
            yield return CustomEffects.JobDark.HauntedHallucination(player);
        }

        public static IEnumerator DeathEffect(this Networker networker, PlayerControllerB player)
        {
            yield return CustomEffects.JobDark.DeathHallucination(player, null, networker);
        }
    }

    internal class Premium : IEffect
    {
        public string Name => "Premium Scraps";
        public EffectType Outcome => EffectType.Good;
        public bool ShowDefaultTooltip => true;
        public string Tooltip => "Spawning some nice scraps !";

        public void Use()
        {
            var chosenScrap = Random.Range(0, 11) switch
            {
                0 => "The King",
                1 => "Harry Mason",
                2 => "Rupee",
                3 => "Comically Large Spoon",
                4 => "crouton",
                5 => "Balan Statue",
                6 => "Stick",
                7 => "The talking orb",
                8 => "Bomb",
                9 => "CuteItempack",
                _ => "DangerousItempack"
            };
            if (chosenScrap == "CuteItempack" || chosenScrap == "DangerousItempack")
            {
                var pack = new string[4];
                if (chosenScrap == "CuteItempack")
                    pack = new string[4] { "Frieren", "Chocobo", "Puppy Shark", "Moogle" };
                else if (chosenScrap == "DangerousItempack")
                    pack = new string[4] { "El Gazpacho", "Friendship ender", "Scroll of Town Portal", "Job application" };
                for (int i = 0; i < pack.Length; i++)
                    Networker.Instance.SameScrapServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, 1, pack[i]);
            }
            else
                Networker.Instance.SameScrapServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, Random.Range(2, 5), chosenScrap);
        }
    }

    internal class Haunted : IEffect
    {
        public string Name => "Haunted hallucination";
        public EffectType Outcome => EffectType.Bad;
        public bool ShowDefaultTooltip => true;
        public string Tooltip => "You are now cursed";

        public void Use()
        {
            Networker.Instance.StartCoroutine(Networker.Instance.StartHallucination(GameNetworkManager.Instance.localPlayerController.playerClientId, 2));
        }
    }

    internal class Death : IEffect
    {
        public string Name => "Death hallucination";
        public EffectType Outcome => EffectType.Awful;
        public bool ShowDefaultTooltip => true;
        public string Tooltip => "The air feels different...";

        public void Use()
        {
            Networker.Instance.StartCoroutine(Networker.Instance.StartHallucination(GameNetworkManager.Instance.localPlayerController.playerClientId, 3));
        }
    }
}
