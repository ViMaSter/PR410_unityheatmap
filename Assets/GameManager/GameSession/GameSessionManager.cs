using System.Collections.Generic;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
	// player prefabs
	public Transform LocalPlayerPrefab;
	private Transform LocalPlayerInstance;
	public Transform Spawn;

	void Update()
	{
		if (LocalPlayerInstance == null)
		{
			LocalPlayerInstance = GameObject.Instantiate(LocalPlayerPrefab, Spawn.position, Quaternion.identity);
		}
	}
}
