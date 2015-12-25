using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public static class LevelEditorUtils {

	/// <summary>
	/// TODO: not working as expected ! Ignore it for the moment. Uses undocumented classes and methods from the editor.
	/// Focuses the scene camera on a specified target. 
	/// </summary>
	/// <param name='target'>
	/// Target.
	/// </param>
	public static void FocusSceneCameraOn(Transform target) {
		Debug.Log("Called 1");
		if ( !Application.isPlaying && target != null ) {
			Debug.Log("Called 2");
			SceneView.lastActiveSceneView.orthographic = true;
			//TODO: hard-coded distance of camera from board renderer for faster implementation.
			SceneView.lastActiveSceneView.LookAtDirect(target.position + target.forward * 20f, target.rotation, 1f);
			SceneView.lastActiveSceneView.Repaint();
		}
	}
	
	/// <summary>
	/// Determines whether the specified prefab is present in the "prefabsArray".
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is prefab on board renderer the specified tilePrefab prefabsArray; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='prefab'>
	/// If set to <c>true</c> tile prefab.
	/// </param>
	/// <param name='prefabsArray'>
	/// If set to <c>true</c> prefabs array.
	/// </param>
	public static bool IsPrefabInArray(GameObject[] prefabsArray, GameObject prefab) {
		if (prefabsArray != null && prefabsArray.Length > 0) {
			int tileIdx = System.Array.IndexOf<GameObject>(prefabsArray, prefab);
			if (tileIdx >= 0) {
				return true;
			}
		}

		return false;
	}
	
	public static T GetPrefabFromArray<T>(System.Type prefabType, GameObject[] prefabsArray) where T : MonoBehaviour {
		if (prefabsArray == null || prefabsArray.Length == 0) {
			return null;
		}
		
		for(int i = 0; i < prefabsArray.Length; i++) {
			if (prefabsArray[i] == null) {
				continue;
			}
			
			Component component = prefabsArray[i].GetComponent(prefabType);
			if (component != null && component.GetType() == prefabType) {
				return (T)component;
			}
		}
		
		return null;
	}
}
