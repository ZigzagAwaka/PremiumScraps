using GameNetcodeStuff;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class ControllerMovement
    {
        internal enum ControllerActions
        {
            Jump,
            Crouch,
            Interact
        }

        private static bool IsInControlMode(PlayerControllerB player, out Controller controller)
        {
            controller = null;
            if (player == null)
                return false;
            var item = player.currentlyHeldObjectServer;
            if (item != null && item.itemProperties.name == "ControllerItem" && item is Controller controllerItem && controllerItem.isInControlMode)
            {
                controller = controllerItem;
                return true;
            }
            else
                return false;
        }

        private static bool IsBeingControlled(PlayerControllerB player)
        {
            return player != null && player.isTypingChat && player.disableMoveInput && player.disableLookInput;
        }

        public static bool ChatAction(HUDManager hudmanager)
        {
            var player = hudmanager.localPlayer ?? GameNetworkManager.Instance.localPlayerController;
            var verif = IsInControlMode(player, out _) || IsBeingControlled(player);
            if (verif)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                hudmanager.chatTextField.text = "";
                hudmanager.PingHUDElement(hudmanager.Chat, 1f);
                ((Behaviour)(object)hudmanager.typingIndicator).enabled = false;
            }
            return !verif;
        }

        public static bool PlayerAction(PlayerControllerB player, ControllerActions action)
        {
            if (IsInControlMode(player, out var controller))
            {
                controller.SendActionControlModeServerRpc((int)action, controller.targetClientId);
                return false;
            }
            return true;
        }


        // CONTROLLER MOVEMENT CODES

        public static void Jump(PlayerControllerB player)
        {
            if (((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer) && !player.inSpecialInteractAnimation && (player.isMovementHindered <= 0 || player.isUnderwater) && !player.isExhausted && (player.thisController.isGrounded || (!player.isJumping && player.IsPlayerNearGround())) && !player.isJumping && (!player.isPlayerSliding || player.playerSlidingTimer > 2.5f) && !player.isCrouching)
            {
                player.playerSlidingTimer = 0f;
                player.isJumping = true;
                player.sprintMeter = Mathf.Clamp(player.sprintMeter - 0.08f, 0f, 1f);
                StartOfRound.Instance.PlayerJumpEvent.Invoke(player);
                player.PlayJumpAudio();
                if (player.jumpCoroutine != null)
                {
                    player.StopCoroutine(player.jumpCoroutine);
                }
                player.jumpCoroutine = player.StartCoroutine(player.PlayerJump());
                if (StartOfRound.Instance.connectedPlayersAmount != 0)
                {
                    player.PlayerJumpedServerRpc();
                }
            }
        }

        public static void Crouch(PlayerControllerB player)
        {
            if (((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer) && !player.inSpecialInteractAnimation && player.thisController.isGrounded && !player.isJumping && !player.isSprinting)
            {
                player.crouchMeter = Mathf.Min(player.crouchMeter + 0.3f, 1.3f);
                player.Crouch(!player.isCrouching);
            }
        }

        public static void Interact(PlayerControllerB player)
        {
            if (((!player.IsOwner || !player.isPlayerControlled || (player.IsServer && !player.isHostPlayerObject)) && !player.isTestingPlayer) || player.timeSinceSwitchingSlots < 0.2f)
            {
                return;
            }
            ShipBuildModeManager.Instance.CancelBuildMode();
            if (!player.isGrabbingObjectAnimation && !player.inTerminalMenu && !player.throwingObject && !player.IsInspectingItem && player.inAnimationWithEnemy == null && !player.jetpackControls && !player.disablingJetpackControls && !StartOfRound.Instance.suckingPlayersOutOfShip)
            {
                if (!player.activatingItem && !player.waitingToDropItem)
                {
                    player.BeginGrabObject();
                }
                if (player.hoveringOverTrigger != null && !player.hoveringOverTrigger.holdInteraction && (!player.isHoldingObject || player.hoveringOverTrigger.oneHandedItemAllowed) && (!player.twoHanded || (player.hoveringOverTrigger.twoHandedItemAllowed && !player.hoveringOverTrigger.specialCharacterAnimation)) && player.InteractTriggerUseConditionsMet())
                {
                    player.hoveringOverTrigger.Interact(player.thisPlayerBody);
                }
            }
        }
    }
}
