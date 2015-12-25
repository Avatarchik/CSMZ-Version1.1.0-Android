#if UNITY_EDITOR
//using UnityEditor;
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JsonFx.Json;

//[ExecuteInEditMode]
public class Kochava : MonoBehaviour
{
	
	#region Hardware Integration
	
	#if UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string GetExternalKochavaInfo(bool incognitoMode);
	#endif
	
	#endregion
	
	#region Settings
	
	// Application-Configurable Settings
	public string kochavaAppId = "";
	public string kochavaAppIdIOS = "";
	public string kochavaAppIdAndroid = "";
	public string kochavaAppIdKindle = "";
	public string kochavaAppIdBlackberry = "";
	public string kochavaAppIdWindowsPhone = "";
	public bool debugMode = false;
	public static bool DebugMode { get { return _S.debugMode; } set { _S.debugMode = value; } }
	public bool incognitoMode = false;
	public static bool IncognitoMode { get { return _S.incognitoMode; } set { _S.incognitoMode = value; } }
	public bool requestAttribution = false;
	public static bool RequestAttribution { get { return _S.requestAttribution; } set { _S.requestAttribution = value; } }
	private bool debugServer = false;
	public string appVersion = "";
	public string appIdentifier = "";
	[HideInInspector]
	public string partnerId = "";
	[HideInInspector]
	public string partnerName = "";
	
	// Hardwired Helpers
	private string appPlatform = "desktop";
	private string kochavaDeviceId = "";
	
	// Attribution
	private Dictionary<string, object> attributionData = null;
	public static Dictionary<string, object> AttributionData { get { return _S.attributionData; } set { _S.attributionData = value; } }
	private string attributionDataStr = "";
	public static string AttributionDataStr { get { return _S.attributionDataStr; } set { _S.attributionDataStr = value; } }
	
	// Identifier Blacklisting
	private List<string> devIdBlacklist = new List<string> ();
	public static List<string> DevIdBlacklist { get { return _S.devIdBlacklist; } set { _S.devIdBlacklist = value; } }
	
	// Economics
	public string appCurrency = "USD";
	
	// Session Tracking
	public enum KochSessionTracking {
		full,			// Accuratley track active users - register launch, pause, resume, and exit
		basic,			// Track active users - register launch and exit
		minimal,		// Track user play time	- register exit
		disabled		// Do not track session data
	};
	public KochSessionTracking sessionTracking = KochSessionTracking.full;
	public static KochSessionTracking SessionTracking { get { return _S.sessionTracking; } set { _S.sessionTracking = value; } }
	
	#endregion
	
	#region Initilization
	
	// Global Constants
	
	public const string KOCHAVA_VERSION = "20131010";
	public const string KOCHAVA_PROTOCOL_VERSION = "2";
	private const int MAX_LOG_SIZE = 50;
	private const int MAX_QUEUE_SIZE = 75;
	private const int MAX_POST_TIME = 15;
	private const int POST_FAIL_RETRY_DELAY = 30;
	private const int QUEUE_KVINIT_WAIT_DELAY = 15;
	private const string API_URL = "https://control.kochava.com";
	private const string TRACKING_URL = API_URL + "/track/kvTracker";
	private const string INIT_URL = API_URL + "/track/kvinit";
	private const string AD_URL = "http://bidder.kochava.com/adserver/request/";
	private const string KOCHAVA_QUEUE_STORAGE_KEY = "kochava_queue_storage";
	private const string KOCHAVA_UPGRADE_CHECK_KEY = "kochava_upgrade_check";
	
	// Logging
	
	public enum KochLogLevel {
		error,
		warning,
		debug
	};
	
	public class LogEvent
	{
		public string text;
		public float time;
		public KochLogLevel level;
		
		public LogEvent (string text, KochLogLevel level)
		{
			this.text = text;
			this.time = Time.time;
			this.level = level;
		}
	}
	
	private List<LogEvent> _EventLog = new List<LogEvent> ();
	public static List<LogEvent> EventLog { get { return _S._EventLog; } }
			
	// Data Tracking
		
	private Dictionary<string, object> hardwareIntegrationData = new Dictionary<string, object> ();
	private Dictionary<string, object> appData;
	
	// Event Queue Handling
	
	public class QueuedEvent
	{
		public float eventTime;
		public Dictionary<string, object> eventData;
	}
	
	private Queue<QueuedEvent> eventQueue = new Queue<QueuedEvent> ();
	public static int eventQueueLength { get { return _S.eventQueue.Count; } }
	
	private float processQueueKickstartTime = 0;
	private bool queueIsProcessing = false;
	private float _eventPostingTime = 0.0f;
	public static float eventPostingTime { get { return _S._eventPostingTime; } }
	
	// Singleton instance reference
	private static Kochava _S;
	
	public void Awake ()
	{
		
		if(!Application.isPlaying)
			return;
		
		// /We are a duplicate, brought to life by the original constructor scene being reloaded.
		if (_S) {
			Log ("detected two concurrent integration objects - please place your integration object in a scene which will not be reloaded.");
			Destroy (this.gameObject);
			return;
		}
		DontDestroyOnLoad (this.gameObject);
		
		Log ("Kochava SDK Initialized.\nVersion: " + KOCHAVA_VERSION + "\nProtocol Version: " + KOCHAVA_PROTOCOL_VERSION + "", KochLogLevel.debug);

		loadQueue ();
		
	}
	
	public void Start ()
	{
		appIdentifier = PlatformUtilsManager.GetBundleID();
		appVersion = PlatformUtilsManager.GetBundleVersion();
	
		if(!Application.isPlaying)
			return;
		
		Init();
	}
	
	public void OnEnable ()
	{
		if(!Application.isPlaying)
			return;
		
		Kochava._S = this;
	}
	
	private void Init()
	{
		
		// Poll Hardware Integration libraries for device-specific data
		
		#if !UNITY_EDITOR && UNITY_IPHONE
		if(incognitoMode)
		{
			Log ("Incognito Mode enabled, using simplified handshake protocol");
		}
		
		appPlatform = "ios";
		if(kochavaAppIdIOS != "")
			kochavaAppId = kochavaAppIdIOS;
		string hardwareIntegrationString = GetExternalKochavaInfo(incognitoMode);
		hardwareIntegrationData = JsonReader.Deserialize<Dictionary<string, object>> (hardwareIntegrationString);
		Log ("Recieved (" + hardwareIntegrationData.Count + ") parameters from Hardware Integration Library: " + hardwareIntegrationString);
		
		#endif
		
		#if !UNITY_EDITOR && UNITY_ANDROID
		
		// Get Android context
		AndroidJNIHelper.debug = true;
        using(AndroidJavaClass androidUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
	        AndroidJavaObject androidActivity = androidUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	        AndroidJavaObject androidContext = androidActivity.Call<AndroidJavaObject>("getApplicationContext");
			
			AndroidJavaClass androidHelperClass = new AndroidJavaClass("com.kochava.android.tracker.lite.KochavaSDKLite");
			string hardwareIntegrationString = androidHelperClass.CallStatic<string>("getIndentifiers", androidContext);
			
			//string hardwareIntegrationLog = androidHelperClass.CallStatic<string>("getUserAgentLog");
			//Log ("Hardware Integration Diagnostics: " + hardwareIntegrationLog);
			
			hardwareIntegrationData = JsonReader.Deserialize<Dictionary<string, object>> (hardwareIntegrationString);
			Log ("Recieved (" + hardwareIntegrationData.Count + ") parameters from Hardware Integration Library: " + hardwareIntegrationString);
		}
		
		// Kindle
		bool forceKindle = false;
#if AMAZON_ANDROID 
		forceKindle = true;
#endif

		string userAgent = (hardwareIntegrationData.ContainsKey("user_agent") ? hardwareIntegrationData["user_agent"].ToString().ToLower() : "");
		if(userAgent.Contains("kindle") || userAgent.Contains("silk") || forceKindle)
		{
			appPlatform = "kindle";
			if(kochavaAppIdKindle != "")
				kochavaAppId = kochavaAppIdKindle;
		}
		
		// Other Android Device
		else
		{
			appPlatform = "android";
			if(kochavaAppIdAndroid != "")
				kochavaAppId = kochavaAppIdAndroid;
		}
		
		#endif
		
		#if !UNITY_EDITOR && UNITY_BLACKBERRY
		appPlatform = "blackberry";
		if(kochavaAppIdBlackberry != "")
			kochavaAppId = kochavaAppIdBlackberry;
		#endif
		
		#if !UNITY_EDITOR && UNITY_WP8
		appPlatform = "wp8";
		if(kochavaAppIdWindowsPhone != "")
			kochavaAppId = kochavaAppIdWindowsPhone;
		#endif
		
		// autoprovisioned kochava_app_ids
		
		if(PlayerPrefs.HasKey("kochava_app_id"))
		{
			kochavaAppId = PlayerPrefs.GetString("kochava_app_id");
			Log ("Loaded kochava_app_id from persistent storage: " + kochavaAppId, KochLogLevel.debug);
		}
		
		// kochava_device_id determination
		
		// have device id on file, good to go
		if(PlayerPrefs.HasKey("kochava_device_id"))							// most likely autoprovisioned during a previous app launch
		{
			kochavaDeviceId = PlayerPrefs.GetString("kochava_device_id");
			if(PlayerPrefs.HasKey("kochava_device_id_strategy"))
				hardwareIntegrationData.Add("dev_id_strategy", PlayerPrefs.GetString("kochava_device_id_strategy"));
			Log ("Loaded kochava_device_id from persistent storage: " + kochavaDeviceId, KochLogLevel.debug);
		}
		
		// no device id on file, scrape something together
		else
		{
			// incognito mode - use a (nondeterministic) guid
			if(incognitoMode)
			{
				kochavaDeviceId = "KA" + System.Guid.NewGuid().ToString().Replace ("-", "");
				if(!hardwareIntegrationData.ContainsKey("dev_id_strategy"))
					hardwareIntegrationData.Add("dev_id_strategy", "5");
				Log ("Using autogenerated \"incognito\" kochava_device_id: " + kochavaDeviceId, KochLogLevel.debug);
			}
			
			// standard mode - anything goes
			else
			{
				// check for original id
				string kochavaDeviceIdOrig = "";
				if(PlayerPrefs.HasKey("data_orig_kochava_device_id"))
					kochavaDeviceIdOrig = PlayerPrefs.GetString("data_orig_kochava_device_id");
				if(kochavaDeviceIdOrig != "")
				{
					kochavaDeviceId = kochavaDeviceIdOrig;
					Log ("Using \"orig\" kochava_device_id: " + kochavaDeviceId, KochLogLevel.debug);
				}
				
				// use id provided by hardware integration library
				else if(hardwareIntegrationData.ContainsKey("kochava_device_id") && hardwareIntegrationData["kochava_device_id"].ToString().Length > 3)
				{
					kochavaDeviceId = hardwareIntegrationData["kochava_device_id"].ToString();
					Log ("Using \"hardware integration\" kochava_device_id: " + kochavaDeviceId, KochLogLevel.debug);
				}
				
				// make something up on the spot
				else 															
				{
					kochavaDeviceId = "KU" + PulseEngine.Utils.GetDeviceGUID().Replace("-", "");
					
					if(!hardwareIntegrationData.ContainsKey("dev_id_strategy"))
						hardwareIntegrationData.Add("dev_id_strategy", "5");
					Log ("Using autogenerated kochava_device_id: " + kochavaDeviceId, KochLogLevel.debug);
				}
			}
		}
		
		// preserve original SDK values for posterity
		
		if(!PlayerPrefs.HasKey("data_orig_kochava_app_id") && kochavaAppId != "")
			PlayerPrefs.SetString("data_orig_kochava_app_id", kochavaAppId);
		
		if(!PlayerPrefs.HasKey("data_orig_kochava_device_id") && kochavaDeviceId != "")
			PlayerPrefs.SetString("data_orig_kochava_device_id", kochavaDeviceId);
		
		if(!PlayerPrefs.HasKey("data_orig_kochava_device_id_strategy") && hardwareIntegrationData.ContainsKey("dev_id_strategy"))
			PlayerPrefs.SetString("data_orig_kochava_device_id_strategy", hardwareIntegrationData["dev_id_strategy"].ToString());
		
		if(!PlayerPrefs.HasKey("data_orig_session_tracking"))
			PlayerPrefs.SetString("data_orig_session_tracking", sessionTracking.ToString());
		
		if(!PlayerPrefs.HasKey("data_orig_currency") && appCurrency != "")
			PlayerPrefs.SetString("data_orig_currency", appCurrency);
	
		// other prior kvinit response flag overrides
		
		if(PlayerPrefs.HasKey("currency"))
		{
			appCurrency = PlayerPrefs.GetString("currency");
			Log ("Loaded currency from persistent storage: " + appCurrency, KochLogLevel.debug);
		}
		if(PlayerPrefs.HasKey("blacklist"))
		{
			try
			{
				string devIdBlacklistStr = PlayerPrefs.GetString("blacklist");
				devIdBlacklist = new List<string> ();
				string[] devIdBlacklistArr = (string[]) JsonReader.Deserialize<string[]> (devIdBlacklistStr);
				for ( int i = devIdBlacklistArr.Length-1; i >= 0; i-- )
					devIdBlacklist.Add(devIdBlacklistArr[i]);
				Log ("Loaded device_id blacklist from persistent storage: " + devIdBlacklistStr, KochLogLevel.debug);
			}
			catch(Exception e)
			{
				Log ("Failed loading device_id blacklist from persistent storage: " + e, KochLogLevel.warning);
			}
		}
		
		if(PlayerPrefs.HasKey("attribution"))
		{
			try
			{
				attributionDataStr = PlayerPrefs.GetString("attribution");
				attributionData = JsonReader.Deserialize<Dictionary<string, object>> (attributionDataStr);
				Log ("Loaded attribution data from persistent storage: " + attributionDataStr, KochLogLevel.debug);
			}
			catch(Exception e)
			{
				Log ("Failed loading attribution data from persistent storage: " + e, KochLogLevel.warning);
			}
		}
		
		if(PlayerPrefs.HasKey("session_tracking"))
		{
			try
			{
				string sessionTrackingStr = PlayerPrefs.GetString("session_tracking");
				sessionTracking = (KochSessionTracking) System.Enum.Parse( typeof( KochSessionTracking ), sessionTrackingStr, true);
				Log ("Loaded session tracking mode from persistent storage: " + sessionTrackingStr, KochLogLevel.debug);
			}
			catch(Exception e)
			{
				Log ("Failed loading session tracking mode from persistent storage: " + e, KochLogLevel.warning);
			}
		}
		
		// initiate kvinit request
		
		Dictionary<string, object> initData_data = new Dictionary<string, object>() {
			{"partner_id", partnerId},
			{"partner_name", partnerName},
			{"package_name", appIdentifier},
			{"platform", appPlatform},
			{"session_tracking", sessionTracking.ToString()},
			{"currency", appCurrency},
			{"os_version", SystemInfo.operatingSystem}
		};
		
		if(requestAttribution && attributionData == null)	// @TODO - this is a suboptimal protocol for a number of reasons, we should rearchitect it at some point
			initData_data.Add("request_attribution", true);
		
		if(hardwareIntegrationData.ContainsKey("IDFA"))
			initData_data.Add("idfa", hardwareIntegrationData["IDFA"]);
		if(hardwareIntegrationData.ContainsKey("IDFV"))
			initData_data.Add("idfv", hardwareIntegrationData["IDFV"]);
		
		Dictionary<string, object> initData_orig = new Dictionary<string, object>() {
			{"kochava_app_id", PlayerPrefs.GetString("data_orig_kochava_app_id")},
			{"kochava_device_id", PlayerPrefs.GetString("data_orig_kochava_device_id")},
			{"session_tracking", PlayerPrefs.GetString("data_orig_session_tracking")},
			{"currency", PlayerPrefs.GetString("data_orig_currency")}
		};
		Dictionary<string, object> initData = new Dictionary<string, object>() {
			{"data", initData_data},
			{"data_orig", initData_orig},
			{"kochava_app_id", kochavaAppId },
			{"kochava_device_id", kochavaDeviceId },
			{"sdk_version", "u3d-" + KOCHAVA_VERSION},
			{"sdk_protocol", KOCHAVA_PROTOCOL_VERSION},
		};
		
		StartCoroutine(Init_KV (JsonWriter.Serialize (initData)));
		
	}
	
	private IEnumerator Init_KV(string postData)
	{
		
		Log ("Initiating kvinit handshake...", KochLogLevel.debug);
		
		float postTime = Time.time;
		WWW www = new WWW (INIT_URL, System.Text.Encoding.UTF8.GetBytes (postData), new Hashtable () {{ "Content-Type", "application/xml" }});
		Log(postData, KochLogLevel.debug);
		
		while (!www.isDone) {
			// Check for posting timeout
			if (Time.time - postTime > MAX_POST_TIME)
				break;
			
			// Wait for event to post
			yield return null;
		}
		
		Dictionary<string, object> serverResponse = null;
		if (www.error == null && www.isDone)
		{
			// Deserialize JSON response from server
			serverResponse = new Dictionary<string, object> ();
			if (www.isDone && www.text != "")
				serverResponse = JsonReader.Deserialize<Dictionary<string, object>> (www.text);

			Log(www.text, KochLogLevel.debug);
		}
		
		// kvinit failure
		if (www.error != null || !www.isDone || !serverResponse.ContainsKey ("success")) {
			Log("Kvinit handshake Failed: " + www.error, KochLogLevel.warning);
			yield return new WaitForSeconds(POST_FAIL_RETRY_DELAY);
			StartCoroutine(Init_KV (postData));	// retry
		}

		// kvinit success!
		else {
			
			Log ("...kvinit handshake complete, processing response flags...", KochLogLevel.debug);
			
			// response flags
			if(serverResponse.ContainsKey("flags"))
			{
				
				Dictionary<string, object> serverResponseFlags = (Dictionary<string, object>) serverResponse["flags"];
				
				if(serverResponseFlags.ContainsKey("kochava_app_id"))
				{
					kochavaAppId = serverResponseFlags["kochava_app_id"].ToString ();
					PlayerPrefs.SetString("kochava_app_id", kochavaDeviceId);
					Log ("Saved kochava_app_id to persistent storage: " + kochavaAppId, KochLogLevel.debug);
				}
				if(serverResponseFlags.ContainsKey("kochava_device_id"))
				{
					kochavaDeviceId = serverResponseFlags["kochava_device_id"].ToString ();
				}
				if(serverResponseFlags.ContainsKey("resend_initial") && (bool) serverResponseFlags["resend_initial"] == true)
				{
					PlayerPrefs.SetString (KOCHAVA_UPGRADE_CHECK_KEY, "resend_initial");
					Log ("Refiring initial event, as requested by kvinit response flag", KochLogLevel.debug);
				}
				if(serverResponseFlags.ContainsKey("session_tracking"))
				{
					sessionTracking = (KochSessionTracking) System.Enum.Parse( typeof( KochSessionTracking ), serverResponseFlags["session_tracking"].ToString () );
					PlayerPrefs.SetString("session_tracking", sessionTracking.ToString ());
					Log ("Saved session_tracking mode to persistent storage: " + sessionTracking.ToString (), KochLogLevel.debug);
				}
				if(serverResponseFlags.ContainsKey("currency"))
				{
					appCurrency = serverResponseFlags["currency"].ToString ();
					PlayerPrefs.SetString("currency", appCurrency);
					Log ("Saved currency to persistent storage: " + appCurrency, KochLogLevel.debug);
				}
			}
			
			// attribution data
			if(serverResponse.ContainsKey("attribution"))
			{
				attributionData = (Dictionary<string, object>) serverResponse["attribution"];
				try
				{
					attributionDataStr = JsonWriter.Serialize (serverResponse["attribution"]);
					PlayerPrefs.SetString("attribution", attributionDataStr);
					Log ("Saved attribution data to persistent storage: " + attributionDataStr, KochLogLevel.debug);
				}
				catch(Exception e)
				{
					Log ("Failed saving attribution data to persistent storage: " + e, KochLogLevel.warning);
				}
			}
			
			// blacklisting
			if(serverResponse.ContainsKey("blacklist"))
			{
				devIdBlacklist = new List<string> ();
				if(serverResponse["blacklist"].GetType().GetElementType() == typeof(string))
				{
					try
					{
						string[] devIdBlacklistArr = (string[]) serverResponse["blacklist"];
						for ( int i = devIdBlacklistArr.Length-1; i >= 0; i-- )
							devIdBlacklist.Add(devIdBlacklistArr[i]);
					}
					catch(Exception e)
					{
						Log ("Failed parsing device_identifier blacklist recieved from server: " + e, KochLogLevel.warning);
					}
				}
				try
				{
					string devIdBlacklistStr = JsonWriter.Serialize (devIdBlacklist);
					PlayerPrefs.SetString("blacklist", devIdBlacklistStr);
					Log ("Saved device_identifier blacklist (" + devIdBlacklist.Count + " elements) to persistent storage: " + devIdBlacklistStr, KochLogLevel.debug);
				}
				catch(Exception e)
				{
					Log ("Failed saving device_identifier blacklist to persistent storage: " + e, KochLogLevel.warning);
				}
			}
			
			// Populate initial app data
			appData = new Dictionary<string, object>() {
				{"kochava_app_id", kochavaAppId },
				{"kochava_device_id", kochavaDeviceId },
				{"dev_id_strategy", (hardwareIntegrationData.ContainsKey("dev_id_strategy") ? hardwareIntegrationData["dev_id_strategy"] : "") },
				{"sdk_version", "Unity3D-" + KOCHAVA_VERSION},
				{"sdk_protocol", KOCHAVA_PROTOCOL_VERSION},
			};
			
			// general data persistence
			PlayerPrefs.SetString("kochava_device_id", kochavaDeviceId);
			Log ("Saved kochava_device_id to persistent storage: " + kochavaDeviceId, KochLogLevel.debug);
			
			// Fire Initial event for App or OS upgrades
			// We have to wait for kvinit completeon as blacklisted device_identifiers, etc can be customized via kvinit response flags
			string upgradeCheckString = SystemInfo.operatingSystem + appVersion;
			if (!PlayerPrefs.HasKey (KOCHAVA_UPGRADE_CHECK_KEY) || PlayerPrefs.GetString (KOCHAVA_UPGRADE_CHECK_KEY) != upgradeCheckString)
			{
				// Set "Upgrade Check String"
				PlayerPrefs.SetString (KOCHAVA_UPGRADE_CHECK_KEY, upgradeCheckString);
				
				initInitial();
			}
			else
			{
				if(sessionTracking == KochSessionTracking.full || sessionTracking == KochSessionTracking.basic)
					_S._fireEvent ("session", new Dictionary<string, object> () {
						{ "state", "launch" }
					});
			}
		
		}
		
	}
	
	public static void InitInitial ()
	{
		_S.initInitial();
	}
	
	private void initInitial()
	{
		Dictionary<string, object> initialParams = new Dictionary<string, object>();
		
		try
		{
			initialParams.Add ("device", SystemInfo.deviceModel);
			initialParams.Add ("device_name", ( SystemInfo.deviceName.Contains ("unknown") ? "" : SystemInfo.deviceName ) );
			initialParams.Add ("app_version", appVersion);
			initialParams.Add ("package_name", appIdentifier);
			
			initialParams.Add ("currency", appCurrency);
			initialParams.Add ("disp_h", Screen.height);
			initialParams.Add ("disp_w", Screen.width);
			initialParams.Add ("os_version", SystemInfo.operatingSystem);
			if(!devIdBlacklist.Contains ("hardware")) {
				initialParams.Add ("device_processor", SystemInfo.processorType);
				initialParams.Add ("device_cores", SystemInfo.processorCount);
				initialParams.Add ("device_memory", SystemInfo.systemMemorySize);
				initialParams.Add ("graphics_memory_size", SystemInfo.graphicsMemorySize);
				initialParams.Add ("graphics_device_name", SystemInfo.graphicsDeviceName);
				initialParams.Add ("graphics_device_vendor", SystemInfo.graphicsDeviceVendor);
				initialParams.Add ("graphics_device_id", SystemInfo.graphicsDeviceID);
				initialParams.Add ("graphics_device_vendor_id", SystemInfo.graphicsDeviceVendorID);
				initialParams.Add ("graphics_device_version", SystemInfo.graphicsDeviceVersion);
				initialParams.Add ("graphics_shader_level", SystemInfo.graphicsShaderLevel);
				initialParams.Add ("graphics_pixel_fillrate", SystemInfo.graphicsPixelFillrate);
			}
			
			// Unity UDID
			if(!devIdBlacklist.Contains ("unity_udid"))
				initialParams.Add ("unity_udid", PulseEngine.Utils.GetDeviceGUID());
			
			// Piracy Check
			if(!devIdBlacklist.Contains ("is_genuine") && Application.genuineCheckAvailable)
				initialParams.Add("is_genuine", (Application.genuine ? "1" : "0"));
			
			// IDFA
			if(!devIdBlacklist.Contains ("idfa") && hardwareIntegrationData.ContainsKey("IDFA"))
				initialParams.Add("idfa", hardwareIntegrationData["IDFA"]);
			
			// IDFV
			if(!devIdBlacklist.Contains ("idfv") && hardwareIntegrationData.ContainsKey("IDFV"))
				initialParams.Add("idfv", hardwareIntegrationData["IDFV"]);
			
			// UDID
			if(!devIdBlacklist.Contains ("udid") && hardwareIntegrationData.ContainsKey("UDID"))
				initialParams.Add("udid", hardwareIntegrationData["UDID"]);
			
			// GUID
			if(!devIdBlacklist.Contains ("guid")) {
				string myGUID = System.Guid.NewGuid().ToString();
				if(myGUID != "")
					initialParams.Add("guid", myGUID);
			}
			
			// Android ID
			if(!devIdBlacklist.Contains ("android_id") && hardwareIntegrationData.ContainsKey("android_id"))
				initialParams.Add("android_id", hardwareIntegrationData["android_id"]);
			
			// UA (for click > install fingerprinting)
			if(!devIdBlacklist.Contains ("user_agent") && hardwareIntegrationData.ContainsKey("user_agent"))
				initialParams.Add("user_agent", hardwareIntegrationData["user_agent"]);
			
			// MAC Address
			if(!devIdBlacklist.Contains ("mac")) {
		    	string macAddress = "";
				if(hardwareIntegrationData.ContainsKey("mac"))
					macAddress = hardwareIntegrationData["mac"].ToString ();
				if(macAddress == "") {
					#if !UNITY_WEBPLAYER
					System.Net.NetworkInformation.NetworkInterface[] nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
					foreach (System.Net.NetworkInformation.NetworkInterface adapter in nics) {
						macAddress = adapter.GetPhysicalAddress ().ToString ();
				        if(macAddress != "")
							break;
				    }
					#endif
				}
				if(macAddress != "")
					initialParams.Add("mac", macAddress);
			}
			
			// ODIN
			if(!devIdBlacklist.Contains ("odin")) {
				string myODIN = "";
				if(hardwareIntegrationData.ContainsKey("odin"))
					myODIN = hardwareIntegrationData["odin"].ToString ();
				//if(myODIN == "" && macAddress != "")
				//	myODIN = CalculateSHA1Hash(macAddress);		//TODO: this is wrong, see Koby's "issues seen in device_db / devices" email for right way to calculate odin
				// if(myODIN == "")	// TODO: Probably better to have no odin than to have a nonstandard odin
				// 	myODIN = CalculateSHA1Hash(SystemInfo.deviceUniqueIdentifier);
				if(myODIN != "")
					initialParams.Add("odin", myODIN);		// f68836ca7aa6b595bb6d83c054dd57e3990a4652
			}
			
			// OpenUDID
			if(!devIdBlacklist.Contains ("open_udid") && hardwareIntegrationData.ContainsKey("openUDID") && !hardwareIntegrationData["openUDID"].ToString().Contains("WIFIMAC") && !hardwareIntegrationData["openUDID"].ToString().Contains("ANDROID"))
				initialParams.Add("open_udid", hardwareIntegrationData["openUDID"]);	// A8F4247EAE1617D9EB07061A9A099EDF
			
			// imei
			// Dragonhere: Implementation pending
			
			// Facebook Attribution ID
			if(!devIdBlacklist.Contains ("fb_attribution_id") && hardwareIntegrationData.ContainsKey("fb_attribution_id"))
				initialParams.Add("fb_attribution_id", hardwareIntegrationData["fb_attribution_id"]);
			
			// Limit Ad Tracking
			if(!devIdBlacklist.Contains ("limit_ad_tracking") && hardwareIntegrationData.ContainsKey("limitAdTracking"))
				initialParams.Add("limitAdTracking", hardwareIntegrationData["limitAdTracking"]);
		}
		catch(Exception e)
		{
			Log ("Error preparing initial event: " + e, KochLogLevel.error);
		}
		finally
		{
			_fireEvent ("initial", initialParams);
		}
	}
	
	#endregion
	
	#region Continous Monitoring
	
	public void Update ()
	{
	
		if(!Application.isPlaying)
		{
			
			// automatic app-meta update hack
			/*#if UNITY_EDITOR
			if(PlayerSettings.bundleVersion != "") //appVersion == "")
				appVersion = PlayerSettings.bundleVersion;
			if(appVersion == "")
				appVersion = "1.0";
			if(PlayerSettings.bundleIdentifier != "") //appIdentifier == "")
				appIdentifier = PlayerSettings.bundleIdentifier;
			#endif*/
	
		} else {
			
			// @todo - automatically watched game state variables such as level, score, etc
			
			// Processing queue is waiting to be kickstarted at some point in the future
			if (processQueueKickstartTime != 0 && Time.time > processQueueKickstartTime) {
				processQueueKickstartTime = 0;
				StartCoroutine ("processQueue");
			}
			
		}
		
	}
	
	#endregion
	
	#region Event Tracking, Identity Linking
	
	static public void FireEvent (Dictionary<string, object> properties)
	{
		_S._fireEvent ("event", properties);
	}
		
	static public void FireEvent (Hashtable propHash)
	{
		Dictionary<string, object> properties = new Dictionary<string, object> ();
		foreach (DictionaryEntry row in propHash)
			properties.Add((string)row.Key, (object)row.Value);
		_S._fireEvent ("event", properties);
	}
	
	static public void FireEvent (string eventName, string eventData)
	{
		_S._fireEvent ("event", new Dictionary<string, object> () {
			{ "event_name", eventName },
			{ "event_data", eventData }
		});
	}
	
	static public void FireSpatialEvent (string eventName, float x, float y)
	{
		FireSpatialEvent (eventName, x, y, 0, "");
	}
	
	static public void FireSpatialEvent (string eventName, float x, float y, string eventData)
	{
		FireSpatialEvent (eventName, x, y, 0, eventData);
	}
	
	static public void FireSpatialEvent (string eventName, float x, float y, float z)
	{
		FireSpatialEvent (eventName, x, y, z, "");
	}
	
	static public void FireSpatialEvent (string eventName, float x, float y, float z, string eventData)
	{
		_S._fireEvent ("spatial", new Dictionary<string, object> () {
			{ "event_name", eventName },
			{ "event_data", eventData },
			{"x", x},
			{"y", y},
			{"z", z}
		});
	}
	
	static public void IdentityLink (string key, string val)
	{
		_S._fireEvent ("identityLink", new Dictionary<string, object> () {
			{ key, val }
		});
	}
	
	static public void IdentityLink (Dictionary<string, object> identities)
	{
		_S._fireEvent ("identityLink", identities);
	}
	
	private void _fireEvent (string eventAction, Dictionary<string, object> eventData)
	{
		
		Dictionary<string, object> postData = new Dictionary<string, object> ();
		
		if(!eventData.ContainsKey("usertime"))
			eventData.Add("usertime", (UInt32)CurrentTime());
	
       	if(!eventData.ContainsKey("uptime") && (UInt32)Time.time > 0)
			eventData.Add("uptime", (UInt32)Time.time);	// Dont' use Time.realtimeSinceStartup, as it would continue incrementing when app is in background
       	
		float upDelta = UptimeDelta();
		if(!eventData.ContainsKey("updelta") && upDelta >= 1)
			eventData.Add("updelta", (UInt32)upDelta);
		
		/*	// @TODO - This is where we add geotracking
		if(!eventData.ContainsKey("geo_lat"))
			eventData.Add("geo_lat", "");
		if(!eventData.ContainsKey("geo_lon"))
			eventData.Add("geo_lon", "");
		*/
		
		postData.Add ("action", eventAction);
		postData.Add ("data", eventData);
		
		if(eventPostingTime != 0.0f)
			postData.Add ("last_post_time", (float)eventPostingTime);
		
		if(debugMode)
			postData.Add ("debug", (bool)true);
		if(debugServer)
			postData.Add ("debugServer", (bool)true);
		
		postEvent (postData);
	}
	
	private void postEvent (Dictionary<string, object> data)
	{
		if (eventQueue.Count >= MAX_QUEUE_SIZE) {
			Log ("MAX_QUEUE_SIZE (" + MAX_QUEUE_SIZE + ") reached, dequeuing first event in queue", KochLogLevel.error);
			eventQueue.Dequeue ();
		}
		
		QueuedEvent queuedEvent = new QueuedEvent ();
		queuedEvent.eventTime = Time.time;
		queuedEvent.eventData = data;
		
		eventQueue.Enqueue (queuedEvent);
		StartCoroutine ("processQueue");
	}
	
	#endregion
	
	#region Queue Management
	
	private IEnumerator processQueue ()
	{
		// Queue should only be processed by one processQueue coroutine at a time
		if (queueIsProcessing)
			yield break;
		
		// We now have an exclusive lock on queue processing
		queueIsProcessing = true;
		
		// wait for Kvinit to provide us with AppData
		while(appData == null)
		{
			yield return new WaitForSeconds(QUEUE_KVINIT_WAIT_DELAY);
			if(appData == null)
				Log ("Event posting delayed (AppData null, kvinit handshake incomplete or Unity reloaded assemblies)", KochLogLevel.debug);
		}
		
		while (eventQueue.Count > 0) {
			
			// copy active event out of the queue
			QueuedEvent queuedEvent = eventQueue.Peek ();
			float postTime = Time.time;
			string postData = "";
			
			try
			{
				// create a copy of eventData so we can augment it without modifying data in queue (which will be reposted later if this posting fails)
				Dictionary<string, object> eventData = queuedEvent.eventData;
				
				// augment event with standardized AppData
				foreach(KeyValuePair<string, object> row in appData)
				{
					if(!eventData.ContainsKey(row.Key))
						eventData.Add(row.Key, row.Value);
				}
				
				// post event!
				postData = JsonWriter.Serialize (eventData);
			}
			catch(Exception e)
			{
				Log ("Event posting failure: " + e, KochLogLevel.error);
				eventQueue.Dequeue();
			}
			
			if(postData != "")
			{
				Log ("Posting event: " + postData.Replace("{", "{\n").Replace(",", ",\n"), KochLogLevel.debug);
				WWW www = new WWW (TRACKING_URL, System.Text.Encoding.UTF8.GetBytes (postData), new Hashtable () {{ "Content-Type", "application/xml" }});
				
				while (!www.isDone)
				{
					// Check for posting timeout
					if (Time.time - postTime > MAX_POST_TIME)
						break;
					
					// Wait for event to post
					yield return null;
				}
			
				try
				{
					// Deserialize JSON response from server
					Dictionary<string, object> serverResponse = new Dictionary<string, object> ();
					if (www.error == null && www.isDone && www.text != "") {
						Log ("Server Response Recieved: " + WWW.UnEscapeURL(www.text), KochLogLevel.debug);
						serverResponse = JsonReader.Deserialize<Dictionary<string, object>> (www.text);
					}
					
					// Event posting failure - timeout or otherwise
					bool retry = true;
					bool success = (serverResponse.ContainsKey ("success") /*&& serverResponse ["success"].ToString () == "1"*/);
					if (www.error != null || !www.isDone || !success)
					{
						_eventPostingTime = -1;
						
						if (www.error != null) {
							Log ("Event Posting Failed: " + www.error, KochLogLevel.error);
						} else if (www.isDone) {
							Log ("Event Posting Did Not Succeed: " + (www.text == "" ? "(Blank response from server)" : www.text), KochLogLevel.error);
							if (serverResponse.ContainsKey ("error") || www.text == "")
								retry = false;
						} else {
							Log ("MAX_POST_TIME exceeded during event posting", KochLogLevel.error);
						}
						
						// Transport success, posting failure
						if (retry == false) {
							eventQueue.Dequeue ();
							Log ("Event posting failure, event dequeued: " + serverResponse ["error"], KochLogLevel.warning);
						}
						
						// Transport failure, wait then resend
						else
						{
							// Configure queue posting kickstarter, then kill queue posting
							processQueueKickstartTime = Time.time + POST_FAIL_RETRY_DELAY;			// Set a time in the future for processQueue to be automatically relaunched by the update loop 
							queueIsProcessing = false;
							yield break;				// Terminate queue processing for now, update loop will relaunch queue processing in POST_FAIL_RETRY_DELAY seconds
						}
					
						// Event posting success!
					} else {
						
						eventQueue.Dequeue ();		// Event was posted successfully, and can now be safely removed from the queue
						_eventPostingTime = (Time.time - postTime);
						
						Log ("Event Posted (" + _eventPostingTime + " seconds to upload)");
						
						// Browser Flashing
						if (serverResponse.ContainsKey("cta") && serverResponse ["CTA"].ToString () == "1") {
							Application.OpenURL (serverResponse ["URL"].ToString ());
						}
					}
				}
				catch(Exception e)
				{
					Log ("Event posting response processing failure: " + e, KochLogLevel.error);
				}
			}
		}
		
		queueIsProcessing = false;
	}
	
	public void OnApplicationPause (bool didPause)
	{
		// Register state changes (appData != null ensures we don't register a resume state during app launch)
		if(sessionTracking == KochSessionTracking.full && appData != null)
		{
			_S._fireEvent ("session", new Dictionary<string, object> () {
				{ "state", (didPause ? "pause" : "resume") }
			});
		}
		
		// Queue loading on resume is unnecessary, but it is important to save the queue on pause in case the application is exited before being resumed
		if (didPause)
			saveQueue ();
	}
	
	public void OnApplicationQuit ()
	{
		
		if(sessionTracking == KochSessionTracking.full || sessionTracking == KochSessionTracking.basic || sessionTracking == KochSessionTracking.minimal)
		{
			_S._fireEvent ("session", new Dictionary<string, object> () {
				{ "state", "quit" }
			});
		}
		
		saveQueue ();
	}
	
	private void saveQueue ()
	{
		if (eventQueue.Count > 0) {
			try
			{
				string jsonStr = JsonWriter.Serialize (eventQueue);
				PlayerPrefs.SetString (KOCHAVA_QUEUE_STORAGE_KEY, jsonStr);
				Log ("Event Queue saved: " + jsonStr, KochLogLevel.debug);
			}
			catch(Exception e)
			{
				Log ("Failure saving event queue: " + e, KochLogLevel.error);
			}
		}
	}
	
	private void loadQueue ()
	{
		try
		{
			if (PlayerPrefs.HasKey (KOCHAVA_QUEUE_STORAGE_KEY))
			{
				string jsonStr = PlayerPrefs.GetString (KOCHAVA_QUEUE_STORAGE_KEY);
				int eventsLoaded = 0;
				QueuedEvent[] jsonQueue = JsonReader.Deserialize<QueuedEvent[]> (jsonStr); 
				foreach (QueuedEvent jsonEvent in jsonQueue) {
					if (!eventQueue.Contains (jsonEvent))
					{
						eventQueue.Enqueue (jsonEvent);
						eventsLoaded++;
					}
				}
				
				Log ("Loaded (" + eventsLoaded + ") events from persistent storage", KochLogLevel.debug);
				
				PlayerPrefs.DeleteKey (KOCHAVA_QUEUE_STORAGE_KEY);
				StartCoroutine ("processQueue");
			}
		}
		catch(Exception e)
		{
			Log ("Failure loading event queue: " + e, KochLogLevel.debug);
		}
	}
	
	public static void ClearQueue ()
	{
		_S.StartCoroutine ("clearQueue");
	}
	
	private IEnumerator clearQueue ()
	{
		try
		{
			Log ("Clearing (" + eventQueueLength + ") events from upload queue...");
			_S.StopCoroutine ("processQueue");
		}
		catch(Exception e)
		{
			Log ("Failure clearing event queue: " + e, KochLogLevel.error);
		}
		
		yield return null;
		
		try
		{
			_S.queueIsProcessing = false;
			_S.eventQueue = new Queue<QueuedEvent> ();
		}
		catch(Exception e)
		{
			Log ("Failure clearing event queue: " + e, KochLogLevel.error);
		}
	}
	
	#endregion
	
	#region Ad Serving
	
	// Ad Serving
	public void GetAd (int webView, int height, int width)
	{
		Log ("Adserver Implementation Pending"); // @TODO - implement ad server
	}
	
	#endregion
	
	#region Helper Utilities
	
	private void Log (string msg)
	{
		Log (msg, KochLogLevel.warning);
	}
	
	private void Log (string msg, KochLogLevel level)
	{
		if (level == KochLogLevel.error)
			Debug.Log ("*** Kochava Error: " + msg + " ***");
		else if (debugMode) 
			Debug.Log ("Kochava: " + msg);
		
		if(debugMode || level == KochLogLevel.error || level == KochLogLevel.warning)
			_EventLog.Add(new LogEvent(msg, level));
		
		if(_EventLog.Count > MAX_LOG_SIZE)
			_EventLog.RemoveAt (0);
	}
	
	public static void ClearLog ()
	{
		_S._EventLog.Clear ();
	}
	
	private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
    protected internal static double CurrentTime()
    {
		System.TimeSpan unix_time = (System.DateTime.UtcNow - Jan1st1970);
    	return unix_time.TotalSeconds;
    }
	
	private static float uptimeDelta; 
	private static float uptimeDeltaUpdate; 
    protected internal static float UptimeDelta()
    {
		uptimeDelta = Time.time - uptimeDeltaUpdate;
		// if(uptimeDelta < .1) uptimeDelta = 1;	// Launching the app probably took more than a second...
		uptimeDeltaUpdate = Time.time;
    	return uptimeDelta;
    }
	
	private string CalculateMD5Hash(string input)
	{
		try
		{
		    // step 1, calculate MD5 hash from input
		    System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
		    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
		    byte[] hash = md5.ComputeHash(inputBytes);
		 
		    // step 2, convert byte array to hex string
		    System.Text.StringBuilder sb = new System.Text.StringBuilder();
		    for (int i = 0; i < hash.Length; i++)
		    {
		        // sb.Append(hash[i].ToString("X2"));	// Uppercase
				sb.Append(hash[i].ToString("x2"));		// Lowercase
		    }
		    return sb.ToString();
		}
		catch(Exception e)
		{
			Log ("Failure calculating MD5 hash: " + e, KochLogLevel.error);
			return "";
		}
	}
	
	private string CalculateSHA1Hash(string input)
	{
		try
		{
		    byte[] hashData = new System.Security.Cryptography.SHA1Managed().ComputeHash (System.Text.Encoding.ASCII.GetBytes (input));
		    string hash = string.Empty;
	        foreach (var b in hashData)
	            hash += b.ToString("x2");
			return hash;
		}
		catch(Exception e)
		{
			Log ("Failure calculating SHA1 hash: " + e, KochLogLevel.error);
			return "";
		}
	}
	
	#endregion
	
}