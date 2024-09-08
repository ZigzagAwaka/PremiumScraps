namespace PremiumScraps.CustomEffects
{
    internal class JobDark : NoisemakerProp
    {
        public float maxLoudness = 1.0f;
        public float minLoudness = 0.9f;
        public float maxPitch = 0.9f;
        public float minPitch = 0.4f;
        public JobDark() { }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
        }
    }
}
