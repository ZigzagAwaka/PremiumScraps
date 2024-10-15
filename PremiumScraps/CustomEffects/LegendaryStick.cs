using GameNetcodeStuff;
using PremiumScraps.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class LegendaryStick : PhysicsProp
    {
        public int knockbackPowerMin = 1;
        public int knockbackPowerMax = 10;
        public int weaponHitForce = 1;
        public bool reelingUp;
        public bool isHoldingButton;
        private Coroutine? reelingUpCoroutine;
        private RaycastHit[] objectsHitByWeapon;
        private List<RaycastHit> objectsHitByWeaponList = new List<RaycastHit>();
        private PlayerControllerB previousPlayerHeldBy;
        private readonly int weaponMask = 11012424;

        public LegendaryStick() { }

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
            AudioServerRpc(3, playerHeldBy.transform.position, 2.5f);
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
                AudioServerRpc(4, previousPlayerHeldBy.transform.position, 2.5f);
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
                Debug.DrawRay(previousPlayerHeldBy.gameplayCamera.transform.position + previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f,
                    previousPlayerHeldBy.gameplayCamera.transform.forward * 1.85f, Color.blue, 5f);
                objectsHitByWeapon = Physics.SphereCastAll(previousPlayerHeldBy.gameplayCamera.transform.position + previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f,
                    0.75f, previousPlayerHeldBy.gameplayCamera.transform.forward, 1.85f, weaponMask, QueryTriggerInteraction.Collide);
                objectsHitByWeaponList = objectsHitByWeapon.OrderBy((x) => x.distance).ToList();
                Vector3 start = previousPlayerHeldBy.gameplayCamera.transform.position;
                for (int i = 0; i < objectsHitByWeaponList.Count; i++)
                {
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
                    else if (objectsHitByWeaponList[i].transform.TryGetComponent(out IHittable component) && !(objectsHitByWeaponList[i].transform == previousPlayerHeldBy.transform)
                        && (objectsHitByWeaponList[i].point == Vector3.zero || !Physics.Linecast(start, objectsHitByWeaponList[i].point, out RaycastHit hitInfo, StartOfRound.Instance.collidersAndRoomMaskAndDefault)))
                    {
                        flag = true;
                        if (component.GetType() == typeof(PlayerControllerB))
                        {
                            if (!((PlayerControllerB)component).inSpecialInteractAnimation)
                            {
                                int power = Random.Range(knockbackPowerMin, knockbackPowerMax);
                                KnockbackServerRpc(previousPlayerHeldBy.transform.position, power);
                            }
                        }
                    }
                }
            }
            if (flag)
            {
                FindObjectOfType<RoundManager>().PlayAudibleNoise(transform.position, 17f, 0.8f);
                playerHeldBy.playerBodyAnimator.SetTrigger("stickHit");
                AudioServerRpc(5, playerHeldBy.transform.position, 2.5f);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void KnockbackServerRpc(Vector3 position, int power)
        {
            KnockbackClientRpc(position, power);
        }

        [ClientRpc]
        private void KnockbackClientRpc(Vector3 position, int power)
        {
            if (playerHeldBy != null && GameNetworkManager.Instance.localPlayerController.playerClientId == playerHeldBy.playerClientId)
                return;
            Effects.Knockback(position, 1, 0, power);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AudioServerRpc(int audioID, Vector3 position, float volume)
        {
            AudioClientRpc(audioID, position, volume);
        }

        [ClientRpc]
        private void AudioClientRpc(int audioID, Vector3 position, float volume)
        {
            Effects.Audio(audioID, position, volume);
        }
    }
}
