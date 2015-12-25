using UnityEditor;
using UnityEngine;
using System.Collections;

public class MenuItems : MonoBehaviour {

	//[MenuItem("Project Tools/MenuItems/PauseResume _F5")]
	static void PauseResume() {
		EditorApplication.isPaused = !EditorApplication.isPaused;
	}
	
	//[MenuItem("Project Tools/MenuItems/Step _F6")]
	static void MenuItemStep() {
		EditorApplication.Step();
	}
}
