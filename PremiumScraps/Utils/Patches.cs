using HarmonyLib;

namespace PremiumScraps.Utils
{
    [HarmonyPatch(typeof(ItemCharger))]
    internal class BombItemChargerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        public static void UpdatePatch(ref ItemCharger __instance)
        {
            if ((float)Traverse.Create(__instance).Field("updateInterval").GetValue() == 0f)
            {
                if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
                {
                    __instance.triggerScript.interactable = GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer != null && (GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer.itemProperties.requiresBattery ||
                        (GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer.itemProperties.name == "BombItem" && GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer is CustomEffects.Bomb bomb && !bomb.activated));
                    return;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("ChargeItem")]
        public static bool ChargeItemPatch()
        {
            return CustomEffects.Bomb.ChargeItemUnstable();
        }
    }


    [HarmonyPatch(typeof(LethalThings.Patches.PowerOutletStun))]
    internal class LethalThingsBombItemChargerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ItemCharger_ChargeItem")]
        public static bool ChargeItemPatch()
        {
            return CustomEffects.Bomb.ChargeItemUnstable();
        }
    }


    /*[HarmonyPatch(typeof(StormyWeather))]
    internal class BombItemStormyWeatherPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        public static void UpdatePatch(ref StormyWeather __instance)
        {
            var targetingMetalObject = Traverse.Create(__instance).Field("targetingMetalObject").GetValue() as GrabbableObject;
            if (targetingMetalObject != null && targetingMetalObject.itemProperties.name == "BombItem" && targetingMetalObject is CustomEffects.Bomb bomb && bomb.activated)
                Traverse.Create(__instance).Field("targetingMetalObject").SetValue(null);
        }

        [HarmonyPostfix]
        [HarmonyPatch("LightningStrike")]
        public static void LightningStrikePatch(ref StormyWeather __instance)
        {
            var strikeMetalObjectTimer = (float)Traverse.Create(__instance).Field("strikeMetalObjectTimer").GetValue();
            var targetingMetalObject = Traverse.Create(__instance).Field("targetingMetalObject").GetValue() as GrabbableObject;
            if (strikeMetalObjectTimer <= 0f && targetingMetalObject != null && !targetingMetalObject.isInFactory && targetingMetalObject.targetFloorPosition.x != 3000f && targetingMetalObject.itemProperties.name == "BombItem" && targetingMetalObject is CustomEffects.Bomb bomb && !bomb.activated)
                bomb.BombExplosionUnstableServerRpc();
        }
    }*/
}
