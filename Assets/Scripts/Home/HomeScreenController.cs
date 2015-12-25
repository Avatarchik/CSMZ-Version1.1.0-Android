using UnityEngine;
using System.Collections;

public class HomeScreenController : MonoBehaviour {

	public GameObject bannerIOS;
	public GameObject bannerGooglePlay;
	public GameObject bannerAmazon;

	public GameObject xPromoBannerIOS;
	public GameObject xPromoBannerGooglePlay;
	public GameObject xPromoBannerAmazon;

	void Awake()
	{
		_ShowBannerIfNeeded();
		_ShowXpromoIfNeeded();

		Debug.Log("trying to show banner!");
	}


	void _ShowBannerIfNeeded()
	{
		GameObject bannerToShow = null;	
#if UNITY_IOS
		bannerToShow = bannerIOS;
#elif UNITY_ANDROID
		bannerToShow = bannerGooglePlay;
		//TODO amazon
#endif
		if (bannerToShow != null) {
			_ShowObjectIfNeeded("EnableBanner", bannerToShow);
		}
		
	}


	void _ShowXpromoIfNeeded()
	{
		GameObject bannerToShow = null;	
#if UNITY_IOS
		bannerToShow = xPromoBannerIOS;
#elif UNITY_ANDROID
		bannerToShow = xPromoBannerGooglePlay;
		//TODO amazon
#endif
		if (bannerToShow != null) {
			_ShowObjectIfNeeded("EnableXpromo", bannerToShow);
		}
	}


	void _ShowObjectIfNeeded(string tweaksKey, GameObject obj)
	{
		bool shouldShow = false;

		if (TweaksSystem.Instance.intValues.ContainsKey(tweaksKey)) {
			shouldShow = (TweaksSystem.Instance.intValues[tweaksKey] != 0);
		}

		obj.SetActive(shouldShow);
	}
}
