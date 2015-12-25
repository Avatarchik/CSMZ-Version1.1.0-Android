using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class AMPSBinding
{	
	private const string APP_NAME 			= "maleficent_free_fall";
	private const string LANGUAGE_ASSET 	= "ge";
	private const string QUALITY_ASSET 		= "hq";
	private const string APP_VERSION 		= "1";
	
	public delegate void UnityCallbackDelegate(string objectName, string commandName, string commandData);
	
#region Android native interface
	#if !UNITY_EDITOR && UNITY_ANDROID
	private static AndroidJavaClass javaBinding;
		
 	static AMPSBinding() 
	{
		javaBinding = new AndroidJavaClass("com.mobilitygames.amps.AMPHook");
	}
	
	#endif
#endregion
	
#region iOS native interface
	
	#if !UNITY_EDITOR && UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void InitDMOAssetManagerWithPath(string unityListenerName, string assetDownloadPath, string appName, string language, string quality, string version);
	
	[DllImport ("__Internal")]
	private static extern void DownloadAssetWithName(string assetDownloadPath, string name);
	
	[DllImport ("__Internal")]
	private static extern string VersionOfFile(string assetDownloadPath, string name);
	
	[DllImport ("__Internal")]
	private static extern bool IsCatalogDownloaded();
	#endif
	
#endregion
	
	public static void InitDMOAssetManager(string unityListenerName, string assetDownloadPath, string appName = null, string language = null, string quality = null, string version = null)
	{
		string _appName 	= string.IsNullOrEmpty(appName) 	? APP_NAME 			: appName;
		string _language 	= string.IsNullOrEmpty(language)	? LANGUAGE_ASSET 	: language;
		string _quality 	= string.IsNullOrEmpty(quality) 	? QUALITY_ASSET 	: quality;
		string _version 	= string.IsNullOrEmpty(version) 	? APP_VERSION 		: version;

		if (Debug.isDebugBuild) {
			_appName += "_dev";
		}
	
//		AMPSListener.Instance.Init();

		#if !UNITY_EDITOR && UNITY_IPHONE
//			InitDMOAssetManagerWithPath(assetDownloadPath, _appName, _language, _quality, _version);
			InitDMOAssetManagerWithPath(unityListenerName, assetDownloadPath, _appName, _language, _quality, _version);
		#elif !UNITY_EDITOR && UNITY_ANDROID
			javaBinding.CallStatic("InitDMOAssetManagerWithPath", unityListenerName, assetDownloadPath, _appName, _language, _quality, _version, "0480x0800");
		#endif
	}
	
	public static void DownloadAsset(string assetDownloadPath, string assetName)
	{
		#if !UNITY_EDITOR && UNITY_IPHONE
//			DownloadAssetWithName(assetName);
			DownloadAssetWithName(assetDownloadPath, assetName);
		#elif !UNITY_EDITOR && UNITY_ANDROID
			javaBinding.CallStatic("DownloadAssetWithName", assetDownloadPath, assetName);
		#endif
	}
	
	public static string GetVersionOfFile(string assetDownloadPath, string assetName)
	{
		Debug.Log("[AMPSBinding] GetVersionOfFile: " + assetName);

		#if !UNITY_EDITOR && UNITY_IPHONE
//			return VersionOfFile(assetName);
			return VersionOfFile(assetDownloadPath, assetName);
		#elif !UNITY_EDITOR && UNITY_ANDROID
			return javaBinding.CallStatic<string>("VersionOfFile", assetDownloadPath, assetName);
		#else
			return " ";
		#endif
	}
	
	public static bool IsCatalogDownloaded(string assetDownloadPath)
	{
		#if !UNITY_EDITOR && UNITY_IPHONE
		return IsCatalogDownloaded(assetDownloadPath);
		#else
		return true;
		#endif
	}
	
	/// <summary>
	/// Gets the size of the android screen as string in a special format Ex: 0480x1024.
	/// </summary>
	private static string GetAndroidScreenSize()
	{
		string x = Screen.width.ToString();
		string y = Screen.height.ToString();
		
		if(Screen.width < 1000)
			x = 0 + x;
		
		if(Screen.height < 1000)
		 	y = 0 + y;
		
		return string.Format("{0}x{1}", x, y);
	}
}
