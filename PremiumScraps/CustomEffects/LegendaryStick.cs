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
        public int knockbackPowerMin = 1;
        public int knockbackPowerMax = 10;
        public readonly bool specialEffectEnabled = true;
        public int weaponHitForce = 1;
        public bool reelingUp;
        public bool isHoldingButton;
        private Coroutine reelingUpCoroutine;
        private RaycastHit[] objectsHitByWeapon;
        private List<RaycastHit> objectsHitByWeaponList = new List<RaycastHit>();
        private PlayerControllerB previousPlayerHeldBy;
        private readonly int weaponMask = 11012424;
        public LethalClientMessage<PosId> network;
        public LethalClientMessage<PosId> networkAudio;
        public LegendaryStick()
        {
            network = new LethalClientMessage<PosId>(identifier: "premiumscrapsStickID");
            networkAudio = new LethalClientMessage<PosId>(identifier: "premiumscrapsStickAudioID");
            network.OnReceivedFromClient += KnockbackNetwork;
            networkAudio.OnReceivedFromClient += InvokeAudioNetwork;
        }

        private void KnockbackNetwork(PosId info, ulong clientId)
        {
            Effects.Knockback(info.position, 1, 0, info.Id);
        }

        private void InvokeAudioNetwork(PosId posId, ulong clientId)
        {
            Effects.Audio(posId.Id, posId.position, 2.5f);
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
            networkAudio.SendAllClients(new PosId(3, playerHeldBy.transform.position));
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
                networkAudio.SendAllClients(new PosId(4, previousPlayerHeldBy.transform.position));
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

                        if (specialEffectEnabled && component.GetType() == typeof(PlayerControllerB))
                        {
                            if (!((PlayerControllerB)component).inSpecialInteractAnimation)
                            {
                                int power = Random.Range(knockbackPowerMin, knockbackPowerMax);
                                network.SendAllClients(new PosId(power, previousPlayerHeldBy.transform.position), false);
                            }
                        }
                        //else
                        //{
                        //component.Hit(weaponHitForce, forward, previousPlayerHeldBy, true);
                        //}
                    }
                }
            }
            if (flag)
            {
                FindObjectOfType<RoundManager>().PlayAudibleNoise(transform.position, 17f, 0.8f);
                playerHeldBy.playerBodyAnimator.SetTrigger("stickHit");
                networkAudio.SendAllClients(new PosId(5, playerHeldBy.transform.position));
            }
        }
    }
}
