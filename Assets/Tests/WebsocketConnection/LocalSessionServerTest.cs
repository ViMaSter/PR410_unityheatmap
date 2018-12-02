public class LocalSessionServerTest : SessionServerTest, UnityEngine.TestTools.IPrebuildSetup
{
    public void Setup()
    {
        SessionServerConfig.Host = "ws://127.0.0.1";
		SessionServerConfig.Port = 7000;
		UnityEngine.Debug.LogWarning("Local test at:" + SessionServerConfig.Host+":"+SessionServerConfig.Port);
    }
}
