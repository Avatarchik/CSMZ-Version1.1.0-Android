using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class PulseEngineEditorStartup {
	//Note: uncomment this constructor to activate this script when opening this project.
//	static PulseEngineEditorStartup() {
//		EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
//		EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemVisible;
//	}
	
	static void OnHierarchyWindowItemVisible(int instanceID, Rect selectionRect) {
		Object currentObj = EditorUtility.InstanceIDToObject(instanceID);
		
		// Draw PulseEngine custom icons
		if ( (currentObj as GameObject).GetComponent<NarrationBlock>() ) {
			selectionRect.x = selectionRect.x + selectionRect.width - selectionRect.height - 5;
			selectionRect.width = selectionRect.height;
			
			EditorGUI.DrawPreviewTexture(selectionRect, EditorGUIUtility.whiteTexture);
		}
	}
	
	static void OnHierarchyWindowChanged() {

	}
}
