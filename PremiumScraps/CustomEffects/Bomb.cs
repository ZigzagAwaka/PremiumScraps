﻿using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class Bomb : ThrowableItem
    {
        public bool isBeeingActivated = false;
        public bool activated = false;
        public AudioSource? alarm;
        public ParticleSystem? bombParticle;
        public ParticleSystem? smokeParticle;
        public Animator? bombAnimator;
        private Coroutine? activateCoroutine;

        public Bomb() { }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            alarm = transform.GetChild(5).GetComponent<AudioSource>();
            bombParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
            smokeParticle = transform.GetChild(4).GetComponent<ParticleSystem>();
            bombAnimator = transform.GetChild(0).GetComponent<Animator>();
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (isBeeingActivated)
                return;
            if (!activated && activateCoroutine == null)
            {
                activateCoroutine = StartCoroutine(ActivatingBomb());  // activate bomb
            }
            else if (IsOwner)
            {
                base.ItemActivate(used, buttonDown);  // throw bomb
            }
        }

        public override void DiscardItem()
        {
            if (!isBeeingActivated)
            {
                bool unlucky = Effects.IsUnlucky(playerHeldBy != null ? playerHeldBy.playerSteamId : 0);
                base.DiscardItem();
                if (!activated && (Random.Range(0, 100) >= 95 || (unlucky && Random.Range(0, 100) >= 20)))  // 5% (or 80% if unlucky)
                    BombExplosionUnstableServerRpc();
            }
        }

        public override void ActivatePhysicsTrigger(Collider other)
        {
            base.ActivatePhysicsTrigger(other);
            bool unlucky = false;
            if (other.tag == "Player")
            {
                var player = other.gameObject.GetComponent<PlayerControllerB>();
                unlucky = Effects.IsUnlucky(player != null ? player.playerSteamId : 0);
            }
            if (!activated && (Random.Range(0, 100) >= 97 || (unlucky && Random.Range(0, 100) >= 30)))  // 3% (or 70% if unlucky)
                BombExplosionUnstableServerRpc();
        }

        public static bool ChargeItemUnstable()  // used by harmony patch
        {
            GrabbableObject currentlyHeldObjectServer = GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer;
            if (currentlyHeldObjectServer == null)
            {
                return false;
            }
            if (currentlyHeldObjectServer.itemProperties.name == "BombItem" && currentlyHeldObjectServer is Bomb bomb && !bomb.activated)
            {
                bomb.BombExplosionUnstableServerRpc();
                return false;
            }
            return true;
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
            string[] allLines = ((!activated) ? new string[1] { "Activate bomb : [RMB]" } : new string[1] { "Throw bomb : [RMB]" });
            if (IsOwner)
            {
                HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.ChangeControlTipMultiple(allLines, holdingItem: true, itemProperties);
            }
        }

        private IEnumerator ActivatingBomb()
        {
            isBeeingActivated = true;
            playerHeldBy.activatingItem = true;
            playerHeldBy.doingUpperBodyEmote = 1.16f;
            playerHeldBy.playerBodyAnimator.SetTrigger("PullGrenadePin");
            yield return new WaitForSeconds(1f);
            if (playerHeldBy != null)
                playerHeldBy.activatingItem = false;
            isBeeingActivated = false;
            activated = true;
            itemUsedUp = true;
            if (IsOwner && playerHeldBy != null)
            {
                SetControlTips();
            }
            StartCoroutine(BombExplosion());
        }

        private IEnumerator BombExplosion()
        {
            alarm?.Play();
            bombParticle?.Play();
            smokeParticle?.Play();
            bombAnimator?.SetTrigger("CaVaPeter");  // bomb turning red
            yield return new WaitForSeconds(2f);
            bombAnimator?.SetFloat("SpeedMult", 4);
            yield return new WaitForSeconds(1f);
            bombAnimator?.SetFloat("SpeedMult", 8);
            yield return new WaitForSeconds(1.2f);
            bombParticle?.Stop();
            smokeParticle?.Stop();
            if (playerHeldBy != null && !playerHeldBy.isPlayerDead && isPocketed)  // not a good idea to put a bomb in your pocket
                playerHeldBy.DropAllHeldItems();
            var position = transform.position;
            DestroyObjectInHand(playerHeldBy != null && isHeld ? playerHeldBy : null);
            Effects.Explosion(position, 2f, 90, 5);
            yield return new WaitForSeconds(0.15f);
            Effects.Explosion(position, 4f, 100, 20);
        }

        [ServerRpc(RequireOwnership = false)]
        private void BombExplosionUnstableServerRpc()
        {
            BombExplosionUnstableClientRpc();
        }

        [ClientRpc]
        private void BombExplosionUnstableClientRpc()
        {
            activated = true;
            itemUsedUp = true;
            grabbable = false;
            grabbableToEnemies = false;
            StartCoroutine(BombExplosionUnstable(this));
        }

        private IEnumerator BombExplosionUnstable(Bomb bomb)
        {
            if (bomb.alarm != null)
            {
                bomb.alarm.clip = Plugin.audioClips[6];
                bomb.alarm.volume = 2f;
                bomb.alarm.Play();  // landmine audio
            }
            bombAnimator?.SetTrigger("CaVaPeterMaintenant");  // bomb turning red directly
            yield return new WaitForSeconds(0.8f);
            if (bomb.playerHeldBy != null && !bomb.playerHeldBy.isPlayerDead && bomb.isPocketed)  // not a good idea to put a bomb in your pocket
                bomb.playerHeldBy.DropAllHeldItems();
            var position = bomb.transform.position;
            bomb.DestroyObjectInHand(bomb.playerHeldBy != null && bomb.isHeld ? bomb.playerHeldBy : null);
            Effects.Explosion(position, 2f, 90, 5);
            yield return new WaitForSeconds(0.15f);
            Effects.Explosion(position, 4f, 100, 20);
        }
    }
}
