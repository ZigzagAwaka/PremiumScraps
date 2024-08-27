using Unity.Netcode;
using UnityEngine;

public class SoccerBallProp : GrabbableObject
{
    [Space(5f)]
    public float ballHitUpwardAmount = 0.5f;

    public AnimationCurve grenadeFallCurve;

    public AnimationCurve grenadeVerticalFallCurve;

    public AnimationCurve soccerBallVerticalOffset;

    public AnimationCurve grenadeVerticalFallCurveNoBounce;

    private Ray soccerRay;

    private RaycastHit soccerHit;

    private int soccerBallMask = 301992193;

    private int previousPlayerHit;

    private float hitTimer;

    public AudioClip[] hitBallSFX;

    public AudioClip[] ballHitFloorSFX;

    public AudioSource soccerBallAudio;

    public override void ActivatePhysicsTrigger(Collider other)
    {
    }

    public override void FallWithCurve()
    {
    }

    public override void PlayDropSFX()
    {
    }
}