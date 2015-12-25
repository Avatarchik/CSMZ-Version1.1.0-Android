using UnityEngine;
using System.Collections;

public class AdController : MonoBehaviour {

	public string tweaksSystemKey;

	public BurstlyAdController banneriOS;
	public BurstlyAdController bannerGooglePlay;
	public BurstlyAdController bannerAmazon;

	BurstlyAdController _platformBanner = null;
	bool _isShown = false;
	public static bool buyLivesPanelFlag;

	void Start()
	{
		_EnablePlatformBanner();
	}


	void Destroy()
	{
		if (_platformBanner != null) {
			BurstlyEventsListener.Instance.UnregisterEventsListener(_platformBanner.adZoneId, this.gameObject);
		}
	}


	public void ShowInterstitialIfNeeded()
	{
		if(_platformBanner != null 
		   && buyLivesPanelFlag
		   && !_isShown)
		{
			_isShown = true;
			buyLivesPanelFlag = false;
			_platformBanner.CreateAndShowAd();
		}
	}


	void _EnablePlatformBanner()
	{
		bool enabledInterstitial = false;
		BurstlyAdController platformBanner = null;


		if(TweaksSystem.Instance.intValues.ContainsKey(tweaksSystemKey)) {
			int enabled = TweaksSystem.Instance.intValues[tweaksSystemKey];
			enabledInterstitial = (enabled != 0);
#if UNITY_IOS
			platformBanner = banneriOS;
#elif UNITY_ANDROID
#if AMAZON_ANDROID
			platformBanner = bannerAmazon;
#else
			platformBanner = bannerGooglePlay;
#endif
#endif
		}

		if (platformBanner != null) {
			_platformBanner = platformBanner;
			_platformBanner.gameObject.SetActive(enabledInterstitial);

			BurstlyEventsListener.Instance.RegisterEventsListener(_platformBanner.adId, this.gameObject);
		}
	}


	void OnBurstlyEventTakeoverFullscreen(string bannerId)
	{
//		if (_platformBanner != null && _isShown && _platformBanner.adId == bannerId) {
//			_isShown = false;
//			_PlayMusic();
//		}
		if (_platformBanner != null && _platformBanner.adId == bannerId) {
			_StopMusic();
		}

	}


	void OnBurstlyEventDismissFullscreen(string bannerId)
	{
		if (_platformBanner != null && _platformBanner.adId == bannerId) {
			_PlayMusic();
		}
	}


	void _StopMusic()
	{
		MusicController.Instance.audioSrc.mute = true;
	}


	void _PlayMusic()
	{
		MusicController.Instance.audioSrc.mute = false;
	}
}
