using PremiumScraps.Utils;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class StupidBook : PhysicsProp
    {
        public bool finish = false;
        public int nbFinish = 0;
        public int actualPage = -1;
        public List<string> pages;
        private readonly List<string> messages = new List<string>();

        public StupidBook()
        {
            Effects.FillMessagesFromLang(messages, new string[] {
                "BOOK_USAGE", "BOOK_END", "BOOK_END2", "LIAM_STORY", "LIAM_STORY2", "LIAM_STORY3",
                "LIAM_STORY4", "LIAM_STORY5", "LIAM_STORY6", "LIAM_STORY7" });
            pages = new List<string>() { messages[3], messages[4], messages[5], messages[6], messages[7], messages[8], messages[9] };
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsHost && !IsServer)
                SyncStateServerRpc();
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
            string[] allLines = ((actualPage != -1) ? new string[2] { messages[0], pages[actualPage] } : new string[2] { messages[0], "" });
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
                AudioServerRpc(7, playerHeldBy.transform.position, 0.9f, 0.65f);
                actualPage++;
                if (actualPage == 7)
                {
                    actualPage = -1;
                    finish = true;
                    nbFinish++;
                }
                SetControlTips();
                if (finish)
                {
                    if (nbFinish <= 4)
                    {
                        SpawnScrapServerRpc("SquareSteelItem", playerHeldBy.transform.position, nbFinish);
                        AudioServerRpc(15, playerHeldBy.transform.position, 1.5f, 0.75f);
                    }
                    else
                    {
                        Effects.Message(messages[1], messages[2]);
                    }
                    finish = false;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnScrapServerRpc(string scrapName, Vector3 position, int nbFinishLocal)
        {
            StartCoroutine(Effects.SyncScrap(Effects.Spawn(Effects.GetScrap(scrapName), position)));
            UpdateNbFinishClientRpc(nbFinishLocal);
        }

        [ClientRpc]
        private void UpdateNbFinishClientRpc(int nbFinishLocal)
        {
            nbFinish = nbFinishLocal;
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
        private void SyncStateServerRpc()
        {
            if (nbFinish != 0)
                UpdateNbFinishClientRpc(nbFinish);
        }
    }
}
