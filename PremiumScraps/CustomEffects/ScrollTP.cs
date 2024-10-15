using PremiumScraps.Utils;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class ScrollTP : PhysicsProp
    {
        public ScrollTP() { }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                if (StartOfRound.Instance.inShipPhase)
                {
                    Effects.Message("Can't be used at the moment", "");
                    return;
                }
                if (playerHeldBy.isInElevator && playerHeldBy.isInHangarShipRoom)
                {
                    Effects.Message("Wait", "You are already in the ship?");
                    return;
                }
                Effects.Teleportation(playerHeldBy, StartOfRound.Instance.middleOfShipNode.position, true);
                AudioServerRpc(2, playerHeldBy.transform.position, 1.5f, 2.5f);
                DestroyObjectServerRpc(StartOfRound.Instance.localPlayerController.playerClientId);
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

        [ServerRpc(RequireOwnership = false)]
        private void DestroyObjectServerRpc(ulong playerID)
        {
            DestroyObjectClientRpc(playerID);
        }

        [ClientRpc]
        private void DestroyObjectClientRpc(ulong playerID)
        {
            DestroyObjectInHand(StartOfRound.Instance.allPlayerScripts[playerID]);
        }
    }
}
