using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Disney.DMOAnalytics;

using System.Runtime.InteropServices;

public class AnalyticsBinding : MonoBehaviour
{
	static string PLAYER_ID = "";

	static bool DisneyAnalyticsInitialized = false;
	static DMOAnalytics DisneyAnalyticsInstance {
		get {
			if (!DisneyAnalyticsInitialized) {
				Initialize();
			}
			return DMOAnalytics.SharedAnalytics;
		}
	}

	public static void Initialize ()
	{
		if (!DisneyAnalyticsInitialized) {
		
#if !UNITY_EDITOR && !UNITY_WEBPLAYER
			PLAYER_ID = PulseEngine.Utils.GetDeviceGUID();// SystemInfo.deviceUniqueIdentifier;
			
			//handle the exceptions when the gameobject is null
			var iGameOjb = new GameObject("Dummy - DMOAnalytics Coroutines");
			UnityEngine.Object.DontDestroyOnLoad(iGameOjb);
			MonoBehaviour gameObj = iGameOjb.AddComponent<AnalyticsBinding>();
			
			DMOAnalyticsBlackboardConfig config = Blackboard.Instance.GetComponent<DMOAnalyticsBlackboardConfig>();
			DMOAnalyticsBlackboardConfig.DMOAnalyticsPlatformConfig platformConfig = null;

#if UNITY_IOS
			platformConfig = config.iosConfig;
#endif
			
#if UNITY_ANDROID
			//TODO: android and iphone
			platformConfig = config.googlePlayConfig;

#if AMAZON_ANDROID
			platformConfig = config.amazonConfig;
#endif
#endif
			
			string key = "";
			string secret = "";
			string appBundleId = "";
			
			if (platformConfig != null) {
				key = platformConfig.key;
				secret = platformConfig.secret;
				appBundleId = platformConfig.bundleId;
			}
			
			DMOAnalytics.SharedAnalytics.initWithAnalyticsKeySecretAppInfo(gameObj, key, secret, appBundleId, "1.0");
#if UNITY_IOS
			DMOAnalytics.SharedAnalytics.SetDIIDA(iPhone.advertisingIdentifier);
#endif
			DMOAnalytics.SharedAnalytics.LogAppStart();
			
#endif
			DisneyAnalyticsInitialized = true;


			LogEventPlayerInfo();
		}
	}

	private static Dictionary<string, object> DefaultInfoDict () 
	{
		Dictionary<string, object> dict = new Dictionary<string, object>();
		dict["player_id"] = PLAYER_ID;

		return dict;
	} 


	/// <summary>
	/// Logs Player Info
	/// </summary>
	private static void LogEventPlayerInfo()
	{
		Dictionary<string, object> dict = DefaultInfoDict();
		dict["user_id_domain"] = "Disney";
		dict["score"] = UserManagerCloud.Instance.CurrentUser.LastFinishedLvl.ToString();

		Dictionary<string,string> softCurrency = new Dictionary<string, string>(1);
		softCurrency["magic"] = TokensSystem.Instance.ManaPoints.ToString();
		dict["soft_currency"] = softCurrency;


#if !UNITY_EDITOR && !UNITY_WEBPLAYER
		DisneyAnalyticsInstance.LogAnalyticsEventWithContext("player_info", dict);
#endif

		Debug.Log("[AnalyticsBinding] Analytics_LogEventPlayerInfo()");
	}
	
	/* Wrapper methods to native implementation */
		
	/// <summary>
	/// Logs a Game Action event.
	/// </summary>
	/// <param name='context'>
	/// Context
	/// </param>
	/// <param name='action'>
	/// action
	/// </param>
	/// <param name='type'>
	/// type
	/// </param>
	/// <param name='message'>
	/// message
	/// </param>
	/// <param name='level'>
	/// level
	/// </param>
	public static void LogEventGameAction(string context, string action, string type, string message, int level = -1, string subtype = "")
	{
		Dictionary<string, object> dict = DefaultInfoDict();
		dict["context"] = context;
		dict["action"] = action;
		dict["type"] = type;
		dict["message"] = message;

		if (level > -1) {
			dict["level"] = ""+level;
		}

//		if (subtype != "") {
//			dict["subtype"] = subtype;
//		}
#if !UNITY_EDITOR && !UNITY_WEBPLAYER
		DisneyAnalyticsInstance.LogGameAction(dict);
#endif
		Debug.Log("[AnalyticsBinding] Analytics_LogEventGameAction(context, action, type, message, level, subtype): " + context + ", " + action + ", " + type + ", " + message + 
		          ", " + level + ", " + subtype);
	}
	
		
	/// <summary>
	/// Logs a Navitagion Action event.
	/// </summary>
	/// <param name='button_pressed'>
	/// button_pressed
	/// </param>
	/// <param name='from_location'>
	/// from_location
	/// </param>
	/// <param name='to_location'>
	/// to_location
	/// </param>
	/// <param name='target_url'>
	/// target_url
	/// </param>
	public static void LogEventNavigationAction(string button_pressed, string from_location, string to_location, string target_url)
	{
		Dictionary<string, object> dict = DefaultInfoDict();
		dict["button_pressed"] = button_pressed;
		dict["from_location"] = from_location;
		dict["to_location"] = to_location;
		
		if ( (target_url != null) && (target_url.Length > 0) ) {
			dict["target_url"] = target_url;
		}

#if !UNITY_EDITOR  && !UNITY_WEBPLAYER
		DisneyAnalyticsInstance.LogAnalyticsEventWithContext("navigation_action", dict);
#endif

		Debug.Log("[AnalyticsBinding] Analytics_LogEventNavigationAction(button_pressed, from_location, to_location, target_url): " + 
			button_pressed + ", " + from_location + ", " + to_location + ", " + target_url);
	}
	
	
	
	
		
	/// <summary>
	/// Logs a Timing Action Event.
	/// </summary>
	/// <param name='location'>
	/// location
	/// </param>
	/// <param name='elapsed_time'>
	/// elapsed_time
	/// </param>
	public static void LogEventTimingAction(string location, float elapsed_time)
	{
		Dictionary<string, object> dict = DefaultInfoDict();
		dict["location"] = location;
		dict["elapsed_time"] = "" + elapsed_time;

#if !UNITY_EDITOR && !UNITY_WEBPLAYER
		DisneyAnalyticsInstance.LogAnalyticsEventWithContext("timing", dict);
#endif

		Debug.Log("[AnalyticsBinding] Analytics_LogEventTimingAction(location, elapsed_time):" + location + ", " + elapsed_time);
	}
	
	
	
		
	/// <summary>
	/// Logs a Page View event.
	/// </summary>
	/// <param name='location'>
	/// location
	/// </param>
	/// <param name='pageUrl'>
	/// pageUrl
	/// </param>
	/// <param name='message'>
	/// message
	/// </param>
	public static void LogEventPageView(string location, string pageUrl, string message)
	{
		Dictionary<string, object> dict = DefaultInfoDict();
		dict["location"] = location;
		
		if ( (pageUrl!=null) && (pageUrl.Length>0) ) {
			dict["page_url"] = pageUrl;
		}

		if ( (message!=null) && (message.Length>0) ) {
			dict["message"] = message;
		}

#if !UNITY_EDITOR && !UNITY_WEBPLAYER
		DisneyAnalyticsInstance.LogAnalyticsEventWithContext("page_view", dict);
#endif

		Debug.Log("[AnalyticsBinding] Analytics_LogEventPageView(location, pageUrl, message): " + location + ", " + pageUrl + ". " + message);
	}
	
	
	
		
	/// <summary>
	/// Logs a Payment Action Event
	/// </summary>
	/// <param name='currency'>
	/// currency
	/// </param>
	/// <param name='locale'>
	/// locale
	/// </param>
	/// <param name='amountPaid'>
	/// amountPaid
	/// </param>
	/// <param name='itemId'>
	/// itemId
	/// </param>
	/// <param name='itemCount'>
	/// itemCount
	/// </param>
	/// <param name='type'>
	/// type
	/// </param>
	/// <param name='subtype'>
	/// subtype
	/// </param>
	/// <param name='context'>
	/// context
	/// </param>
	/// <param name='level'>
	/// level
	/// </param>
	public static void LogEventPaymentAction(string currency, string locale, float amountPaid, string itemId, int itemCount,
                                     string type, string subtype, string context, int level)
	{
		Dictionary<string, object> itemInfo = new Dictionary<string, object>();
		itemInfo["item_id"] = itemId;
		itemInfo["item_count"] = itemCount.ToString();
		
		Dictionary<string, object> dict = DefaultInfoDict();
		dict["currency"] = (object) currency;
		dict["locale"] = (object) locale;
		dict["amount_paid"] = (object)amountPaid;
		dict["item"] = (object) itemInfo;
		dict["type"] = (object)type;
		
		if ( (subtype!=null) && (subtype.Length>0) ) {
			dict["subtype"] = subtype;
		}

		if ( (context!=null) && (context.Length>0) ) {
			dict["context"] = context;
		}

		if (level>=0) {
			dict["level"] = (object)(""+level);
		}

#if !UNITY_EDITOR && !UNITY_WEBPLAYER
#if UNITY_ANDROID
		DisneyAnalyticsInstance.LogAnalyticsEventWithContext("payment_action", dict);
#else
		DisneyAnalyticsInstance.LogMoneyAction(dict);
#endif
#endif

		Debug.Log("[AnalyticsBinding] Analytics_LogEventPaymentAction(currency, locale, amountPaid, itemId, itemCount, type, subtype, context, level): " +
			currency + ", " + locale + ", " + amountPaid + ", " + itemId + ", " + itemCount + ", " + type + ", " + subtype + ", " + context + ", " + level);
	}

	public static void LogInAppCurrencyAction(string currency, float amount, string itemId, int itemCount, string context, 
	                                          string action, float balance, string type, string message, int level) 
	{

		Dictionary<string, object> dict = DefaultInfoDict();
		dict["currency"] = currency;
		dict["amount"] = amount.ToString();
		dict["action"] = action;
		dict["balance"] = balance.ToString();
		dict["type"] = type;

		if (itemId != null) {
			Dictionary<string, object> itemInfo = new Dictionary<string, object>();
			itemInfo["item_id"] = itemId;
			itemInfo["item_count"] = itemCount.ToString();
			dict["item"] = itemInfo;
		}

		if (context != null) {
			dict["context"] = context;
		}

		if (message != null) {
			dict["message"] = message;
		}

		if(level >= 0) {
			dict["level"] = level.ToString();
		}

#if !UNITY_EDITOR && !UNITY_WEBPLAYER
		DisneyAnalyticsInstance.LogAnalyticsEventWithContext("in_app_currency_action", dict);
#endif

		Debug.Log("[AnalyticsBinding] Analytics_LogAnalyticsEventWithContext " + "currency:" + currency + " amount:" + amount 
		          + " action:" + action + " balance:" + balance + " type:" + type);

	}

	public static void LogAdAction(string creative, string placement, string offer, string type, string gross_revenue, 
	                               string currency, string locale)
	{
		Dictionary<string, object> dict = DefaultInfoDict();
		dict["creative"] = creative;
		dict["placement"] = placement;
		dict["type"] = type;

		if(offer != null) {
			dict["offer"] = offer;
		}

		if (gross_revenue != null) {
			dict["gross_revenue"] = gross_revenue;
		}

		if (currency != null) {
			dict["currency"] = currency;
		}

		if (locale != null) {
			dict["locale"] = locale;
		}

#if !UNITY_EDITOR && !UNITY_WEBPLAYER
		DisneyAnalyticsInstance.LogAnalyticsEventWithContext("ad_action", dict);
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
