using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Disney.DMOAnalytics.Framework {
public class DMOBinderAndroid {
	#if UNITY_ANDROID
	private static AndroidJavaObject _plugin;
	private static AndroidJavaObject playerActivityContext;
	static DMOBinderAndroid()
	{
		if( Application.platform == RuntimePlatform.Android ) {
			using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
				playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			// find the plugin instance
			using( var pluginClass = new AndroidJavaClass( "com.dismo.AnalyticsPlugin" ) ) {
				if (pluginClass != null) {
				  _plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
						if (_plugin!= null)
							_plugin.Call("setContext", playerActivityContext);
				}
			}
		}
	}
	
	public static void initDMOAnalytics( string appKey, string appSecret )
	{
		if( Application.platform == RuntimePlatform.Android &&_plugin!=null) {
			_plugin.Call( "init", appKey, appSecret );
		}
	}
	
	public static void dmoAnalyticsLogEvent ( string appEvent )
	{
		if( Application.platform == RuntimePlatform.Android &&_plugin!=null) {
			_plugin.Call( "dmoAnalyticsLogEvent", appEvent);
		}
	}
	
	public static void dmoAnalyticsLogAppStart() {
		if( Application.platform == RuntimePlatform.Android&&_plugin!=null ) {
			_plugin.Call( "dmoAnalyticsLogAppStart");
		}
	}
		
	public static void dmoAnalyticsLogAppForeground() {
		if( Application.platform == RuntimePlatform.Android&&_plugin!=null ) {
			_plugin.Call( "dmoAnalyticsLogAppForeground");
		}
	}	
		
	
	public static void dmoAnalyticsLogAppBackground() {
		if( Application.platform == RuntimePlatform.Android&&_plugin!=null ) {
			_plugin.Call( "dmoAnalyticsLogAppBackground");
		}
	}
		
		
	public static void dmoAnalyticsLogAppEnd() {
		if( Application.platform == RuntimePlatform.Android&&_plugin!=null ) {
			_plugin.Call( "dmoAnalyticsLogAppEnd");
		}
	}
		
	public static void dmoAnalyticsLogEventWithContext(string eventName, string parameters) {
		if( Application.platform == RuntimePlatform.Android&&_plugin!=null ) {
			_plugin.Call( "dmoAnalyticsLogEventWithContext", eventName, parameters);
		}
	}
	
	public static void dmoAnalyticsflushAnalyticsQueue() {
		if( Application.platform == RuntimePlatform.Android ) {
			_plugin.Call( "dmoAnalyticsflushAnalyticsQueue");
		}
	}
	
	public static void dmoAnalyticsLogGameAction(string parameters) {
		if( Application.platform == RuntimePlatform.Android &&_plugin!=null ) {
			_plugin.Call( "dmoAnalyticsLogGameAction", parameters);
		}
	}
	
	public static void dmoAnalyticsLogMoneyAction(string parameters) {
		if( Application.platform == RuntimePlatform.Android &&_plugin!=null ) {
			_plugin.Call( "dmoAnalyticsLogMoneyAction", parameters);
		}
	}
	
	
	public static void dmoAnalyticsSetDebugLogging(bool isEnable) {
		if( Application.platform == RuntimePlatform.Android ) {
			_plugin.Call( "dmoAnalyticsSetDebugLogging", isEnable);
		}
	}
	
	public static void dmoAnalyticsSetCanUseNetwork(bool isEnable) {
		if( Application.platform == RuntimePlatform.Android ) {
			_plugin.Call( "dmoAnalyticsSetCanUseNetwork", isEnable);
		}
	}
	
#endif
}
}