using BepInEx;
using GameNetcodeStuff;
using PremiumScraps.CustomEffects;

namespace PremiumScraps.Utils
{
    internal class SSSConditions
    {
        public static void Setup(BepInPlugin sssMetaData)
        {
            if (new System.Version("1.0.0").CompareTo(sssMetaData.Version) <= 0)
            {
                SelfSortingStorage.Cupboard.SmartCupboard.AddTriggerValidation(PremiumScrapsCondition, "[Item not allowed]");
            }
        }

        private static bool PremiumScrapsCondition(PlayerControllerB player)
        {
            var item = player.currentlyHeldObjectServer;
            if ((item.itemProperties.name == "BombItem" && item is Bomb bomb && bomb.activated) ||
                (item.itemProperties.name == "ControllerItem" && item is Controller controller && controller.isInControlMode) ||
                (item.itemProperties.name == "JobApplicationItem" && item is JobDark) ||
                (item.itemProperties.name == "GazpachoItem" && item is SpanishDrink))
                return false;
            return true;
        }
    }
}
