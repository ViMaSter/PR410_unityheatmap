using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

// borrowed from https://gamedev.stackexchange.com/a/137526
[Serializable]
struct SerializableDateTime {
    public long value;
    public static implicit operator DateTime(SerializableDateTime serializableDateTime) {
        return DateTime.FromFileTimeUtc(serializableDateTime.value);
    }
    public static implicit operator SerializableDateTime(DateTime source) {
        SerializableDateTime serializableDateTime = new SerializableDateTime();
        serializableDateTime.value = source.ToFileTimeUtc();
        return serializableDateTime;
    }
}

public class HeatmapRecorder : MonoBehaviour
{
	#region Pseudo-Singleton
	public static HeatmapRecorder Instance
	{
		get
		{
			return _instance;
		}
	}
	private static HeatmapRecorder _instance = null;
	void Start()
	{
		if (_instance != null)
		{
			Debug.LogWarning("Another instance of HeatmapRecorder exists - ensure you're cleaning up properly");
		}
		else
		{
			_instance = this;
		}
	}
	#endregion

	DateTime sessionStartAt = new DateTime();
	void Awake()
	{
		sessionStartAt = DateTime.Now;
	}

	[Serializable]
	class SessionInfo
	{
		public string mapName;
		public Bounds mapBounds;
		public SerializableDateTime mapSessionStart;
		public SerializableDateTime mapSessionEnd;

		public List<DeathInfo> deaths;
	}

	[Serializable]
	class DeathInfo
	{
		public Vector3 position;
		public SerializableDateTime timestamp;
	}

	private List<DeathInfo> deathsThisRound = new List<DeathInfo>();
	public Bounds mapBounds;

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawCube(mapBounds.center, mapBounds.size);
	}

	public void RecordDeath(Vector3 position)
	{
		deathsThisRound.Add(new DeathInfo{
			position = position,
			timestamp = DateTime.Now
		});
		Debug.Log("Added death:\r\n"+deathsThisRound[deathsThisRound.Count-1]);
	}

	void OnApplicationQuit()
	{
		SerializeCurrentDeaths();
	}

	public string SerializeCurrentDeaths()
	{
		string serializedSessionInfo = JsonUtility.ToJson(new SessionInfo
		{
			mapName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path,
			mapSessionStart = sessionStartAt,
			mapSessionEnd = DateTime.Now,
			mapBounds = mapBounds,
			deaths = deathsThisRound
		});

		string targetFolder = Path.Combine(
			Application.persistentDataPath,
			"heatmapData"
		);

		Directory.CreateDirectory(targetFolder);

		string targetFileName = Path.Combine(
			targetFolder,
			DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss") + ".txt"
		);	

		using (StreamWriter writer = new StreamWriter(targetFileName, false))
		{
			writer.Write(serializedSessionInfo);
		}
		Debug.Log("Wrote to " + targetFileName);

		return serializedSessionInfo;
	}
}
