using BepInEx;

namespace PremiumScraps.Utils
{
    internal class SSSConditions
    {
        public static void Setup(BepInPlugin sssMetaData)
        {
            if (new System.Version("1.0.0").CompareTo(sssMetaData.Version) <= 0)
            {
                SelfSortingStorage.Cupboard.SmartCupboard.AddTriggerValidation(ShipInventoryConditions.PremiumScrapsCondition, "[Item not allowed]");
            }
        }
    }
}
