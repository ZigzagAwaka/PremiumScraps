using PremiumScraps.Utils;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.CustomEffects
{
    internal class Controller : PhysicsProp
    {
        public MeshRenderer? renderer;
        public ParticleSystem? chargingParticle;
        public AudioSource? chargingAudio;
        private Transform? chargingTransform;
        public Light? screenLight;
        private Color screenColor;

        public Controller()
        {
            useCooldown = 2;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            itemProperties.batteryUsage = 10;
            renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            chargingParticle = transform.GetChild(2).GetComponent<ParticleSystem>();
            chargingAudio = transform.GetChild(2).GetComponent<AudioSource>();
            chargingTransform = transform.GetChild(2).transform;
            screenLight = transform.GetChild(3).GetComponent<Light>();
            screenColor = renderer.materials[3].GetColor("_EmissiveColor");
            if (insertedBattery != null)
                insertedBattery.charge = 1;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (playerHeldBy == null || insertedBattery == null || insertedBattery.empty)
                return;
            isBeingUsed = true;
            AnimationServerRpc();
            if (playerHeldBy.gameplayCamera.targetTexture != null)
                Debug.LogError("oui");
            else
                Debug.LogError("non");
        }

        public override void ChargeBatteries()
        {
            base.ChargeBatteries();
            if (playerHeldBy != null && insertedBattery != null && insertedBattery.charge == 1f && renderer != null && screenLight != null)
            {
                //renderer.materials[3].SetColor("_EmissiveColor", screenColor);
                screenLight.enabled = true;
            }
        }

        public override void UseUpBatteries()
        {
            base.UseUpBatteries();
            if (renderer != null && screenLight != null)
            {
                //renderer.materials[3].SetColor("_EmissiveColor", Color.black);
                screenLight.enabled = false;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void AnimationServerRpc()
        {
            AnimationClientRpc(Random.Range(0.9f, 1f));
        }

        [ClientRpc]
        private void AnimationClientRpc(float chargePitch)
        {
            if (chargingParticle == null || chargingAudio == null || chargingTransform == null)
                return;
            chargingParticle.Play();
            chargingAudio.pitch = chargePitch;
            chargingAudio.Play();
            Effects.SpawnLightningBolt(StartOfRound.Instance.middleOfShipNode.transform.position, false, false, false, chargingTransform.position);
            if (renderer != null)
                renderer.materials[3].SetTexture("_ScreenTexture", playerHeldBy.gameplayCamera.targetTexture);
            Debug.LogError("" + playerHeldBy.playerUsername + " " + playerHeldBy.name);
        }
    }
}
