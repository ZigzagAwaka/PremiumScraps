/*
 Modified from https://github.com/EvaisaDev/LethalThings/blob/main/LethalThings/MonoBehaviours/ToyHammer.cs
*/

using GameNetcodeStuff;
using LethalNetworkAPI;
using PremiumScraps.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class LegendaryStick : PhysicsProp
    {
        public int knockbackPowerMin = 10;
        public int knockbackPowerMax = 60;
        public int weaponHitForce = 1;
        public bool reelingUp;
        public bool isHoldingButton;
        private Coroutine reelingUpCoroutine;
        private RaycastHit[] objectsHitByWeapon;
        private List<RaycastHit> objectsHitByWeaponList = new List<RaycastHit>();
        private PlayerControllerB previousPlayerHeldBy;
        private int weaponMask = 11012424;
        public LethalClientMessage<PlayerDir> network;
        public LethalServerMessage<PlayerDir> networkServer;
        public LethalClientMessage<SfxId> networkAudio;
        public LegendaryStick()
        {
            network = new LethalClientMessage<PlayerDir>(identifier: "premiumscrapsStickID");
            networkServer = new LethalServerMessage<PlayerDir>(identifier: "premiumscrapsStickID");
            networkAudio = new LethalClientMessage<SfxId>(identifier: "premiumscrapsStickAudioID");
            network.OnReceived += KnockbackNetwork;
            networkServer.OnReceived += ReceiveKnockbackInfoNetwork;
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
        }

        private void KnockbackNetwork(PlayerDir playerDir)
        {
            if (playerDir.playerId.GetPlayerController() != null)
            {
                StartCoroutine(Effects.Knockback(playerDir.playerId.GetPlayerController(), playerDir.direction, Random.Range(knockbackPowerMin, knockbackPowerMax)));
            }
        }

        private void ReceiveKnockbackInfoNetwork(PlayerDir playerDir, ulong clientId)
        {
            networkServer.SendClient(playerDir, playerDir.playerId);
        }

        private void InvokeAudioNetwork(SfxId sfxId, ulong clientId)
        {
            Effects.Audio(sfxId.audioId, sfxId.position, 2.5f);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (playerHeldBy != null)
            {
                isHoldingButton = buttonDown;
                if (!reelingUp && buttonDown)
                {
                    reelingUp = true;
                    previousPlayerHeldBy = playerHeldBy;
                    if (reelingUpCoroutine != null)
                    {
                        StopCoroutine(reelingUpCoroutine);
                    }
                    reelingUpCoroutine = StartCoroutine(ReelUpWeapon());
                }
            }
        }

        public override void DiscardItem()
        {
            playerHeldBy.activatingItem = false;
            base.DiscardItem();
        }

        private IEnumerator ReelUpWeapon()
        {
            playerHeldBy.activatingItem = true;
            playerHeldBy.twoHanded = true;
            playerHeldBy.playerBodyAnimator.ResetTrigger("stickHit");
            playerHeldBy.playerBodyAnimator.SetBool("reelingUp", true);
            networkAudio.SendAllClients(new SfxId(3, playerHeldBy.transform.position));
            yield return new WaitForSeconds(0.35f);
            yield return new WaitUntil(() => !isHoldingButton || !isHeld);
            SwingWeapon(!isHeld);
            yield return new WaitForSeconds(0.13f);
            HitWeapon(!isHeld);
            yield return new WaitForSeconds(0.3f);
            reelingUp = false;
            reelingUpCoroutine = null;
        }

        public void SwingWeapon(bool cancel = false)
        {
            previousPlayerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: false);
            if (!cancel)
            {
                networkAudio.SendAllClients(new SfxId(4, previousPlayerHeldBy.transform.position));
                previousPlayerHeldBy.UpdateSpecialAnimationValue(specialAnimation: true, (short)previousPlayerHeldBy.transform.localEulerAngles.y, 0.4f);
            }
        }

        public void HitWeapon(bool cancel = false)
        {
            if (previousPlayerHeldBy == null)
            {
                return;
            }
            previousPlayerHeldBy.activatingItem = false;
            bool flag = false;
            int hitSurfaceID = -1;
            if (!cancel)
            {
                previousPlayerHeldBy.twoHanded = false;
                Debug.DrawRay(previousPlayerHeldBy.gameplayCamera.transform.position + previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f, previousPlayerHeldBy.gameplayCamera.transform.forward * 1.85f, Color.blue, 5f);
                objectsHitByWeapon = Physics.SphereCastAll(previousPlayerHeldBy.gameplayCamera.transform.position + previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f, 0.75f, previousPlayerHeldBy.gameplayCamera.transform.forward, 1.85f, weaponMask, QueryTriggerInteraction.Collide);
                objectsHitByWeaponList = objectsHitByWeapon.OrderBy((x) => x.distance).ToList();
                Vector3 start = previousPlayerHeldBy.gameplayCamera.transform.position;
                for (int i = 0; i < objectsHitByWeaponList.Count; i++)
                {
                    IHittable component;
                    RaycastHit hitInfo;
                    if (objectsHitByWeaponList[i].transform.gameObject.layer == 8 || objectsHitByWeaponList[i].transform.gameObject.layer == 11)
                    {
                        start = objectsHitByWeaponList[i].point + objectsHitByWeaponList[i].normal * 0.01f;
                        flag = true;
                        string text = objectsHitByWeaponList[i].collider.gameObject.tag;
                        for (int j = 0; j < StartOfRound.Instance.footstepSurfaces.Length; j++)
                        {
                            if (StartOfRound.Instance.footstepSurfaces[j].surfaceTag == text)
                            {
                                AudioSource.PlayClipAtPoint(StartOfRound.Instance.footstepSurfaces[j].hitSurfaceSFX, previousPlayerHeldBy.transform.position);
                                hitSurfaceID = j;
                                break;
                            }
                        }
                    }
                    else if (objectsHitByWeaponList[i].transform.TryGetComponent(out component) && !(objectsHitByWeaponList[i].transform == previousPlayerHeldBy.transform) && (objectsHitByWeaponList[i].point == Vector3.zero || !Physics.Linecast(start, objectsHitByWeaponList[i].point, out hitInfo, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
                        && !StartOfRound.Instance.inShipPhase)
                    {
                        flag = true;
                        Vector3 forward = previousPlayerHeldBy.gameplayCamera.transform.forward.normalized;

                        if (component.GetType() == typeof(PlayerControllerB))
                        {
                            if (!((PlayerControllerB)component).inSpecialInteractAnimation)
                            {
                                if (playerHeldBy.IsHost)
                                    ReceiveKnockbackInfoNetwork(new PlayerDir(((PlayerControllerB)component).GetClientId(), forward), playerHeldBy.GetClientId());
                                else
                                    network.SendServer(new PlayerDir(((PlayerControllerB)component).GetClientId(), forward));
                            }
                        }
                        else
                        {
                            //component.Hit(weaponHitForce, forward, previousPlayerHeldBy, true);
                        }
                    }
                }
            }
            if (flag)
            {
                FindObjectOfType<RoundManager>().PlayAudibleNoise(transform.position, 17f, 0.8f);
                playerHeldBy.playerBodyAnimator.SetTrigger("stickHit");
                networkAudio.SendAllClients(new SfxId(5, playerHeldBy.transform.position));
            }
        }
    }
}
