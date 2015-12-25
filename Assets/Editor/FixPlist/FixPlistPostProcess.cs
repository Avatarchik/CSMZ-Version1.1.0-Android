using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Diagnostics;

public class FixPlistPostProcess : MonoBehaviour {

	[PostProcessBuild(300)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		if (target == BuildTarget.iPhone) {

			UnityEngine.Debug.Log("[FixPlist] iOS post process started");

			string objCPath = Application.dataPath;
			Process pythonProcess = new Process();
			pythonProcess.StartInfo.FileName = "python";
			pythonProcess.StartInfo.Arguments = string.Format("Assets/Editor/FixPlist/post_process.py \"{0}\" \"{1}\"", path, objCPath);
			pythonProcess.StartInfo.UseShellExecute = false;
			pythonProcess.StartInfo.RedirectStandardOutput = false;
			pythonProcess.Start ();
			pythonProcess.WaitForExit();

			UnityEngine.Debug.Log("[FixPlist] iOS post process completed");
		}
	}
}
