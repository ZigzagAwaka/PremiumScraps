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

    public class PosId
    {
        public int Id;
        public Vector3 position;

        public PosId(int Id, Vector3 position)
        {
            this.Id = Id;
            this.position = position;
        }
    }

    public class SetupScript
    {
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
