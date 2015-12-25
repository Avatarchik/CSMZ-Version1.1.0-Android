using UnityEngine;
using UnityEditor;
using System.Collections;

public static class UnityEditorInfo 
{
	[MenuItem ("Mobility Games/Editor/Show persistentDataPath")]
	public static void ShowEditorPersistentDataPath()
	{
		EditorUtility.DisplayDialog("Info", "Path: " + Application.persistentDataPath, "Ok");
		EditorUtility.RevealInFinder(Application.persistentDataPath);
	}

}
