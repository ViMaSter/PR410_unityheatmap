using System.Collections.Generic;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
	public static GameSessionManager Instance
	{
		get
		{
			return _instance;
		}
	}
	private static GameSessionManager _instance = null;

	[Header("Local player settings")]
	public Transform LocalPlayerPrefab;
	private Transform localPlayerInstance;
	public Transform LocalPlayerInstance
	{
		get
		{
			return localPlayerInstance;
		}
	}
	public Transform LocalPlayerSpawn;

	[Header("Enemy settings")]
	public float spawnRate = 0.5f;
	private float lastSpawnAt = 0.0f;
	public Transform EnemyPrefab;
	public Transform[] EnemySpawns;
	private int currentSpawnIndex = 0;

	void Start()
	{
		if (_instance != null)
		{
			Debug.LogWarning("Another instance of GameSessionManager exists - ensure you're cleaning up properly");
		}
		else
		{
			_instance = this;
		}

		// spawn player if we don't exist yet
		if (localPlayerInstance == null)
		{
			localPlayerInstance = GameObject.Instantiate(LocalPlayerPrefab, LocalPlayerSpawn.position, Quaternion.identity);
		}
	}

	void EnemyLogic()
	{
		if (lastSpawnAt + spawnRate < Time.time)
		{
			// reset timer for next spawn
			lastSpawnAt = Time.time;

			// spawn new pawn + controller combo
			Instantiate(EnemyPrefab, EnemySpawns[currentSpawnIndex].position, Quaternion.identity);

			// move to next spawn point...
			++currentSpawnIndex;
			// ...or reset if we've reached the last one
			if (currentSpawnIndex == EnemySpawns.Length)
			{
				currentSpawnIndex = 0;
			}
		}
	}

	void Update()
	{
		EnemyLogic();
	}
}
