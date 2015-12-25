using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GenericSettings))]
public class GenericSettingsInspector : Editor
{
	public override void OnInspectorGUI()
	{
		GenericSettings settings = (GenericSettings)target;
		settings.Narration = EditorGUILayout.Toggle("Narration", settings.Narration);
		settings.UseRecording = EditorGUILayout.Toggle("Use Recording", settings.UseRecording);
		settings.Autoplay = EditorGUILayout.Toggle("Autoplay", settings.Autoplay);
		settings.Arrows = EditorGUILayout.Toggle("Arrows", settings.Arrows);
		settings.Music = EditorGUILayout.Toggle("Music", settings.Music);
		settings.Sound = EditorGUILayout.Toggle("Sound", settings.Sound);
		settings.Tutorials = EditorGUILayout.Toggle("Tutorials", settings.Tutorials);
   }
}

