using GameNetcodeStuff;
using HarmonyLib;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.InputSystem;


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
        [HarmonyPatch("ItemCharger_Update")]
        public static bool UpdatePatch(ItemCharger self)
        {
            bool flag = false;
            if ((float)Traverse.Create(self).Field("updateInterval").GetValue() == 0f)
            {
                if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
                {
                    self.triggerScript.interactable = GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer != null && (GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer.itemProperties.requiresBattery ||
                        (GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer.itemProperties.name == "BombItem" && GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer is CustomEffects.Bomb bomb && !bomb.activated));
                    flag = self.triggerScript.interactable;
                }
            }
            return !flag;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ItemCharger_ChargeItem")]
        public static bool ChargeItemPatch()
        {
            return CustomEffects.Bomb.ChargeItemUnstable();
        }
    }


    [HarmonyPatch(typeof(StartOfRound))]
    internal class MattyFixesAirhornPositionPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        [HarmonyPriority(Priority.Normal)]
        public static void AwakePatch()
        {
            var fakeAirhorn = LethalLib.Modules.Items.scrapItems.Find(i => i.modName == "PremiumScraps" && i.item.itemName == "Airhorn");
            if (fakeAirhorn != null && fakeAirhorn != default)
            {
                fakeAirhorn.item.restingRotation = new Vector3(0, -180, 270);
                fakeAirhorn.item.floorYOffset = -180;
            }
        }
    }


    [HarmonyPatch(typeof(HUDManager))]
    internal class FrenchModeItemTooltipsPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ChangeControlTip")]
        public static void ChangeControlTipPatch(HUDManager __instance, string changeTo)
        {
            if (StartOfRound.Instance.localPlayerUsingController)
                return;
            changeTo = changeTo.Replace("[Z]", "[W]");
            changeTo = changeTo.Replace("[Q]", "[A]");
            changeTo = changeTo.Replace("[Q/E]", "[A/E]");
        }

        [HarmonyPrefix]
        [HarmonyPatch("ChangeControlTipMultiple")]
        public static void ChangeControlTipMultiplePatch(HUDManager __instance, string[] allLines, bool holdingItem)
        {
            if (allLines == null || StartOfRound.Instance.localPlayerUsingController)
                return;
            for (int i = 0; i < allLines.Length && i + (holdingItem ? 1 : 0) < __instance.controlTipLines.Length; i++)
            {
                allLines[i] = allLines[i].Replace("[Z]", "[W]");
                allLines[i] = allLines[i].Replace("[Q]", "[A]");
                allLines[i] = allLines[i].Replace("[Q/E]", "[A/E]");
            }
        }
    }


    [HarmonyPatch(typeof(Terminal))]
    internal class ControllerTerminalPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("QuitTerminal")]
        public static void QuitTerminalPatch()
        {
            CustomEffects.ControllerMovement.FlagsFix();
        }
    }


    [HarmonyPatch(typeof(HUDManager))]
    internal class ControllerHUDManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("EnableChat_performed")]
        public static bool EnableChatControllerPatch(HUDManager __instance)
        {
            return CustomEffects.ControllerMovement.ChatPatch(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch("SubmitChat_performed")]
        public static bool SubmitChatControllerPatch(HUDManager __instance)
        {
            return CustomEffects.ControllerMovement.ChatPatch(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch("OpenMenu_performed")]
        public static bool OpenMenuControllerPatch(HUDManager __instance)
        {
            return CustomEffects.ControllerMovement.ChatPatch(__instance);
        }
    }


    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class ControllerPlayerControllerBPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Jump_performed")]
        public static bool JumpControllerPatch(PlayerControllerB __instance)
        {
            return CustomEffects.ControllerMovement.PlayerPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.Jump);
        }

        [HarmonyPrefix]
        [HarmonyPatch("Crouch_performed")]
        public static bool CrouchControllerPatch(PlayerControllerB __instance)
        {
            return CustomEffects.ControllerMovement.PlayerPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.Crouch);
        }

        [HarmonyPrefix]
        [HarmonyPatch("Interact_performed")]
        public static bool InteractControllerPatch(PlayerControllerB __instance)
        {
            return CustomEffects.ControllerMovement.PlayerPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.Interact);
        }

        [HarmonyPrefix]
        [HarmonyPatch("ActivateItem_performed")]
        public static bool ActivateItemControllerPatch(PlayerControllerB __instance)
        {
            return CustomEffects.ControllerMovement.PlayerPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.ActivateItem);
        }

        [HarmonyPrefix]
        [HarmonyPatch("ActivateItem_canceled")]
        public static bool CancelItemControllerPatch(PlayerControllerB __instance)
        {
            return CustomEffects.ControllerMovement.PlayerPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.CancelItem);
        }

        [HarmonyPrefix]
        [HarmonyPatch("Discard_performed")]
        public static bool DiscardControllerPatch(PlayerControllerB __instance)
        {
            return CustomEffects.ControllerMovement.PlayerPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.DropItem, true);
        }

        [HarmonyPrefix]
        [HarmonyPatch("ScrollMouse_performed")]
        public static bool ScrollControllerPatch(PlayerControllerB __instance, InputAction.CallbackContext context)
        {
            var verif = CustomEffects.ControllerMovement.PlayerDataPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.SwitchItem, context.ReadValue<float>());
            if (verif)
                verif = !CustomEffects.SteelBar.VerifyPreventSwitch(__instance);
            return verif;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Emote1_performed")]
        public static bool Emote1ControllerPatch(PlayerControllerB __instance)
        {
            return CustomEffects.ControllerMovement.PlayerDataPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.Emote, 1);
        }

        [HarmonyPrefix]
        [HarmonyPatch("Emote2_performed")]
        public static bool Emote2ControllerPatch(PlayerControllerB __instance)
        {
            return CustomEffects.ControllerMovement.PlayerDataPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.Emote, 2);
        }

        [HarmonyPrefix]
        [HarmonyPatch("CheckConditionsForEmote")]
        public static bool EmoteConditionControllerPatch(PlayerControllerB __instance, ref bool __result)
        {
            var verif = CustomEffects.ControllerMovement.EmoteConditionPatch(__instance);
            if (!verif)
                __result = true;
            return verif;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static void UpdateControllerPrePatch(PlayerControllerB __instance)
        {
            CustomEffects.ControllerMovement.PlayerVectorPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.Move);
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        public static void UpdateControllerPostPatch()
        {
            CustomEffects.ControllerMovement.InvertFlagsFix();
        }

        [HarmonyPrefix]
        [HarmonyPatch("PlayerLookInput")]
        public static void PlayerLookInputControllerPrePatch(PlayerControllerB __instance)
        {
            CustomEffects.ControllerMovement.PlayerVectorPatch(__instance, CustomEffects.ControllerMovement.ControllerActions.Look);
        }

        [HarmonyPostfix]
        [HarmonyPatch("PlayerLookInput")]
        public static void PlayerLookInputControllerPostPatch()
        {
            CustomEffects.ControllerMovement.InvertFlagsFix();
        }
    }


    [HarmonyPatch(typeof(VehicleController))]
    internal class ControllerVehicleControllerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetVehicleInput")]
        public static void GetVehicleInputControllerPrePatch(VehicleController __instance)
        {
            CustomEffects.ControllerMovement.ControlledCruiserSaveValuesPatch(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetVehicleInput")]
        public static void GetVehicleInputControllerPostPatch(VehicleController __instance)
        {
            CustomEffects.ControllerMovement.ControlledCruiserMovePatch(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch("DoTurboBoost")]
        public static bool DoTurboBoostControllerPatch(VehicleController __instance)
        {
            return !CustomEffects.ControllerMovement.ControlledCruiserJumpPatch(__instance);
        }
    }


    internal static class PremiumScrapsMonoModPatches
    {
        public static void Load()
        {
            IL.GameNetcodeStuff.PlayerControllerB.Update += IsBeingControlledMove;
        }

        private static void IsBeingControlledMove(ILContext il)
        {
            var c = new ILCursor(il);
            for (int i = 0; i < 3; i++)  // 3(-1) instances of "moveInputVector = Vector2.zero" to patch
            {
                c.GotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchCall<Vector2>("get_" + nameof(Vector2.zero)),
                    x => x.MatchStfld<PlayerControllerB>(nameof(PlayerControllerB.moveInputVector))
                );
                if (i == 1)  // ignore instance n.1 which is not supposed to be reached with the Controller item
                    continue;
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
                c.EmitDelegate<System.Action<PlayerControllerB>>((self) =>
                {
                    CustomEffects.ControllerMovement.ControlledMovePatch(self);
                });
                if (i == 0)  // sprint is only patched on the 1st instance
                {
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
                    c.EmitDelegate<System.Func<PlayerControllerB, float>>((self) =>
                    {
                        return CustomEffects.ControllerMovement.ControlledSprintPatch(self);
                    });
                    c.Emit(Mono.Cecil.Cil.OpCodes.Stloc_0);
                }
            }
        }
    }
}
