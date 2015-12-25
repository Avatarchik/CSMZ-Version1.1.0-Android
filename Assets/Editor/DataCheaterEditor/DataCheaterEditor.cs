using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[CustomEditor( typeof( DataCheater ) )]
public class DataCheaterEditor : Editor{

	public override void OnInspectorGUI()
	{ 
		serializedObject.Update();
		SerializedProperty currentLevel = serializedObject.FindProperty ("currentLevel");

		EditorGUIUtility.LookLikeControls();
		
		EditorGUILayout.BeginHorizontal ();
		
		if(GUILayout.Button("Reset Data"))
		{
			DataCheater.DoReset();
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
	
		if(GUILayout.Button("Apply"))
		{
			if(Application.isPlaying)
			{
				if(ChaptersManager.Instance != null)
				{
					ChaptersManager.Instance.Cheat((target as DataCheater).currentLevel);
				}
				else
				{
					PlayerPrefs.SetInt("reset",1);
					PlayerPrefs.SetInt("unlockLevels",(target as DataCheater).currentLevel);

				}
			}
			else
			{
				PlayerPrefs.SetInt("unlockLevels",(target as DataCheater).currentLevel);
			}
			PlayerPrefs.Save();
		}

		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();

		if(GUILayout.Button("Unlock Next Level")){
			if(Application.isPlaying)
			{
				if(ChaptersManager.Instance != null)
				{
					(target as DataCheater).currentLevel += 1;
					ChaptersManager.Instance.Cheat((target as DataCheater).currentLevel);
				}
			}
		}

		EditorGUILayout.EndHorizontal ();


		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(currentLevel, true);
		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}
}
