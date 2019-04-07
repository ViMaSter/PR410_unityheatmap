 using UnityEngine;
 using UnityEditor;
 
 [CustomEditor(typeof(HeatmapRecorder))]
 public class HeatmapRecorderEditor : Editor {
     void OnSceneGUI()
	 {
		HeatmapRecorder recorder = target as HeatmapRecorder;

		// draw handles
		{
			Vector3 oldBottomLeft = recorder.mapBounds.center + new Vector3(-recorder.mapBounds.extents.x, -recorder.mapBounds.extents.y);
			Vector3 newBottomLeft = Handles.PositionHandle(oldBottomLeft, Quaternion.identity);
			Vector3 offset = (newBottomLeft - oldBottomLeft)/2;

			recorder.mapBounds.center += offset;
			recorder.mapBounds.extents -= offset;
		}
		
		{
			Vector3 oldBottomRight = recorder.mapBounds.center + new Vector3(recorder.mapBounds.extents.x, -recorder.mapBounds.extents.y);
			Vector3 newBottomRight = Handles.PositionHandle(oldBottomRight, Quaternion.identity);
			Vector3 offset = (newBottomRight - oldBottomRight)/2;

			recorder.mapBounds.center += offset;
			offset.y *= -1;
			recorder.mapBounds.extents += offset;
		}

		{
			Vector3 oldTopLeft = recorder.mapBounds.center + new Vector3(-recorder.mapBounds.extents.x, recorder.mapBounds.extents.y);
			Vector3 newTopLeft = Handles.PositionHandle(oldTopLeft, Quaternion.identity);
			Vector3 offset = (newTopLeft - oldTopLeft)/2;

			recorder.mapBounds.center += offset;
			offset.x *= -1;
			recorder.mapBounds.extents += offset;
		}	

		{
			Vector3 oldTopRight = recorder.mapBounds.center + new Vector3(recorder.mapBounds.extents.x, recorder.mapBounds.extents.y);
			Vector3 newTopRight = Handles.PositionHandle(oldTopRight, Quaternion.identity);
			Vector3 offset = (newTopRight - oldTopRight)/2;

			recorder.mapBounds.center += offset;
			offset.x *= -1;
			offset.y *= -1;
			recorder.mapBounds.extents -= offset;
		}

		Handles.Label(recorder.mapBounds.center + new Vector3(-recorder.mapBounds.extents.x, -recorder.mapBounds.extents.y), "Heatmap bounds");

     }
 }