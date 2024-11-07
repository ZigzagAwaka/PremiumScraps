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
        public List<string> pages = new List<string> {
            "How to design a 0.1 square meter apartment into a functional house? liam worked hard for 10 years in new york and finally saved up to buy this tiny 0.1 square meter apartment. every night he had to tie himself to the door with steel wire to sleep. eventually, it broke down and needed an absolute redesign.",
            "firstly, he welded a frame from galvanized square steel, and borrowed some expansion screws from his aunt to secure it to the wall. he covered it with wood veneers durable for 10,000 years and installed large floor to ceiling windows made of broken bridge aluminium for a stylish look.",
            "then he added a big fluffy mattress so he and his girlfriend could comfortably sleep together, and even have space for a baby. he installed a special alarm because he struggles to get up in the morning so lets god decide. below, he built a bedside table, installed a socket and set up a projector for reading books.",
            "attached a soft padding around the bed for extra comfort. and placed a folding table at the end of the bed to use as a workspace. he can also sit there to relax and fish, turning his catch into a hearty meal. next, liam built a cabinet against the wall and a retractable card holder below it.",
            "the small chair doubles as a bed. he added a folding dining table so the whole family can enjoy meals together, and save space when folded. all the familys clothes hang in the wardrobe. next, he installed a modular cabinet by the door with a waterproof pool on one side to create a mini kitchen.",
            "he placed an induction cooker nearby with a mirror cabinet above and compartments below for seasonings and toiletries. a wall mounted toilet is installed next to the door, perfect for sitting comfortably and taking a shower. liam hung an overhead curtain at the end of the bed to watch korean dramas daily.",
            "with this setup, even limited space can offer unlimited possibilities."
        };

        public StupidBook() { }

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
            string[] allLines = ((actualPage != -1) ? new string[2] { "Read book : [RMB]", pages[actualPage] } : new string[2] { "Read book : [RMB]", "" });
            if (IsOwner)
            {
                HUDManager.Instance.ChangeControlTipMultiple(allLines, holdingItem: true, itemProperties);
            }
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                AudioServerRpc(7, playerHeldBy.transform.position, 1f, 2f);
                actualPage++;
                if (actualPage == 7)
                {
                    actualPage = -1;
                    if (!StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded && StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion")
                    {
                        finish = true;
                        nbFinish++;
                    }
                }
                SetControlTips();
                if (finish)
                {
                    if (nbFinish <= 4)
                    {
                        SpawnScrapServerRpc("SquareSteelItem", playerHeldBy.transform.position, nbFinish);
                        AudioServerRpc(15, playerHeldBy.transform.position, 1.8f, 3f);
                    }
                    else
                    {
                        Effects.Message("Unworthy", "Get back to work !");
                    }
                    finish = false;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnScrapServerRpc(string scrapName, Vector3 position, int nbFinishLocal)
        {
            Effects.Spawn(scrapName, position);
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
    }
}
