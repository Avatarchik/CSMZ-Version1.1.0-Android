using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BurstlyBannerPosition
{
	TopScreen = 0,
	BottomScreen,
}

public class AdBannerController : BurstlyAdController 
{	
	public enum DeviceScreenOrientation
	{
		Any = 0,
		Landscape,
		Portrait,
	}
		
	public enum BannerPositionMode
	{
		Manual = 0,
		TopLeft,
		TopRight,
		TopCenter,
		BottomCenter,
		BottomRight,
		BottomLeft,
	}

	/// <summary>
	/// The refresh rate of the banner. If is major to -1 will be setted to the banner.
	/// </summary>
	public float refreshRate = 0.0f;
	
	/// <summary>
	/// Position parameters for auto-positioning the banner in different positions depending on the device orientation.
	/// </summary>
	[System.Serializable]
	public class PositionParams
	{
		/// <summary>
		/// The device orientation for which these positioning params will apply to. If set to <c>DeviceScreenOrientatino.Any</c>, these params will apply to any
		/// device orientation.
		/// </summary>
		public DeviceScreenOrientation deviceOrientation = DeviceScreenOrientation.Any;
		
//		/// <summary>
//		/// If <c>true</c> <see cref="screenPosArea"/> (X, Y) position will be used as coordinates in the interval [0, 1] relative to the Screen.width and Screen.height.
//		/// Otherwise the coordinates will be used as absolute positions.
//		/// </summary>
//		public bool useRelativePositions;

		/// <summary>
		/// The screen position and size of the banner container area used when manual positioning or manual size is set for this banner.
		/// </summary>
		public Rect screenPosArea;
		/// <summary>
		/// The position mode of the banner for this selected deviceOrientation. If <c>BannerAutoPosition.Manual</c> the <see cref="screenPosArea"/>
		/// device screen coordinates will be used. The anchor of the banner frame area is TopLeft and currently it can't be changed.
		///Note: On iOS these coordinates are device points not pixels.
		/// </summary>
		public BannerPositionMode positionMode = BannerPositionMode.BottomCenter;
	}
	
	/// <summary>
	/// The supported banner frame sizes that can be requested from the server.
	/// When setting <see cref="IsAutoSizeBanner "/> property to true, the controller will pick the closest size that fits the current device resolution.
	/// Note: The below resolutions are in device points not pixels (dpi-independent coordinates).
	/// This property is used only for Auto-Sized banners.
	/// </summary>
	public Vector2[] supportedBannerSizes = new Vector2[]
	{
		new Vector2(320f, 50f), // For iPhone/iPod Touch devices
		new Vector2(728f, 90f), // For iPad/Tablet devices
	};

	public List<PositionParams> positionParams = new List<PositionParams>();
	
	/// <summary>
	/// The currently active positioning param decided by the <see cref="positionParams"/> specified.
	/// </summary>
	[System.NonSerialized]
	public PositionParams activePositionParam;
	
	/// <summary>
	/// The size of the banner frame size that will be requested from the server is auto-determined based
	/// on the current device screen orientation, resolution and density.
	/// </summary>
	[SerializeField]
	private bool isAutoSizeBanner = false;		
	
	protected override void Awake()
	{
		base.Awake();
		
		if (positionParams == null || positionParams.Count == 0)
		{
			Debug.LogError("[AdBannerController] The current banner doesn't have at least one positioning param set: " + name + " bannerId = " + adId);
		}
	}
		
	void OnEnable()
	{		
		OrientationListener.Instance.OnOrientationChanged += OnScreenOrientationChanged;

//		Debug.Log("[AdBannerController] Enabled " + name + " banner: " + bannerId);
		
		// If the banner wasn't re-created by the banner settings updater, we have to create and add the banner back to the view.
		if ( UpdateBannerSettings() )
		{
			// Create and show the currently configured banner
			DestroyAdView();
			CreateAndShowAd();
		}
	}

	void OnDisable()
	{
		if(OrientationListener.IsInstantiated())
		{
			OrientationListener.Instance.OnOrientationChanged -= OnScreenOrientationChanged;
		}

		// Hide the current banner
		IsVisible = false;
	}
	
	/// <summary>
	/// Gets the width of the device screen in device point coordinates (not pixel coordinates / dpi-independent).
	/// Note: This property will not change according to device orientation.
	/// </summary>
	/// <value>
	/// The width of the device screen in portrait mode.
	/// </value>
	public static float DeviceScreenWidth
	{
		get
		{
#if UNITY_EDITOR
			return Screen.width;
#elif UNITY_IPHONE
			return NativeMessageBinding.GetNativeScreenWidth();
#else		
			return Screen.width;
#endif
		}
	}

	/// <summary>
	/// Gets the height of the device screen in device point coordinates (not pixel coordinates / dpi-independent).
	/// Note: This property will not change according to device orientation.
	/// </summary>
	/// <value>
	/// The height of the device screen in portrait mode.
	/// </value>
	public static float DeviceScreenHeight
	{
		get
		{
#if UNITY_EDITOR
			return Screen.height;
#elif UNITY_IPHONE
			return NativeMessageBinding.GetNativeScreenHeight();
#else
			return Screen.height;
#endif
		}
	}
	
	public static bool IsDeviceInLandscape
	{
		get {
			return Screen.width > Screen.height;
		}
	}
		
	/// <summary>
	/// Gets or sets whether the Ad banner view is visible and added over the Unity view.
	/// Note: this doesn't necessarilly mean that the Ad content is also visible (depends on internet connection, if there's content sent from server, etc.).
	/// </summary>
	/// <value>
	/// <c>true</c> if Ad view is added and visible; otherwise, <c>false</c>.
	/// </value>
	public override bool IsVisible
	{
		set
		{			
			base.IsVisible = value;
			
			if (IsVisible) {
				BurstlyAds.addBannerToView(adId);
			}
			else {
				BurstlyAds.removeBannerFromView(adId);
			}
		}
	}
	
	public bool IsAutoSizeBanner
	{
		get {
			return isAutoSizeBanner;
		}
		set {
			isAutoSizeBanner = value;
		}
	}
		
	public override void CreateAndShowAd()
	{
		StartCoroutine( CreateAndShowAdCoroutine() );
	}
	
	private IEnumerator CreateAndShowAdCoroutine()
	{
//		Debug.Log("[AdBannerController] Creating new ad banner on GameObject " + name + ": " + bannerId);
		if (activePositionParam == null) {
			yield break;
		}
		
		BurstlyAds.createBannerPlacement(adId, adAppId, adZoneId, activePositionParam.screenPosArea.x, activePositionParam.screenPosArea.y,
										 activePositionParam.screenPosArea.width, activePositionParam.screenPosArea.height);

		// Apply ad parameters
		ApplyAdParameters(AdParameters);
		IsVisible = true;

		// The sample app for the Burstly Ads require one frame delay before showing the created ad banner.
		yield return null;
		
		// Start displaying add content.
		BurstlyAds.showAd(adId);
		if (refreshRate > 0.0f) {
			BurstlyAds.setBannerRefreshRate(adId, refreshRate);
		}
	}
	
	/// <summary>
	/// Tries to update the active positioning parameter based on the current device orientation specified in the 
	/// <see cref="positionsParams"/> of this banner.
	/// <returns>
	/// <c>true</c> If a positioning param from the specified ones <see cref="positionParams"/> has been successfully selected.
	/// <c>false</c> otherwise and the current active positioning param is going to be null.
	/// </returns>
	/// </summary>
	private PositionParams GetActivePositioningParam()
	{
		bool isLandscape = IsDeviceInLandscape;
		
		PositionParams newPositionParam = null;
		
		for(int i = 0; i < positionParams.Count; i++)
		{
			if (positionParams[i].deviceOrientation == DeviceScreenOrientation.Any ||
				positionParams[i].deviceOrientation == DeviceScreenOrientation.Landscape && isLandscape || 
				positionParams[i].deviceOrientation == DeviceScreenOrientation.Portrait && !isLandscape)
			{
				newPositionParam = positionParams[i];
				break;
			}
		}
		
		return newPositionParam;
	}

	private void AutoSetBannerPosition()
	{
		if (activePositionParam == null) {
			return;
		}
			
		float adjustedDeviceScreenWidth = DeviceScreenWidth;
		float adjustedDeviceScreenHeight = DeviceScreenHeight;

#if !UNITY_EDITOR && UNITY_IPHONE
		// Switch the device screen width and height depending on the device screen orientation.
		//Note: This is done because the ad banners on iOS are added using another view controller child to Unity's view controller and this one doesn't
		// auto-rotate when the device is rotated. (cool Burstly library support ain't it? :D)
		if (IsDeviceInLandscape)
		{
			// Device is in landscape
			adjustedDeviceScreenWidth = DeviceScreenHeight;
			adjustedDeviceScreenHeight = DeviceScreenWidth;
		}
		else
		{
			// Device is in portrait
			adjustedDeviceScreenWidth = DeviceScreenWidth;
			adjustedDeviceScreenHeight = DeviceScreenHeight;
		}
#endif		
		switch (activePositionParam.positionMode)
		{
			case BannerPositionMode.TopLeft:
			{
				activePositionParam.screenPosArea.x = 0f;
				activePositionParam.screenPosArea.y = 0f;
			}
			break;
			
			case BannerPositionMode.TopRight:
			{
				activePositionParam.screenPosArea.x = adjustedDeviceScreenWidth - activePositionParam.screenPosArea.width;
				activePositionParam.screenPosArea.y = 0f;
			}
			break;

			case BannerPositionMode.TopCenter:
			{
				activePositionParam.screenPosArea.x = adjustedDeviceScreenWidth * 0.5f - activePositionParam.screenPosArea.width * 0.5f;
				activePositionParam.screenPosArea.y = 0f;
			}
			break;
			
			case BannerPositionMode.BottomCenter:
			{
				activePositionParam.screenPosArea.x = adjustedDeviceScreenWidth * 0.5f - activePositionParam.screenPosArea.width * 0.5f;
				activePositionParam.screenPosArea.y = adjustedDeviceScreenHeight - activePositionParam.screenPosArea.height;
			}
			break;
			
			case BannerPositionMode.BottomRight:
			{
				activePositionParam.screenPosArea.x = adjustedDeviceScreenWidth - activePositionParam.screenPosArea.width;
				activePositionParam.screenPosArea.y = adjustedDeviceScreenHeight - activePositionParam.screenPosArea.height;
			}
			break;

			case BannerPositionMode.BottomLeft:
			{
				activePositionParam.screenPosArea.x = 0f;
				activePositionParam.screenPosArea.y = adjustedDeviceScreenHeight - activePositionParam.screenPosArea.height;
			}
			break;

			default:
			break;
		}
		
		//Note: the banner might not have been created when this method is called, but it won't crash. 
		// This is because there is no way to to check from Unity Burstly lib if the banner was actually registered and created previously on the native side.
		BurstlyAds.setBannerOrigin(adId, activePositionParam.screenPosArea.x, activePositionParam.screenPosArea.y);
	}
	
	/// <summary>
	/// Auto-set the size of the banner frame that will be requested from the server based on the current device resolution and the supported banner frame sizes.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the banner size changed from the previous size, otherwise <c>false</c>.
	/// </returns>
	private bool AutoSetBannerSize()
	{	
		int closestFitIdx = -1;
		bool didBannerFrameChanged = false;
		
		if (activePositionParam == null) {
			return false;
		}
		
		// Sort supported banner sizes ascending.
		System.Array.Sort<Vector2>(supportedBannerSizes, (a, b) => {
			if (a.x < b.x) {
				return -1;
			}
			else if (a.x > b.x) {
				return 1;
			}
			else
				return 0;
		});
		
		// Pick the best banner size width to fit the current screen width.
		float deviceScreenWidth = DeviceScreenWidth;
		for (int i = supportedBannerSizes.Length - 1; i >= 0; i--)
		{
			if (supportedBannerSizes[i].x <= deviceScreenWidth) {
				closestFitIdx = i;
				break;
			}
		}
		
		if (closestFitIdx < 0)
		{
			Debug.LogWarning("[AdBannerController] " + name + " -> " + adId + 
				"->AutoSetBannerSize: Couldn't find any good banner frame size to fit current screen width: " + DeviceScreenWidth);
		}
		else 
		{
			if (supportedBannerSizes[closestFitIdx].x != activePositionParam.screenPosArea.width || 
				supportedBannerSizes[closestFitIdx].y != activePositionParam.screenPosArea.height)
			{
				didBannerFrameChanged = true;
			}
			
			// Update banner container size according to device resolution.
			activePositionParam.screenPosArea.width = supportedBannerSizes[closestFitIdx].x * DisplayMetricsAndroid.Density;
			activePositionParam.screenPosArea.height = supportedBannerSizes[closestFitIdx].y * DisplayMetricsAndroid.Density;

		}

//		Debug.Log("[AdBannerController] DeviceScreenWidth = " + DeviceScreenWidth);
//		Debug.Log("[AdBannerController] DeviceScreenHeight = " + DeviceScreenHeight);
//		Debug.Log("[AdBannerController] screenPosArea.width = " + screenPosArea.width);
//		Debug.Log("[AdBannerController] screenPosArea.height = " + screenPosArea.height);

		return didBannerFrameChanged;
	}
	
	/// <summary>
	/// Event raised by the OrientationListener component when the screen orientation changes.
	/// </summary>
	/// <param name='orientation'>
	/// Orientation.
	private void OnScreenOrientationChanged(ScreenOrientation orientation)
	{
//		Debug.Log("[AdBannerController] OnScreenOrientationChanged called on: " + name);
		if ( UpdateBannerSettings() )
		{
			Debug.Log("[AdBannerController] OnScreenOrientationChanged: re-creating banner view for: " + name);
			DestroyAdView();
			CreateAndShowAd();
		}
	}
		
	/// <summary>
	/// Updates the banner settings (visibility, active positioning parameters, screen position and container banner size, etc.)
	/// </summary>
	/// <returns>
	/// <c>true</c> if the banner view needs to be destroyed and re-created because of some changed settings.
	/// <c>false</c> if the banner doesn't need to be re-created.
	/// </returns>
	public bool UpdateBannerSettings()
	{		
		bool bannerNeedsRecreation = false;
		
		PositionParams prevPositionParam = activePositionParam;
		PositionParams newPositionParam = GetActivePositioningParam();
			
		if (newPositionParam == null && IsVisible)
		{
			// Remove the current banner if there's no positioning param found for it.
			IsVisible = false;
		}
		else if (newPositionParam != null && !IsVisible)
		{
			// Display the current banner if there is a valid positioning param for it.
			IsVisible = true;
		}

		activePositionParam = newPositionParam;
		
		if (activePositionParam == null) {
			return false;
		}
		
		if (IsAutoSizeBanner) 
		{
			// Check if after updating the banner size, the banner container size is different from the previous container size.
			if ( AutoSetBannerSize() )
			{
				// If the banner needs to change its container size (because the current Burstly Unity wrapper doesn't support resizing the container)
				// we need to destroy it and re-create it.
				bannerNeedsRecreation = true;
			}
		}
		else if (prevPositionParam == null || (prevPositionParam.screenPosArea.width != activePositionParam.screenPosArea.width || 
			 								   prevPositionParam.screenPosArea.height != activePositionParam.screenPosArea.height))
		{
			bannerNeedsRecreation = true;
		}
		
		if (activePositionParam.positionMode != BannerPositionMode.Manual)
		{
			AutoSetBannerPosition();
		}
		else if ( !bannerNeedsRecreation ) 
		{
			// Update banner position if it doesn't require a re-create.
			BurstlyAds.setBannerOrigin(adId, activePositionParam.screenPosArea.x, activePositionParam.screenPosArea.y);
		}
		
		return bannerNeedsRecreation;
	}
	
#if UNITY_EDITOR		
	void OnGUI()
	{
		if ( !simulateBannerInEditor ) {
			return;
		}
		
		if (activePositionParam == null)
		{
//				Debug.LogWarning("[AdBannerController] No positioning param can be found corresponding to the current device orientation: "
//								+ name + " bannerId = " + bannerId);
			return;
		}
		
		GUI.Button(activePositionParam.screenPosArea, "SimulatedBanner: " + name);
	}
#endif
}
