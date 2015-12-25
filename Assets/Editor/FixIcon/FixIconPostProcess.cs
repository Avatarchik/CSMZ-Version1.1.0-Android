using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Diagnostics;

public class FixIconPostProcess : MonoBehaviour {

	private static string processName = "FixIcon";
	private static Process process;
	
	[PostProcessBuild(300)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		if (target == BuildTarget.iPhone) {
			
			Log("iOS process started");
			
			string objCPath = Application.dataPath;
			process = new Process();
			process.StartInfo.FileName = "python";
			process.StartInfo.Arguments = string.Format("Assets/Editor/FixIcon/fix_icon.py \"{0}\" \"{1}\"", path, objCPath);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;

			process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
				Log (e.Data);
			};
			process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				LogError(e.Data);
			};

			process.Start ();
			process.BeginOutputReadLine();
			process.WaitForExit();


			Log("iOS process ended");
		}
	}

	void OnApplicationQuit()	
	{
		if( process != null && !process.HasExited ) {
			process.Kill();
		}
	}


	static void Log(string message) {
		UnityEngine.Debug.Log(string.Format("[{0}] {1}", processName, message));
	}

	static void LogError(string message) {
		UnityEngine.Debug.LogError(string.Format("[{0}] {1}", processName, message));
	}
}
