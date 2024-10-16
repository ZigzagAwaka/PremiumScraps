using HarmonyLib;

namespace PremiumScraps.Utils
{
    [HarmonyPatch(typeof(ItemCharger))]
    internal class ItemChargerPatch  // BOMBITEM PATCH CHARGE
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        public static void UpdatePatch(ref ItemCharger __instance)
        {
            if ((float)Traverse.Create(__instance).Field("updateInterval").GetValue() == 0f)
            {
                if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
                {
                    __instance.triggerScript.interactable = GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer != null && (GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer.itemProperties.requiresBattery || GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer.itemProperties.name == "BombItem");
                    if (__instance.triggerScript.interactable && ((CustomEffects.Bomb)GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer).activated)
                        __instance.triggerScript.interactable = false;
                    return;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("ChargeItem")]
        public static bool ChargeItemPatch(ref ItemCharger __instance)
        {
            GrabbableObject currentlyHeldObjectServer = GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer;
            if (currentlyHeldObjectServer == null)
            {
                return false;
            }
            if (currentlyHeldObjectServer.itemProperties.name == "BombItem")
            {
                __instance.StartCoroutine(CustomEffects.Bomb.BombExplosionUnstable(currentlyHeldObjectServer));
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(StormyWeather))]
    internal class StormyWeatherPatch  // BOMBITEM PATCH STORMY
    {
        [HarmonyPostfix]
        [HarmonyPatch("LightningStrike")]
        public static void LightningStrikePatch(ref ItemCharger __instance)
        {
            var targetingMetalObject = Traverse.Create(__instance).Field("targetingMetalObject").GetValue() as GrabbableObject;
            if (targetingMetalObject != null && targetingMetalObject.itemProperties.name == "BombItem" && !targetingMetalObject.isInFactory && targetingMetalObject.targetFloorPosition.x != 3000f)
                if (!((CustomEffects.Bomb)targetingMetalObject).activated)
                    __instance.StartCoroutine(CustomEffects.Bomb.BombExplosionUnstable(targetingMetalObject));
        }
    }
}
