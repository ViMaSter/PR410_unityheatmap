using UnityEngine;

using WebSocketSharp;

namespace NetworkMessages
{
	[System.Serializable]
	public class SessionJoin
	{
		[SerializeField]
	    private string command = "sessionJoin";

		// valid sessions 0-Int32.MaxValue; -1 = last available session
		[SerializeField]
	    private int sessionID;

	    // without an explicit sessionID specified, join the last available session
	    public SessionJoin(int sessionID = -1)
	    {
			this.sessionID = sessionID;
	    }
	}
}

public class SessionConnector : MonoBehaviour
{
	WebSocket websocketConnection;

	void Awake()
	{
		websocketConnection = new WebSocket("ws://echo.websocket.org");
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
	}

	void ConnectToLastSession(object sender, System.EventArgs e)
	{
		SendWebsocketMessage(JsonUtility.ToJson(new NetworkMessages.SessionJoin()));
	}

	void SendWebsocketMessage(string message)
	{
		Debug.LogFormat("Sent network message: '{0}'", message);
		websocketConnection.Send(message);
	}
}
