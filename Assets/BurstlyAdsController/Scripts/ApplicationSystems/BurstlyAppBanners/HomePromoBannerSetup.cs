using UnityEngine;
using System.Collections;

public class HomePromoBannerSetup : MonoBehaviour 
{
	public InterstitialController targetInterstitial;
	
	protected bool isRetinaDevice;
	private AdBannerController banner;
	
	
	void Awake()
	{
		banner = GetComponent<AdBannerController>();
		//TODO: this hackish approach should be fixed by using a different type of base class (a UnityPureSingleton)
		BurstlyEventsListener.canCreate = true;
		BurstlyEventsListener.Instance.RegisterEventsListener(banner.adId, gameObject);
			
		// Detect if this is a retina device
		isRetinaDevice = Screen.dpi >= 250f;
				
#if UNITY_IPHONE
		string deviceModel = SystemInfo.deviceModel.ToLower();
		bool isiPhoneOriPod = deviceModel.Contains("iphone") || deviceModel.Contains("ipod");
		
		if (isiPhoneOriPod)
		{
			// This is an iPhone or iPod device, which must have the 95x95 screen size (in iOS points, meaning it will be 190x190 pixels).
			// On retina devices the banner size must be 190x190 screen size (in iOS points, meaning it will be 190x190 pixels).
			if (isRetinaDevice) 
			{
				banner.positionParams[0].screenPosArea.width = 95f;
				banner.positionParams[0].screenPosArea.height = 95f;
			}
			else
			{
				banner.positionParams[0].screenPosArea.width = 190f;
				banner.positionParams[0].screenPosArea.height = 190f;
			}
		}
		else
		{
			// This is an iPad. On iPad retina the banner must have the screen size 190x190 (in iOS points, meaning it will be 380x380 pixels).
			// On iPad non-retina the banner must also have 190x190 (in iOS points meaning it will stay in 190x190 pixels).
			banner.positionParams[0].screenPosArea.width = 190f;
			banner.positionParams[0].screenPosArea.height = 190f;
		}
#else
		float scaleRatio = 1f;
		if (Screen.dpi > 0)
		{
			scaleRatio = Screen.dpi / 250f;
			if (scaleRatio < 1f)
			{
				scaleRatio = 1f;
			}
		}
		
		banner.positionParams[0].screenPosArea.width = 190f * scaleRatio;
		banner.positionParams[0].screenPosArea.height = 190f * scaleRatio;
#endif
		
		// Turn on the banner after changing its appearance settings.
		banner.enabled = true;
	}
		
	#region HomePromo banner events
	void OnBurstlyEventClicked(string adId)
	{
		Debug.Log("[HomePromoBannerSetup] OnBurstlyEventClicked called for: " + adId);
		if (targetInterstitial != null)
		{
			Debug.Log("targetInterstitial IsVisible = " + targetInterstitial.IsVisible);
			targetInterstitial.gameObject.SetActive(true);
			targetInterstitial.CreateAndShowAd();
		}
	}
	#endregion HomePromo banner events
	
	void OnDebugGUI()
	{
		GUILayout.BeginVertical();
		{
			GUILayout.Box("isRetinaDevice = " + isRetinaDevice, GUILayout.Height(40f));
			GUILayout.Box("bannerWidth = " + banner.positionParams[0].screenPosArea.width, GUILayout.Height(40f));
			GUILayout.Box("Screen.dpi = " + Screen.dpi, GUILayout.Height(40f));
			GUILayout.Box("SystemInfo.deviceModel = " + SystemInfo.deviceModel, GUILayout.Height(40f));
			
			if ( GUILayout.Button("LaunchPostGame", GUILayout.Height(40f)) )
			{
				Application.LoadLevel("GameInterstitial");
			}
		}
		GUILayout.EndVertical();
	}
}
