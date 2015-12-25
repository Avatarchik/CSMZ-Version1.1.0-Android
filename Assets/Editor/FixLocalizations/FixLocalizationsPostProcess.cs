using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Diagnostics;

public class FixLocalizationsPostProcess : MonoBehaviour {
	
	private static string processName = "FixLocalizations";
	
	[PostProcessBuild(300)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		if (target == BuildTarget.iPhone) {
			
			Log("iOS process started");
			
			string objCPath = Application.dataPath;
			Process pythonProcess = new Process();
			pythonProcess.StartInfo.FileName = "python";
			pythonProcess.StartInfo.Arguments = string.Format("Assets/Editor/FixLocalizations/localize_files_xcode.py \"{0}\" \"{1}\"", path, objCPath);
			pythonProcess.StartInfo.UseShellExecute = false;
			pythonProcess.StartInfo.RedirectStandardOutput = true;
			pythonProcess.StartInfo.RedirectStandardError = true;
			
			pythonProcess.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
				Log (e.Data);
			};
			pythonProcess.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				LogError(e.Data);
			};
			
			pythonProcess.Start ();
			pythonProcess.BeginOutputReadLine();
			
			pythonProcess.WaitForExit();
			
			
			Log("iOS process ended");
		}
	}
	
	void OnApplicationQuit()	
	{
//		if( process != null && !process.HasExited ) {
//			process.Kill();
//		}
	}
	
	
	static void Log(string message) {
		UnityEngine.Debug.Log(string.Format("[{0}] {1}", processName, message));
	}
	
	static void LogError(string message) {
		UnityEngine.Debug.LogError(string.Format("[{0}] {1}", processName, message));
	}
}
