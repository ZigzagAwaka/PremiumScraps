using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class EvilCard : PhysicsProp
    {
        public bool specialSfxReady = true;
        public int dropSfxId = 0;
        public AudioSource? itemAudio;

        public EvilCard() { }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            itemAudio = transform.GetComponent<AudioSource>();
            if (Lang.ACTUAL_LANG == "fr")
            {
                itemProperties.grabSFX = Plugin.audioClips[35];
                itemProperties.dropSFX = Plugin.audioClips[36];
            }
            if (!IsHost && !IsServer)
                SyncItemServerRpc();
        }

        public override void GrabItem()
        {
            base.GrabItem();
            dropSfxId = Random.Range(0, 2);
            itemProperties.dropSFX = Plugin.audioClips[32 + dropSfxId + (Lang.ACTUAL_LANG == "fr" ? 4 : 0)];
            SyncItemServerRpc(true, dropSfxId);
        }

        public override void InspectItem()
        {
            base.InspectItem();
            if (itemProperties.canBeInspected && IsOwner && playerHeldBy != null && !playerHeldBy.IsInspectingItem && specialSfxReady
                && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded && !StartOfRound.Instance.shipIsLeaving)
            {
                var unlucky = Effects.IsUnlucky(playerHeldBy.playerSteamId) && Random.Range(0, 10) <= 7;  // unlucky 80%
                if (unlucky)
                    BadEffectServerRpc(playerHeldBy.playerClientId, playerHeldBy.isInsideFactory);
            }
        }

        public override void Update()
        {
            base.Update();
            if (StartOfRound.Instance.inShipPhase && !specialSfxReady)
                specialSfxReady = true;
        }

        [ServerRpc(RequireOwnership = false)]
        private void BadEffectServerRpc(ulong playerId, bool insideFactory)
        {
            BadEffectClientRpc(playerId, insideFactory);
        }

        [ClientRpc]
        private void BadEffectClientRpc(ulong playerId, bool insideFactory)
        {
            specialSfxReady = false;
            StartCoroutine(PlaySpecialSfx(playerId, insideFactory));
        }

        private IEnumerator PlaySpecialSfx(ulong playerId, bool insideFactory)
        {
            itemAudio?.PlayOneShot(Plugin.audioClips[34], 1.2f);
            yield return new WaitForSeconds(1.8f);
            if (StartOfRound.Instance.inShipPhase)
                yield break;
            Effects.ExplosionLight(transform.position, 4f);
            if (StartOfRound.Instance.currentLevel.PlanetName == "71 Gordion")
            {
                yield return new WaitForSeconds(1f);
                if (StartOfRound.Instance.inShipPhase)
                    yield break;
                Effects.Explosion(transform.position, 2.2f);
                yield break;
            }
            if (IsServer || IsHost)
            {
                yield return new WaitForSeconds(0.1f);
                var spawnPosition = RoundManager.Instance.GetNavMeshPosition(transform.position, sampleRadius: 5f);
                if (!RoundManager.Instance.GotNavMeshPositionResult)
                    spawnPosition = Effects.GetClosestAINodePosition(insideFactory ? RoundManager.Instance.insideAINodes : RoundManager.Instance.outsideAINodes, transform.position);
                if (insideFactory)
                    Effects.SpawnMaskedOfPlayer(playerId, spawnPosition);
                for (int i = 0; i < (insideFactory ? 2 : 3); i++)
                    Effects.Spawn(insideFactory ? GetEnemies.BunkerSpider : GetEnemies.BaboonHawk, spawnPosition);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncItemServerRpc(bool overrideId = false, int id = 0)
        {
            SyncItemClientRpc(overrideId ? id : dropSfxId);
        }

        [ClientRpc]
        private void SyncItemClientRpc(int id)
        {
            dropSfxId = id;
            itemProperties.dropSFX = Plugin.audioClips[32 + dropSfxId + (Lang.ACTUAL_LANG == "fr" ? 4 : 0)];
        }
    }
}
