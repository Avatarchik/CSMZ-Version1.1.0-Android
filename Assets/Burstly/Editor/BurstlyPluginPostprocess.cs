using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Diagnostics;

public class MaleficentPostBuildTrigger : MonoBehaviour {
	
	[PostProcessBuild(300)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		if (target == BuildTarget.iPhone) {
			
			UnityEngine.Debug.Log("[BurstlyManager] iOS postprocess script started");
			
			string unityPath = Application.dataPath;
			Process process = new Process();
			process.StartInfo.FileName = "python";
			process.StartInfo.Arguments = string.Format("Assets/Burstly/Editor/BurstlyPostprocess-iOS.py {1}", unityPath, path);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = false;
			process.Start();
			process.WaitForExit();

			UnityEngine.Debug.Log("[BurstlyManager] iOS postprocess completed");
		}
	}
}