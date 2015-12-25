using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TransformTools {
	[MenuItem ("Mobility Games/Transform/Duplicate To Empty GO")]
	public static void CloneEmptyGO() {
		Undo.RegisterSceneUndo("Duplicate To Empty GO");

		
		Transform[] selected = Selection.transforms;
		GameObject firstCreated = null;
		for(int i = 0; i < selected.Length; i++) {
			GameObject newObj = null;
			if (selected.Length > 1) {
				newObj = new GameObject(selected[i].name + "_" + i);
			} else {
				newObj = new GameObject(selected[i].name);
			}
			if (i == 0) {
				firstCreated = newObj;
			}
			    
			Transform xForm = newObj.transform;
			xForm.parent = selected[i].parent;
			xForm.localPosition = selected[i].localPosition;
			xForm.localRotation = selected[i].localRotation;
			xForm.localScale = selected[i].localScale;
		}
		
		Selection.activeGameObject = firstCreated;
	}
	
	[MenuItem ("Mobility Games/Transform/Auto Scale")]
	public static void Transform_AutoScale() {	
		Transform[] selectedTransforms = Selection.GetTransforms(SelectionMode.Deep);
		Vector3 newScale;
			
		for (int i = 0; i < selectedTransforms.Length; ++i) {
			if (selectedTransforms[i].renderer) {
				newScale = selectedTransforms[i].localScale;
				newScale.z = newScale.x * selectedTransforms[i].renderer.sharedMaterial.mainTexture.height / 
					selectedTransforms[i].renderer.sharedMaterial.mainTexture.width;
				selectedTransforms[i].localScale = newScale;
			}
		}
	}
	
	[MenuItem ("CONTEXT/Transform/Duplicate To Empty GO")]
	public static void ContextCloneEmptyGO(MenuCommand command) {
		CloneEmptyGO();
	}

	[MenuItem ("CONTEXT/Transform/Select all children with non 1 scale")]
	public static void SelectAllChildrenWithWrongScale(MenuCommand command) {
		Transform xForm = command.context as Transform;
		List<GameObject> selection = new List<GameObject>();
		
		foreach (var child in xForm.GetComponentsInChildren<Transform>()) {
			if (child.localScale != Vector3.one) {
				selection.Add(child.gameObject);
			}
		}
		
		Selection.objects = selection.ToArray();
	}
}