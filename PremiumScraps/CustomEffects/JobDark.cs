using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class JobDark : PhysicsProp
    {
        public int hallucinationID = 0;
        public int summonFriends = 0;
        public bool canInspectPaper = true;
        public bool itsTooLate = false;
        public float timeUntilItsTooLate = 7f;
        private Coroutine? darkEffectCoroutine;
        private bool OneTimeUse = false;
        private bool OneTimeActionSp = false;
        private bool isUnlucky = false;
        private readonly int debug = -1;  // force choose hallucination if not -1

        public JobDark() { }

        public override void Start()
        {
            base.Start();
            SelectHallucination();
        }

        public override void EquipItem()
        {
            SetControlTips();
            EnableItemMeshes(enable: true);
            isPocketed = false;
            if (!hasBeenHeld)
            {
                hasBeenHeld = true;
                if (!isInShipRoom && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.currentLevel.spawnEnemiesAndScrap)
                {
                    RoundManager.Instance.valueOfFoundScrapItems += scrapValue;
                }
            }
        }

        public override void SetControlTipsForItem()
        {
            SetControlTips();
        }

        private void SetControlTips()
        {
            string[] allLines;
            if (summonFriends == -1)
                allLines = new string[1] { "" };
            else if (summonFriends >= 1)
                allLines = new string[1] { "Summon friends : [RMB]" };
            else
                allLines = new string[1] { "Read: [Z]" };
            if (IsOwner)
            {
                HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.ChangeControlTipMultiple(allLines, holdingItem: true, itemProperties);
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
            DarkJobEffectServerRpc(2, Vector3.up);
            summonFriends = -1;
            SetControlTips();
            yield return new WaitForEndOfFrame();
            if (RoundManager.Instance.dungeonGenerator != null)
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
                DarkJobEffectServerRpc(0, position);
            }
        }

        private void SelectHallucination()
        {
            if (debug == -1)
                hallucinationID = Random.Range(0, 4);
            else
                hallucinationID = debug;
        }

        public static IEnumerator HazardHallucination(PlayerControllerB player, JobDark? jobDark)
        {
            if (jobDark != null && !jobDark.OneTimeActionSp)
            {
                yield return new WaitForSeconds(1);
                Effects.SpawnQuicksand(30);
                jobDark.OneTimeActionSp = true;
            }
            else
            {
                yield return new WaitForSeconds(jobDark != null && jobDark.isUnlucky ? 0.38f : 0.5f);
                Vector3 position;
                if (player.isInsideFactory)
                    position = RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length - 1)].transform.position;
                else
                {
                    position = RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position;
                    if (Random.Range(0, 100) >= 80)
                        Effects.SpawnLightningBolt(RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position);
                }
                Effects.ExplosionLight(position, 4f, 20);
            }
        }

        public static IEnumerator GirlsHallucination(PlayerControllerB player, JobDark? jobDark)
        {
            if (jobDark != null)
            {
                jobDark.OneTimeUse = true;
                if (!player.isInsideFactory)
                    yield return new WaitUntil(() => player.isInsideFactory == true || StartOfRound.Instance.shipIsLeaving == true);
                if (!StartOfRound.Instance.shipIsLeaving && !player.isPlayerDead)
                {
                    yield return new WaitForSeconds(5);
                    if (!StartOfRound.Instance.shipIsLeaving && !StartOfRound.Instance.inShipPhase && !player.isPlayerDead)
                    {
                        jobDark.DarkJobEffectServerRpc(1, player.transform.position, Effects.IsUnlucky(player.playerSteamId));
                    }
                }
            }
        }

        public static IEnumerator HauntedHallucination(PlayerControllerB player)
        {
            yield return new WaitForSeconds(0.2f);
            if (player.isPlayerDead)
                yield return new WaitForSeconds(1);
            else if (player.isInHangarShipRoom || player.isInsideFactory)
            {
                yield return new WaitUntil(() => (player.isInHangarShipRoom == false && player.isInsideFactory == false) || StartOfRound.Instance.shipIsLeaving == true);
                for (int n = 0; n < (Effects.IsUnlucky(player.playerSteamId) ? 0 : 3); n++)
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
                        Effects.Audio(new int[] { 8, 9, 10, 11 }, player.transform.position, 7f);
                        player.JumpToFearLevel(0.1f);
                        yield return new WaitForSeconds(2);
                        Effects.Audio(new int[] { 8, 9, 10, 11 }, player.transform.position, 8f);
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
                        Effects.Audio(13, 1.8f);
                        player.drunkness = 1;
                        player.drunknessInertia = 1;
                        player.drunknessSpeed = 1;
                        player.JumpToFearLevel(1f);
                        player.playersManager.fearLevelIncreasing = false;
                        yield return new WaitForSeconds(15);
                        break;
                    case 5:
                        var original = player.movementSpeed;
                        player.movementSpeed = 0.25f;
                        yield return new WaitForSeconds(15);
                        player.movementSpeed = original;
                        yield return new WaitForSeconds(5);
                        break;
                    case 6:
                        Effects.Audio(11, 2f);
                        Effects.Teleportation(player, RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position);
                        yield return new WaitForSeconds(5);
                        break;
                    case 7:
                        Effects.Audio(9, 2f);
                        Effects.Teleportation(player, RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position);
                        yield return new WaitForSeconds(5);
                        break;
                    default:
                        yield return new WaitForSeconds(5);
                        break;
                }
            }
        }

        public static IEnumerator DeathHallucination(PlayerControllerB player, JobDark? jobDark, MonoBehaviour? monoBehaviour = null)
        {
            if (jobDark != null)
                jobDark.OneTimeUse = true;
            else if (monoBehaviour == null)
                yield break;
            var unlucky = jobDark != null ? jobDark.isUnlucky : Effects.IsUnlucky(player.playerSteamId);
            if (!player.isInsideFactory)
                yield return new WaitUntil(() => player.isInsideFactory == true || StartOfRound.Instance.shipIsLeaving == true);
            if (!StartOfRound.Instance.shipIsLeaving && !player.isPlayerDead)
            {
                yield return new WaitForSeconds(unlucky ? 5 : 15);
                if (!StartOfRound.Instance.shipIsLeaving && !StartOfRound.Instance.inShipPhase && !player.isPlayerDead)
                {
                    player.JumpToFearLevel(1);
                    player.playersManager.fearLevelIncreasing = false;
                    yield return new WaitForSeconds(25);
                    if (!StartOfRound.Instance.shipIsLeaving && !StartOfRound.Instance.inShipPhase && !player.isPlayerDead)
                    {
                        Effects.Audio(new int[] { 8, 9, 10, 11 }, player.transform.position, 5f);
                        player.JumpToFearLevel(1.5f);
                        player.playersManager.fearLevelIncreasing = false;
                        yield return new WaitForSeconds(25);
                        if (!StartOfRound.Instance.shipIsLeaving && !StartOfRound.Instance.inShipPhase && !player.isPlayerDead)
                        {
                            var instance = jobDark != null ? jobDark : monoBehaviour;
                            var statusCoroutine = instance.StartCoroutine(Effects.Status("WARNING ! UNSTABLE HEART RATE DETECTED. RETURN TO YOUR ASSIGNED SHIP IMMEDIATELY !"));
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
                                    yield return new WaitForSeconds(unlucky ? 7.5f : 10f);
                                }
                                if (StartOfRound.Instance.inShipPhase || player.isPlayerDead)
                                { i = 0; break; }
                                if (player.isInHangarShipRoom)
                                    break;
                            }
                            instance.StopCoroutine(statusCoroutine);
                            player.playersManager.fearLevelIncreasing = false;
                            yield return null;
                            if (i == 5 && !player.isInHangarShipRoom)
                            {
                                player.playersManager.fearLevel = 0;
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
            Effects.DropItem(player.transform.position);
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
                        canInspectPaper = false;
                        if (playerHeldBy != null && !isPocketed)
                            SetControlTips();
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
                        case 0: yield return HazardHallucination(player, this); break;
                        case 1: yield return GirlsHallucination(player, this); break;
                        case 2: yield return HauntedHallucination(player); break;
                        case 3: yield return DeathHallucination(player, this); break;
                        default: break;
                    }
                }
                else
                    yield return new WaitUntil(() => StartOfRound.Instance.shipIsLeaving == true);
            }
        }

        private IEnumerator UnluckyDarkEffect()
        {
            timeUntilItsTooLate = 0f;
            if (playerHeldBy == null || playerHeldBy.isPlayerDead)
                yield break;
            while (playerHeldBy.isGrabbingObjectAnimation)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return DarkEffect();
        }

        public override void GrabItem()
        {
            base.GrabItem();
            if (playerHeldBy != null && IsOwner)
                isUnlucky = Effects.IsUnlucky(playerHeldBy.playerSteamId);
            if (!itsTooLate && canInspectPaper && itemProperties.canBeInspected && IsOwner &&
                playerHeldBy != null && isUnlucky)
            {
                if (Random.Range(0, 10) < 8)  // 80%
                    darkEffectCoroutine = StartCoroutine(UnluckyDarkEffect());
            }
            else if (itsTooLate && IsOwner && playerHeldBy != null && isUnlucky
                && summonFriends >= 1 && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded)
            {
                if (Random.Range(0, 10) < 8)  // 80%
                    StartCoroutine(SummonFriends(playerHeldBy));
            }
        }

        public override void InspectItem()
        {
            base.InspectItem();
            if (itemProperties.canBeInspected && IsOwner && playerHeldBy != null)
            {
                if (canInspectPaper && !itsTooLate)
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
                else if (!playerHeldBy.IsInspectingItem)
                {
                    if (summonFriends >= 1 && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded)
                        StartCoroutine(SummonFriends(playerHeldBy));
                    else
                        Effects.Message("You are already cursed", "", true);
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

        public override void OnNetworkDespawn()
        {
            if (darkEffectCoroutine != null)
                StopCoroutine(darkEffectCoroutine);
            if (playerHeldBy != null)
                playerHeldBy.playersManager.fearLevelIncreasing = false;
            base.OnNetworkDespawn();
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

        [ServerRpc(RequireOwnership = false)]
        private void DarkJobEffectServerRpc(int type, Vector3 position, bool unlucky = false)
        {
            switch (type)
            {
                case 0:
                    for (int i = 0; i < Effects.NbOfPlayers() + 1; i++)
                        Effects.Spawn(GetEnemies.Masked, position);
                    break;
                case 1:
                    for (int i = 0; i < (unlucky ? 12 : 6); i++)
                        Effects.Spawn(GetEnemies.GhostGirl, position);
                    break;
                case 2: DarkJobEffectType2ClientRpc(); break;
                default: return;
            }
        }

        [ClientRpc]
        private void DarkJobEffectType2ClientRpc()
        {
            Effects.AddCombinedWeather(LevelWeatherType.Eclipsed);
            Effects.Message("Warning", "Abnormal amount of employees detected !", true);
            summonFriends = -1;
        }
    }
}
