using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextStyleManager)), CanEditMultipleObjects]
public class TextStyleManagerEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		if (GUI.changed) {
			TextStyleManager.Instance.NotifyStyleChange();
		}
	}
}
