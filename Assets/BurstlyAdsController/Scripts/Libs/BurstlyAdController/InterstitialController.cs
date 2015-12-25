using UnityEngine;
using System.Collections;

public class InterstitialController : BurstlyAdController 
{
	public bool autoShowOnStart = true;
	
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		if (autoShowOnStart) {
			CreateAndShowAd();
		}
	}
		
	public override void CreateAndShowAd()
	{
#if UNITY_EDITOR
		interstitialShowTime = Time.time;
#endif
		IsVisible = true;
		StartCoroutine( CreateAndShowInterstitialCoroutine() );
	}
	
	private IEnumerator CreateAndShowInterstitialCoroutine()
	{
		BurstlyAds.createInterstitialPlacement(adId, adAppId, adZoneId);
		// Apply ad parameters
		ApplyAdParameters(AdParameters);

		yield return null;
		
		// Display interstitial.
		BurstlyAds.showAd(adId);
	}

	
#if UNITY_EDITOR
	private float interstitialDelay = 3f;
	private float interstitialShowTime;
	
	// Simulate banner in editor
	void OnGUI()
	{
		if ( !simulateBannerInEditor ) {
			return;
		}
		

		if (Time.time - interstitialShowTime <= interstitialDelay)
		{
			GUI.Button(new Rect(0f, 0f, Screen.width, Screen.height), "Simulated interstitial. Skipping video in: " + 
						(int)(interstitialDelay - (Time.time - interstitialShowTime)));
		}
	}
#endif
}
