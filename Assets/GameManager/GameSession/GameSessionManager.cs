using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
	// player prefabs
	public Transform LocalPlayerPrefab;
	private Transform LocalPlayerInstance;
	public Transform RemotePlayerPrefab;
	private Transform RemotePlayerInstance;

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
	public void HandleMessage(string JSONMessage)
	{
		NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData> sessionJoinData = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData>>(JSONMessage);
		NetworkDefinitions.Response.SessionLeave sessionLeaveData = JsonUtility.FromJson<NetworkDefinitions.Response.SessionLeave>(JSONMessage);
		Debug.Log("aaaa");
		if (sessionJoinData.IsValid)
		{
			OnJoinSession(sessionJoinData);
		}
		else if (sessionLeaveData.IsValid)
		{
			OnLeaveSession(sessionLeaveData);
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
			LocalPlayerInstance = GameObject.Instantiate(LocalPlayerPrefab, Vector3.zero, Quaternion.identity);
			LocalPlayerInstance.SendMessage("AssignNetworkID", currentPlayerID);
		}

		if (currentPlayerID == -1 && LocalPlayerInstance != null)
		{
			Destroy(LocalPlayerInstance.gameObject);
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

	string requestedSessionID = "";
	void OnGUI()
	{
		switch (currentSessionState)
		{
			case SessionState.NOSESSION:
				if (GUI.Button(new Rect(0, 0, Screen.width / 2, Screen.height / 4), "Create Session"))
				{
					RequestCreateSession();
				}

				requestedSessionID = GUI.TextField(new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height / 4), requestedSessionID);
				if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 4, Screen.width / 2, Screen.height / 4), "Join Session"))
				{
					RequestJoinSession(int.Parse(requestedSessionID));
				}
				break;
			case SessionState.INSESSION:
				GUI.Label(new Rect(0, 0, Screen.width, 20), "In session "+currentSessionID);
				if (GUI.Button(new Rect(0, 20, Screen.width / 2, Screen.height / 2), "Leave Session"))
				{
					RequestLeaveSession();
				}
				break;
		}
	}
}
