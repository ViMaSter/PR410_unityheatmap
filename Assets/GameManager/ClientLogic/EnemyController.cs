using UnityEngine;

public class EnemyController : MonoBehaviour {

	public Pawn pawn;

	public Pawn localPlayer;

	void Awake()
	{
		gameObject.name = "Enemy";
		localPlayer = GameSessionManager.Instance.LocalPlayerInstance.GetComponentInChildren<Pawn>();
	}

	void Update()
	{
		pawn.Move(new Vector2(
			localPlayer.transform.position.x - pawn.transform.position.x,
			localPlayer.transform.position.y - pawn.transform.position.y
		));
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(pawn.transform.position, localPlayer.transform.position);
	}
}
