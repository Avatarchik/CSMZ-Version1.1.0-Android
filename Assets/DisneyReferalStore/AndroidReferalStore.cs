using UnityEngine;
using System.Collections;

public class AndroidReferalStore : ReferalStore 
{

	public AndroidReferalStore() : base()
	{

	}

	public override void ShowReferalView()
	{
#if UNITY_ANDROID
		//Get current activity
		AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject currentActivity = unityPlayerClass.GetStatic< AndroidJavaObject >("currentActivity"); 
		
		//Create intent
		Debug.Log("Getting DMNReferralStoreActivity class");
		AndroidJavaClass DMNReferealStoreActivityClass = new AndroidJavaClass("com.mobilenetwork.referralstore.DMNReferralStoreActivity");
		Debug.Log("Creating intent");
		AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", currentActivity, DMNReferealStoreActivityClass);


		//Bundle
		AndroidJavaObject bundle = new AndroidJavaObject("android.os.Bundle");
		//bundle.Call("putString", DMNReferealStoreActivityClass.GetStatic<string>("BOOT_URL"), "https://disneynetwork0-a.akamaihd.net/mobilenetwork/referralstore/bootstrap/");
		//bundle.Call("putString", DMNReferealStoreActivityClass.GetStatic<string>("GCS_URL"), "https://api.disney.com/mobilenetwork/referralstore/v1/config");
		bundle.Call("putString", DMNReferealStoreActivityClass.GetStatic<string>("APP_ID"), currentActivity.Call<string>("getPackageName"));
		intent.Call< AndroidJavaObject >("putExtras", bundle);

		
		//Start intent
		Debug.Log("start intent");
		currentActivity.Call("startActivity", intent);

#endif
	}

	public void OnApplicationPause(bool _paused) {
		if(!_paused) {
			_CallOnReferalStoreClosed();
		}
	}
}
