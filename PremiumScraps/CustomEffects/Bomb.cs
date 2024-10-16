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
        private Coroutine? activateCoroutine;

        public Bomb() { }

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
                base.DiscardItem();
                if (!activated && Random.Range(0, 100) >= 95)  // 5%
                    BombExplosionUnstableServerRpc();
            }
        }

        public override void ActivatePhysicsTrigger(Collider other)
        {
            base.ActivatePhysicsTrigger(other);
            if (!activated && Random.Range(0, 100) >= 97)  // 3%
                BombExplosionUnstableServerRpc();
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
            transform.GetChild(1).GetComponent<ParticleSystem>().Play();  // bomb particle
            GetComponent<AudioSource>().Play();  // bomb alarm
            yield return new WaitForSeconds(4.2f);
            transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
            if (playerHeldBy != null && !playerHeldBy.isPlayerDead && isPocketed)  // not a good idea to put a bomb in your pocket
                playerHeldBy.DropAllHeldItems();
            var position = transform.position;
            DestroyObjectInHand(playerHeldBy != null && isHeld ? playerHeldBy : null);
            Effects.Explosion(position, 2f, 90, 5);
            yield return new WaitForSeconds(0.15f);
            Effects.Explosion(position, 4f, 100, 20);
        }

        [ServerRpc(RequireOwnership = false)]
        public void BombExplosionUnstableServerRpc()
        {
            BombExplosionUnstableClientRpc();
        }

        [ClientRpc]
        private void BombExplosionUnstableClientRpc()
        {
            activated = true;
            itemUsedUp = true;
            grabbable = false;
            StartCoroutine(BombExplosionUnstable(this));
        }

        private IEnumerator BombExplosionUnstable(GrabbableObject bomb)
        {
            var audio = bomb.GetComponent<AudioSource>();
            audio.clip = Plugin.audioClips[6];
            audio.volume = 2f;
            audio.Play();  // landmine audio
            yield return new WaitForSeconds(0.8f);
            if (bomb.playerHeldBy != null && !bomb.playerHeldBy.isPlayerDead && bomb.isPocketed)  // not a good idea to put a bomb in your pocket
                bomb.playerHeldBy.DropAllHeldItemsAndSync();
            var position = bomb.transform.position;
            bomb.DestroyObjectInHand(bomb.playerHeldBy != null && bomb.isHeld ? bomb.playerHeldBy : null);
            Effects.Explosion(position, 2f, 90, 5);
            yield return new WaitForSeconds(0.15f);
            Effects.Explosion(position, 4f, 100, 20);
        }
    }
}
