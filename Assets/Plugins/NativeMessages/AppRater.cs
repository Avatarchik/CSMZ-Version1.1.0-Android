using UnityEngine;
using System.Collections;

public class AppRater : MonoBehaviour
{
	public static bool distributionBuild = true; //TODO TALIN - SET THIS TO TRUE FOR DISTRIBUTION!!!!!!!!!!!
		
	public static AppRater instance;
	
	public static System.DateTime baseDate = new System.DateTime(2013, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
	
	protected static string lastRateKey = "LastAppRate";
	protected static string neverRateKey = "AppRateNever";
	protected static string ratedKey = "AppRateDone";
	
	public static int minDaysShow = 5;
	
	// Time when the app rater was last shown, in days since base date
	public int days;
	
	public bool neverShow = false;
	public bool alreadyRated = false;
	
	protected int rateIdx = 1;
	protected int neverIdx = 2;
	
#if UNITY_IPHONE
	protected static string clientId = "839618264";
	protected static string devId = "839618264";
	protected string appId = clientId;
#elif UNITY_ANDROID
	protected static string clientId = "com.disney.maleficent_goo";
	protected static string devId = "com.pastagames.ro1mobile";
	protected string appId = clientId;
#endif
	
	void Awake ()
	{
		instance = this;
		
		neverShow = PlayerPrefs.GetInt(neverRateKey, 0) == 1;
		alreadyRated = PlayerPrefs.GetInt(ratedKey, 0) == 1;
		days = PlayerPrefs.GetInt(lastRateKey, -1);
		
		if (days <= -1) 
		{
			days = 0;//Mathf.FloorToInt((float)System.DateTime.Now.Subtract(baseDate).TotalDays);
			//PlayerPrefs.SetInt(lastRateKey, days);
		}
#if UNITY_ANDROID || UNITY_IPHONE
		if (distributionBuild) {
			appId = clientId;
		}
		else {
			appId = devId;
		}
#endif
	}
	
	/// <summary>
	/// Tries to show the app rater.
	/// </summary>
	/// <param name='force'>
	/// Force show.
	/// </param>
	/// <param name='minDays'>
	/// Minimum days that need to have passed to be able to force show. Defaults to 0 so if 'force' is true, the app rater is displayed.
	/// </param>
	public void TryShowAppRater(string title, string message, string buttonRate, string buttonNever, string buttonCancel, bool force = false, int minDays = 0)
	{
		Debug.Log("Try show app rater");
		
		if (neverShow || alreadyRated) {
			return;
		}

		int currentDays = Mathf.FloorToInt((float)System.DateTime.Now.Subtract(baseDate).TotalDays);
		int daysPassed = currentDays - days;
		
		if ((force && minDays <= daysPassed) || daysPassed >= minDaysShow) 
		{
			days = currentDays;
			PlayerPrefs.SetInt(lastRateKey, currentDays);
			
			NativeMessagesSystem.OnButtonPressed += OnRateButtonPressed;
			NativeMessagesSystem.Instance.ShowMessage(title, message, buttonCancel, buttonRate, buttonNever);
		}
	}

	void OnRateButtonPressed(int index)
	{
		NativeMessagesSystem.OnButtonPressed -= OnRateButtonPressed;
		
		Debug.Log("RATE APP: " + index);
		
		//TODO - take action based on button index
		if (index == neverIdx) 
		{
			PlayerPrefs.SetInt(neverRateKey, 1);
		}
		else if (index == rateIdx) 
		{
			PlayerPrefs.SetInt(ratedKey, 1);
#if !UNITY_EDITOR && UNITY_IPHONE
			Application.OpenURL(NativeMessageBinding.Native_GetRateLink(appId));
#elif UNITY_ANDROID
#if AMAZON_ANDROID
			Application.OpenURL("amzn://apps/android?asin=B00KOEHQCW");
#else
			Application.OpenURL("market://details?id=" + appId);
#endif
#endif
		}
	}
	
	void OnDestroy()
	{
		NativeMessagesSystem.OnButtonPressed -= OnRateButtonPressed;
	}
}

