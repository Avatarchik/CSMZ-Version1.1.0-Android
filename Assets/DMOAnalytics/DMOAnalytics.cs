using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Disney.DMOAnalytics.Framework;
namespace Disney.DMOAnalytics
{
	/**
	 * DMOAnalytics is your one-stop analytics object. 
	 * Make sure you have a pair of appkey and secret for analytics. 
	 * The first time you call DMOAnalytics, please call initWithAnalyticsKeySecretAppInfo to assign the correct app key and secret for analytics call. 
	 * */
	public class DMOAnalytics
	{
		private static DMOAnalytics mInstance;
		public string AnalyticsKey {get; private set;}
		public string AnalyticsSecret {get; private set;}
		
		/* 
		 * Switch On DebugLogging by setting it to true. By default, it's false. 
		 * */
		public bool DebugLogging {
			get {
				return DMOAnalyticsHelper.isDebugEnvLog;
			}
			set {
				DMOAnalyticsHelper.isDebugEnvLog = value;
				#if UNITY_IPHONE && !UNITY_EDITOR
				if (Application.platform == RuntimePlatform.IPhonePlayer) DMOAnalyticsiOSBinder.dmoAnalyticsSetDebugLogging(value);
				#elif UNITY_ANDROID && !UNITY_EDITOR
				if (Application.platform == RuntimePlatform.Android) DMOBinderAndroid.dmoAnalyticsSetDebugLogging(value);
				#endif
			}
		}
		
		/*
		 * Pause or resume the network access in DMOAnalytics. By default, it's true.
		 * */
		public bool CanUseNetwork {
			get {
				return DMOAnalyticsHelper.ICanUseNetwork;
			}
			set {
				DMOAnalyticsHelper.ICanUseNetwork = value;
				#if UNITY_IPHONE && !UNITY_EDITOR
				if (Application.platform == RuntimePlatform.IPhonePlayer) DMOAnalyticsiOSBinder.dmoAnalyticsSetCanUseNetwork(value);
				#elif UNITY_ANDROID && !UNITY_EDITOR
				if (Application.platform == RuntimePlatform.Android) DMOBinderAndroid.dmoAnalyticsSetCanUseNetwork(value);
				#endif
			}
		}
		
		
		public void SetDIIDA (string advertisingId) {
			#if (UNITY_IPHONE) && !UNITY_EDITOR
			DMOAnalyticsiOSBinder.dmoAnalyticsSetDIIDA(advertisingId);
			#endif	
		}	
       
			
		/* 
		 * Get the current library version.
		 * */
		public static string GetLibVersion() {
			return DMOAnalyticsSysInfo.mVersion;
		}
		
		//create a singleton analytics class init with the AnalyticsKey and AnalyticsSecret
		private DMOAnalytics ()
		{
			
		}
		
		/* 
		 * singleton instance of DMOAnalytics
		 * */
		public static DMOAnalytics SharedAnalytics {
			get {
				if (mInstance == null) {
					mInstance = new DMOAnalytics();
				}
				return mInstance;
			}
		}
		
		/**
		 * Convenient method to initialize the analytics with GameObject, analytics key, secret for iOS and Android apps. 
		 */
		public void initWithAnalyticsKeySerect(MonoBehaviour GameObj, string AppKey, string AppSecret) {
			#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
			//DMCT-1682) For iOS, the DMOAnalytics library will automatically get the Bundle Identifier and Bundle Version from info.plist .
            //For Android, the DMOAnalytics library will automatically get the package name and app version from AndroidManifest.xml file.
			//Therefore, we are passing a default dummy value here as it won't be used for analytics call. 
			initWithAnalyticsKeySecretAppInfo(GameObj, AppKey, AppSecret, "com.unity.player", "1.0");
			#else
			Debug.LogError("initWithAnalyticsKeySerect works only for iOS and Android platforms.");
            #endif
		}
		
		
		/**
		 * Initialize the analytics with GameObject, analytics key, secret, BundleIdentifier and AppVersion, 
		 */
		public void initWithAnalyticsKeySecretAppInfo( MonoBehaviour GameObj, string AppKey, string AppSecret, string BundleIdentifier, string AppVersion ) {
			if (GameObj == null || String.IsNullOrEmpty( AppKey ) || String.IsNullOrEmpty( AppSecret ) || String.IsNullOrEmpty( BundleIdentifier ) || String.IsNullOrEmpty( AppVersion )) {
			 	Debug.LogError("Please make sure you have the correct Analytics Key,  Analytics Secret, BundleIdentifier and AppVersion for DMOAnalytics");
				return;
			}
			AnalyticsKey = AppKey;
			AnalyticsSecret = AppSecret;
			DMOAnalyticsSysInfo.setAppVersion(AppVersion);
			DMOAnalyticsSysInfo.setBundelIdentifer(BundleIdentifier);
			DMOAnalyticsManager.sharedGameObject = GameObj;
			DMOAnalyticsManager.dmoAnalyticsInitWithAnalyticsKeySecret();
		}
		
		/**
		 * An all-purpose analytics logging call. Use this for arbitrary events.
		 */
		public void LogEvent(String EventInfo)
	    {
			if (EventInfo == null) return;
			DMOAnalyticsHelper.Log("Log Event: "+ EventInfo);
			DMOAnalyticsManager.dmoAnalyticsLogEvent(EventInfo);
		}
		
		
		/**
		 * Application core events
		 */ 
		public void LogAppStart() {
			DMOAnalyticsHelper.Log("Log app_start ");
			DMOAnalyticsManager.dmoAnalyticsLogAppStart();
		}

		
		public void LogAppForeground() {
			DMOAnalyticsHelper.Log("Log app_foreground ");
			DMOAnalyticsManager.dmoAnalyticsLogAppForeground();
		}
		
		public void LogAppBackground() {
			DMOAnalyticsHelper.Log("Log app_background ");
			DMOAnalyticsManager.dmoAnalyticsLogAppBackground();
		}
		 
		public void LogAppEnd() {
			DMOAnalyticsHelper.Log("Log app_end ");
			DMOAnalyticsManager.dmoAnalyticsLogAppEnd();
		}
		
		/**
		 * Log a game action analytics event with details
		 */
		public void LogGameAction( Dictionary<string, object> GameData ) {
			if (GameData == null) return;
			DMOAnalyticsHelper.Log("Log game_action");
			DMOAnalyticsManager.dmoAnalyticsLogGameAction(GameData);
		}
		
		/**
		 * Log a money action analytics event with details
		 */
		public void LogMoneyAction( Dictionary<string, object> MoneyData ) {
			if (MoneyData == null) return;
			DMOAnalyticsHelper.Log("Log money action");
			DMOAnalyticsManager.dmoAnalyticsLogMoneyAction(MoneyData);
		}
		
		/**
		 * Use this for arbitrary events which require more complex 
		 * description of context. The scope string should be the top-level 
		 * event scope for the rest of the data.  An example would be "store".  
		 * The'dataDetails' dictionary might look something like 
		 * {"itemname": "fingle", "price": "200", "page": "3" }.  In most 
		 * cases, this should conform to whatever schema or contract you have for 
		 * your application's tracking data.
		 * 
		 * @method    logAnalyticsEventWithContext
		 * @abstract  Log an event which has a top-level scope and some finer level detail in a dictionary.
		 * @param     eventName: the string name for the event to be fired
		 * @param     dataDetails: a dictionary to be sent with the request as the data field
		 */
		public void LogAnalyticsEventWithContext(string eventName, Dictionary<string, object> dataDetails) {
			if (eventName == null || dataDetails == null) return;
			DMOAnalyticsHelper.Log("Log arbitrary action with Context: "+ eventName);
			DMOAnalyticsManager.dmoAnalyticsLogAnalyticsEventWithContext(eventName, dataDetails);
		}
		
		/**
		 * Attempt to post any queued analytic events to the network. Won't do anything if canUseNetworkis set to NO. 
		 * Otherwise post the queued up analytics events to the server.
		 */
		public void FlushAnalyticsQueue() {
			DMOAnalyticsManager.dmoAnalyticsFlushAnalyticsQueue();
		}
		

	}
}

