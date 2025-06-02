using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class TrollFace : PhysicsProp
    {
        public readonly int numberOfUse = 3;
        public int usage = 0;
        public bool unlucky = false;
        public bool StarlancerAIFix = false;
        private readonly List<string> messages = new List<string>();

        public TrollFace()
        {
            useCooldown = 3;
            StarlancerAIFix = Plugin.config.StarlancerAIFix;
            Effects.FillMessagesFromLang(messages, new string[] {
                "END_FRIENDSHIP", "END_FRIENDSHIP2", "TROLL_USAGE", "TROLL_USAGE2", "TROLL_INFO", "TROLL_INFO2",
                "TROLL_WARNING", "TROLL_WARNING2", "TROLL_FINAL" });
            customGrabTooltip = messages[0];
        }

        public override void Start()
        {
            base.Start();
            itemProperties.toolTips[0] = messages[1];
        }

        public override void GrabItem()
        {
            base.GrabItem();
            if (!unlucky && IsOwner && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded
                && playerHeldBy != null && Effects.IsUnlucky(playerHeldBy.playerSteamId))
            {
                if (Random.Range(0, 10) < 8)  // 80%
                    StartCoroutine(BadLuck(playerHeldBy));
            }
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                if (StartOfRound.Instance.inShipPhase || !StartOfRound.Instance.shipHasLanded || unlucky)
                {
                    Effects.Message(messages[2], messages[3]);
                    return;
                }
                AudioServerRpc(1, playerHeldBy.transform.position, 1.5f, 0.8f);
                if (playerHeldBy.health > 90)
                {
                    Effects.Damage(playerHeldBy, 10);
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
                    Effects.Message(messages[4], messages[5], true);
                }
                else if (playerHeldBy.health > 70 && playerHeldBy.health <= 90)
                {
                    Effects.Damage(playerHeldBy, 20);
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
                    Effects.Message(messages[6], messages[7], true);
                }
                else if (playerHeldBy.health > 20 && playerHeldBy.health <= 70)
                {
                    Effects.Damage(playerHeldBy, playerHeldBy.health - 10);
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
                    Effects.Message(messages[8], "", true);
                }
                else
                {
                    var playerTmp = playerHeldBy;
                    if (playerHeldBy.IsHost)
                        StartCoroutine(Effects.DamageHost(playerHeldBy, 100, CauseOfDeath.Strangulation, (int)Effects.DeathAnimation.NoHead1));
                    else
                        Effects.Damage(playerHeldBy, 100, CauseOfDeath.Strangulation, (int)Effects.DeathAnimation.NoHead1);
                    EndFriendshipServerRpc(playerTmp.transform.position, playerTmp.isInsideFactory);
                }
            }
        }

        private IEnumerator BadLuck(PlayerControllerB player)
        {
            unlucky = true;
            Effects.Message("bro ?", "", true);
            yield return new WaitForSeconds(5f);
            if (player.isPlayerDead || StartOfRound.Instance.shipIsLeaving)
                yield break;
            float effectTime = 8f;
            bool isInside = player.isInsideFactory;
            var position = player.transform.position;
            while (!player.isPlayerDead && !StartOfRound.Instance.shipIsLeaving)
            {
                if (!player.isPlayerDead && player.health - 10 <= 0)
                {
                    isInside = player.isInsideFactory;
                    position = player.transform.position;
                }
                Effects.Message(messages[8], "", true);
                Effects.Damage(player, 10, CauseOfDeath.Strangulation, (int)Effects.DeathAnimation.NoHead1);
                yield return new WaitForSeconds(effectTime);
                if (effectTime != 0.5f)
                    effectTime /= 2f;
            }
            EndFriendshipServerRpc(position, isInside);
            unlucky = false;
        }

        public override void Update()
        {
            base.Update();
            if (StartOfRound.Instance.inShipPhase && IsServer && usage != 0)
                usage = 0;
        }

        [ServerRpc(RequireOwnership = false)]
        private void EndFriendshipServerRpc(Vector3 position, bool isInsideFactory)
        {
            usage++;
            var spawnPosition = RoundManager.Instance.GetNavMeshPosition(position, sampleRadius: 3f);
            if (!RoundManager.Instance.GotNavMeshPositionResult)
                spawnPosition = Effects.GetClosestAINodePosition(isInsideFactory ? RoundManager.Instance.insideAINodes : RoundManager.Instance.outsideAINodes, position);
            if (usage > numberOfUse)
            {
                if (StarlancerAIFix || isInsideFactory)
                    Effects.Spawn(GetEnemies.CoilHead, spawnPosition);
                else
                    Effects.Spawn(GetEnemies.ForestKeeper, spawnPosition);
                return;
            }
            SpawnableEnemyWithRarity enemy;
            var i = Random.Range(0, 10);
            if (isInsideFactory)
            {
                if (!StarlancerAIFix)
                    enemy = GetEnemies.Maneater;
                else if (i <= 4)  // 50%
                    enemy = GetEnemies.ForestKeeper;
                else if (i == 5 || i == 6 || i == 7)  // 30%
                    enemy = GetEnemies.EyelessDog;
                else if (i == 8)  // 10%
                    enemy = GetEnemies.Tourist ?? GetEnemies.ForestKeeper;
                else  // 10%
                    enemy = GetEnemies.ShyGuy ?? GetEnemies.ForestKeeper;
            }
            else
            {
                if (!StarlancerAIFix || i == 0)  // 10%
                    enemy = GetEnemies.ForestKeeper;
                else if (i == 1 || i == 2)  // 20%
                    enemy = GetEnemies.Maneater;
                else if (i == 3 || i == 4)  // 20%
                    enemy = GetEnemies.Barber;
                else if (i == 5)  // 10%
                    enemy = GetEnemies.Bruce ?? GetEnemies.Nutcracker;
                else if (i == 6)  // 10%
                    enemy = GetEnemies.Nutcracker;
                else if (i == 7 || i == 8)  // 20%
                {
                    if (GetEnemies.SparkTower != null)
                    {
                        Effects.Spawn(GetEnemies.Maneater, spawnPosition);
                        Effects.Spawn(GetEnemies.ForestKeeper, spawnPosition);
                        for (int p = 0; p < 15; p++)
                            Effects.Spawn(GetEnemies.SparkTower, RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position);
                        return;
                    }
                    else
                        enemy = GetEnemies.Maneater;
                }
                else  // 10%
                {
                    if (GetEnemies.BigBertha != null)
                    {
                        Effects.Spawn(GetEnemies.Jester, spawnPosition);
                        Effects.Spawn(GetEnemies.BigBertha, spawnPosition);
                        Effects.Spawn(GetEnemies.BigBertha, StartOfRound.Instance.shipDoorNode.position);
                        for (int p = 0; p < 8; p++)
                            Effects.Spawn(GetEnemies.BigBertha, RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length - 1)].transform.position);
                        return;
                    }
                    else
                        enemy = GetEnemies.Jester;
                }
            }
            if (StarlancerAIFix && isInsideFactory && i == 8 && GetEnemies.Tourist != null)
                StartCoroutine(TouristSpawn(spawnPosition));
            else
            {
                for (int n = 0; n < 4; n++)
                {
                    Effects.Spawn(enemy, spawnPosition);
                }
                if (!isInsideFactory && (i == 0 || i == 3 || i == 4))
                    Effects.Spawn(GetEnemies.Kiwi, RoundManager.Instance.outsideAINodes[0].transform.position);
            }
        }

        private IEnumerator TouristSpawn(Vector3 spawnPosition)
        {
            for (int n = 0; n < 2; n++)
            {
                yield return new WaitForSeconds(5f);
                Effects.Spawn(GetEnemies.Tourist, spawnPosition);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void AudioServerRpc(int audioID, Vector3 clientPosition, float localVolume, float clientVolume = default)
        {
            AudioClientRpc(audioID, clientPosition, localVolume, clientVolume == default ? localVolume : clientVolume);
        }

        [ClientRpc]
        private void AudioClientRpc(int audioID, Vector3 clientPosition, float localVolume, float clientVolume)
        {
            Effects.Audio(audioID, clientPosition, localVolume, clientVolume, playerHeldBy);
        }
    }
}
