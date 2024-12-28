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
        public int knockbackPowerMin = 5;
        public int knockbackPowerMax = 15;
        public int chanceForUltimateKnockback = 5;
        public int ultimateKnockback = 50;
        public bool reelingUp;
        public bool isHoldingButton;
        private Coroutine? reelingUpCoroutine;
        private RaycastHit[] objectsHitByWeapon;
        private List<RaycastHit> objectsHitByWeaponList = new List<RaycastHit>();
        private PlayerControllerB previousPlayerHeldBy;

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
            AudioServerRpc(3, 1f);
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
                AudioServerRpc(4, 0.75f);
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
                objectsHitByWeapon = Physics.SphereCastAll(previousPlayerHeldBy.gameplayCamera.transform.position + previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f,
                    0.75f, previousPlayerHeldBy.gameplayCamera.transform.forward, 1.85f, 11012424, QueryTriggerInteraction.Collide);
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
                            KnockbackServerRpc(((PlayerControllerB)component).OwnerClientId, previousPlayerHeldBy.gameplayCamera.transform.forward.normalized);
                        }
                    }
                }
            }
            if (flag)
            {
                FindObjectOfType<RoundManager>().PlayAudibleNoise(transform.position, 17f, 0.8f);
                playerHeldBy.playerBodyAnimator.SetTrigger("stickHit");
                AudioServerRpc(5, 0.85f);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void KnockbackServerRpc(ulong clientId, Vector3 direction)
        {
            int power = Random.Range(0, 100) <= chanceForUltimateKnockback - 1 ? ultimateKnockback : Random.Range(knockbackPowerMin, knockbackPowerMax);
            var clientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { clientId } } };
            KnockbackClientRpc(power, direction, clientRpcParams);
        }

        [ClientRpc]
        private void KnockbackClientRpc(int power, Vector3 direction, ClientRpcParams clientRpcParams = default)
        {
            if (playerHeldBy != null)
            {
                Effects.Knockback(GameNetworkManager.Instance.localPlayerController.transform.position - direction, 5, 0, power);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void AudioServerRpc(int audioID, float volume)
        {
            AudioClientRpc(audioID, volume);
        }

        [ClientRpc]
        private void AudioClientRpc(int audioID, float volume)
        {
            if (playerHeldBy != null)
                playerHeldBy.itemAudio.PlayOneShot(Plugin.audioClips[audioID], volume);
        }
    }
}
