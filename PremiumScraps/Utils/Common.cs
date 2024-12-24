using PremiumScraps.CustomEffects;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace PremiumScraps.Utils
{
    public class Scrap
    {
        public string asset;
        public int rarity;
        public int behaviourId;

        public Scrap(string asset, int rarity, int behaviourId = 0)
        {
            this.asset = asset;
            this.rarity = rarity;
            this.behaviourId = behaviourId;
        }
    }

    public class SpecialEvent
    {
        public static int todayMonth = System.DateTime.Today.Month;

        public static void LoadSpecialEvent(GameObject itemPrefab)
        {
            if (todayMonth != 12) return;
            var christmas = itemPrefab.transform.Find("Christmas");
            christmas?.gameObject.SetActive(true);
        }
    }

    public class NetworkReference
    {
        public NetworkObjectReference netObjectRef;
        public int value;

        public NetworkReference(NetworkObjectReference netObjectRef, int value)
        {
            this.netObjectRef = netObjectRef;
            this.value = value;
        }
    }

    public class SetupScript
    {
        public static void Network()
        {
            IEnumerable<System.Type> types;
            try
            {
                types = Assembly.GetExecutingAssembly().GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }

        public static void Copy(ThrowableItem target, Item item)
        {
            var source = item.spawnPrefab.GetComponent<ThrowableItem>();
            target.itemFallCurve = source.itemFallCurve;
            target.itemVerticalFallCurve = source.itemVerticalFallCurve;
            target.itemVerticalFallCurveNoBounce = source.itemVerticalFallCurveNoBounce;
            var collider = item.spawnPrefab.GetComponentInChildren<GrabbableObjectPhysicsTrigger>();
            if (collider != null)
                collider.itemScript = target;
            Object.Destroy(source);
        }

        public static void Copy(NoisemakerProp target, Item item)
        {
            var source = item.spawnPrefab.GetComponent<NoisemakerProp>();
            target.useCooldown = source.useCooldown;
            target.noiseAudio = source.noiseAudio;
            target.noiseAudioFar = source.noiseAudioFar;
            target.noiseSFX = new AudioClip[source.noiseSFX.Length];
            for (int i = 0; i < source.noiseSFX.Length; i++)
            {
                target.noiseSFX[i] = source.noiseSFX[i];
            }
            target.noiseSFXFar = new AudioClip[source.noiseSFXFar.Length];
            for (int i = 0; i < source.noiseSFXFar.Length; i++)
            {
                target.noiseSFXFar[i] = source.noiseSFXFar[i];
            }
            target.noiseRange = source.noiseRange;
            target.maxLoudness = source.maxLoudness;
            target.minLoudness = source.minLoudness;
            target.minPitch = source.minPitch;
            target.maxPitch = source.maxPitch;
            target.triggerAnimator = source.triggerAnimator;
            Object.Destroy(source);
        }

        public static void Copy(SoccerBallProp target, Item item)
        {
            var source = item.spawnPrefab.GetComponent<SoccerBallProp>();
            target.ballHitUpwardAmount = source.ballHitUpwardAmount;
            target.grenadeFallCurve = source.grenadeFallCurve;
            target.grenadeVerticalFallCurve = source.grenadeVerticalFallCurve;
            target.soccerBallVerticalOffset = source.soccerBallVerticalOffset;
            target.grenadeVerticalFallCurveNoBounce = source.grenadeVerticalFallCurveNoBounce;
            target.hitBallSFX = new AudioClip[source.hitBallSFX.Length];
            for (int i = 0; i < source.hitBallSFX.Length; i++)
            {
                target.hitBallSFX[i] = source.hitBallSFX[i];
            }
            target.ballHitFloorSFX = new AudioClip[source.ballHitFloorSFX.Length];
            for (int i = 0; i < source.ballHitFloorSFX.Length; i++)
            {
                target.ballHitFloorSFX[i] = source.ballHitFloorSFX[i];
            }
            target.soccerBallAudio = source.soccerBallAudio;
            target.ballCollider = source.ballCollider;
            var collider = item.spawnPrefab.GetComponentInChildren<GrabbableObjectPhysicsTrigger>();
            collider.itemScript = target;
            Object.Destroy(source);
        }
    }
}
