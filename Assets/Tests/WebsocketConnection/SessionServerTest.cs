using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using WebSocketSharp;

public class SessionServerTest
{
	void SendWebsocketMessage(string message)
	{
		Debug.LogFormat("Sent network message: '{0}'", message);
		websocketConnection.Send(message);
	}

	WebSocket websocketConnection;
	int currentSessionID = -1;

    [Test]
    public void CreateSessionReceiveID()
    {
    	bool receivedMessage = false;

		websocketConnection = new WebSocket("ws://127.0.0.1:7000");
		websocketConnection.OnOpen += (object sender, System.EventArgs e) => {
			SendWebsocketMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.CreateSession<Game.SessionData>(new Game.SessionData(
				currentSessionID,
				10, 20
			))));
		};
		websocketConnection.OnMessage += (object sender, WebSocketSharp.MessageEventArgs e) =>
		{
			Debug.Log(e.Data);
			NetworkDefinitions.Response.SessionJoin<Game.SessionData> sessionJoinData = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData>>(e.Data);
			if (sessionJoinData.IsValid)
			{
				currentSessionID = sessionJoinData.SessionID;
			}
			receivedMessage = true;
		};
		websocketConnection.Connect ();

		Assert.That(() => {
			return receivedMessage;
		}, Is.True.After(1000, 100));
    }

    [Test]
    public void CreateSessionIDIncreases()
    {
    	bool receivedMessage = false;
    	int secondSessionID = -1;

		websocketConnection = new WebSocket("ws://127.0.0.1:7000");
		websocketConnection.OnOpen += (object sender, System.EventArgs e) => {
			SendWebsocketMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.CreateSession<Game.SessionData>(new Game.SessionData(
				currentSessionID,
				10, 20
			))));
		};
		websocketConnection.OnMessage += (object sender, WebSocketSharp.MessageEventArgs e) =>
		{
			if (!receivedMessage)
			{
				NetworkDefinitions.Response.SessionJoin<Game.SessionData> sessionJoinData = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData>>(e.Data);
				if (sessionJoinData.IsValid)
				{
					currentSessionID = sessionJoinData.SessionID;
				}	
				SendWebsocketMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.CreateSession<Game.SessionData>(new Game.SessionData(
					currentSessionID,
					10, 20
				))));
			}
			else
			{
				NetworkDefinitions.Response.SessionJoin<Game.SessionData> sessionJoinData = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData>>(e.Data);
				if (sessionJoinData.IsValid)
				{
					secondSessionID = sessionJoinData.SessionID;
				}	
			}
			receivedMessage = true;
		};
		websocketConnection.Connect ();

		Assert.That(() => {
			return currentSessionID != -1 && secondSessionID != -1 && currentSessionID < secondSessionID;
		}, Is.True.After(1000, 100));
    }
}
