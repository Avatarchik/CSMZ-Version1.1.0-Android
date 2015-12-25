using UnityEngine;
using System.Collections;

public abstract class BurstlyAdController : MonoBehaviour
{
	/// <summary>
	/// The dont destroy this controller on load level. 
	/// If the controller is destroyed its banner is also destroyed and removed.
	/// </summary>
	public bool dontDestroyOnLoad = false;

	/// <summary>
	/// The ad parameters that will be sent when the ad banner starts. (ad targetting params as described in the Burstly docs)
	/// </summary>
	[SerializeField]
	private string adParameters = "";

	/// <summary>
	/// The interstitial identifier. Must be unique.
	/// Used to access/manage natively allocated ad interstitials.
	/// </summary>
	public string adId = "";
	public string adAppId = "";
	public string adZoneId = "";

	public bool simulateBannerInEditor = true;
	
	private bool isVisible = false;
	
	
	protected virtual void Awake()
	{
		if (dontDestroyOnLoad)
		{
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}

	protected virtual void Start() { }
	
	public abstract void CreateAndShowAd();
	
	/// <summary>
	/// Gets or sets the ad parameters.
	/// </summary>
	/// <value>
	/// The ad parameters.
	/// </value>
	public string AdParameters
	{
		get {
			return adParameters;
		}
		set {
			adParameters = value;
		}
	}
	
	/// <summary>
	/// Gets or sets whether the Ad banner view is visible and added over the Unity view.
	/// Note: this doesn't necessarilly mean that the Ad content is also visible (depends on internet connection, if there's content sent from server, etc.).
	/// </summary>
	/// <value>
	/// <c>true</c> if Ad view is added and visible; otherwise, <c>false</c>.
	/// </value>
	public virtual bool IsVisible
	{
		get {
			return isVisible;
		}
		set 
		{			
			isVisible = value;
		}
	}
	
	public void ApplyAdParameters(string _adParameters)
	{
		AdParameters = _adParameters;
		BurstlyAds.setTargettingParameters(adId, _adParameters);
	}
	
	public virtual void DestroyAdView()
	{
		IsVisible = false;
		BurstlyAds.destroyAdPlacement(adId);
	}
	
	protected virtual void OnDestroy() 
	{
		// Destroy the Ad view.
		DestroyAdView();
	}
}
