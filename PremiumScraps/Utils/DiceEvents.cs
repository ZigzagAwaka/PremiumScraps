using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using MysteryDice;
using MysteryDice.Effects;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.Utils
{
    internal class DiceEvents
    {
        public static void RegisterDiceEvents(ManualLogSource logger, BepInPlugin diceMetadata)
        {
            if (diceMetadata.Name == "Emergency Dice Updated" && new System.Version("1.6.1").CompareTo(diceMetadata.Version) <= 0)
            {
                MysteryDice.MysteryDice.RegisterNewEffect(new Premium(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Haunted(), false);
                MysteryDice.MysteryDice.RegisterNewEffect(new Death(), false);
            }
            else
                logger.LogWarning("Compatibility with 'Emergency Dice Updated' was enabled but you are not using the targeted 1.6.1+ version. Custom events will not be loaded.");
        }
    }

    public static class NetworkerExtensions
    {
        [ServerRpc(RequireOwnership = false)]
        public static void SpawnScrapsServerRpc(this Networker networker, string[] scraps, Vector3 position, int amount = 1)
        {
            for (int i = 0; i < scraps.Length; i++)
                for (int j = 0; j < amount; j++)
                    networker.StartCoroutine(Effects.SyncScrap(Effects.Spawn(Effects.GetScrap(scraps[i]), position)));
        }

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
                0 => "TheKingItem",
                1 => "HarryMasonItem",
                2 => "RupeeItem",
                3 => "SpoonItem",
                4 => "CroutonItem",
                5 => "BalanItem",
                6 => "StickItem",
                7 => "AbiItem",
                8 => "BombItem",
                9 => "CuteItempack",
                _ => "DangerousItempack"
            };
            if (chosenScrap == "CuteItempack")
                Networker.Instance.SpawnScrapsServerRpc(new string[4] { "FrierenItem", "ChocoboItem", "PuppySharkItem", "MoogleItem" }, GameNetworkManager.Instance.localPlayerController.transform.position);
            else if (chosenScrap == "DangerousItempack")
                Networker.Instance.SpawnScrapsServerRpc(new string[4] { "AirHornCustomItem", "CustomFaceItem", "ScrollItem", "JobApplicationItem" }, GameNetworkManager.Instance.localPlayerController.transform.position);
            else
                Networker.Instance.SpawnScrapsServerRpc(new string[1] { chosenScrap }, GameNetworkManager.Instance.localPlayerController.transform.position, Random.Range(2, 5));
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
