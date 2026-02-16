using PremiumScraps.Utils;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class ScrollTP : PhysicsProp
    {
        public Light? light;

        public ScrollTP() { }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            light = transform.GetChild(0).GetComponent<Light>();
        }

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
                var previousPos = playerHeldBy.transform.position;
                TeleportationServerRpc(playerHeldBy.playerClientId, true, false, false);
                AudioServerRpc(2, previousPos, 1.2f, 0.9f);
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
            if (playerHeldBy != null && GameNetworkManager.Instance.localPlayerController.playerClientId == playerHeldBy.playerClientId)
                Effects.Audio(audioID, localVolume);
            else
            {
                Effects.Audio3D(audioID, clientPosition, clientVolume);
                Effects.Audio3D(audioID, StartOfRound.Instance.middleOfShipNode.position, clientVolume);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void TeleportationServerRpc(ulong playerID, bool ship, bool exterior, bool interior)
        {
            TeleportationClientRpc(playerID, ship, exterior, interior);
        }

        [ClientRpc]
        private void TeleportationClientRpc(ulong playerID, bool ship, bool exterior, bool interior)
        {
            Effects.TeleportationLocal(playerHeldBy, StartOfRound.Instance.middleOfShipNode.position);
            Effects.SetPosFlags(playerID, ship, exterior, interior);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroyObjectServerRpc(ulong playerID)
        {
            DestroyObjectClientRpc(playerID);
        }

        [ClientRpc]
        private void DestroyObjectClientRpc(ulong playerID)
        {
            if (light != null)
            {
                light.enabled = false;
            }
            DestroyObjectInHand(StartOfRound.Instance.allPlayerScripts[playerID]);
        }
    }
}
