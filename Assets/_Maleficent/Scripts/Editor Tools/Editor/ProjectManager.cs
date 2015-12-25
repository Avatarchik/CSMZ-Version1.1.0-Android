using UnityEngine;
using UnityEditor;
using System.Collections;

public class ProjectManager  : EditorWindow {
	
	public class ProjectManagerWindow {	
		public void Repaint() {
			ProjectManager.GetWindow().Repaint();
		}
		
		public virtual void OnGUI(){}
		public virtual void OnSelectionChange(){}
	};
	
	private static ProjectManager window;
	private static ProjectManagerWindow[] windows = {
		new SelectionChange(),
		new BlackboardSelector(),
		new GameScenes()	
	};
	
	// Add menu named "My Window" to the Window menu
    [MenuItem ("Bravo/Window/Project Manager2")]
    static void Init () {		
		GetWindow();
    }
	
	public static ProjectManager GetWindow()
	{
		if(window == null) {
			// Get existing open window or if none, make a new one:
			window = (ProjectManager)ProjectManager.GetWindow< ProjectManager >();
		}
		return window;
	}
	
	void OnGUI ()
	{
		GUILayout.Label ("Project manager", EditorStyles.boldLabel);

		foreach(ProjectManagerWindow pmw in windows) {
			pmw.OnGUI();
		}
	}
	
	void OnSelectionChange() 
	{
		foreach(ProjectManagerWindow pmw in windows) {
			pmw.OnSelectionChange();
		}
	}
}
