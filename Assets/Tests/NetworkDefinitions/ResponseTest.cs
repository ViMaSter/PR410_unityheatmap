using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using WebSocketSharp;

public class ResponseTest
{
    [Test]
    public void PlayerData()
    {
        Game.PlayerData playerData;

        playerData = JsonUtility.FromJson<Game.PlayerData>("{\"name\":\"Snasen\"}");
        Assert.That(playerData.Name, Does.Match("Snasen"));
        Assert.AreEqual(playerData.Position.x, 0.0f);
        Assert.AreEqual(playerData.Position.y, 0.0f);
        Assert.AreEqual(playerData.ColorHex, new Color32(0, 0, 0, 255));

        playerData = JsonUtility.FromJson<Game.PlayerData>("{\"position\":{\"x\":1.0}}");
        Assert.IsEmpty(playerData.Name);
        Assert.AreEqual(playerData.Position.x, 1.0f);
        Assert.AreEqual(playerData.Position.y, 0.0f);
        Assert.AreEqual(playerData.ColorHex, new Color32(0, 0, 0, 255));

        playerData = JsonUtility.FromJson<Game.PlayerData>("{\"position\":{\"y\":10.0}}");
        Assert.IsEmpty(playerData.Name);
        Assert.AreEqual(playerData.Position.x, 0.0f);
        Assert.AreEqual(playerData.Position.y, 10.0f);
        Assert.AreEqual(playerData.ColorHex, new Color32(0, 0, 0, 255));

        playerData = JsonUtility.FromJson<Game.PlayerData>("{\"colorHex\":49407}");
        Assert.IsEmpty(playerData.Name);
        Assert.AreEqual(playerData.Position.x, 0.0f);
        Assert.AreEqual(playerData.Position.y, 0.0f);
        Assert.AreEqual(playerData.ColorHex, new Color32(0, 192, 255, 255));

        playerData = JsonUtility.FromJson<Game.PlayerData>("{\"position\":{\"x\":1.0, \"y\":10.0}}");
        Assert.IsEmpty(playerData.Name);
        Assert.AreEqual(playerData.Position.x, 1.0f);
        Assert.AreEqual(playerData.Position.y, 10.0f);
        Assert.AreEqual(playerData.ColorHex, new Color32(0, 0, 0, 255));

        playerData = JsonUtility.FromJson<Game.PlayerData>("{\"name\":\"Don't look now\", \"position\":{\"x\":1.0, \"y\":10.0}, \"colorHex\":49407}");
        Assert.AreEqual(playerData.Name, "Don't look now");
        Assert.AreEqual(playerData.Position.x, 1.0f);
        Assert.AreEqual(playerData.Position.y, 10.0f);
        Assert.AreEqual(playerData.ColorHex, new Color32(0, 192, 255, 255));
    }

    [Test]
    public void SessionData()
    {
        Game.SessionData sessionData;

        sessionData = JsonUtility.FromJson<Game.SessionData>("{\"mapName\":\"castle\"}");
        Assert.That(sessionData.MapName, Does.Match("castle"));
        Assert.AreEqual(sessionData.Timelimit, 0L);
        Assert.AreEqual(sessionData.CurrentMatchStart, 0L);

        sessionData = JsonUtility.FromJson<Game.SessionData>("{\"timelimit\":12345678987654321}");
        Assert.IsEmpty(sessionData.MapName);
        Assert.AreEqual(sessionData.Timelimit, 12345678987654321L);
        Assert.AreEqual(sessionData.CurrentMatchStart, 0L);

        sessionData = JsonUtility.FromJson<Game.SessionData>("{\"currentMatchStart\":12345678087654321}");
        Assert.IsEmpty(sessionData.MapName);
        Assert.AreEqual(sessionData.Timelimit, 0L);
        Assert.AreEqual(sessionData.CurrentMatchStart, 12345678087654321L);

        sessionData = JsonUtility.FromJson<Game.SessionData>("{\"mapName\":\"castle\", \"timelimit\":12345678987654321, \"currentMatchStart\":12345678087654321}");
        Assert.That(sessionData.MapName, Does.Match("castle"));
        Assert.AreEqual(sessionData.Timelimit, 12345678987654321L);
        Assert.AreEqual(sessionData.CurrentMatchStart, 12345678087654321L);
    }

    [Test]
    public void CreateSession()
    {
        NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData> sessionJoinResponse;
        sessionJoinResponse = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData>>("{\"command\":\"joinSession\",\"session\":{\"mapName\":\"castle\",\"timelimit\":180000,\"currentMatchStart\":0},\"player\":{\"name\":\"U2L8\", \"position\":{\"x\":-1.0, \"y\":10.0}, \"colorHex\":49407}}");
       
        Assert.That(sessionJoinResponse.Command, Does.Match("joinSession"));
        Assert.That(sessionJoinResponse.Player.Name, Does.Match("U2L8"));
        Assert.AreEqual(sessionJoinResponse.Player.Position.x, -1.0f);
        Assert.AreEqual(sessionJoinResponse.Player.Position.y, 10.0f);
        Assert.AreEqual(sessionJoinResponse.Player.ColorHex, new Color32(0, 192, 255, 255));
        // @TODO See test above

        Assert.That(sessionJoinResponse.Command, Does.Not.Match("updateSession"));
        Assert.That(sessionJoinResponse.Player.Name, Does.Not.Match("NothingPersonal"));
        Assert.AreNotEqual(sessionJoinResponse.Player.Position.x, 0.0f);
        Assert.AreNotEqual(sessionJoinResponse.Player.Position.y, 20.0f);
        Assert.AreNotEqual(sessionJoinResponse.Player.ColorHex, new Color32(255, 193, 0, 255));
        // @TODO See test above
    }

}
