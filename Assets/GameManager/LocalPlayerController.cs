using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerController : MonoBehaviour {

	public Pawn pawn;

	void Update()
	{
		if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f)
		{
			pawn.Move(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
		}

		if (Input.GetMouseButtonDown(0))
		{
			pawn.Shoot(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}
	}
}
