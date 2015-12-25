using UnityEngine;
using UnityEditor;
using System.Collections;

public class CustomBuilds {

	enum BuildPlatform { Amazon, GooglePlay, Apple };

	const string defaultAndroidDefines = "disableLocationApi;disableVibrateApi;disableGoogleCloud;";

	static string[] ScenesPaths()
	{
		string[] scenes = new string[EditorBuildSettings.scenes.Length];
		
		for(int i = 0; i < scenes.Length; i++)
		{
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}
		
		return scenes;
	}

	[MenuItem("Project Tools/CustomBuilds/Development - GooglePlay")]
	static void DevelopmentBuildForGooglePlay()
	{
		BuildProyect(BuildPlatform.GooglePlay, true, false);
	}

	[MenuItem("Project Tools/CustomBuilds/Release - GooglePlay")]
	static void ReleaseBuildForGooglePlay()
	{
		BuildProyect(BuildPlatform.GooglePlay, false, false);
	}

	[MenuItem("Project Tools/CustomBuilds/Splitted - GooglePlay")]
	static void StoreBuildForGooglePlay()
	{
		BuildProyect(BuildPlatform.GooglePlay, false, true);
	}

	[MenuItem("Project Tools/CustomBuilds/Development - Amazon")]
	static void DevelopmentBuildForAmazon()
	{
		BuildProyect(BuildPlatform.Amazon, true, false);
	}

	[MenuItem("Project Tools/CustomBuilds/Release - Amazon")]
	static void ReleaseBuildForAmazon()
	{
		BuildProyect(BuildPlatform.Amazon, false, false);
	}

	[MenuItem("Project Tools/CustomBuilds/Debug - iOS")]
	static void DebugBuildForiOS()
	{
		BuildProyect(BuildPlatform.Apple, true, false);
	}

	[MenuItem("Project Tools/CustomBuilds/Release - iOS")]
	static void ReleaseBuildForiOS()
	{
		BuildProyect(BuildPlatform.Apple, false, false);
	}


	static void BuildProyect(BuildPlatform _platform, bool _development, bool _splitted)
	{
		string defines = _development?"DEBUG_ON;":"";
		BuildTarget target;
		BuildTargetGroup targetGroup;
		BuildOptions options = BuildOptions.None;
		EditorUserBuildSettings.appendProject = false;
		string productName = "Free Fall"; 
		string bundleIdentifier = "";

		if (_development) {
			options |= BuildOptions.Development;
		}
	
		switch(_platform) {
		case BuildPlatform.Amazon:
			target = BuildTarget.Android;
			targetGroup = BuildTargetGroup.Android;
			defines += defaultAndroidDefines+"AMAZON_ANDROID";
			productName = "Maleficent " + productName;
			bundleIdentifier = "com.disney.maleficent_ama";
			PlayerSettings.Android.useAPKExpansionFiles = false;
			break;
		case BuildPlatform.GooglePlay:
			targetGroup = BuildTargetGroup.Android;
			target = BuildTarget.Android;
			defines += defaultAndroidDefines;
			bundleIdentifier = "com.disney.maleficent_goo";
#if UNITY_ANDROID
			PlayerSettings.Android.useAPKExpansionFiles = _splitted;
#endif
			break;
		default:
			target = BuildTarget.iPhone;
			targetGroup = BuildTargetGroup.iPhone;
			bundleIdentifier = "com.disney.maleficent";
			break;
		}

		PlayerSettings.bundleIdentifier = bundleIdentifier;
		PlayerSettings.productName = productName;
		PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);

		string buildPath;
		
		#if UNITY_ANDROID
		PlayerSettings.Android.keystoreName = "Assets/_Maleficent/maleficent2014[1].keystore";
		PlayerSettings.Android.keystorePass = "maleficentgoofy";
		PlayerSettings.Android.keyaliasName = "maleficent";
		PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass;

		int bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
//		PlayerSettings.Android.bundleVersionCode = _splitted? bundleVersionCode +1: bundleVersionCode;

		buildPath = EditorUtility.SaveFilePanel("Build output file", System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "", "apk"); 

		#else

		buildPath = EditorUtility.OpenFolderPanel("Build output folder", System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "");
	
		if(System.IO.Directory.Exists(buildPath+"/Unity-iPhone.xcodeproj")) {
			EditorUserBuildSettings.appendProject = EditorUtility.DisplayDialog("Warning", "Build folder already exists. Would you like to append or replace it?", "Append", "Replace");
		}

		#endif

		if (buildPath != null && buildPath.Length > 0) {
#if UNITY_ANDROID
			if(EditorUtility.DisplayDialog("Auto Run Player", "Do you want to launch the app in the device ?", "Yes please", "Not today")) {
				options |= BuildOptions.AutoRunPlayer;
			}
#endif

		BuildPipeline.BuildPlayer(ScenesPaths(), buildPath, target, options);

		}
	}
}
