using UnityEngine;
using UnityEditor;
using System.Collections;

public class BlackboardSelector : ProjectManager.ProjectManagerWindow {

	public override void OnGUI () {	
		GUILayout.BeginVertical("box");

		GUI.contentColor = Color.black;
		if(GUILayout.Button("Blackboard")) {
			Selection.activeObject = Resources.Load("Blackboard");
		}
		GUI.contentColor = Color.white;

		
		GUILayout.EndVertical();
	}	
}
