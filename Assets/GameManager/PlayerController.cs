﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Range(0.0001f, 3f)]
	public float movementSpeed = 1.5f;
	[Range(0.01f, 1f)]
	public float falloutRatio = 0.15f;
	[Range(0f, 1f)]
	public float rotationLerp = 0.3f;
	[Range(0f, 1f)]
	public float shootLerp = 0.75f;
	[Range(0f, 1f)]
	public float shootPause = 0.75f;

	IEnumerator currentFade = null;

	private Vector2 currentMovement = Vector3.zero;

	private new Rigidbody2D rigidbody2D;

	public Transform laser;

	void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	public Vector2 GetVelocity()
	{
		return currentMovement;
	}

	void Update()
	{
		if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f)
		{
			currentMovement += new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * movementSpeed;
		}

		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = transform.position.z;
			Shoot(mousePosition);
		}
	}

	void Shoot(Vector3 target)
	{
		laser.rotation = Quaternion.LookRotation(transform.forward, transform.position - target) * Quaternion.Euler(0, 0, 90);

		RaycastHit2D hit = Physics2D.Raycast(transform.position, target - transform.position, Mathf.Infinity, ~(1 << 9));
		laser.localScale = new Vector3(Vector2.Distance(transform.position, hit.point), 1, 1);
		laser.position = Vector2.Lerp(transform.position, hit.point, 0.5f);
		if (hit.transform && hit.transform.gameObject.layer == 8 && hit.collider != null)
		{
			hit.transform.GetComponent<Hittable>().Hit((new Vector3(hit.point.x, hit.point.y) - transform.position).normalized);
		}

		if (currentFade != null)
		{
			StopCoroutine(currentFade);
		}
		currentFade = Fade();
		StartCoroutine(currentFade);
	}

	IEnumerator Fade()
	{
		SpriteRenderer renderer = laser.GetComponent<SpriteRenderer>();

		Color newColor = renderer.color;
		newColor.a = 1.0f;

		renderer.color = newColor;
	
		yield return new WaitForSeconds(shootPause);

		while (renderer.color.a > 0)
		{
			newColor.a = Mathf.Lerp(renderer.color.a, 0, shootLerp);
	
			renderer.color = newColor;
			yield return new WaitForSeconds(0.025f);
		}

		currentFade = null;
	}

	void OnDrawGizmos()
	{
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = transform.position.z;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(mousePosition, transform.position);
	}

	void FixedUpdate()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(currentMovement.y, currentMovement.x) * Mathf.Rad2Deg - 90)), rotationLerp);
		rigidbody2D.velocity = currentMovement;
		currentMovement *= 1 - falloutRatio;
	}
}
