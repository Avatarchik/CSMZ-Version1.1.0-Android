using UnityEngine;
using System.Collections;

public class BurstlyManagerSample : MonoBehaviour {


	const string APP_ID = "P2aGXDX9cEKVt2MsQEWwTg";
	const string BANNER_ZONE_ID = "0850164859113244507";
	const string INTERSTITIAL_ZONE_ID = "0050164959113244507";
	
	bool isBannerShown = false;


	void Start()
	{
		BurstlyManager.burstlyBinding.ConfigureBanner(BANNER_ZONE_ID, 0, 0, 320, 53, BurstlyBindingBannerAnchor.Bottom);
		BurstlyManager.burstlyBinding.ConfigureInterstitial (INTERSTITIAL_ZONE_ID, true);
	}


	void OnMouseDown()
	{
		if (!isBannerShown) {
			BurstlyManager.burstlyBinding.ShowBanner(BANNER_ZONE_ID);
		} else {
			BurstlyManager.burstlyBinding.HideBanner(BANNER_ZONE_ID);
		}
		isBannerShown = !isBannerShown;

		BurstlyManager.burstlyBinding.ShowInterstitial (INTERSTITIAL_ZONE_ID);
	}
}
