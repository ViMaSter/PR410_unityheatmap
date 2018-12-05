using NUnit.Framework;
using UnityEngine;

public class RequestTest
{
    [Test]
    public void SessionData()
    {
    	Assert.That(
    		"{\"mapName\":\"castle\",\"timelimit\":180000,\"currentMatchStart\":0}", Does.Match(JsonUtility.ToJson(
    		new Game.SessionData("castle", 3L * 60L * 1000L, 0L)
		)));
    	Assert.That(
    		"{\"mapName\":\"castle\", \"timelimit\":180000, \"currentMatchStart\":0}", Does.Not.Match(JsonUtility.ToJson(
    		new Game.SessionData("desert", 3L * 60L * 1000L, 1L)
		)));
    }

    [Test]
    public void CreateSession()
    {
        Debug.Log(JsonUtility.ToJson(new NetworkDefinitions.Request.CreateSession<Game.SessionData, Game.PlayerData>(new Game.SessionData("castle", 12345678987654321L, 12345678987654321L),
        new Game.PlayerData("NowYouSeeMe", new Vector2(0, 0), new Color32(0, 192, 255, 255)))));

    	Assert.That(
    		"{\"command\":\"createSession\",\"session\":{\"mapName\":\"castle\",\"timelimit\":180000,\"currentMatchStart\":0},\"player\":{\"name\":\"NowYouSeeMe\",\"position\":{\"x\":0.0,\"y\":0.0},\"colorHex\":49407}}", Does.Match(JsonUtility.ToJson(
    		new NetworkDefinitions.Request.CreateSession<Game.SessionData, Game.PlayerData>(new Game.SessionData("castle", 3L * 60L * 1000L, 0L), new Game.PlayerData("NowYouSeeMe", new Vector2(0, 0), new Color32(0, 192, 255, 255)))
		)));
    	Assert.That(
    		"{\"command\":\"createSession\",\"session\":{\"mapName\":\"castle\",\"timelimit\":180000,\"currentMatchStart\":0},\"player\":{\"name\":\"NowYouSeeMe\",\"position\":{\"x\":0.0,\"y\":0.0},\"colorHex\":49407}}", Does.Not.Match(JsonUtility.ToJson(
    		new NetworkDefinitions.Request.CreateSession<Game.SessionData, Game.PlayerData>(new Game.SessionData("desert", 2L * 60L * 1000L, 1L), new Game.PlayerData("NowYouDont", new Vector2(-1, -1), new Color32(255, 193, 0, 255)))
		)));
    }

    [Test]
    public void UpdateSession()
    {
        Assert.That(
            "{\"command\":\"updateSession\",\"session\":{\"mapName\":\"castle\",\"timelimit\":180000,\"currentMatchStart\":0},\"player\":{\"name\":\"NowYouSeeMe\",\"position\":{\"x\":0.0,\"y\":0.0},\"colorHex\":49407}}", Does.Match(JsonUtility.ToJson(
            new NetworkDefinitions.Request.UpdateSession<Game.SessionData, Game.PlayerData>(new Game.SessionData("castle", 3L * 60L * 1000L, 0L), new Game.PlayerData("NowYouSeeMe", new Vector2(0, 0), new Color32(0, 192, 255, 255)))
        )));
        Assert.That(
            "{\"command\":\"updateSession\",\"session\":{\"mapName\":\"castle\",\"timelimit\":180000,\"currentMatchStart\":0},\"player\":{\"name\":\"NowYouSeeMe\",\"position\":{\"x\":0.0,\"y\":0.0},\"colorHex\":49407}}", Does.Not.Match(JsonUtility.ToJson(
            new NetworkDefinitions.Request.UpdateSession<Game.SessionData, Game.PlayerData>(new Game.SessionData("desert", 2L * 60L * 1000L, 1L), new Game.PlayerData("NowYouDont", new Vector2(-1, -1), new Color32(255, 193, 0, 255)))
        )));
    }

    [Test]
    public void UpdatePlayer()
    {
        Assert.That(
            "{\"command\":\"updatePlayer\",\"player\":{\"name\":\"NowYouSeeMe\",\"position\":{\"x\":0.0,\"y\":0.0},\"colorHex\":49407}}", Does.Match(JsonUtility.ToJson(
            new NetworkDefinitions.Request.UpdatePlayer<Game.PlayerData>(new Game.PlayerData("NowYouSeeMe", new Vector2(0, 0), new Color32(0, 192, 255, 255)))
        )));
        Assert.That(
            "{\"command\":\"updateSPlayer\",\"player\":{\"name\":\"NowYouSeeMe\",\"position\":{\"x\":0.0,\"y\":0.0},\"colorHex\":49407}}", Does.Not.Match(JsonUtility.ToJson(
            new NetworkDefinitions.Request.UpdatePlayer<Game.PlayerData>(new Game.PlayerData("NowYouDont", new Vector2(-1, -1), new Color32(255, 193, 0, 255)))
        )));
    }


}
