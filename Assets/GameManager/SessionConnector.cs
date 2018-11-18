using UnityEngine;
using WebSocketSharp;

// supress 'warning CS0414: The private field `[BLANK]' is assigned but its value is never used' messages
//   as they are assigned/read when constructing/parsing JSON messages with JsonUtility.ToJson()/JsonUtility.FromJson<>()
#pragma warning disable 0414

namespace Network
{
	[System.Serializable]
	public abstract class GameData
	{
	}

	namespace Request
	{
		[System.Serializable]
		public class JoinSession
		{
			[SerializeField]
		    private string command = "sessionJoin";

			// valid sessions 0-Int32.MaxValue; -1 = last available session
			[SerializeField]
		    private int sessionID;

		    // without an explicit sessionID specified, join the last available session
		    public JoinSession(int sessionID = -1)
		    {
				this.sessionID = sessionID;
		    }
		}

		[System.Serializable]
		public class CreateSession<CustomGameData> where CustomGameData : Network.GameData
		{
			[SerializeField]
		    private string command = "createSession";
			[SerializeField]
		    private CustomGameData parameters;

		    public CreateSession(CustomGameData parameters)
		    {
		    	Debug.Log(parameters);
		    	this.parameters = parameters;
		    }
		}

		[System.Serializable]
		public class UpdateSession<CustomGameData> where CustomGameData : Network.GameData
		{
			[SerializeField]
		    private string command = "updateSession";
			[SerializeField]
		    private int sessionID;
			[SerializeField]
		    private CustomGameData parameters;

		    // without an explicit sessionID specified, join the last available session
		    public UpdateSession(int sessionID, CustomGameData parameters)
		    {
		    	this.sessionID = sessionID;
		    	this.parameters = parameters;
		    }
		}
	}

	namespace Response
	{
		[System.Serializable]
		public class SessionJoin
		{
			[SerializeField]
		    private string command = "";

			[SerializeField]
		    private int sessionID;
		    public int SessionID
		    {
		    	get 
		    	{
		    		return sessionID;
		    	}
		    }

		    private SessionJoin()
		    {
		    }

		    public bool IsValid
		    {
		    	get
		    	{
			    	return command == "sessionJoin";
		    	}
		    }
		}
	}
}

namespace Game
{
	[System.Serializable]
	public class SessionData : Network.GameData
	{
		[SerializeField]
		private int playerPositionX = -1;
		[SerializeField]
		private int playerPositionY = -1;
		[SerializeField]
		private int health = -1;

	    public SessionData(int playerPositionX, int playerPositionY, int health)
	    {
			this.playerPositionX = playerPositionX;
			this.playerPositionY = playerPositionY;
			this.health = health;
	    }
	}
}

#pragma warning restore

public class SessionConnector : MonoBehaviour
{
	public string hostname = "127.0.0.1";
	public int port = 7000;

	WebSocket websocketConnection;
	int currentSessionID = -1;

	void Awake()
	{
		websocketConnection = new WebSocket("ws://"+hostname+":"+port);
		websocketConnection.Log.Level = LogLevel.Trace;
		websocketConnection.Log.Output = OnLog;
		websocketConnection.OnOpen += ConnectToLastSession;
		websocketConnection.OnMessage += OnMessage;
		websocketConnection.Connect ();
	}

	void OnLog(WebSocketSharp.LogData data, string logPath)
	{
		Debug.Log(data);
	}

	void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
	{
		Debug.LogFormat("Received network message: '{0}'", e.Data);
		Network.Response.SessionJoin sessionJoinData = JsonUtility.FromJson<Network.Response.SessionJoin>(e.Data);
		if (sessionJoinData.IsValid)
		{
			currentSessionID = sessionJoinData.SessionID;
			SendWebsocketMessage(JsonUtility.ToJson(new Network.Request.UpdateSession<Game.SessionData>(currentSessionID, new Game.SessionData(
				currentSessionID,
				50, 60
			))));
		}
	}

	void ConnectToLastSession(object sender, System.EventArgs e)
	{
		SendWebsocketMessage(JsonUtility.ToJson(new Network.Request.CreateSession<Game.SessionData>(new Game.SessionData(
			currentSessionID,
			10, 20
		))));
	}

	void SendWebsocketMessage(string message)
	{
		Debug.LogFormat("Sent network message: '{0}'", message);
		websocketConnection.Send(message);
	}
}
