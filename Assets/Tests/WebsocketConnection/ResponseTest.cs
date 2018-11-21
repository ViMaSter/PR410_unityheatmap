using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using WebSocketSharp;

public class ResponseTest
{
    [Test]
    public void SessionData()
    {
        Game.SessionData sessionData;

        sessionData = JsonUtility.FromJson<Game.SessionData>("{\"playerPositionX\":1.0}");
        Assert.AreEqual(sessionData.PlayerPositionX, 1.0f);
        Assert.AreNotEqual(sessionData.PlayerPositionY, -1.0f);
        Assert.AreNotEqual(sessionData.Health, -1.0f);

        sessionData = JsonUtility.FromJson<Game.SessionData>("{\"playerPositionY\":10.0}");
        Assert.AreNotEqual(sessionData.PlayerPositionX, -1.0f);
        Assert.AreEqual(sessionData.PlayerPositionY, 10.0f);
        Assert.AreNotEqual(sessionData.Health, -1.0f);

        sessionData = JsonUtility.FromJson<Game.SessionData>("{\"health\":40.0}");
        Assert.AreNotEqual(sessionData.PlayerPositionX, -1.0f);
        Assert.AreNotEqual(sessionData.PlayerPositionY, -1.0f);
        Assert.AreEqual(sessionData.Health, 40.0f);

        sessionData = JsonUtility.FromJson<Game.SessionData>("{\"playerPositionX\":1.0, \"playerPositionY\":10.0}");
        Assert.AreEqual(sessionData.PlayerPositionX, 1.0f);
        Assert.AreEqual(sessionData.PlayerPositionY, 10.0f);
        Assert.AreNotEqual(sessionData.Health, -1.0f);

        sessionData = JsonUtility.FromJson<Game.SessionData>("{\"playerPositionX\":1.0, \"playerPositionY\":10.0, \"health\":40.0}");
        Assert.AreEqual(sessionData.PlayerPositionX, 1.0f);
        Assert.AreEqual(sessionData.PlayerPositionY, 10.0f);
        Assert.AreEqual(sessionData.Health, 40.0f);
    }

    [Test]
    public void CreateSession()
    {
        NetworkDefinitions.Response.SessionJoin<Game.SessionData> sessionJoinResponse;
        sessionJoinResponse = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData>>("{\"command\":\"joinSession\",\"session\":{\"playerPositionX\":-1.0,\"playerPositionY\":10.0,\"health\":20.0}}");
        Assert.That(sessionJoinResponse.Command, Does.Match("joinSession"));
        Assert.AreEqual(sessionJoinResponse.Session.PlayerPositionX, -1.0f);
        Assert.AreEqual(sessionJoinResponse.Session.PlayerPositionY, 10.0f);
        Assert.AreEqual(sessionJoinResponse.Session.Health, 20.0f);

        sessionJoinResponse = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData>>("{\"command\":\"leaveSession\",\"session\":{\"playerPositionX\":0.0,\"playerPositionY\":30.0,\"health\":40.0}}");
        Assert.That(sessionJoinResponse.Command, Does.Not.Match("createSession"));
        Assert.AreNotEqual(sessionJoinResponse.Session.PlayerPositionX, -1.0f);
        Assert.AreNotEqual(sessionJoinResponse.Session.PlayerPositionY, 10.0f);
        Assert.AreNotEqual(sessionJoinResponse.Session.Health, 20.0f);
    }


}
