using UnityEngine;
using System.Collections;
using Disney.DMOAnalytics;
public class DMOAnalyticsAutoLog : MonoBehaviour {
	public string DMOAnalyticsKey;
	public string DMOAnalyticsSecret;
	public string ApplicationBundleIdentifer;
	public string ApplicationVersion;
	void Start () {
		DMOAnalytics.SharedAnalytics.LogAppStart();
	}
	
	void Update () {
		
	}
	
	void Awake () {
		DontDestroyOnLoad(this);
		#if UNITY_IPHONE && !UNITY_EDITOR
		DMOAnalytics.SharedAnalytics.initWithAnalyticsKeySerect(this, DMOAnalyticsKey, DMOAnalyticsSecret);
		#elif UNITY_ANDROID && !UNITY_EDITOR
		DMOAnalytics.SharedAnalytics.initWithAnalyticsKeySerect(this, DMOAnalyticsKey, DMOAnalyticsSecret);
		#else
		DMOAnalytics.SharedAnalytics.initWithAnalyticsKeySecretAppInfo(this, DMOAnalyticsKey, DMOAnalyticsSecret, ApplicationBundleIdentifer, ApplicationVersion);
		#endif
	}
	
	void OnApplicationPause(bool paused) {
		if(paused) {
			DMOAnalytics.SharedAnalytics.LogAppBackground();
		} else {
			DMOAnalytics.SharedAnalytics.LogAppForeground();
		}
	}
	
	void OnApplicationQuit() {
		DMOAnalytics.SharedAnalytics.LogAppEnd();
	}
}
