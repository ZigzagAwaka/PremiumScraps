using PremiumScraps.Utils;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class TrollFace : PhysicsProp
    {
        public TrollFace()
        {
            useCooldown = 3;
            customGrabTooltip = "Friendship ends here : [E]";
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null)
            {
                if (StartOfRound.Instance.inShipPhase || !StartOfRound.Instance.shipHasLanded)
                {
                    Effects.Message("Not now", "Try it a little bit later :)");
                    return;
                }
                AudioServerRpc(1, playerHeldBy.transform.position, 1.5f, 2f);
                if (playerHeldBy.health > 90)
                {
                    Effects.Damage(playerHeldBy, 10);
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
                    Effects.Message("Don't do this bro", "Don't listen to the voices in your head.", true);
                }
                else if (playerHeldBy.health > 70 && playerHeldBy.health <= 90)
                {
                    Effects.Damage(playerHeldBy, 20);
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
                    Effects.Message("We warned you", "You know there's no turning back from what you're about to do, right?", true);
                }
                else if (playerHeldBy.health > 20 && playerHeldBy.health <= 70)
                {
                    Effects.Damage(playerHeldBy, playerHeldBy.health - 10);
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
                    Effects.Message("W̴ͪ̅e̤̲̞ ḏ͆ȍ̢̥ a̵̿͘ l̙ͭ͠ittle b̈́͠it of troll͢i̗̍͜n͙̆͠g", "", true);
                }
                else
                {
                    var playerTmp = playerHeldBy;
                    if (playerHeldBy.IsHost)
                        StartCoroutine(Effects.DamageHost(playerHeldBy, 100, CauseOfDeath.Strangulation, (int)Effects.DeathAnimation.NoHead1));
                    else
                        Effects.Damage(playerHeldBy, 100, CauseOfDeath.Strangulation, (int)Effects.DeathAnimation.NoHead1);
                    SpawnEnemyServerRpc(playerTmp.transform.position);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnEnemyServerRpc(Vector3 position)
        {
            for (int i = 0; i < 4; i++)
                Effects.Spawn(GetEnemies.ForestKeeper, position);
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
