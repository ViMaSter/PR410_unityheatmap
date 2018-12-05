using UnityEngine;

public class RemotePlayerController : MonoBehaviour {

	public Pawn pawn;
	private int networkPlayerID;
	public Vector2 networkPosition;

	void AssignNetworkID(int networkID)
	{
		gameObject.name = "RemotePlayer_"+networkID;
		networkPlayerID = networkID;
	}

	void Awake()
	{
		// supress movement based on network messages, but update the rotation still
		pawn.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
		WebsocketMessageSystem.Instance.AddMessageListener(this.OnMessageReceived);
	}

	void Update()
	{
		// we're supressing movement but are still converned about movemend to render rotation
		pawn.Move((networkPosition - (Vector2)pawn.transform.position).normalized);
		pawn.transform.position = networkPosition;
	}

	void OnUpdatePlayer(Vector2 position)
	{
		networkPosition = position;
	}

	void OnMessageReceived(string JSONMessage)
	{
		NetworkDefinitions.Response.PlayerUpdate<Game.PlayerData> playerUpdateData = JsonUtility.FromJson<NetworkDefinitions.Response.PlayerUpdate<Game.PlayerData>>(JSONMessage);
		if (!playerUpdateData.IsValid)
		{
			Debug.Log("Received message that couldn't be parsed to an update method");
			return;
		}
		if (playerUpdateData.PlayerID != networkPlayerID)
		{
			Debug.Log("Received updatePlayer we're not concerned about (listening for ID "+networkPlayerID+", received "+playerUpdateData.PlayerID+")");
			return;
		}

		Debug.Log("Handled update for player "+networkPlayerID);
		OnUpdatePlayer(playerUpdateData.Player.Position);
	}
}