using UnityEngine;
using System.Collections;

#if (UNITY_ANDROID) && (!UNITY_EDITOR)
public class AndroidPlatformUtils : IPlatformUtils {

	public override string BundleVersion() {	
		//Get current activity
		AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject currentActivity = unityPlayerClass.GetStatic< AndroidJavaObject >("currentActivity"); 
		
		//currentActivity.getPackageManager().getPackageInfo(this.getPackageName(), 0).versionName;
		AndroidJavaObject packageManager = currentActivity.Call< AndroidJavaObject >("getPackageManager");
		AndroidJavaObject packageInfo = packageManager.Call< AndroidJavaObject >("getPackageInfo", currentActivity.Call< string >("getPackageName"), 0);
		return packageInfo.Get< string >("versionName");
	}
	
	public override string BundleID() {	
		//Get current activity
		AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject currentActivity = unityPlayerClass.GetStatic< AndroidJavaObject >("currentActivity"); 
		return currentActivity.Call< string >("getPackageName");
	}
}
#endif
