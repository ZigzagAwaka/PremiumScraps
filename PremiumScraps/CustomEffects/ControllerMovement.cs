using GameNetcodeStuff;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class ControllerMovement
    {
        public static Vector2 prevMoveInput = Vector2.zero;
        public static Vector2 moveInput = Vector2.zero;
        public static float prevMoveSpeed = 0f;
        public static float moveSpeed = 0f;
        public static Vector2 cruiserMoveInput = Vector2.zero;
        public static float cruiserSteeringInput;
        public static float cruiserSteeringWheelAudioVolume;

        internal enum ControllerActions
        {
            Jump,
            Crouch,
            Interact,
            ActivateItem,
            CancelItem,
            DropItem,
            SwitchItem,
            Emote,
            Move,
            Look
        }

        private static bool IsInControlMode(PlayerControllerB player, out Controller controller)
        {
            controller = null;
            if (player == null || player.isPlayerDead)
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

        private static bool IsBeingControlled(PlayerControllerB player, bool verifyLook = true)
        {
            return player != null && !player.isPlayerDead && player.isTypingChat && player.disableMoveInput && (!verifyLook || player.disableLookInput);
        }

        public static void FlagsFix()
        {
            var player = GameNetworkManager.Instance.localPlayerController;
            if (IsBeingControlled(player, false))
                player.disableLookInput = true;
        }

        public static void InvertFlagsFix()
        {
            var player = GameNetworkManager.Instance.localPlayerController;
            if (IsInControlMode(player, out _))
            {
                player.disableMoveInput = false;
                player.disableLookInput = false;
            }
        }

        public static bool ChatPatch(HUDManager hudmanager)
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

        public static bool PlayerPatch(PlayerControllerB player, ControllerActions action, bool canBeCanceled = false)
        {
            if (IsInControlMode(player, out var controller))
            {
                if (canBeCanceled && controller.targetIsValid)
                {
                    var target = controller.targetPlayer ?? StartOfRound.Instance.allPlayerObjects[controller.targetPlayerId].GetComponent<PlayerControllerB>();
                    if (target == null || !target.isHoldingObject || target.currentlyHeldObjectServer == null)
                        return true;
                }
                controller.SendActionControlModeServerRpc((int)action, controller.targetClientId);
                return false;
            }
            return true;
        }

        public static bool PlayerDataPatch(PlayerControllerB player, ControllerActions action, float data)
        {
            if (((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer)
                && IsInControlMode(player, out var controller))
            {
                if ((action == ControllerActions.SwitchItem && !player.inTerminalMenu && !player.quickMenuManager.isMenuOpen && !player.isTypingChat)
                    || (action == ControllerActions.Emote && !player.inSpecialInteractAnimation && !player.isJumping && !player.isWalking && !player.isClimbingLadder && !player.inTerminalMenu && !player.isTypingChat))
                {
                    controller.SendActionControlModeServerRpc((int)action, controller.targetClientId, data: data);
                    return false;
                }
            }
            return true;
        }

        public static void PlayerVectorPatch(PlayerControllerB player, ControllerActions action)
        {
            if (((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer)
                && IsInControlMode(player, out var controller))
            {
                if (action == ControllerActions.Move)
                {
                    if (player.quickMenuManager.isMenuOpen || player.isTypingChat || player.disableMoveInput || (player.inSpecialInteractAnimation && !player.isClimbingLadder && !player.inShockingMinigame))
                        return;
                    var input = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Move").ReadValue<Vector2>();
                    float speed = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Sprint").ReadValue<float>();
                    player.disableMoveInput = true;
                    if (input != prevMoveInput || speed != prevMoveSpeed)
                    {
                        prevMoveInput = input;
                        prevMoveSpeed = speed;
                        controller.SendActionControlModeServerRpc((int)action, controller.targetClientId, input, speed);
                    }
                }
                else if (action == ControllerActions.Look)
                {
                    if (player.quickMenuManager.isMenuOpen || player.inSpecialMenu || StartOfRound.Instance.newGameIsLoading || player.disableLookInput)
                        return;
                    var input = player.playerActions.Movement.Look.ReadValue<Vector2>() * 0.008f * IngamePlayerSettings.Instance.settings.lookSensitivity;
                    if (IngamePlayerSettings.Instance.settings.invertYAxis)
                        input.y *= -1f;
                    if (input.x != 0 || input.y != 0)
                    {
                        player.disableLookInput = true;
                        controller.SendActionControlModeServerRpc((int)action, controller.targetClientId, input);
                    }
                }
            }
        }

        public static bool EmoteConditionPatch(PlayerControllerB player)
        {
            if (IsBeingControlled(player) && !player.inSpecialInteractAnimation && !player.isPlayerDead && !player.isJumping && !player.isWalking && !player.isCrouching && !player.isClimbingLadder && !player.isGrabbingObjectAnimation && !player.inTerminalMenu)
                return false;
            return true;
        }

        public static void ControlledMovePatch(PlayerControllerB player)
        {
            if (IsBeingControlled(player))
            {
                player.moveInputVector = moveInput;
            }
        }

        public static float ControlledSprintPatch(PlayerControllerB player)
        {
            if (IsBeingControlled(player))
                return moveSpeed;
            else
                return IngamePlayerSettings.Instance.playerInput.actions.FindAction("Sprint").ReadValue<float>();
        }

        public static void ControlledCruiserSaveValuesPatch(VehicleController vehicle)
        {
            cruiserSteeringInput = vehicle.steeringInput;
            cruiserSteeringWheelAudioVolume = vehicle.steeringWheelAudio.volume;
        }

        public static void ControlledCruiserMovePatch(VehicleController vehicle)
        {
            if (vehicle.localPlayerInControl && IsBeingControlled(vehicle.currentDriver))
            {
                vehicle.moveInputVector = cruiserMoveInput;
                vehicle.steeringInput = Mathf.Clamp(cruiserSteeringInput + vehicle.moveInputVector.x * vehicle.steeringWheelTurnSpeed * Time.deltaTime, -3f, 3f);
                if (Mathf.Abs(vehicle.moveInputVector.x) > 0.1f)
                    vehicle.steeringWheelAudio.volume = Mathf.Lerp(cruiserSteeringWheelAudioVolume, Mathf.Abs(vehicle.moveInputVector.x), 5f * Time.deltaTime);
                else
                    vehicle.steeringWheelAudio.volume = Mathf.Lerp(cruiserSteeringWheelAudioVolume, 0f, 5f * Time.deltaTime);
                vehicle.steeringAnimValue = vehicle.moveInputVector.x;
                vehicle.drivePedalPressed = vehicle.moveInputVector.y > 0.1f;
                vehicle.brakePedalPressed = vehicle.moveInputVector.y < -0.1f;
            }
        }

        public static bool ControlledCruiserJumpPatch(VehicleController vehicle)
        {
            return vehicle.localPlayerInControl && IsBeingControlled(vehicle.currentDriver);
        }

        // CONTROLLER ACTIONS

        public static void PerformAction(PlayerControllerB player, ControllerActions action, Vector2 input, float data)
        {
            switch (action)
            {
                case ControllerActions.Jump: Jump(player); break;
                case ControllerActions.Crouch: Crouch(player); break;
                case ControllerActions.Interact: Interact(player); break;
                case ControllerActions.ActivateItem: ActivateItem(player); break;
                case ControllerActions.CancelItem: CancelItem(player); break;
                case ControllerActions.DropItem: DropItem(player); break;
                case ControllerActions.SwitchItem: SwitchItem(player, data); break;
                case ControllerActions.Emote: Emote(player, data); break;
                case ControllerActions.Move: Move(player, input, data); break;
                case ControllerActions.Look: Look(player, input); break;
                default: break;
            }
        }

        private static void Jump(PlayerControllerB player)
        {
            if (((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer) && !player.inSpecialInteractAnimation && (player.isMovementHindered <= 0 || player.isUnderwater)
                && !player.isExhausted && (player.thisController.isGrounded || (!player.isJumping && player.IsPlayerNearGround())) && !player.isJumping && (!player.isPlayerSliding || player.playerSlidingTimer > 2.5f) && !player.isCrouching)
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
            if (((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer)
                && player.inSpecialInteractAnimation && player.inVehicleAnimation)
            {
                foreach (var vehicle in Object.FindObjectsOfType<VehicleController>())
                {
                    if (vehicle.currentDriver != null && vehicle.currentDriver == player)
                    {
                        if (vehicle.localPlayerInControl && !vehicle.jumpingInCar && !vehicle.keyIsInDriverHand)
                        {
                            vehicle.UseTurboBoostLocalClient(cruiserMoveInput);
                            vehicle.UseTurboBoostServerRpc();
                        }
                        break;
                    }
                }
            }
        }

        private static void Crouch(PlayerControllerB player)
        {
            if (((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer) && !player.inSpecialInteractAnimation && player.thisController.isGrounded && !player.isJumping && !player.isSprinting)
            {
                player.crouchMeter = Mathf.Min(player.crouchMeter + 0.3f, 1.3f);
                player.Crouch(!player.isCrouching);
            }
        }

        private static void Interact(PlayerControllerB player)
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
                if (player.hoveringOverTrigger != null && (!player.isHoldingObject || player.hoveringOverTrigger.oneHandedItemAllowed) && (!player.twoHanded || (player.hoveringOverTrigger.twoHandedItemAllowed && !player.hoveringOverTrigger.specialCharacterAnimation)) && player.InteractTriggerUseConditionsMet())
                {
                    player.hoveringOverTrigger.Interact(player.thisPlayerBody);
                }
            }
        }

        private static void ActivateItem(PlayerControllerB player)
        {
            if ((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject) || player.isTestingPlayer) && !player.isPlayerDead && player.isHoldingObject && player.currentlyHeldObjectServer != null
                && !player.isGrabbingObjectAnimation && !player.inTerminalMenu && (!player.inSpecialInteractAnimation || player.inShockingMinigame) && player.timeSinceSwitchingSlots >= 0.075f)
            {
                ShipBuildModeManager.Instance.CancelBuildMode();
                player.currentlyHeldObjectServer.gameObject.GetComponent<GrabbableObject>().UseItemOnClient();
                player.timeSinceSwitchingSlots = 0f;
            }
        }

        private static void CancelItem(PlayerControllerB player)
        {
            if ((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject) || player.isTestingPlayer) && !player.isPlayerDead && player.isHoldingObject && player.currentlyHeldObjectServer != null
                && !player.isGrabbingObjectAnimation && !player.inTerminalMenu && (!player.inSpecialInteractAnimation || player.inShockingMinigame) && player.currentlyHeldObjectServer.itemProperties.holdButtonUse)
            {
                ShipBuildModeManager.Instance.CancelBuildMode();
                player.currentlyHeldObjectServer.gameObject.GetComponent<GrabbableObject>().UseItemOnClient(false);
            }
        }

        private static void DropItem(PlayerControllerB player)
        {
            if (!player.IsOwner || !player.isPlayerControlled || (player.IsServer && !player.isHostPlayerObject))
                return;
            if (StartOfRound.Instance.localPlayerUsingController && ShipBuildModeManager.Instance.InBuildMode)
                ShipBuildModeManager.Instance.StoreObjectLocalClient();
            else
            {
                if (player.timeSinceSwitchingSlots < 0.2f || player.isGrabbingObjectAnimation || player.inSpecialInteractAnimation || player.activatingItem)
                    return;
                ShipBuildModeManager.Instance.CancelBuildMode();
                if (player.throwingObject || !player.isHoldingObject || player.currentlyHeldObjectServer == null)
                    return;
                var desk = Object.FindObjectOfType<DepositItemsDesk>();
                if (desk != null && player.currentlyHeldObjectServer != null)
                {
                    if (desk.triggerCollider.bounds.Contains(player.currentlyHeldObjectServer.transform.position))
                    {
                        desk.PlaceItemOnCounter(player);
                        return;
                    }
                }
                player.StartCoroutine(player.waitToEndOfFrameToDiscard());
            }
        }

        private static void SwitchItem(PlayerControllerB player, float data)
        {
            if (player.inTerminalMenu)
            {
                player.terminalScrollVertical.value += data / 3f;
            }
            else if (((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer) && player.timeSinceSwitchingSlots >= 0.3f
                && !player.isGrabbingObjectAnimation && !player.inSpecialInteractAnimation && !player.throwingObject && !player.twoHanded && !player.activatingItem && !player.jetpackControls && !player.disablingJetpackControls)
            {
                ShipBuildModeManager.Instance.CancelBuildMode();
                player.playerBodyAnimator.SetBool("GrabValidated", value: false);
                bool forward = data > 0f;
                player.SwitchToItemSlot(player.NextItemSlot(forward));
                player.SwitchItemSlotsServerRpc(forward);
                player.currentlyHeldObjectServer?.gameObject.GetComponent<AudioSource>().PlayOneShot(player.currentlyHeldObjectServer.itemProperties.grabSFX, 0.6f);
                player.timeSinceSwitchingSlots = 0f;
            }
        }

        private static void Emote(PlayerControllerB player, float data)
        {
            if (((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer) && player.CheckConditionsForEmote() && player.timeSinceStartingEmote >= 0.5f)
            {
                player.timeSinceStartingEmote = 0f;
                player.performingEmote = true;
                player.playerBodyAnimator.SetInteger("emoteNumber", (int)data);
                player.StartPerformingEmoteServerRpc();
            }
        }

        private static void Move(PlayerControllerB player, Vector2 input, float speed)
        {
            if ((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer)
            {
                if (player.inSpecialInteractAnimation && !player.isClimbingLadder && !player.inShockingMinigame)
                {
                    if (player.inVehicleAnimation)
                        cruiserMoveInput = input;
                    input = Vector2.zero;
                }
                moveInput = input;
                moveSpeed = speed;
            }
        }

        private static void Look(PlayerControllerB player, Vector2 input)
        {
            if (StartOfRound.Instance.newGameIsLoading || player.isFreeCamera || player.isPlayerDead)
                return;
            if ((player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject)) || player.isTestingPlayer)
            {
                StartOfRound.Instance.playerLookMagnitudeThisFrame = input.magnitude * Time.deltaTime;
                if (player.inSpecialInteractAnimation && (player.isClimbingLadder || player.clampLooking))
                {
                    if (player.isClimbingLadder)
                    {
                        player.minVerticalClamp = 25f;
                        player.maxVerticalClamp = -60f;
                        player.horizontalClamp = 60f;
                    }
                    player.LookClamped(input);
                    player.SyncFullRotWithClients(player.inVehicleAnimation);
                    return;
                }
                if (player.smoothLookMultiplier != 25f)
                    player.CalculateSmoothLookingInput(input);
                else
                    player.CalculateNormalLookingInput(input);
                if (player.isTestingPlayer || (player.IsServer && player.playersManager.connectedPlayersAmount < 1))
                    return;
                if (player.jetpackControls || player.disablingJetpackControls)
                    player.SyncFullRotWithClients();
                else if (player.updatePlayerLookInterval > 0.1f && Physics.OverlapSphere(player.transform.position, 35f, player.playerMask).Length != 0)
                {
                    player.updatePlayerLookInterval = 0f;
                    if (Mathf.Abs(player.oldCameraUp + player.previousYRot - (player.cameraUp + player.thisPlayerBody.eulerAngles.y)) > 3f && !player.playersManager.newGameIsLoading)
                    {
                        player.UpdatePlayerRotationServerRpc((short)player.cameraUp, (short)player.thisPlayerBody.localEulerAngles.y);
                        player.oldCameraUp = player.cameraUp;
                        player.previousYRot = player.thisPlayerBody.localEulerAngles.y;
                    }
                }
            }
        }
    }
}
