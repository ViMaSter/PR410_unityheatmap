using UnityEngine;
using WebSocketSharp;

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
		NetworkDefinitions.Response.SessionJoin<Game.SessionData> sessionJoinData = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData>>(e.Data);
		if (sessionJoinData.IsValid)
		{
			currentSessionID = sessionJoinData.SessionID;
			SendWebsocketMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.UpdateSession<Game.SessionData>(currentSessionID, new Game.SessionData(
				currentSessionID,
				50, 60
			))));
		}
	}

	void ConnectToLastSession(object sender, System.EventArgs e)
	{
		SendWebsocketMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.CreateSession<Game.SessionData>(new Game.SessionData(
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
