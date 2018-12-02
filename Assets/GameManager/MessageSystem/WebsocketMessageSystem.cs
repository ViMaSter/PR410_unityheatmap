using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebsocketMessageSystem : MonoBehaviour
{
	// MonoBehaviour-esque Singleton pattern
	//
	// Instantiate this object at scene-start - it will automatically allow you to send and
	// receive messages (see `WebsocketMessageSystem API` in this file)
	static private WebsocketMessageSystem _instance = null;
	public static WebsocketMessageSystem Instance
	{
		get 
		{
			return _instance;
		}
		set
		{
			if (_instance != null)
			{
				Debug.LogErrorFormat("WebsocketMessageSystem already initialized - attempting to replace existing system:\r\nExisting:{0}\r\nNew:{1}", _instance, value);
			}
			_instance = value;
		}
	}

	private void InitializeSingleton()
	{
		WebsocketMessageSystem.Instance = this;
	}

	/// WebsocketMessageSystem API
	///
	/// Send messages by calling WebsocketMessageSystem.SendJSONMessage(string JSONMessage) (static)
	///  optionally handle the return value of SendJSONMessage - a boolean indicating succcess
	///
	/// Subscribe to events by calling `WebsocketMessageSystem.AddMessageListener(yourfunction)` (static)
	/// `with `yourfunction` using a `void(string JSONMessage)`-signature.
	public bool SendJSONMessage(string message)
	{
		if (websocketConnection.ReadyState != WebSocketState.Open)
		{
			Debug.LogError("No connection established! Cannot send message! Current readyState: " + websocketConnection.ReadyState);
			return false;
		}
		Debug.LogFormat("Sent JSON message: '{0}'", message);
		websocketConnection.Send(message);
		return true;
	}

    public delegate void MessageListener(string JSONMessage);
    private event MessageListener OnJSONMessage;
	public void AddMessageListener(MessageListener newEvent)
	{
		WebsocketMessageSystem.Instance.OnJSONMessage += newEvent;
	}

	public void ClearMessageListener()
	{
	    foreach (Delegate delegateInstance in WebsocketMessageSystem.Instance.OnJSONMessage.GetInvocationList())
	    {
	        WebsocketMessageSystem.Instance.OnJSONMessage -= (MessageListener)delegateInstance;
	    }
	}

	// Websocket implementation
	public string hostname = "127.0.0.1";
	public int port = 7000;
	private WebSocket websocketConnection;

	private void InitializeWebsocket()
	{
		websocketConnection = new WebSocket("ws://"+hostname+":"+port);
		websocketConnection.Log.Level = LogLevel.Trace;
		websocketConnection.Log.Output = OnLog;
		websocketConnection.OnMessage += OnMessage;
		websocketConnection.Connect ();
	}

	private void OnLog(WebSocketSharp.LogData data, string logPath)
	{
		Debug.Log(data);
	}

	private void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
	{
		Debug.LogFormat("Received network message: '{0}'", e.Data);
		WebsocketMessageSystem.Instance.OnJSONMessage(e.Data);
	}

	// MonoBehaviour
	private void Awake()
	{
		InitializeSingleton();
		InitializeWebsocket();
	}
}
