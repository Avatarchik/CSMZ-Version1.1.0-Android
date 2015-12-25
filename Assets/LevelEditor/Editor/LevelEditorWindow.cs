using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelEditorWindow : EditorWindow {
	public LevelEditorWindowGUI windowGUI;
	public LevelEditorSceneGUI sceneGUI;
	public LevelEditorData editor;
			
	[MenuItem("Project Tools/Level Editor")]
	public static void EnableFrozenMatch3LevelEditor() {
//		if ( (new LevelEditorData()).Init() ) {
		EditorWindow.GetWindow<LevelEditorWindow>(false, "Level Editor", true);
//		}
	}
		
	/// <summary>
	/// Unity event. Initialization point of this window.
	/// </summary>
	void OnEnable() {
		// Init editor data model.
		if (editor == null) {
			editor = new LevelEditorData();
		}
		
		if ( !editor.Init() ) {
//			return;
		}
		
		// Init scene GUI renderer
		if (sceneGUI == null) {
			sceneGUI = new LevelEditorSceneGUI(editor);
		}
		sceneGUI.Init();
		
		// Init window GUI renderer.
		if (windowGUI == null) {
			windowGUI = new LevelEditorWindowGUI(this, editor);
		}
		windowGUI.Init();
		
		// Register the scene view GUI renderer
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}
		
	/// <summary>
	/// Unity event called usually called when the window is about to get destroyed.
	/// </summary>
	void OnDisable() {		
		// Unregister the scene view GUI renderer
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}
	
	/// <summary>
	/// Unity event raised when drawing the window GUI.
	/// </summary>
	void OnGUI() {
		windowGUI.Draw();
	}
	
	/// <summary>
	/// Event raised by Unity. This event is registered/unregistered in OnEnable/OnDisable.
	/// </summary>
	/// <param name='sceneView'>
	/// Scene view.
	/// </param>
	public void OnSceneGUI(SceneView sceneView) {
		sceneGUI.Draw();
	}
}
