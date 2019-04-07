using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

public class HeatmapVisualizer : EditorWindow, IHasCustomMenu
{
	#region pseudo-singleton
	public static HeatmapVisualizer _instance = null;
	public static HeatmapVisualizer Instance
	{
		get
		{
			return _instance;
		}
		set
		{
			_instance = value;
		}
	}
	#endregion
	
	#region data
	private List<string> filesForThisMap;
	private HeatmapRecorder.SessionInfo parsedSessionInfo;

	private int selectedMapIndex = 0;
	private int SelectedMapIndex
	{
		get
		{
			return selectedMapIndex;
		}
		set
		{
			if (selectedMapIndex >= filesForThisMap.Count)
			{
				Debug.LogWarningFormat("Only {0} maps are available; {1} is too high of an index; resetting back to 0.", filesForThisMap.Count, selectedMapIndex);
				selectedMapIndex = 0;
			}
			else
			{
				selectedMapIndex = value;
			}

			char[] restoredFilename = filesForThisMap[selectedMapIndex].Replace("/", "-").ToCharArray();
			restoredFilename[10] = '_';

			string recordingsFolder = Path.Combine(
				Application.persistentDataPath,
				"heatmapData"
			);
			StreamReader reader = new StreamReader(Path.Combine(recordingsFolder, new string(restoredFilename))); 
			parsedSessionInfo = JsonUtility.FromJson<HeatmapRecorder.SessionInfo>(reader.ReadToEnd());
			reader.Close();

			RecalculatePlot();
		}
	}

	// plotted values
	public int highestValue = 0;
	public int[,] values;
	#endregion

	#region visualization
    public Gradient plottingGradient = new Gradient();
    int plottingDensity = 1;
	float plottingUnits
	{
		get
		{
			return parsedSessionInfo.mapBounds.size.x / plottingDensity;		
		}
	}
	
    float matchProgressionStart = 0.0f;
    float matchProgressionEnd = 1.0f;
	private DateTime lastKillAt = new DateTime(0);
	#endregion

	#region derivatives
    [MenuItem("Window/Heatmap Visualizer")]
    static void Init()
    {
        _instance = (HeatmapVisualizer)EditorWindow.GetWindow(typeof(HeatmapVisualizer), false, "Heatmap visualizer");
        _instance.Show();
    }

    void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Refresh heatmap files"), false, RefreshFiles);
    }
	#endregion

	private List<string> GetHeatmapFilesForMap()
	{
		string path = Path.Combine(
			Application.persistentDataPath,
			"heatmapData"
		);
		string[] filePaths = Directory.GetFiles(@path, "*.json");
		List<string> relevantRecordings = new List<string>(filePaths.Length);
		foreach (string file in filePaths)
		{
			//Read the text from directly from the test.txt file
			StreamReader reader = new StreamReader(file); 
			string rawJSON = reader.ReadToEnd();
			reader.Close();
			
			HeatmapRecorder.SessionInfo sessionInfo = JsonUtility.FromJson<HeatmapRecorder.SessionInfo>(rawJSON);
			if (sessionInfo.mapName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().path)
			{
				relevantRecordings.Add(System.IO.Path.GetFileName(file).Replace("-", "/").Replace("_", "/"));
			}
		}
		return relevantRecordings;
	}

	void RefreshFiles()
	{
		filesForThisMap = GetHeatmapFilesForMap();
		#pragma warning disable 1717 // Assignment made to same variable; did you mean to assign something else?
		SelectedMapIndex = SelectedMapIndex; // refresh index, if we now have less maps
		#pragma warning restore
	}

	void RecalculatePlot()
	{
		// generate slots for density
		float total = Mathf.Max( Mathf.Abs(parsedSessionInfo.mapBounds.min.y) + parsedSessionInfo.mapBounds.max.y, Mathf.Abs(parsedSessionInfo.mapBounds.min.x) + parsedSessionInfo.mapBounds.max.x );
		int steps = (int)Mathf.Floor(total/plottingUnits);
		values = new int[steps, steps];

		// reset highest value
		highestValue = 0;

		// generate cut-off point
		long matchRange = (parsedSessionInfo.mapSessionEnd.value - parsedSessionInfo.mapSessionStart.value);
		long[] cutoffRange = new long[]{
			parsedSessionInfo.mapSessionStart.value + (long)(matchRange * matchProgressionStart),
			parsedSessionInfo.mapSessionStart.value + (long)(matchRange * matchProgressionEnd),
		};

		foreach (HeatmapRecorder.DeathInfo deathInfo in parsedSessionInfo.deaths)
		{
			// ignore deaths after cutoff point
			if (deathInfo.timestamp.value < cutoffRange[0] || deathInfo.timestamp.value > cutoffRange[1])
			{
				continue;
			}

			// ignore deaths after cutoff point
			Vector3 adjustedPositon = deathInfo.position + new Vector3(Mathf.Abs(parsedSessionInfo.mapBounds.min.x), Mathf.Abs(parsedSessionInfo.mapBounds.min.y), 0);
			adjustedPositon = new Vector3(
				adjustedPositon.x / parsedSessionInfo.mapBounds.size.x,
				adjustedPositon.y / parsedSessionInfo.mapBounds.size.y,
				adjustedPositon.z / parsedSessionInfo.mapBounds.size.z
			);
			adjustedPositon *= steps;
			if (highestValue < ++values[(int)Mathf.Floor(adjustedPositon.x), (int)Mathf.Floor(adjustedPositon.y)])
			{
				highestValue = values[(int)Mathf.Floor(adjustedPositon.x), (int)Mathf.Floor(adjustedPositon.y)];
			}
		}

		// force redraw of gizmos		
		SceneView.RepaintAll();
	}

	void OnEnable()
	{
		HeatmapVisualizer._instance = this;
		RefreshFiles();
		RecalculatePlot();

		// provide default color gradient
		GradientColorKey[] colorKey = new GradientColorKey[3];
        colorKey[0].color = Color.red;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.yellow;
        colorKey[1].time = 0.5f;
        colorKey[2].color = Color.green;
        colorKey[2].time = 1.0f;
		GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
		plottingGradient.SetKeys(colorKey, alphaKey);
	}

    void OnGUI()
    {
		SelectedMapIndex = EditorGUILayout.Popup(SelectedMapIndex, filesForThisMap.ToArray());

        GUILayout.Label("Visualizer settings", EditorStyles.boldLabel);
		int newDensity = EditorGUILayout.IntSlider("Row/column density", plottingDensity, 1, 100);
        if (plottingDensity != newDensity)
		{
			plottingDensity = newDensity;
			RecalculatePlot();
		}
		
		float newProgressionStart = matchProgressionStart;
		float newProgressionEnd = matchProgressionEnd;
		EditorGUILayout.MinMaxSlider("Match progression", ref newProgressionStart, ref newProgressionEnd, 0.0f, 1.0f);
        if (matchProgressionStart != newProgressionStart || matchProgressionEnd != newProgressionEnd)
		{
			matchProgressionStart = newProgressionStart;
			matchProgressionEnd = newProgressionEnd;
			RecalculatePlot();
		}

		EditorGUI.BeginChangeCheck();
		SerializedObject serializedGradient = new SerializedObject(this);
		SerializedProperty colorGradient = serializedGradient.FindProperty("plottingGradient");
		EditorGUILayout.PropertyField(colorGradient, true, null);
		if(EditorGUI.EndChangeCheck())
		{
			serializedGradient.ApplyModifiedProperties();
		}
    }
 
    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    private static void GizmoTest(HeatmapRecorder recorder, GizmoType gizmoType)
    {
		if (HeatmapVisualizer.Instance == null)
		{
			return;
		}

		for (int y = 0; y < HeatmapVisualizer.Instance.values.GetLength(1); ++y)
		{
			for (int x = 0; x < HeatmapVisualizer.Instance.values.GetLength(0); ++x)
			{
				float intensity = HeatmapVisualizer.Instance.values[x, y] / (float)HeatmapVisualizer.Instance.highestValue;

				Gizmos.color = HeatmapVisualizer.Instance.plottingGradient.Evaluate(intensity);
				Gizmos.DrawCube(
					recorder.mapBounds.min +
					new Vector3(
						x * HeatmapVisualizer.Instance.plottingUnits,
						y * HeatmapVisualizer.Instance.plottingUnits,
						3
					) + Vector3.one*(HeatmapVisualizer.Instance.plottingUnits / 2),
					Vector3.one * HeatmapVisualizer.Instance.plottingUnits * 1f
				);
			}
		}
    }

}