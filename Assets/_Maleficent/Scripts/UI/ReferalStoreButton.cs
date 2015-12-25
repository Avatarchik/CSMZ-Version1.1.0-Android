using UnityEngine;
using System.Collections;

public class ReferalStoreButton : MonoBehaviour 
{
	//To take care of multiple instances of the button in the scene
	public static bool firstReferralButtonShown = false;

	void Awake()
	{
#if AMAZON_ANDROID
		Destroy(gameObject);
#endif
	}

	void Start()
	{
		if (!firstReferralButtonShown) {
			Debug.Log("Analytics banner disney");
			firstReferralButtonShown = true;
			AnalyticsBinding.LogAdAction("Main_Button", "More_Disney", null, "Impression", null, null, null);
		}
	}

	void OnDestroy()
	{
		if (firstReferralButtonShown) {
			firstReferralButtonShown = false;
		}
	}


	void ShowReferalView()
	{
		_StopMusic();

		ReferalStore rs = ReferalStoreManager.Instance.referalStore;
		rs.OnReferalStoreClosed += HandleOnReferalStoreClosed;
		rs.ShowReferalView();
	}

	void HandleOnReferalStoreClosed (ReferalStore rs)
	{
		rs.OnReferalStoreClosed -= HandleOnReferalStoreClosed;
		_PlayMusic();
	}



	//Private

	void _StopMusic()
	{
		MusicController.Instance.audioSrc.mute = true;
	}
	
	
	void _PlayMusic()
	{
		MusicController.Instance.audioSrc.mute = false;
	}
}
