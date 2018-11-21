using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using WebSocketSharp;

public class RequestTest
{
    [Test]
    public void SessionData()
    {
    	Assert.That(
    		"{\"playerPositionX\":-1.0,\"playerPositionY\":10.0,\"health\":20.0}", Does.Match(JsonUtility.ToJson(
    		new Game.SessionData(-1.0f, 10.0f, 20.0f)
		)));
    	Assert.That(
    		"{\"playerPositionX\":-1.0,\"playerPositionY\":10.0,\"health\":20.0}", Does.Not.Match(JsonUtility.ToJson(
    		new Game.SessionData(0.0f, 30.0f, 40.0f)
		)));
    }

    [Test]
    public void CreateSession()
    {
    	Assert.That(
    		"{\"command\":\"createSession\",\"parameters\":{\"playerPositionX\":-1.0,\"playerPositionY\":10.0,\"health\":20.0}}", Does.Match(JsonUtility.ToJson(
    		new NetworkDefinitions.Request.CreateSession<Game.SessionData>(new Game.SessionData(-1.0f, 10.0f, 20.0f))
		)));
    	Assert.That(
    		"{\"command\":\"createSession\",\"parameters\":{\"playerPositionX\":-1.0,\"playerPositionY\":10.0,\"health\":20.0}}", Does.Not.Match(JsonUtility.ToJson(
    		new NetworkDefinitions.Request.CreateSession<Game.SessionData>(new Game.SessionData(0.0f, 30.0f, 40.0f))
		)));
    }

    [Test]
    public void UpdateSession()
    {
    	Assert.That(
    		"{\"command\":\"updateSession\",\"sessionID\":-1,\"parameters\":{\"playerPositionX\":-1.0,\"playerPositionY\":10.0,\"health\":20.0}}", Does.Match(JsonUtility.ToJson(
    		new NetworkDefinitions.Request.UpdateSession<Game.SessionData>(-1, new Game.SessionData(-1.0f, 10.0f, 20.0f))
		)));
    	Assert.That(
    		"{\"command\":\"updateSession\",\"sessionID\":-1,\"parameters\":{\"playerPositionX\":-1.0,\"playerPositionY\":10.0,\"health\":20.0}}", Does.Not.Match(JsonUtility.ToJson(
    		new NetworkDefinitions.Request.UpdateSession<Game.SessionData>(-1, new Game.SessionData(0.0f, 30.0f, 40.0f))
		)));
    }


}
