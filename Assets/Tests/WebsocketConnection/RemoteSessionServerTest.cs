public class RemoteSessionServerTest : SessionServerTest, UnityEngine.TestTools.IPrebuildSetup
{
	[NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        SessionServerConfig.Host = "ws://cr350server.herokuapp.com";
		SessionServerConfig.Port = 80;
		UnityEngine.Debug.LogWarning("Remote test at:" + SessionServerConfig.Host+":"+SessionServerConfig.Port);
    }
}
