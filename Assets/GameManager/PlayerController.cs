using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Range(0.0001f, 3f)]
	public float movementSpeed = 1.5f;
	[Range(0.01f, 1f)]
	public float falloutRatio = 0.15f;
	[Range(0f, 1f)]
	public float rotationLerp = 0.3f;

	private Vector2 currentMovement = Vector3.zero;

	private new Rigidbody2D rigidbody2D;

	void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	public Vector2 GetVelocity()
	{
		return currentMovement;
	}

	void FixedUpdate()
	{
		if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f)
		{
			currentMovement += new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * movementSpeed;
		}

		Debug.Log(Mathf.Atan2(currentMovement.y, currentMovement.x));

		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(currentMovement.y, currentMovement.x) * Mathf.Rad2Deg - 90)), rotationLerp);
		rigidbody2D.velocity = currentMovement;
		currentMovement *= 1 - falloutRatio;
	}
}
