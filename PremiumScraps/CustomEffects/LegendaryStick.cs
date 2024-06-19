namespace PremiumScraps.CustomEffects
{
    internal class LegendaryStick : PhysicsProp
    {
        public LegendaryStick()
        {

        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded)
            {

            }
        }
    }
}
