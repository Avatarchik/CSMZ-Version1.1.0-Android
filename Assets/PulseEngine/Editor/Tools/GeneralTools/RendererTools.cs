using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RendererTools {
	[MenuItem ("Mobility Games/Renderer/Deactivate Render &%o")]
	public static void DeactivateRenders() {
		
		Transform[] selectedTransforms = Selection.GetTransforms(SelectionMode.Unfiltered);
		
		for (int i = 0; i < selectedTransforms.Length; ++i) {
			Renderer[] renderers = selectedTransforms[i].GetComponentsInChildren<Renderer>();
			for (int j = 0; j < renderers.Length; ++j) {
				renderers[j].enabled = false;
			}
		}
	}

	[MenuItem ("Mobility Games/Renderer/Activate Render &%u")]
	public static void ActivateRenders() {
		
		Transform[] selectedTransforms = Selection.GetTransforms(SelectionMode.Unfiltered);
		
		for (int i = 0; i < selectedTransforms.Length; ++i) {
			Renderer[] renderers = selectedTransforms[i].GetComponentsInChildren<Renderer>();
			for (int j = 0; j < renderers.Length; ++j) {
				renderers[j].enabled = true;
			}
		}
	}
	
	[MenuItem ("Mobility Games/Renderer/Disable Shadow Settings")]
	public static void Renderer_DisableShadowSettings() {	
		Transform[] selectedTransforms = Selection.GetTransforms(SelectionMode.Deep);
		
		for (int i = 0; i < selectedTransforms.Length; ++i) {
			if (selectedTransforms[i].renderer) {
				selectedTransforms[i].renderer.castShadows = false;
				selectedTransforms[i].renderer.receiveShadows = false;
			}
		}
	}	
	
	[MenuItem ("CONTEXT/MeshRenderer/Select all objects with material")]
	public static void SelectAllObjectsWithMaterial(MenuCommand command) {
		Material mat = (command.context as MeshRenderer).sharedMaterial;
		Object[] objs = Object.FindSceneObjectsOfType(typeof(MeshRenderer));
		
		Debug.Log("Total objs: " + objs.Length);
		
		List<GameObject> selection = new List<GameObject>();
		
		foreach (var obj in objs) {
			if ((obj as MeshRenderer).sharedMaterial == mat) {
				selection.Add((obj as MeshRenderer).gameObject);
			}
		}
		
		Debug.Log("Filtered objs: " + selection.Count);
		
		Selection.objects = selection.ToArray();
	}
}