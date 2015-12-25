using UnityEngine;
using System.Collections;
/*
public enum FacebookEventId
{
	FacebookInit,
	UserLoggedIn = 0,
	UserLoggedOut,
	UserAvatarReceived,
}

public class FacebookManager : MonoBehaviour
{
	protected static FacebookManager instance;
	protected static bool canCreate = true;
	protected static bool initialized = false;	

	public event System.Action<bool> OnInitializedEvent;
	public event System.Action<bool> OnUserLoggedInEvent;
	public event System.Action<bool> OnUserLoggedOutEvent;
	public event System.Action<FacebookEventId> OnFacebookStatusChangedEvent;
	
	public System.Action<Texture2D> OnAvatarReceived;
	
	protected bool isWaitingForLogout = false;
	
	public Texture2D userAvatar;
	
	
	public static FacebookManager Instance 
	{
		get 
		{
			if (instance == null && canCreate && GameObject.Find("FacebookManager") != null) 
			{
				new GameObject("FacebookManager", typeof(FacebookManager));
			}
			
			return instance;
		}
	}
	
	
	#region Public properties
	public bool IsUserLogged 
	{
		get {
			return initialized && FB.IsLoggedIn;
		}
	}
	#endregion
	
		
	#region Unity MonoBehavior methods
	void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("Multiple instances of FacebookManager found in scene: " + Application.loadedLevelName);
			Destroy(gameObject);
			return;
		}
		
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
	
	void OnDestroy()
	{
		canCreate = false;
	}
	#endregion
	
	
	#region Facebook init methods/events
	public void Init()
	{
		Debug.Log("[FacebookManager] Init called...");
		
		if ( !initialized ) {
			FB.Init(OnInitComplete);
		}
		else {
			OnInitComplete();
		}
	}
	
	/// <summary>
	/// Raises the init complete event after this method is triggered by the internal FB init event.
	/// </summary>
	protected void OnInitComplete()
    {
		Debug.Log("[FacebookManager] Facebook initialized...");
        initialized = true;
		
		if (OnInitializedEvent != null) {
			OnInitializedEvent(true);
		}
		
		if (OnFacebookStatusChangedEvent != null)
		{
			OnFacebookStatusChangedEvent(FacebookEventId.FacebookInit);
		}
    }
	#endregion
	
	
	#region Login methods/events
	public void Login()
	{
		if (!initialized) {
			return;
		}
		
		Debug.Log("[FacebookManager] Trying to login");
		
		if (!FB.IsLoggedIn) {
			FB.Login("", OnFBLoginComplete);
		}
	}
	
	private void OnFBLoginComplete(FBResult result)
    {
		if ( !string.IsNullOrEmpty(result.Error)) {
			Debug.Log("Login error: " + result.Error);
		}
		
        if (OnUserLoggedInEvent != null) {
			OnUserLoggedInEvent(string.IsNullOrEmpty(result.Error) && FB.IsLoggedIn);
		}
		
		if (OnFacebookStatusChangedEvent != null)
		{
			OnFacebookStatusChangedEvent(FacebookEventId.UserLoggedIn);
		}
    }
	#endregion
	
	
	#region Logout methods/events
	public void Logout()
	{
		if (!initialized || isWaitingForLogout) 
		{
			return;
		}
		
		FB.Logout();
		
		StartCoroutine(WaitForLogout());
	}
	
	IEnumerator WaitForLogout()
	{
		isWaitingForLogout = true;
		
		//TODO: risk of refussing to logout because of server connection (what then? ignore?)
		while(FB.IsLoggedIn)
		{
			yield return new WaitForSeconds(0.1f);

		}
		
		isWaitingForLogout = false;
		
		OnFBLogoutComplete();
	}
		
	private void OnFBLogoutComplete()
    {
        if (OnUserLoggedOutEvent != null) {
			OnUserLoggedOutEvent( !FB.IsLoggedIn );
		}
		
		if (OnFacebookStatusChangedEvent != null)
		{
			OnFacebookStatusChangedEvent(FacebookEventId.UserLoggedOut);
		}
    }
	#endregion
	
	
	public void GetAvatar(System.Action<Texture2D> receivedCallback, bool forceRedownload = false)
	{
		if (!initialized) {
			return;
		}
		
		if (forceRedownload)
		{
			userAvatar = null;
		}
		
		if (userAvatar != null) 
		{
			receivedCallback(userAvatar);			
		}
		else 
		{
			if (!FB.IsLoggedIn) {
				receivedCallback(null);
			}
			else 
			{
				FB.API("/me/picture?height=64&width=64", Facebook.HttpMethod.GET, ((FBResult response) => 
				{
					if (response.Texture != null) {
						userAvatar = response.Texture;
					}
					
					receivedCallback(userAvatar);
					
					if (OnAvatarReceived != null) {
						OnAvatarReceived(userAvatar);
					}
				}));
			}
		}
	}
}
*/
