using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class IOSReferalStore : ReferalStore
{
	
#if (UNITY_IPHONE) && (!UNITY_EDITOR)
	[DllImport ("__Internal")]
	private static extern void ReferalStoreManager_showReferalView();
	[DllImport ("__Internal")]
	private static extern void ReferalStoreManager_setAppID(string appId);
	[DllImport ("__Internal")]
	private static extern void ReferalStoreManager_setCallbackObjectName(string name);
	[DllImport ("__Internal")]
	private static extern void ReferalStoreManager_unsetCallbackObjectName(string name);
#endif

	void Awake()
	{
		_SetCallBackObjectName(gameObject.name);
		Debug.Log("name " + name);
	}


	void OnDestroy()
	{
		_UnsetCallBackObjectName("unset");
	}


	public override void ShowReferalView()
	{
#if (UNITY_IPHONE) && (!UNITY_EDITOR)
		ReferalStoreManager_showReferalView();
#else 
		Debug.Log("[iOSReferalStore] Showing referal view");
#endif

	}

	protected override void _SetAplicationID(string appID)
	{
#if (UNITY_IPHONE) && (!UNITY_EDITOR)
		ReferalStoreManager_setAppID(appID);
#else 
		Debug.Log("[iOSReferalStore] Showing referal view With AppID");
#endif
	}


	public void ReferallCallback(string eventInfo)
	{
		Debug.Log(" ** EventInfo: " + eventInfo);
		if (eventInfo == "ReferalDismissed") 
		{
			_CallOnReferalStoreClosed();
		}
	}


	//Private

	private void _SetCallBackObjectName(string callbackGameObjectName)
	{
		Debug.Log(" ** SetCallBackObjectName: " + callbackGameObjectName);
#if (UNITY_IPHONE) && (!UNITY_EDITOR)
		ReferalStoreManager_setCallbackObjectName(callbackGameObjectName);
#endif
	}

	private void _UnsetCallBackObjectName(string unsetName)
	{
#if (UNITY_IPHONE) && (!UNITY_EDITOR)
		ReferalStoreManager_unsetCallbackObjectName(unsetName);
#endif
	}
}
