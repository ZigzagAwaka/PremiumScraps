using BepInEx;
using GameNetcodeStuff;
using PremiumScraps.CustomEffects;

namespace PremiumScraps.Utils
{
    internal class ShipInventoryConditions
    {
        public static void Setup(BepInPlugin inventoryMetadata)
        {
            if (new System.Version("1.2.2").CompareTo(inventoryMetadata.Version) <= 0)
            {
                ShipInventory.Helpers.InteractionHelper.AddCondition(PremiumScrapsCondition, "[Item not allowed]");
            }
        }

        private static bool PremiumScrapsCondition(PlayerControllerB player)
        {
            var item = player.currentlyHeldObjectServer;
            if ((item.itemProperties.name == "BombItem" && item is Bomb) ||
                (item.itemProperties.name == "ControllerItem" && item is Controller) ||
                (item.itemProperties.name == "JobApplicationItem" && item is JobDark) ||
                (item.itemProperties.name == "GazpachoItem" && item is SpanishDrink) ||
                (item.itemProperties.name == "ScrollItem" && item is ScrollTP) ||
                (item.itemProperties.name == "AbiItem" && item is TalkingBall) ||
                (item.itemProperties.name == "CustomFaceItem" && item is TrollFace))
                return false;
            return true;
        }
    }
}
