using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class GreatRonka : PhysicsProp
    {
        public AudioSource? itemAudio;
        public AudioClip? scree1;
        public AudioClip? scree2;
        public Animator? screeAnimator;
        public bool isInIdleScree = true;
        public Coroutine? idleScreeCoroutine;
        public AudioSource? idleScreeAudio;
        public ScanNodeProperties? scanNode;
        public float feelingLonelyTime;
        public int actualVariantID = 0;

        public GreatRonka() { }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            itemAudio = transform.GetComponent<AudioSource>();
            idleScreeAudio = transform.GetChild(1).GetComponent<AudioSource>();
            screeAnimator = transform.GetComponent<Animator>();
            scanNode = transform.GetChild(0).GetComponent<ScanNodeProperties>();
            scree1 = itemProperties.grabSFX;
            scree2 = itemProperties.dropSFX;
            if (IsHost || IsServer)
            {
                if (idleScreeCoroutine != null)
                    StopCoroutine(idleScreeCoroutine);
                idleScreeCoroutine = StartCoroutine(IdleScree());
                ChooseVariantServerRpc();
            }
            else
                SyncItemServerRpc(false);
        }

        private IEnumerator IdleScree()
        {
            yield return new WaitForSeconds(0.5f);
            while (true)
            {
                RonkaAudioServerRpc(true);
                yield return new WaitForSeconds(Random.Range(3f, 13f));
            }
        }

        public override void GrabItem()
        {
            base.GrabItem();
            if (isInIdleScree)
                SyncItemServerRpc(true);
        }

        public override void Update()
        {
            base.Update();
            if ((IsHost || IsServer) && !isInIdleScree && !isHeld)
            {
                feelingLonelyTime += Time.deltaTime;
                if (feelingLonelyTime >= 30f)
                {
                    if (Random.Range(0, 3) != 0)
                        RonkaAudioServerRpc(true);
                    feelingLonelyTime = 0;
                }
            }
            else if ((IsHost || IsServer) && !isInIdleScree && isHeld)
                feelingLonelyTime = 0;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && !isInIdleScree && playerHeldBy != null && itemAudio != null && !itemAudio.isPlaying)
            {
                RonkaAudioServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RonkaAudioServerRpc(bool inIdle = false)
        {
            RonkaAudioClientRpc(Random.Range(0, 2), inIdle);
        }

        [ClientRpc]
        private void RonkaAudioClientRpc(int selectedAudio, bool inIdle)
        {
            if (GameNetworkManager.Instance.localPlayerController == null || itemAudio == null || idleScreeAudio == null)
                return;
            float pitch = Random.Range(inIdle ? 0.75f : 0.93f, 1f);
            var audioClip = selectedAudio == 0 ? scree1 : scree2;
            screeAnimator?.SetTrigger("Scree" + (selectedAudio + 1));
            if (!inIdle)
            {
                float volume = Random.Range(0.9f, 1f);
                itemAudio.pitch = pitch;
                itemAudio.PlayOneShot(audioClip, volume);
                WalkieTalkie.TransmitOneShotAudio(itemAudio, audioClip, volume);
                RoundManager.Instance.PlayAudibleNoise(transform.position, 20, volume, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
                if (playerHeldBy != null)
                    playerHeldBy.timeSinceMakingLoudNoise = 0f;
            }
            else
            {
                idleScreeAudio.maxDistance = isInFactory ? 55 : 40;
                idleScreeAudio.pitch = pitch;
                idleScreeAudio.PlayOneShot(audioClip);
                WalkieTalkie.TransmitOneShotAudio(idleScreeAudio, audioClip, 0.5f);
                RoundManager.Instance.PlayAudibleNoise(transform.position, 50, 0.5f, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChooseVariantServerRpc()
        {
            ChooseVariantClientRpc(Random.Range(0, 5));
        }

        [ClientRpc]
        private void ChooseVariantClientRpc(int variantID)
        {
            actualVariantID = variantID;
            if (actualVariantID == 0)
                return;
            transform.GetChild(2 + actualVariantID)?.gameObject?.SetActive(true);
            if (actualVariantID != 3)
                transform.GetChild(2)?.gameObject?.SetActive(false);
            if (scanNode == null)
                return;
            switch (actualVariantID)
            {
                case 1: scanNode.headerText = "Behatted Serpent of Ronka"; break;
                case 2: scanNode.headerText = "Behelmeted Serpent of Ronka"; break;
                case 3: scanNode.headerText = "Greatest Serpent of Tural"; break;
                case 4: scanNode.headerText = "Great White Tsuchinoko"; break;
                default: break;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncItemServerRpc(bool overrideStop)
        {
            SyncItemClientRpc(!overrideStop && isInIdleScree);
            if (overrideStop && idleScreeCoroutine != null)
                StopCoroutine(idleScreeCoroutine);
            else if (!overrideStop)
                ChooseVariantClientRpc(actualVariantID);
        }

        [ClientRpc]
        private void SyncItemClientRpc(bool idleValue)
        {
            isInIdleScree = idleValue;
        }
    }
}
