using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using WebSocketSharp;

public abstract class SessionServerTest
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

		Debug.LogWarning(SessionServerConfig.Host+":"+SessionServerConfig.Port);
		websocketConnection = new WebSocket(SessionServerConfig.Host+":"+SessionServerConfig.Port);
		websocketConnection.OnOpen += (object sender, System.EventArgs e) => {
			SendWebsocketMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.CreateSession<Game.SessionData, Game.PlayerData>(
				new Game.SessionData(
					"castle",
					3L * 60L * 1000L,
					0L
				),
				new Game.PlayerData(
					"NowYouSeeMe",
					new Vector2(0, 0),
					new Color32(0, 192, 255, 255)
				)
			)));
		};
		websocketConnection.OnMessage += (object sender, WebSocketSharp.MessageEventArgs e) =>
		{
			Debug.Log(e.Data);
			NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData> sessionJoinData = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData>>(e.Data);
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
    public void CreateTwoSessionsImpossible()
    {
		int receivedMessages = 0;

		Debug.LogWarning(SessionServerConfig.Host+":"+SessionServerConfig.Port);
		websocketConnection = new WebSocket(SessionServerConfig.Host+":"+SessionServerConfig.Port);
		websocketConnection.OnOpen += (object sender, System.EventArgs e) => {
			SendWebsocketMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.CreateSession<Game.SessionData, Game.PlayerData>(
				new Game.SessionData(
					"castle",
					3L * 60L * 1000L,
					0L
				),
				new Game.PlayerData(
					"NowYouSeeMe",
					new Vector2(0, 0),
					new Color32(0, 192, 255, 255)
				)
			)));
		};
		NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData>[] sessionJoins = new NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData>[2];

		websocketConnection.OnMessage += (object sender, WebSocketSharp.MessageEventArgs e) =>
		{
			sessionJoins[receivedMessages] = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData>>(e.Data);
			receivedMessages++;

			if (receivedMessages == 1)
			{
				SendWebsocketMessage(JsonUtility.ToJson(new NetworkDefinitions.Request.CreateSession<Game.SessionData, Game.PlayerData>(
					new Game.SessionData(
						"castle",
						3L * 60L * 1000L,
						0L
					),
					new Game.PlayerData(
						"NowYouSeeMe",
						new Vector2(0, 0),
						new Color32(0, 192, 255, 255)
					)
				)));
			}
		};

		websocketConnection.Connect ();

		Assert.That(() => {
			try
			{
				Assert.IsTrue(sessionJoins[0].IsValid);
				currentSessionID = sessionJoins[0].SessionID;
				
				Assert.IsFalse(sessionJoins[1].IsValid);
				Assert.AreEqual(sessionJoins[1].Error, 1);
				return true;
			}
			catch (NUnit.Framework.AssertionException e)
			{
				throw e;
			}
			catch (System.Exception e)
			{
				Debug.LogWarning(e);
				return false;
			}
		}, Is.True.After(1000, 100));
    }
}
