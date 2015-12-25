using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class UrbanAirshipBinding
{
	public static bool isInitialized = false;
	
#if !UNITY_EDITOR && UNITY_ANDROID
	protected static AndroidJavaClass proxyInterface = null;
#endif
	
	public static void Init() 
	{
		if ( isInitialized ) {
			return;
		}
		
		Debug.Log("[UrbanAirshipBinding] InitUrbanAirship() native call...");
		
#if !UNITY_EDITOR && UNITY_IOS
		GIUALibraryBinding.requestAppRegistration();
#elif !UNITY_EDITOR && UNITY_ANDROID
		proxyInterface = new AndroidJavaClass("com.mobilitygames.FrozenPlugins.FrozenProxyInterface");
		proxyInterface.CallStatic("EnableUAPush");
#endif
		
		isInitialized = true;
	}

}
