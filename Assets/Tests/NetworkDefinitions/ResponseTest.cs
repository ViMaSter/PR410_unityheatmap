using NUnit.Framework;
using UnityEngine;

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
    public void SessionJoin()
    {
        NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData> sessionJoinResponse;
        sessionJoinResponse = JsonUtility.FromJson<NetworkDefinitions.Response.SessionJoin<Game.SessionData, Game.PlayerData>>("{\"command\":\"sessionJoin\",\"error\":0,\"sessionID\":25,\"playerID\":1,\"session\":{\"mapName\":\"castle\",\"timelimit\":180000,\"currentMatchStart\":0},\"player\":{\"name\":\"U2L8\", \"position\":{\"x\":-1.0, \"y\":10.0}, \"colorHex\":49407}}");
       
        Assert.IsTrue(sessionJoinResponse.IsValid);
        Assert.That(sessionJoinResponse.Command, Does.Match("sessionJoin"));

        Assert.AreEqual(sessionJoinResponse.SessionID, 25L);
        Assert.AreEqual(sessionJoinResponse.PlayerID, 1L);
        
        Assert.That(sessionJoinResponse.Session.MapName, Does.Match("castle"));
        Assert.AreEqual(sessionJoinResponse.Session.Timelimit, 180000L);
        Assert.AreEqual(sessionJoinResponse.Session.CurrentMatchStart, 0L);
        
        Assert.That(sessionJoinResponse.Player.Name, Does.Match("U2L8"));
        Assert.AreEqual(sessionJoinResponse.Player.Position.x, -1.0f);
        Assert.AreEqual(sessionJoinResponse.Player.Position.y, 10.0f);
        Assert.AreEqual(sessionJoinResponse.Player.ColorHex, new Color32(0, 192, 255, 255));

        Assert.That(sessionJoinResponse.Command, Does.Not.Match("sessionUpdate"));

        Assert.AreNotEqual(sessionJoinResponse.SessionID, 24L);
        Assert.AreNotEqual(sessionJoinResponse.PlayerID, 0L);
        
        Assert.That(sessionJoinResponse.Session.MapName, Does.Not.Match("desert"));
        Assert.AreNotEqual(sessionJoinResponse.Session.Timelimit, 180001L);
        Assert.AreNotEqual(sessionJoinResponse.Session.CurrentMatchStart, 1L);
        
        Assert.That(sessionJoinResponse.Player.Name, Does.Not.Match("NothingPersonal"));
        Assert.AreNotEqual(sessionJoinResponse.Player.Position.x, 0.0f);
        Assert.AreNotEqual(sessionJoinResponse.Player.Position.y, 20.0f);
        Assert.AreNotEqual(sessionJoinResponse.Player.ColorHex, new Color32(255, 193, 0, 255));
    }

    [Test]
    public void PlayerJoin()
    {
        NetworkDefinitions.Response.PlayerJoin<Game.PlayerData> playerJoinResponse;
        playerJoinResponse = JsonUtility.FromJson<NetworkDefinitions.Response.PlayerJoin<Game.PlayerData>>("{\"command\":\"playerJoin\",\"playerID\":27,\"error\":0,\"player\":{\"name\":\"U2L8\", \"position\":{\"x\":-1.0, \"y\":10.0}, \"colorHex\":49407}}");
       
        Assert.IsTrue(playerJoinResponse.IsValid);
        
        Assert.That(playerJoinResponse.Command, Does.Match("playerJoin"));
        Assert.AreEqual(playerJoinResponse.PlayerID, 27);

        Assert.That(playerJoinResponse.Player.Name, Does.Match("U2L8"));
        Assert.AreEqual(playerJoinResponse.Player.Position.x, -1.0f);
        Assert.AreEqual(playerJoinResponse.Player.Position.y, 10.0f);
        Assert.AreEqual(playerJoinResponse.Player.ColorHex, new Color32(0, 192, 255, 255));

        Assert.That(playerJoinResponse.Command, Does.Not.Match("playerUpdate"));
        Assert.AreNotEqual(playerJoinResponse.PlayerID, 28);
        
        Assert.That(playerJoinResponse.Player.Name, Does.Not.Match("NothingPersonal"));
        Assert.AreNotEqual(playerJoinResponse.Player.Position.x, 0.0f);
        Assert.AreNotEqual(playerJoinResponse.Player.Position.y, 20.0f);
        Assert.AreNotEqual(playerJoinResponse.Player.ColorHex, new Color32(255, 193, 0, 255));
    }

    [Test]
    public void PlayerUpdate()
    {
        NetworkDefinitions.Response.PlayerUpdate<Game.PlayerData> playerUpdateResponse;
        playerUpdateResponse = JsonUtility.FromJson<NetworkDefinitions.Response.PlayerUpdate<Game.PlayerData>>("{\"command\":\"playerUpdate\",\"playerID\":27,\"error\":0,\"player\":{\"name\":\"U2L8\", \"position\":{\"x\":-1.0, \"y\":10.0}, \"colorHex\":49407}}");
       
        Assert.IsTrue(playerUpdateResponse.IsValid);

        Assert.That(playerUpdateResponse.Command, Does.Match("playerUpdate"));
        Assert.AreEqual(playerUpdateResponse.PlayerID, 27);
        
        Assert.That(playerUpdateResponse.Player.Name, Does.Match("U2L8"));
        Assert.AreEqual(playerUpdateResponse.Player.Position.x, -1.0f);
        Assert.AreEqual(playerUpdateResponse.Player.Position.y, 10.0f);
        Assert.AreEqual(playerUpdateResponse.Player.ColorHex, new Color32(0, 192, 255, 255));

        Assert.That(playerUpdateResponse.Command, Does.Not.Match("playerJoin"));
        Assert.AreNotEqual(playerUpdateResponse.PlayerID, 28);

        Assert.That(playerUpdateResponse.Player.Name, Does.Not.Match("NothingPersonal"));
        Assert.AreNotEqual(playerUpdateResponse.Player.Position.x, 0.0f);
        Assert.AreNotEqual(playerUpdateResponse.Player.Position.y, 20.0f);
        Assert.AreNotEqual(playerUpdateResponse.Player.ColorHex, new Color32(255, 193, 0, 255));
    }

}
