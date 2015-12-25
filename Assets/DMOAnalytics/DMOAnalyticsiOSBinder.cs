using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Disney.DMOAnalytics.Framework {
#if UNITY_IPHONE
public class DMOAnalyticsiOSBinder {
	[DllImport("__Internal")]
		private static extern void _dmoAnalyticsInit(string appID, string appKey);
			public static void initDMOAnalytics( string appID, string appKey )
			{
				if( Application.platform == RuntimePlatform.IPhonePlayer )
					_dmoAnalyticsInit( appID, appKey );
			}
	
	[DllImport("__Internal")]
		private static extern void _dmoAnalyticsLogEvent(string eventString);
			public static void dmoAnalyticsLogEvent( string eventString )
			{
				if( Application.platform == RuntimePlatform.IPhonePlayer )
					_dmoAnalyticsLogEvent( eventString );
			}
	
	
	[DllImport("__Internal")]
		private static extern void _dmoAnalyticsLogAppStart();
		public static void dmoAnalyticsLogAppStart() {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_dmoAnalyticsLogAppStart();
		}
	
	
	[DllImport("__Internal")]
		private static extern void _dmoAnalyticsLogEventWithContext(string eventName, string context);
		public static void dmoAnalyticsLogEventWithContext(string eventName, string context) {
		if( Application.platform == RuntimePlatform.IPhonePlayer ) {
			_dmoAnalyticsLogEventWithContext(eventName, context);
		 }
		}
	
	
	[DllImport("__Internal")]
		private static extern void _dmoAnalyticsflushAnalyticsQueue();
		public static void dmoAnalyticsflushAnalyticsQueue() {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_dmoAnalyticsflushAnalyticsQueue();	
	    }
	
	[DllImport("__Internal")]
		private static extern void _dmoAnalyticsSetDebugLogging(bool debugLogging);
		public static void dmoAnalyticsSetDebugLogging(bool debugLogging) {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_dmoAnalyticsSetDebugLogging(debugLogging);	
	    }
	
	[DllImport("__Internal")]
		private static extern void _dmoAnalyticsSetCanUseNetwork(bool canUseNetwork);
		public static void dmoAnalyticsSetCanUseNetwork(bool canUseNetwork) {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_dmoAnalyticsSetCanUseNetwork(canUseNetwork);	
	    }
		
	[DllImport("__Internal")]
		private static extern void _dmoAnalyticsSetDIIDA(string advertisingID);
		public static void dmoAnalyticsSetDIIDA(string advertisingID) {
			_dmoAnalyticsSetDIIDA(advertisingID);
		}
}
#endif
}