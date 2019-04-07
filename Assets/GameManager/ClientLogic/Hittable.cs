using UnityEngine;

public class Hittable : MonoBehaviour
{
	[Range(0.0f, 20000f)]
	public float intensity = 1.0f;
	private new Rigidbody2D rigidbody2D;

	void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	public void Hit(Vector2 direction)
	{
		HeatmapRecorder.Instance.RecordDeath(transform.position);
		Destroy(transform.parent.gameObject);
		// TODO @VM Write to heatmapdata
	}
}
