using PremiumScraps.Utils;
using System.Collections.Generic;

namespace PremiumScraps.CustomEffects
{
    internal class StupidBook : PhysicsProp
    {
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

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                Effects.Audio(7, playerHeldBy.transform.position, 2f);
                if (actualPage == 6)
                {
                    actualPage = -1;
                    itemProperties.toolTips[^1] = "";
                }
                else
                {
                    actualPage++;
                    itemProperties.toolTips[^1] = pages[actualPage];
                }
                base.SetControlTipsForItem();
                //var items = UnityEngine.Resources.FindObjectsOfTypeAll<Item>().ToList();
                //Effects.Spawn(items.FirstOrDefault(i => i.name.Equals("Shotgun")), playerHeldBy.transform.position);
            }
        }
    }
}
