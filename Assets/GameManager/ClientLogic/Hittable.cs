using UnityEngine;

public class Hittable : MonoBehaviour
{
	[Range(0.0f, 2000f)]
	public float intensity = 1.0f;
	private new Rigidbody2D rigidbody2D;

	void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	public void Hit(Vector2 direction)
	{
		rigidbody2D.AddForce(direction.normalized * intensity);
	}
}
