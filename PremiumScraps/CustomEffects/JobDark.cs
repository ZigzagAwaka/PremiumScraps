using GameNetcodeStuff;
using LethalNetworkAPI;
using PremiumScraps.Utils;
using System.Collections;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class JobDark : PhysicsProp
    {
        public int hallucinationID = 0;
        public int summonFriends = 0;
        public bool itsTooLate = false;
        public readonly float timeUntilItsTooLate = 8f;
        private Coroutine? darkEffectCoroutine;
        private bool OneTimeUse = false;
        private bool OneTimeActionSp = false;
        private readonly int debug = -1;  // force choose hallucination if not -1
        public LethalClientMessage<PosId> network;

        public JobDark()
        {
            network = new LethalClientMessage<PosId>(identifier: "premiumscrapsJobApplicationID");
            network.OnReceivedFromClient += NetworkEffect;
            SelectHallucination();
        }

        private void NetworkEffect(PosId posId, ulong clientId)
        {
            switch (posId.Id)
            {
                case 0: Effects.Spawn(GetEnemies.Girl, posId.position); break;
                case 1: Effects.Spawn(GetEnemies.Masked, posId.position); break;
                case 2: Effects.Message("Warning", "Abnormal amount of employees detected !", true); break;
                default: return;
            }
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                if (summonFriends >= 1 && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded)
                {
                    StartCoroutine(SummonFriends(playerHeldBy));
                }
            }
        }

        private IEnumerator SummonFriends(PlayerControllerB player)
        {
            Vector3 position;
            network.SendAllClients(new PosId(2, Vector3.up));
            summonFriends = -1;
            itemProperties.toolTips[0] = "";
            base.SetControlTipsForItem();
            yield return new WaitForEndOfFrame();
            if (StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion")
                position = RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length - 1)].transform.position;
            else
            {
                position = player.transform.position;
                for (int n = 0; n < 6; n++)
                {
                    if (StartOfRound.Instance.inShipPhase)
                        break;
                    yield return new WaitForSeconds(5);
                }
            }
            if (!StartOfRound.Instance.inShipPhase)
            {
                if (player.IsHost)
                    for (int i = 0; i < Effects.NbOfPlayers(); i++)
                        NetworkEffect(new PosId(1, position), 0);
                else
                    network.SendAllClients(new PosId(1, position), false);
            }
        }

        private void SelectHallucination()
        {
            if (debug == -1)
                hallucinationID = Random.Range(0, 4);
            else
                hallucinationID = debug;
        }

        private IEnumerator HazardHallucination(PlayerControllerB player)
        {
            if (!OneTimeActionSp)
            {
                yield return new WaitForSeconds(1);
                Effects.SpawnQuicksand(30);
                OneTimeActionSp = true;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                Vector3 position;
                if (player.isInsideFactory)
                    position = RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length - 1)].transform.position;
                else
                    position = RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position;
                Effects.ExplosionLight(position, 4f, 20);
            }
        }

        private IEnumerator GirlsHallucination(PlayerControllerB player)
        {
            OneTimeUse = true;
            if (!player.isInsideFactory)
                yield return new WaitUntil(() => player.isInsideFactory == true || StartOfRound.Instance.shipIsLeaving == true);
            if (!StartOfRound.Instance.shipIsLeaving && !player.isPlayerDead)
            {
                yield return new WaitForSeconds(5);
                if (!StartOfRound.Instance.shipIsLeaving && !StartOfRound.Instance.inShipPhase && !player.isPlayerDead)
                {
                    if (player.IsHost)
                        for (int i = 0; i < Effects.NbOfPlayers(); i++)
                            NetworkEffect(new PosId(0, player.transform.position), 0);
                    else
                        network.SendAllClients(new PosId(0, player.transform.position), false);
                }
            }
        }

        private IEnumerator HauntedHallucination(PlayerControllerB player)
        {
            yield return new WaitForSeconds(0.2f);
            if (player.isPlayerDead)
                yield return new WaitForSeconds(1);
            else if (player.isInHangarShipRoom || player.isInsideFactory)
            {
                yield return new WaitUntil(() => player.isInHangarShipRoom == false || player.isInsideFactory == false || StartOfRound.Instance.shipIsLeaving == true);
                for (int n = 0; n < 3; n++)
                {
                    if (StartOfRound.Instance.shipIsLeaving || StartOfRound.Instance.inShipPhase)
                        break;
                    yield return new WaitForSeconds(5);
                }
            }
            else
            {
                switch (Random.Range(0, 8))
                {
                    case 0:
                        HUDManager.Instance.RadiationWarningHUD();
                        yield return new WaitForSeconds(3);
                        player.beamUpParticle.Play();
                        yield return new WaitForSeconds(5);
                        break;
                    case 1:
                        Effects.Audio(new int[] { 8, 9, 10, 11 }, player.transform.position, 8f);
                        player.JumpToFearLevel(0.1f);
                        yield return new WaitForSeconds(2);
                        Effects.Audio(new int[] { 8, 9, 10, 11 }, player.transform.position, 9f);
                        player.JumpToFearLevel(2f);
                        player.playersManager.fearLevelIncreasing = false;
                        yield return new WaitForSeconds(15);
                        break;
                    case 2:
                        Effects.Explosion(player.transform.position, 0, 0);
                        yield return new WaitForSeconds(5);
                        break;
                    case 3:
                        Effects.Knockback(player.transform.position + Vector3.forward, 1, 0, 30);
                        yield return new WaitForSeconds(10);
                        Effects.Knockback(player.transform.position + Vector3.right, 1, 0, 40);
                        yield return new WaitForSeconds(10);
                        break;
                    case 4:
                        Effects.Audio(13, 2f);
                        player.drunkness = 1;
                        player.drunknessInertia = 1;
                        player.drunknessSpeed = 1;
                        player.JumpToFearLevel(1f);
                        player.playersManager.fearLevelIncreasing = false;
                        yield return new WaitForSeconds(15);
                        break;
                    case 5:
                        var original = player.movementSpeed;
                        player.movementSpeed = 0.2f;
                        yield return new WaitForSeconds(20);
                        player.movementSpeed = original;
                        yield return new WaitForSeconds(5);
                        break;
                    case 6:
                        Effects.Audio(11, 2f);
                        Effects.Teleportation(player, RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position, true);
                        yield return new WaitForSeconds(5);
                        break;
                    case 7:
                        Effects.Audio(9, 2f);
                        Effects.Teleportation(player, RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position, true);
                        yield return new WaitForSeconds(5);
                        break;
                    default:
                        yield return new WaitForSeconds(5);
                        break;
                }
            }
        }

        private IEnumerator DeathHallucination(PlayerControllerB player)
        {
            OneTimeUse = true;
            if (!player.isInsideFactory)
                yield return new WaitUntil(() => player.isInsideFactory == true || StartOfRound.Instance.shipIsLeaving == true);
            if (!StartOfRound.Instance.shipIsLeaving && !player.isPlayerDead)
            {
                yield return new WaitForSeconds(15);
                if (!StartOfRound.Instance.shipIsLeaving && !StartOfRound.Instance.inShipPhase && !player.isPlayerDead)
                {
                    player.JumpToFearLevel(1);
                    player.playersManager.fearLevelIncreasing = false;
                    yield return new WaitForSeconds(25);
                    if (!StartOfRound.Instance.shipIsLeaving && !StartOfRound.Instance.inShipPhase && !player.isPlayerDead)
                    {
                        Effects.Audio(new int[] { 8, 9, 10, 11 }, player.transform.position, 6f);
                        player.JumpToFearLevel(1.5f);
                        player.playersManager.fearLevelIncreasing = false;
                        yield return new WaitForSeconds(25);
                        if (!StartOfRound.Instance.shipIsLeaving && !StartOfRound.Instance.inShipPhase && !player.isPlayerDead)
                        {
                            var statusCoroutine = StartCoroutine(Effects.Status("WARNING ! UNSTABLE HEART RATE DETECTED. RETURN TO YOUR ASSIGNED SHIP IMMEDIATELY !"));
                            player.JumpToFearLevel(4);
                            player.playersManager.fearLevelIncreasing = true;
                            int i = 0;
                            for (i = 0; i < 5; i++)  // audio warning (55 s in total)
                            {
                                if (i == 4)
                                {
                                    for (int j = 0; j < 5; j++)  // the last 15 s
                                    {
                                        Effects.Audio(12, 10f);
                                        yield return new WaitForSeconds(3);
                                        if (StartOfRound.Instance.inShipPhase || player.isPlayerDead)
                                        { i = 0; break; }
                                        if (player.isInHangarShipRoom)
                                            break;
                                    }
                                }
                                else
                                {
                                    Effects.Audio(12, 10f);
                                    yield return new WaitForSeconds(10);
                                }
                                if (StartOfRound.Instance.inShipPhase || player.isPlayerDead)
                                { i = 0; break; }
                                if (player.isInHangarShipRoom)
                                    break;
                            }
                            StopCoroutine(statusCoroutine);
                            player.playersManager.fearLevelIncreasing = false;
                            yield return null;
                            if (i == 5 && !player.isInHangarShipRoom)
                            {
                                player.playersManager.fearLevel = 0;
                                if (player.IsHost)
                                    StartCoroutine(Effects.DamageHost(player, 100, CauseOfDeath.Inertia, (int)Effects.DeathAnimation.Haunted));  // death (host)
                                else
                                    Effects.Damage(player, 100, CauseOfDeath.Inertia, (int)Effects.DeathAnimation.Haunted);  // death
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator DarkEffect()
        {
            yield return new WaitForSeconds(timeUntilItsTooLate);
            playerHeldBy.JumpToFearLevel(1.5f);
            yield return new WaitForSeconds(timeUntilItsTooLate);
            itsTooLate = true;
            playerHeldBy.playersManager.fearLevelIncreasing = false;
            Effects.Message("The air feels different...", "Something terrible has been done to you", true);
            var player = playerHeldBy;
            Effects.DropItem(placingPosition: player.transform.position);
            grabbable = false;
            if (StartOfRound.Instance.inShipPhase)
                summonFriends = -1;
            yield return new WaitForSeconds(0.2f);
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (StartOfRound.Instance.shipIsLeaving || player.isPlayerDead)
                    yield return new WaitUntil(() => StartOfRound.Instance.inShipPhase == true);
                if (StartOfRound.Instance.inShipPhase)
                {
                    OneTimeUse = false;
                    OneTimeActionSp = false;
                    summonFriends++;
                    SelectHallucination();
                    if (summonFriends >= 1)
                    {
                        grabbable = true;
                        itemProperties.canBeInspected = false;
                        itemProperties.toolTips[0] = "Summon friends : [RMB]";
                        Effects.Message("You can finally meet your c̷̿̂o-̶̔͆w̴̿͜or̵͇̾k̴̹̂er̸̺͋s !",
                            "                                You should use the Job Application next time you're on a moon :)");
                    }
                    yield return new WaitUntil(() => StartOfRound.Instance.inShipPhase == false);
                }
                else if (player.isPlayerDead)
                    yield return new WaitUntil(() => StartOfRound.Instance.shipIsLeaving == true);
                else if (!OneTimeUse)
                {
                    switch (hallucinationID)
                    {
                        case 0: yield return HazardHallucination(player); break;
                        case 1: yield return GirlsHallucination(player); break;
                        case 2: yield return HauntedHallucination(player); break;
                        case 3: yield return DeathHallucination(player); break;
                        default: break;
                    }
                }
                else
                    yield return new WaitUntil(() => StartOfRound.Instance.shipIsLeaving == true);
            }
        }

        public override void InspectItem()
        {
            base.InspectItem();
            if (itemProperties.canBeInspected && IsOwner && playerHeldBy != null && !itsTooLate)
            {
                if (playerHeldBy.IsInspectingItem)
                {
                    darkEffectCoroutine = StartCoroutine(DarkEffect());
                }
                else
                {
                    StopInspect(playerHeldBy);
                }
            }
        }

        public override void PocketItem()
        {
            base.PocketItem();
            StopInspect(playerHeldBy, true);
        }

        public override void DiscardItem()
        {
            var player = playerHeldBy;
            base.DiscardItem();
            StopInspect(player, true);
        }

        private void StopInspect(PlayerControllerB player, bool fixHUD = false)
        {
            if (darkEffectCoroutine != null && !itsTooLate)
            {
                StopCoroutine(darkEffectCoroutine);
                player.playersManager.fearLevelIncreasing = false;
            }
            if (fixHUD)
                HUDManager.Instance.HideHUD(false);
        }
    }
}
