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

	private int soccerBallMask = 369101057;

	private int previousPlayerHit;

	private float hitTimer;

	public AudioClip[] hitBallSFX;

	public AudioClip[] ballHitFloorSFX;

	public AudioSource soccerBallAudio;

	public Transform ballCollider;
    
	public override void Start()
	{
	}

	public override void ActivatePhysicsTrigger(Collider other)
	{
	}

	//public Vector3 GetSoccerKickDestination(Vector3 hitFromPosition)
	//{
	//}

	public void BeginKickBall(Vector3 hitFromPosition, bool hitByEnemy)
	{
	}

	[ServerRpc(RequireOwnership = false)]
	public void KickBallServerRpc(Vector3 dest, int playerWhoKicked, bool setInElevator, bool setInShipRoom)
    {		
    }

	[ClientRpc]
	public void KickBallClientRpc(Vector3 dest, int playerWhoKicked, bool setInElevator, bool setInShipRoom)
    {
    }

	private void KickBallLocalClient(Vector3 destinationPos, bool setInElevator, bool setInShipRoom)
	{
	}

	public override void FallWithCurve()
	{
	}

	public override void PlayDropSFX()
	{
	}
}