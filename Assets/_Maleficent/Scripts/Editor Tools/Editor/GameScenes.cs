using UnityEngine;
using UnityEditor;
using System.Collections;

public class GameScenes : ProjectManager.ProjectManagerWindow {
	
	private string GetSceneName(string path)
	{
		int start = path.LastIndexOf('/') + 1;
		int end = path.LastIndexOf('.');
		return path.Substring(start, end - start);
	}
	
	public override void OnGUI () {	
		GUILayout.BeginVertical("box");
		
		// Button for each scene
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if(GUILayout.Button(GetSceneName(scene.path))) {
				if(EditorApplication.SaveCurrentSceneIfUserWantsTo())
				{
					EditorApplication.OpenScene(scene.path);
					Repaint();
				}
			}
		}
		
		GUILayout.EndVertical();
	}	
}
