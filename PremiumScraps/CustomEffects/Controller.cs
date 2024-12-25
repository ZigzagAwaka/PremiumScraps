using DigitalRuby.ThunderAndLightning;
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
            itemProperties.batteryUsage = 50;
            renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            chargingParticle = transform.GetChild(2).GetComponent<ParticleSystem>();
            chargingAudio = transform.GetChild(2).GetComponent<AudioSource>();
            chargingTransform = transform.GetChild(2).transform;
            screenLight = transform.GetChild(3).GetComponent<Light>();
            //screenColor = renderer.materials[3].GetColor("_EmissiveColor");
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
            LightningZap(chargingTransform.position, StartOfRound.Instance.middleOfShipNode.transform.position);
            if (renderer != null)
                renderer.materials[3].SetTexture("_ScreenTexture", playerHeldBy.gameplayCamera.targetTexture);
        }

        public static void LightningZap(Vector3 source, Vector3 destination, int audioID = 23)
        {
            LightningBoltPrefabScript zap;
            //var random = new System.Random(StartOfRound.Instance.randomMapSeed);
            //random.Next(-32, 32); random.Next(-32, 32);
            zap = Instantiate(FindObjectOfType<StormyWeather>(true).targetedThunder);
            zap.enabled = true;
            zap.Camera = GameNetworkManager.Instance.localPlayerController.gameplayCamera;
            zap.AutomaticModeSeconds = 0.2f;
            //zap.LightningTintColor
            //zap.GlowTintColor
            zap.CountRange = new RangeOfIntegers { Minimum = 1, Maximum = 1 };
            zap.TrunkWidthRange = new RangeOfFloats { Minimum = 0.01f, Maximum = 0.02f };
            zap.Intensity = 0.1f;
            zap.LightParameters.LightIntensity = 0.1f;
            zap.Source.transform.position = source;
            zap.Destination.transform.position = destination;
            zap.CreateLightningBoltsNow();
            Effects.Audio3D(audioID, destination + Vector3.up * 0.5f, 1, 40);
        }
    }
}
