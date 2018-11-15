using UnityEngine;

using WebSocketSharp;

public class SessionConnector : MonoBehaviour
{
	WebSocket websocketConnection;

	void Awake()
	{
		websocketConnection = new WebSocket("ws://echo.websocket.org");
		websocketConnection.Log.Level = LogLevel.Trace;
		websocketConnection.Log.Output = OnLog;
		websocketConnection.OnOpen += OnOpen;
		websocketConnection.OnClose += OnClose;
		websocketConnection.OnMessage += OnMessage;
		websocketConnection.OnError += OnError;
		websocketConnection.Connect ();
	}

	void OnLog(WebSocketSharp.LogData data, string stringData)
	{
		Debug.Log(data);
		Debug.Log(stringData);
	}

	void OnOpen(object sender, System.EventArgs e)
	{
		Debug.Log("Connection opened!");
	}

	void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
	{
		Debug.Log("OnMessage: " + e.Data);
	}

	void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
	{
		Debug.Log("OnError: " + e.Message);
	}

	void OnClose(object sender, WebSocketSharp.CloseEventArgs e)
	{
		Debug.Log("OnClose: " + e.Code + e.Reason);
	}

	void SendWebsocketMessage(string message)
	{
		Debug.Log("Sent: " + message);
		websocketConnection.Send(message);
	}
	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SendWebsocketMessage("BALUS");
		}
	}
}
