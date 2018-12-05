using UnityEngine;

public class LocalPlayerController : MonoBehaviour {

	public Pawn pawn;
	public int networkUpdateFrequencyInMs;
	private int networkPlayerID;

	void AssignNetworkID(int networkID)
	{
		gameObject.name = "LocalPlayer_"+networkID;
		networkPlayerID = networkID;
	}

	void Update()
	{
		if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f) //-V3024
		{
			pawn.Move(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
		}

		if (Input.GetMouseButtonDown(0))
		{
			pawn.Shoot(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}

        InvokeRepeating("SendNetworkMessage", 0, networkUpdateFrequencyInMs * 1000);
	}

	void SendNetworkMessage()
	{
		Game.PlayerData pawnData = pawn.Serialize();

		Debug.Log("Sending player update for " + networkPlayerID + "\r\n"+JsonUtility.ToJson(pawnData));
		WebsocketMessageSystem.Instance.SendJSONMessage(JsonUtility.ToJson(
			new NetworkDefinitions.Request.UpdatePlayer<Game.PlayerData>(pawnData)
		));
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
	}
}
