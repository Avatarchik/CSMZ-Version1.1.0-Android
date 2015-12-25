using UnityEngine;
using System.Collections;

public class PreGameInterstitialListener : MonoBehaviour 
{
	private InterstitialController interstitial;
	

	void Awake()
	{
		interstitial = GetComponent<InterstitialController>();
		//TODO: fix this ugly hack
		BurstlyEventsListener.canCreate = true;
		BurstlyEventsListener.Instance.RegisterEventsListener(interstitial.adId, gameObject);
	}
	
	/// <summary>
	/// Burstly native event raised by the BurstlyEventsListener.
	/// </summary>
	/// <param name='adId'>
	/// Ad identifier.
	/// </param>
	void OnBurstlyEventShown(string adId)
	{
		AnalyticsBinding.LogAdAction("NULL", "pregame_interstitial", "NULL", "Impression", "NULL", "NULL", InAppPurchasesSystem.locale);
	}
	
}
