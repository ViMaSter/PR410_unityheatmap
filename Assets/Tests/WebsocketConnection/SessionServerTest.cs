using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using WebSocketSharp;

// change the PrebuildSetup to either LocalSessionServerTest or RemoteSessionServerTest
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
    [PrebuildSetup(typeof(LocalSessionServerTest))]
    public void CreateSessionReceiveID()
    {
    	bool receivedMessage = false;

		Debug.LogWarning(SessionServerConfig.Host+":"+SessionServerConfig.Port);
		websocketConnection = new WebSocket(SessionServerConfig.Host+":"+SessionServerConfig.Port);
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
    [PrebuildSetup(typeof(LocalSessionServerTest))]
    public void CreateTwoSessionsImpossible()
    {
		Assert.IsTrue(false);
		// @TODO Build test, which verifies message ping-pong of creating an invalid session
    }
}
