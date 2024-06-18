using PremiumScraps.Utils;

namespace PremiumScraps.CustomEffects
{
    internal class Teleportation : PhysicsProp
    {
        public Teleportation() { }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.shipHasLanded)
            {
                Effects.Audio(2, playerHeldBy.transform.position, 3f);
                Effects.Teleportation(playerHeldBy, StartOfRound.Instance.middleOfShipNode.position, true);
            }
        }
    }
}
