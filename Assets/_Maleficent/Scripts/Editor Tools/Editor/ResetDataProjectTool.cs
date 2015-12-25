using UnityEngine;
using UnityEditor;

public class ResetDataProjectTool
{
	[MenuItem("Project Tools/Reset Data")]
	static void OpenWindow()
	{
		DataCheater.DoReset();
	}
}
	