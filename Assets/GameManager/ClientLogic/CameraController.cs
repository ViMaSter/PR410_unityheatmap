using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform target;
	private Pawn pawn;
	private new Transform transform;
	private Vector3 defaultOffsetToTarget = Vector3.zero;

	[Range(0f, 1f)]
	public float followLerp = 0.3f;
	[Range(0f, 1f)]
	public float velocityBias = 0.3f;

	void Awake()
	{
		transform = GetComponent<Transform>();
		pawn = target.GetComponent<Pawn>();
		defaultOffsetToTarget = transform.position - target.position;
	}

	void FixedUpdate()
	{
		Vector3 targetPosition = target.position;
		targetPosition += defaultOffsetToTarget;
		Vector2 targetVelocity = pawn.GetVelocity() * velocityBias;
		targetPosition += new Vector3(targetVelocity.x, targetVelocity.y, 0);

		transform.position = Vector3.Lerp(transform.position, targetPosition, followLerp);
	}
}
