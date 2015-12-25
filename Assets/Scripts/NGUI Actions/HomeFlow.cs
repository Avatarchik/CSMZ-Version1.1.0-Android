//#define AGE_GATE

using System;
using UnityEngine;
using System.Collections;

public class HomeFlow : MonoBehaviour
{
	public const string ageGateShownKey = "AgeGateShown";
	public const string ageGateValueKey = "AgeGateValue";
	
	public GameObject burstlyAdsContainer;
	public GameObject[] androidBanners;
	public GameObject[] iOSBanners;
	private GameObject[] burstlyBanners;
	
	public NumberPickerController agePicker;
	
	public PlayMakerFSM flowFSM;
	public string okEvent = "MessageOk";
	public string showEvent = "MessageOk";
	
	public UILabel IAPWarningLandscape;
	public UILabel IAPWarningPortrait;
	
	
	void Awake()
	{
		//InitBurstlyBanners();
		
		#if UNITY_ANDROID
		
		//AnalyticsBinding.LogEventUserInfo();
		//AnalyticsBinding.LogEventPlayerInfo();
		
		#endif
	}
	
	// Use this for initialization
	IEnumerator Start ()
	{
		PlayerPrefs.SetInt("PURCHASES_WARNING", 1);
		SetNumbersOfDaysPlayed();

#if AGE_GATE
		//TODO:Florin Uncomment below line only for testing purposes. (comment below line when finished testing).
//		PlayerPrefs.SetInt(ageGateShownKey, 0);
		
		if (PlayerPrefs.GetInt(ageGateShownKey, 0) == 0) 
		{
			AnalyticsBinding.LogEventPageView("age_gate", "", "");
#else
		if (PlayerPrefs.GetInt("PURCHASES_WARNING", 0) == 0) 
		{
			
#endif
			yield return null;
			
			string message = Language.Get("PURCHASES_WARNING");

#if UNITY_ANDROID
#if AMAZON_ANDROID
				message = message.Replace("iTunes", "Amazon");
#else
				message = message.Replace("iTunes", "Google");
#endif
#endif
			
#if AGE_GATE
			IAPWarningLandscape.text = message;
			IAPWarningPortrait.text = message;
			flowFSM.SendEvent(showEvent);
#else
			NativeMessagesSystem.OnButtonPressed += OnButtonPressed;
			NativeMessagesSystem.Instance.ShowMessage(Language.Get("PURCHASES_WARNING_TITLE"), 
				message, Language.Get("BUTTON_OK"));

#endif		
		}
		else 
		{
#if AGE_GATE
			if (IsAgeGateConfirmed()) {
				UrbanAirshipBinding.Init();
			}
#endif
			
			flowFSM.SendEvent(okEvent);
				
			ActivateBurstlyBanners();
		}
	}

	public static string GetBurstlyAdParams()
	{
		return string.Format("gate={0}," + 
							 "days='{1}'," +
							 "language='{2}'," +
							 "payer={3}",
							 (IsAgeGateConfirmed() ? "1" : "0"),
				             GetNumbersOfDaysPlayed().ToString(),
							 System.Enum.GetName(typeof(LanguageCode), Language.CurrentLanguage()).ToLower(),
							 (UserManagerCloud.Instance.CurrentUser.IsPayingUser ? "1" : "0"));
	}
	
	public void InitBurstlyBanners()
	{
		GameObject[] burstlyBannersToRemove = null;
			
#if disableBurstly
		// Destroy the Burstly Ads parent container.
		GameObject.Destroy(burstlyAdsContainer);
		burstlyAdsContainer = null;
#endif

		if (TweaksSystem.Instance.intValues["BurstlyAdsEnabled"] <= 0 && burstlyAdsContainer != null)
		{
			// Destroy the burstly ads container if the ads were disabled through the tweaks system.
			GameObject.Destroy(burstlyAdsContainer);
			burstlyAdsContainer = null;
		}

#if UNITY_IPHONE
		if (burstlyAdsContainer != null && iOSBanners != null) 
		{
			burstlyBanners = iOSBanners;
			burstlyBannersToRemove = androidBanners;
		}
#else
		if (burstlyAdsContainer != null && androidBanners != null) 
		{
			burstlyBanners = androidBanners;
			burstlyBannersToRemove = iOSBanners;
		}
#endif
		RemoveUncompatibleBanners(burstlyBannersToRemove);		
	}
		
	public void RemoveUncompatibleBanners(GameObject[] burstlyBannersToRemove)
	{	
		if (burstlyAdsContainer == null || burstlyBanners == null) {
			return;
		}

		// Destroy these game objects immediate because these mustn't be available on this platform.
		// Not destroying them with immediate can allow them to execute code when activating the burstly 
		// banners container in the same frame in Start which causes inconsistencies.
		for (int i = 0; i < burstlyBannersToRemove.Length; i++) {
			GameObject.DestroyImmediate(burstlyBannersToRemove[i]);
		}
	}
		
	public void OnAgeGateContinue()
	{
#if AGE_GATE
		Debug.Log("Selected age: " + agePicker.SelectedNumber);
		
		AnalyticsBinding.LogEventGameAction("gate", "submit_age", (agePicker.SelectedNumber < 13) ? "child" : "adult", "", 
			/*(agePicker.SelectedNumber < 13) ? -1 :*/ agePicker.SelectedNumber);
		
		PlayerPrefs.SetInt(ageGateShownKey, 1);
		PlayerPrefs.SetInt(ageGateValueKey, agePicker.SelectedNumber);
		
		if (agePicker.SelectedNumber >= 13)
		{
			UrbanAirshipBinding.Init();
		}
#endif
		ActivateBurstlyBanners();
	}

	public void ActivateBurstlyBanners()
	{
		if (burstlyAdsContainer != null) 
		{
			AdBannerController[] adBanners = burstlyAdsContainer.GetComponentsInChildren<AdBannerController>(true);
			string strAdParams = GetBurstlyAdParams();
			for (int i = 0; i < adBanners.Length; i++)
			{
				adBanners[i].ApplyAdParameters(strAdParams);
			}
			burstlyAdsContainer.SetActive(true);
		}
	}
		
	void OnButtonPressed(int index)
	{
		NativeMessagesSystem.OnButtonPressed -= OnButtonPressed;
		PlayerPrefs.SetInt("PURCHASES_WARNING", 1);
		flowFSM.SendEvent(okEvent);
			
		ActivateBurstlyBanners();	
	}
	
	void OnDestroy()
	{
		NativeMessagesSystem.OnButtonPressed -= OnButtonPressed;
	}
		
	public static bool IsAgeGateConfirmed()
	{
		if(PlayerPrefs.GetInt(ageGateValueKey, 0) >= 13)
		{
			return true;
		}
			
		return false;
	}

		private static int GetNumbersOfDaysPlayed()
	{
		return PlayerPrefs.GetInt("NUMBERS_OF_DAYS_PLAYED", 0);
	}
		
	private void SetNumbersOfDaysPlayed()
	{
		int noOfDaysPlayed = PlayerPrefs.GetInt("NUMBERS_OF_DAYS_PLAYED", 0);
		
		//The current time to compare with the last time played
        DateTime currentDateTime = System.DateTime.Now;
			
		long lastDateTimePlayed = 0;
		//Grab the last time played from the player prefs as a long
		if (PlayerPrefs.HasKey("LAST_DATE_TIME_PLAYED") == true){
	        lastDateTimePlayed = Convert.ToInt64(PlayerPrefs.GetString("LAST_DATE_TIME_PLAYED") );
		}

		//Convert the last time played from binary to a DataTime variable
        DateTime lastDateTime = DateTime.FromBinary(lastDateTimePlayed);
		
		//Use the Subtract method and store the result as a timespan variable
        TimeSpan difference = currentDateTime.Subtract(lastDateTime);
				
		if( difference.Seconds > 60 * 60 *24 )	
		{
			noOfDaysPlayed++;	
			PlayerPrefs.SetInt("NUMBERS_OF_DAYS_PLAYED", noOfDaysPlayed);

			//Save the current system time as a string in the player prefs class
            PlayerPrefs.SetString("LAST_DATE_TIME_PLAYED", currentDateTime.ToBinary().ToString());
		}

	}

}

