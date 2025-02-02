using PremiumScraps.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class LichKingHelm : PhysicsProp
    {
        public LichKingHelm() { }

        public override void GrabItem()
        {
            base.GrabItem();
            if (Random.Range(0, 100) >= 80)
                EffectServerRpc(20, transform.position, 0.95f, 0.6f, playerHeldBy != null && Effects.IsUnlucky(playerHeldBy.playerSteamId) && Random.Range(0, 10) >= 2);
        }

        [ServerRpc(RequireOwnership = false)]
        private void EffectServerRpc(int audioID, Vector3 position, float localVolume, float clientVolume = default, bool unlucky = false)
        {
            EffectClientRpc(audioID, position, localVolume, clientVolume == default ? localVolume : clientVolume, unlucky);
        }

        [ClientRpc]
        private void EffectClientRpc(int audioID, Vector3 position, float localVolume, float clientVolume, bool unlucky)
        {
            Effects.Audio(audioID, position, localVolume, clientVolume, playerHeldBy);
            if (unlucky && !StartOfRound.Instance.inShipPhase && !StartOfRound.Instance.shipIsLeaving)
                StartCoroutine(DoBoom());
        }

        private IEnumerator DoBoom()
        {
            yield return new WaitForSeconds(4);
            if (!StartOfRound.Instance.inShipPhase && !StartOfRound.Instance.shipIsLeaving)
                Effects.Explosion(transform.position, 1.5f);
        }
    }
}
