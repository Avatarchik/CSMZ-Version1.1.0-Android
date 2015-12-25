using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Disney.DMOAnalytics.Framework {
public class DMOAnalyticsManager {
		public static MonoBehaviour sharedGameObject;
		public static DMONetworkRequest request = DMONetworkRequest.Instance;
		
		public static void dmoAnalyticsInitWithAnalyticsKeySecret()
		{
			#if UNITY_IPHONE && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				DMOAnalyticsiOSBinder.initDMOAnalytics(DMOAnalyticsSysInfo.getAnalyticsKey(), DMOAnalyticsSysInfo.getAnalyticsSecret());
				return;
			}
			#elif UNITY_ANDROID &&!UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				DMOBinderAndroid.initDMOAnalytics(DMOAnalyticsSysInfo.getAnalyticsKey(), DMOAnalyticsSysInfo.getAnalyticsSecret());
				return;
			}
			#endif
		}
		
		
		public static void dmoAnalyticsLogEvent(string EventInfo) {
			#if UNITY_IPHONE && !UNITY_EDITOR			
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				DMOAnalyticsiOSBinder.dmoAnalyticsLogEvent(EventInfo);
				return;
			}
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				DMOBinderAndroid.dmoAnalyticsLogEvent(EventInfo);
				return;
			}
			#else
			request.logEvent(sharedGameObject,EventInfo);
			#endif
		}
		
		public static void dmoAnalyticsLogAppStart() {
			#if UNITY_IPHONE && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				DMOAnalyticsiOSBinder.dmoAnalyticsLogAppStart();
				return;
			}
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				DMOBinderAndroid.dmoAnalyticsLogAppStart();
				return;
			}
			#else
			request.logAppStartEvent(sharedGameObject);
			#endif
		}
		
		public static void dmoAnalyticsLogAppForeground() {
			#if UNITY_IPHONE && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.IPhonePlayer) return;
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				DMOBinderAndroid.dmoAnalyticsLogAppForeground();
				return;
			}
			#else
			request.logAppEvent(sharedGameObject, DMOAnalyticsSysInfo.appForeground);
			#endif
		}
		
		public static void dmoAnalyticsLogAppBackground() {
			#if UNITY_IPHONE && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.IPhonePlayer) return;
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				DMOBinderAndroid.dmoAnalyticsLogAppBackground();
				return;
			}
			#else
			request.logAppEvent(sharedGameObject, DMOAnalyticsSysInfo.appBackground);
			#endif
		}
		
		public static void dmoAnalyticsLogAppEnd() {
			#if UNITY_IPHONE && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.IPhonePlayer) return;
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				DMOBinderAndroid.dmoAnalyticsLogAppBackground();
				return;
			}
			#else
			request.logAppEvent(sharedGameObject, DMOAnalyticsSysInfo.appEnd);
			#endif
		}
		
		public static void dmoAnalyticsLogGameAction(Dictionary<string, object> GameData) {
			#if UNITY_IPHONE && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				string dictData = DMOAnalyticsHelper.GetStringFromDictionary(GameData);
				DMOAnalyticsiOSBinder.dmoAnalyticsLogEventWithContext(DMOAnalyticsSysInfo.gameAction , dictData);
				return;
			}
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				string dictData = DMOAnalyticsHelper.GetStringFromDictionary(GameData);
				DMOBinderAndroid.dmoAnalyticsLogGameAction(dictData);
				return;
			}
			#else
			request.logGameEvent(sharedGameObject, GameData);
			#endif
		}
		
		public static void dmoAnalyticsLogMoneyAction (Dictionary<string, object> MoneyData) {
			#if UNITY_IPHONE && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				string dictData = DMOAnalyticsHelper.GetStringFromDictionary(MoneyData);
				DMOAnalyticsiOSBinder.dmoAnalyticsLogEventWithContext(DMOAnalyticsSysInfo.iOSMoneyAction, dictData);
				return;
			}
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				string dictData = DMOAnalyticsHelper.GetStringFromDictionary(MoneyData);
				DMOBinderAndroid.dmoAnalyticsLogMoneyAction(dictData);
				return;
			}
			#else
			request.logMoneyEvent(sharedGameObject, MoneyData);
			#endif
		}
		
		public static void dmoAnalyticsLogAnalyticsEventWithContext(string eventName, Dictionary<string, object> dataDetails) {
			#if UNITY_IPHONE && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				string dictData = DMOAnalyticsHelper.GetStringFromDictionary(dataDetails);
				DMOAnalyticsiOSBinder.dmoAnalyticsLogEventWithContext(eventName, dictData);
				return;
			}
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				string dictData = DMOAnalyticsHelper.GetStringFromDictionary(dataDetails);
				DMOBinderAndroid.dmoAnalyticsLogEventWithContext(eventName, dictData);
				return;
			}
			#else
			request.logEventWithContext(sharedGameObject, eventName, dataDetails);
			#endif
		}
		
		public static void dmoAnalyticsFlushAnalyticsQueue() {
			#if UNITY_IPHONE && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				DMOAnalyticsiOSBinder.dmoAnalyticsflushAnalyticsQueue();
				return;
			}
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if (Application.platform == RuntimePlatform.Android) {
				DMOBinderAndroid.dmoAnalyticsflushAnalyticsQueue();
				return;
			}
			#else
			request.FlushQueue();
			#endif
		}
}
}