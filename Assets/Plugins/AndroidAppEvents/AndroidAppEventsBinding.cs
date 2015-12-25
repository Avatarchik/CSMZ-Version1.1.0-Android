using UnityEngine;
using System.Collections;

public class AndroidAppEventsBinding 
{
#region Interface with the Android UnityPluginOSControl
	#if !UNITY_EDITOR  && UNITY_ANDROID
	private static AndroidJavaClass javaBinding;
	
	static AndroidAppEventsBinding() 
	{
		Debug.Log("com.mobilitygames.androidappevents.AndroidAppEvents Unity binding initialized...");
		javaBinding = new AndroidJavaClass("com.mobilitygames.androidappevents.AndroidAppEvents");
	}
	#endif
#endregion
	
	public static string GetOSRingerMode()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
			return javaBinding.CallStatic<string>("getOSRingerMode");
		#else
		
			return "";
		#endif
	}
	
	public static void RegisterAudioMgrEventsHandler(string handlerGameObjName, string ringerModeChangedEventName)
	{
		Debug.Log(string.Format("[AndroidAppEventsBinding] RegisterAudioMgrEventsHandler(string handlerGameObjName, string ringerModeChangedEventName): {0}, {1}",
			handlerGameObjName, ringerModeChangedEventName));
			
		#if !UNITY_EDITOR && UNITY_ANDROID
			javaBinding.CallStatic("registerAudioMgrEventsHandler", handlerGameObjName, ringerModeChangedEventName);
		#endif
	}
	
	public static void UnregisterAudioMgrEventsHandler()
	{
		Debug.Log("[AndroidAppEventsBinding] UnregisterAudioMgrEventsHandler()");

		#if !UNITY_EDITOR && UNITY_ANDROID
			javaBinding.CallStatic("unregisterAudioMgrEventsHandler");
		#endif
	}
}
