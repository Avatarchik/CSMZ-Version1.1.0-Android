using UnityEngine;
using UnityEditor;

public class IndexingTools {
	[MenuItem ("Mobility Games/Index/Increase Index &%+")]
	public static void IncreaseIndex() {
		Undo.RegisterSceneUndo("Increase Index");
		
		// get all the selected textures in the editor
		Transform[] selectedTransforms = Selection.GetTransforms(SelectionMode.Unfiltered);
		
		int idx;
		int firstIndexDigit;
		string name;
		for (int i = 0; i < selectedTransforms.Length; ++i) {
			name = selectedTransforms[i].name;
			firstIndexDigit = name.Length - 1;
			while (name[firstIndexDigit] >= '0' && name[firstIndexDigit] <= '9') {
				firstIndexDigit--;
			}
			firstIndexDigit++;
			while (name[firstIndexDigit] == '0' && firstIndexDigit < name.Length - 1) {
				firstIndexDigit++;
			}
			idx = int.Parse(name.Substring(firstIndexDigit)) + 1;
			selectedTransforms[i].name = name.Substring(0, firstIndexDigit) + idx.ToString();
		}
	}
	
	[MenuItem ("Mobility Games/Index/Decrease Index &%-")]
	public static void DecreaseIndex() {
		Undo.RegisterSceneUndo("Decrease Index");
		
		// get all the selected textures in the editor
		Transform[] selectedTransforms = Selection.GetTransforms(SelectionMode.Unfiltered);
		
		int idx;
		int firstIndexDigit;
		string name;
		for (int i = 0; i < selectedTransforms.Length; ++i) {
			name = selectedTransforms[i].name;
			firstIndexDigit = name.Length - 1;
			while (name[firstIndexDigit] >= '0' && name[firstIndexDigit] <= '9') {
				firstIndexDigit--;
			}
			firstIndexDigit++;
			while (name[firstIndexDigit] == '0' && firstIndexDigit < name.Length - 1) {
				firstIndexDigit++;
			}
			idx = int.Parse(name.Substring(firstIndexDigit)) - 1;
			selectedTransforms[i].name = name.Substring(0, firstIndexDigit) + idx.ToString();
		}
	}
}