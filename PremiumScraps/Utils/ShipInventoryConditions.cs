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
            if ((item.itemProperties.name == "BombItem" && item is Bomb bomb && bomb.activated) ||
                (item.itemProperties.name == "ControllerItem" && item is Controller controller && controller.isInControlMode) ||
                (item.itemProperties.name == "JobApplicationItem" && item is JobDark) ||
                (item.itemProperties.name == "GazpachoItem" && item is SpanishDrink) ||
                (item.itemProperties.name == "BookCustomItem" && item is StupidBook stupidbook && stupidbook.nbFinish != 0))
                return false;
            return true;
        }
    }
}
