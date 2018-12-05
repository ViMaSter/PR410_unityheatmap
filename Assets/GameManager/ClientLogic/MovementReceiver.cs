using UnityEngine;

public class MovementReceiver : MonoBehaviour
{
	[SerializeField] 
	private Transform target;
	[SerializeField]
	[Range(0.0f, 1.0f)] 
	private float interpolation = 0.98f;

	public void UpdatePosition(Vector2 positionUpdate)
	{
		target.position = Vector3.Lerp(target.position, positionUpdate, interpolation);
	}
}
