using System.Collections.Generic;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
	// player prefabs
	public Transform LocalPlayerPrefab;
	private Transform LocalPlayerInstance;
	public Transform RemotePlayerPrefab;
	private Transform RemotePlayerInstance;
	public Transform Spawn;

	// connection state information
	public enum SessionState
	{
		INSESSION,
		NOSESSION
	};

    public delegate void StateChangeListener(SessionState previousState, SessionState newState);
	SessionState currentSessionState = SessionState.NOSESSION;


	// connect to message system
	public void Awake()
	{
		WebsocketMessageSystem.Instance.AddMessageListener(this.HandleMessage);
	}

	// session ID
	int currentSessionID = -1;
	int currentPlayerID = -1;
	Queue<int> queuedRemotePlayerIDs = new Queue<int>();
	public void HandleMessage(string JSONMessage)
	{
		NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData> sessionJoinData = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData>>(JSONMessage);
		if (sessionJoinData.IsValid)
		{
			OnJoinSession(sessionJoinData);
			return;
		}
		
		NetworkDefinitions.Response.SessionLeave sessionLeaveData = JsonUtility.FromJson<NetworkDefinitions.Response.SessionLeave>(JSONMessage);
		if (sessionLeaveData.IsValid)
		{
			OnLeaveSession(sessionLeaveData);
			return;
		}
		
		NetworkDefinitions.Response.PlayerJoin<Game.PlayerData> playerJoinData = JsonUtility.FromJson<NetworkDefinitions.Response.PlayerJoin<Game.PlayerData>>(JSONMessage);
		if (playerJoinData.IsValid)
		{
			OnPlayerJoin(playerJoinData);
			return;
		}
		
		NetworkDefinitions.Response.PlayerUpdate<Game.PlayerData> playerUpdateData = JsonUtility.FromJson<NetworkDefinitions.Response.PlayerUpdate<Game.PlayerData>>(JSONMessage);
		if (playerUpdateData.IsValid)
		{
			OnPlayerUpdate(playerUpdateData);
			return;
		}

		else
		{
			Debug.LogWarning("Couldn't handle incoming message: " + JSONMessage);
		}
	}

	void RequestCreateSession()
	{
		WebsocketMessageSystem.Instance.SendJSONMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.CreateSession<Game.SessionData, Game.PlayerData>(
			new Game.SessionData(
				"castle",
				3L * 60L * 1000L,
				((long)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds) * 1000L
			),
			new Game.PlayerData(
				"NowYouSeeMe",
				new Vector2(0, 0),
				new Color32(0, 192, 255, 255)
			)
		)));
		Debug.Log("Create");
	}

	void RequestJoinSession(int sessionID)
	{
		WebsocketMessageSystem.Instance.SendJSONMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.JoinSession(
			sessionID
		)));
		Debug.Log("Join " + sessionID);
	}

	void RequestLeaveSession()
	{
		WebsocketMessageSystem.Instance.SendJSONMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.LeaveSession()));
		Debug.Log("Leave");
	}

	void Update()
	{
		if (currentPlayerID != -1 && LocalPlayerInstance == null)
		{
			LocalPlayerInstance = GameObject.Instantiate(LocalPlayerPrefab, Spawn.position, Quaternion.identity);
			LocalPlayerInstance.SendMessage("AssignNetworkID", currentPlayerID);
		}

		if (currentPlayerID == -1 && LocalPlayerInstance != null)
		{
			Destroy(LocalPlayerInstance.gameObject);
		}

		if (queuedRemotePlayerIDs.Count != 0)
		{
			RemotePlayerInstance = GameObject.Instantiate(RemotePlayerPrefab, Spawn.position, Quaternion.identity);
			RemotePlayerInstance.SendMessage("AssignNetworkID", queuedRemotePlayerIDs.Dequeue());
		}
	}

	void OnJoinSession(NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData> sessionJoinData)
	{
		currentSessionID = sessionJoinData.SessionID;
		currentPlayerID = sessionJoinData.PlayerID;
		currentSessionState = SessionState.INSESSION;

		Debug.LogFormat("Successful join as {0} to session {1}", currentSessionID, currentPlayerID);
	}

	void OnLeaveSession(NetworkDefinitions.Response.SessionLeave sessionLeaveData)
	{
		Debug.Log("Successfully left session" + currentSessionID);

		currentSessionID = -1;
		currentPlayerID = -1;
		currentSessionState = SessionState.NOSESSION;
	}

	void OnPlayerJoin(NetworkDefinitions.Response.PlayerJoin<Game.PlayerData> playerJoinData)
	{
		Debug.Log("Remote player joined, playerID: " + playerJoinData.PlayerID);

		queuedRemotePlayerIDs.Enqueue(playerJoinData.PlayerID);
	}

	void OnPlayerUpdate(NetworkDefinitions.Response.PlayerUpdate<Game.PlayerData> playerUpdateData)
	{
		Debug.Log("Remote player updated, playerID: " + playerUpdateData.PlayerID);
	}

	string requestedSessionID = "";
	void OnGUI()
	{
		switch (currentSessionState)
		{
			case SessionState.NOSESSION:
				if (GUI.Button(new Rect(0, 0, (float)Screen.width / 2, (float)Screen.height / 4), "Create Session"))
				{
					RequestCreateSession();
				}

				requestedSessionID = GUI.TextField(new Rect((float)Screen.width / 2, 0, (float)Screen.width / 2, (float)Screen.height / 4), requestedSessionID);
				if (GUI.Button(new Rect((float)Screen.width / 2, (float)Screen.height / 4, (float)Screen.width / 2, (float)Screen.height / 4), "Join Session"))
				{
					RequestJoinSession(int.Parse(requestedSessionID));
				}
				break;
			case SessionState.INSESSION:
				GUI.Label(new Rect(0, 0, Screen.width, 20), "In session "+currentSessionID);
				if (GUI.Button(new Rect(0, 20, (float)Screen.width / 2, (float)Screen.height / 2), "Leave Session"))
				{
					RequestLeaveSession();
				}
				break;
		}
	}
}
