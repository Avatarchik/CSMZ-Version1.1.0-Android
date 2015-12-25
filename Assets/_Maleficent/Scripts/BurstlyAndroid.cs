using UnityEngine;
using System.Collections;

public class BurstlyAndroid : MonoBehaviour {

	//Delete this
	void Start() {
		ShowAds();
	}

	public static void ShowAds() {
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.disney.maleficent.BurstlyActivity");
		//jc.CallStatic("Init");
		//jc.CallStatic("ShowAds");
#endif
	}
	
	public static void HideAds() {
	
	}
}
