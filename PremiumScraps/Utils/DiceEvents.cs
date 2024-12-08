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
            if (diceMetadata.Name == "Emergency Dice Updated" && new System.Version("1.7.3").CompareTo(diceMetadata.Version) <= 0)
            {
                MysteryDice.MysteryDice.RegisterNewEffect(new Premium(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Haunted(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Death(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Hazards(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Music(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Academy(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Bombs(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Crouton(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Abi(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new HarryMason(), false);
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Zeldahu.LethalAnomalies"))
                    MysteryDice.MysteryDice.RegisterNewEffect(new SparkTowers(), false);
            }
            else
                logger.LogWarning("Compatibility with 'Emergency Dice Updated' was enabled but you are not using the targeted 1.7.4+ version. Custom events will not be loaded.");
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
                    case 0: yield return networker.HazardEffect(player); break;
                    case 2: yield return networker.HauntedEffect(player); break;
                    case 3: yield return networker.DeathEffect(player); break;
                    default: break;
                }
            }
        }

        public static IEnumerator HazardEffect(this Networker networker, PlayerControllerB player)
        {
            yield return CustomEffects.JobDark.HazardHallucination(player, null);
        }

        public static IEnumerator HauntedEffect(this Networker networker, PlayerControllerB player)
        {
            yield return CustomEffects.JobDark.HauntedHallucination(player);
        }

        public static IEnumerator DeathEffect(this Networker networker, PlayerControllerB player)
        {
            yield return CustomEffects.JobDark.DeathHallucination(player, null, networker);
        }

        public static IEnumerator HarryMasonCurse(this Networker networker)
        {
            yield return new WaitForSeconds(10f);
            var position = RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length - 1)].transform.position;
            networker.TeleportInsideServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, position);
            networker.TurnOffAllLightsServerRPC();
            Effects.Audio3D(1, position, 1.5f);
            networker.SameScrapServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, 8, "Harry Mason", true, position);
        }

        public static IEnumerator StartNoise(this Networker networker)
        {
            var position = RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length - 1)].transform.position;
            networker.SameScrapServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, 3, "crouton", true, position);
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(5f * i);
                networker.SpawnEnemyAtPosServerRPC("Hoarding bug", position);
            }
        }

        public static IEnumerator TheAcademyIsNowOpen(this Networker networker)
        {
            int nb = 0;
            bool surrounded = true;
            float waitTimeMultiplicator = 1f;
            while (!StartOfRound.Instance.shipIsLeaving && !StartOfRound.Instance.inShipPhase)
            {
                if (Plugin.config.StarlancerAIFix && surrounded && !GameNetworkManager.Instance.localPlayerController.isPlayerDead && !GameNetworkManager.Instance.localPlayerController.isInsideFactory)
                {
                    networker.MessageToEveryoneServerRPC("Welcome!", "Y̹͍͐o̴ͣͬur c̖ͤͯl̳̤͜as̈̀͡sͬ͘͠m̖ͯates are e̢ͮͥx̗̌̕cit̗̘̒eͩͅd to meet̳ͭͦ y̸̪̤ou͚̔̒");
                    networker.SpawnSurroundedServerRPC("Flowerman", 5, 3, true, Vector3.one * Random.Range(1.5f, 2.3f));
                    surrounded = false;
                }
                else if (!Plugin.config.StarlancerAIFix || nb >= 5)
                {
                    networker.SpawnEnemyAtPosServerRPC("Flowerman", RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length - 1)].transform.position);
                    waitTimeMultiplicator += 2f;
                }
                else if (nb <= 4)
                    networker.SpawnEnemyAtPosServerRPC("Flowerman", RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position);
                nb++;
                yield return new WaitForSeconds(waitTimeMultiplicator * nb);
            }
        }

        public static IEnumerator SparkWarning(this Networker networker)
        {
            yield return new WaitForSeconds(10f);
            if (GetEnemies.SparkTower != null && !StartOfRound.Instance.shipIsLeaving)
            {
                networker.MessageToEveryoneServerRPC("Plasma-powered radio transmitter online!", "");
                for (int i = 0; i < Random.Range(10, 14); i++)
                    networker.SpawnEnemyAtPosServerRPC("SparkTower", RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position);
            }
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
            var chosenPack = Random.Range(0, 3) switch
            {
                0 => "FunItemPack",
                1 => "CuteItempack",
                _ => "DangerousItempack"
            };
            var pack = new string[4];
            if (chosenPack == "FunItemPack")
                pack = new string[4] { "The King", "Harry Mason", "Balan Statue", "SODA" };
            else if (chosenPack == "CuteItempack")
                pack = new string[4] { "Frieren", "Chocobo", "Puppy Shark", "Moogle" };
            else if (chosenPack == "DangerousItempack")
                pack = new string[4] { "El Gazpacho", "Friendship ender", "Scroll of Town Portal", "Job application" };
            for (int i = 0; i < pack.Length; i++)
                Networker.Instance.SameScrapServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, 1, pack[i]);
        }
    }

    internal class Music : IEffect
    {
        public string Name => "Instrument of legends";
        public EffectType Outcome => EffectType.Great;
        public bool ShowDefaultTooltip => true;
        public string Tooltip => "Play a tune !";

        public void Use()
        {
            if (Effects.GetScrap("OcarinaItem") != null)
                Networker.Instance.SameScrapServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, Random.Range(2, 6), "Ocarina");
            else
                Networker.Instance.SameScrapServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, 6, "Clown horn");
        }
    }

    internal class Abi : IEffect
    {
        public string Name => "Abibaland";
        public EffectType Outcome => EffectType.Great;
        public bool ShowDefaultTooltip => true;
        public string Tooltip => "Are you pondering the orb?";

        public void Use()
        {
            for (int i = 0; i < 12; i++)
                Networker.Instance.SameScrapServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, 1, "The talking orb",
                    true, RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position);
        }
    }

    internal class Bombs : IEffect
    {
        public string Name => "Bombs infestation";
        public EffectType Outcome => EffectType.Bad;
        public bool ShowDefaultTooltip => false;
        public string Tooltip => "Quite unstable";

        public void Use()
        {
            for (int i = 0; i < 30; i++)
                Networker.Instance.SameScrapServerRPC(GameNetworkManager.Instance.localPlayerController.playerClientId, 1, "Bomb",
                    true, RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length - 1)].transform.position);
        }
    }

    internal class HarryMason : IEffect
    {
        public string Name => "Where is everybody?";
        public EffectType Outcome => EffectType.Mixed;
        public bool ShowDefaultTooltip => true;
        public string Tooltip => "Where is everybody?";

        public void Use()
        {
            Networker.Instance.StartCoroutine(Networker.Instance.HarryMasonCurse());
        }
    }

    internal class Crouton : IEffect
    {
        public string Name => "Disturbing noise";
        public EffectType Outcome => EffectType.Good;
        public bool ShowDefaultTooltip => true;
        public string Tooltip => "https://crouton.net";

        public void Use()
        {
            Networker.Instance.StartCoroutine(Networker.Instance.StartNoise());
        }
    }

    internal class Hazards : IEffect
    {
        public string Name => "Hazard hallucination";
        public EffectType Outcome => EffectType.Bad;
        public bool ShowDefaultTooltip => true;
        public string Tooltip => "This is not real";

        public void Use()
        {
            Effects.SpawnQuicksand(30);
            Networker.Instance.StartCoroutine(Networker.Instance.StartHallucination(GameNetworkManager.Instance.localPlayerController.playerClientId, 0));
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

    internal class Academy : IEffect
    {
        public string Name => "Flowerman Academy";
        public EffectType Outcome => EffectType.Awful;
        public bool ShowDefaultTooltip => false;
        public string Tooltip => "Y̹͍͐o̴ͣͬur c̖ͤͯl̳̤͜as̈̀͡sͬ͘͠m̖ͯates are e̢ͮͥx̗̌̕cit̗̘̒eͩͅd to meet̳ͭͦ y̸̪̤ou͚̔̒";

        public void Use()
        {
            Networker.Instance.StartCoroutine(Networker.Instance.TheAcademyIsNowOpen());
        }
    }

    internal class SparkTowers : IEffect
    {
        public string Name => "Towers";
        public EffectType Outcome => EffectType.Awful;
        public bool ShowDefaultTooltip => false;
        public string Tooltip => "Plasma-powered radio transmitter online!";

        public void Use()
        {
            Networker.Instance.StartCoroutine(Networker.Instance.SparkWarning());
        }
    }
}
