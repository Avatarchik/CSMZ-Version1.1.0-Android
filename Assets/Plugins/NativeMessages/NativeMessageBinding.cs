using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class NativeMessageBinding 
{
#if UNITY_EDITOR || UNITY_WEBPLAYER
	private static float Native_GetScreenWidth()
	{
		Debug.Log("[NativeMessageBinding] Native_GetScreenWidth() called...");
		return Screen.width;
	}
	
	private static float Native_GetScreenHeight()
	{
		Debug.Log("[NativeMessageBinding] Native_GetScreenHeight() called...");
		return Screen.height;
	}

#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern float Native_GetScreenWidth();
	
	[DllImport ("__Internal")]
	private static extern float Native_GetScreenHeight();

	[DllImport ("__Internal")]
	public static extern void Native_ShowMessage(string title, string message, string button1, string button2, string button3);
	
	[DllImport ("__Internal")]
	public static extern void Native_ScheduleNotification(string title, string message, long showTime);
	
	[DllImport ("__Internal")]
	public static extern void Native_CancelNotifications(string message);
	
	[DllImport ("__Internal")]
	public static extern string Native_GetRateLink(string appId);
	
	[DllImport ("__Internal")]
	public static extern string Native_GetLocaleCountry();
#elif UNITY_ANDROID
	private static float Native_GetScreenWidth()
	{
		return Screen.width;
	}
	
	private static float Native_GetScreenHeight()
	{
		return Screen.height;
	}

#endif
	
#if !UNITY_EDITOR && UNITY_ANDROID
	public static AndroidJavaClass  javaBinding = null;
	
	static NativeMessageBinding()
	{
		javaBinding = new AndroidJavaClass("com.mobilitygames.messagescontroller.AndroidMessagesController");
	}
#endif
	
	
	public static float GetNativeScreenWidth()
	{
		return Native_GetScreenWidth();
	}
	
	public static float GetNativeScreenHeight()
	{
		return Native_GetScreenHeight();
	}
	
	
}
