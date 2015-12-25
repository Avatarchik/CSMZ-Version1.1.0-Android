// un-comment to enable the "B" in the editor to act as the Android hardware Back button for testing purposes
//#define DEBUG_IN_EDITOR_ON

using UnityEngine;
using System.Collections;

public class AndroidAudioEventsHandler : MonoBehaviour
{
	protected const string strRingerModeChangedEventName = "AndroidRingerModeChangedEvent";
	
	protected static AndroidAudioEventsHandler instance = null;
	
	
	public static AndroidAudioEventsHandler Instance 
	{
		get 
		{
			if (instance == null)
			{
				GameObject newInstance = new GameObject("AndroidAudioEventsHandler", typeof(AndroidAudioEventsHandler));
				GameObject.DontDestroyOnLoad(newInstance.gameObject);
			}

			return instance;
		}
	}
	
	void Awake()
	{
		// Destroy this GameObject on platforms different from Android. Keep this handler in the editor for flow tests.
		if (Application.platform != RuntimePlatform.Android && !Application.isEditor)
		{
			Destroy(gameObject);
			
			return;
		}
		
		if (instance != null) 
		{
			Debug.LogWarning("[AndroidAudioEventsHandler] Another instance of this singleton is present in the scene: " + Application.loadedLevelName);
			Destroy(this.gameObject);
			
			return;
		}
		
		instance = this;
		GameObject.DontDestroyOnLoad(gameObject);
		
		AndroidAppEventsBinding.RegisterAudioMgrEventsHandler(name, strRingerModeChangedEventName);
	}

	void Start()
	{
		// Force check the audio manager the first time the app starts
 		SyncAudioToOSSoundMode();
		
	}
	
	public bool IsOSSoundEnabled 
	{
		get {
			string osResult = AndroidAppEventsBinding.GetOSRingerMode();

			return osResult != "silent" && osResult != "vibrate";
		}
	}
	
	/// <summary>
	/// Syncs the Unity sound to OS sound mode. Useful for Android where the OS Sound Mode is independent from the Unity sound mode.
	/// On iOS this method will always enable the AudioListener volume and set it to 1f.
	/// On Android if the OS Ringer Mode is set to silent or vibrate, it will disable the Unity sound (set the AudioListener volume to 0f).
	/// This behavior should be provided out of the box by Unity on Android... :(
	/// </summary>
	public void SyncAudioToOSSoundMode()
	{
		Debug.Log("[AndroidAudioEventsHandler] SyncAudioToOSSoundMode()");
		UpdateAudioRingerMode( AndroidAppEventsBinding.GetOSRingerMode() );
	}
	
	protected void UpdateAudioRingerMode(string ringerMode)
	{
		switch(ringerMode)
		{
			case "silent":
			case "vibrate":
				AudioListener.volume = 0f;
			break;

			case "normal":
				AudioListener.volume = 1f;
			break;
		}		
	}
	
	/// <summary>
	/// Event triggered by the Android native binding when the OS AudioManager ringer mode is changed
	/// </summary>
	/// <param name='eventMsg'>
	/// Event message.
	/// </param>
	private void AndroidRingerModeChangedEvent(string eventMsg) 
	{
		Debug.Log("[AndroidAudioEventsHandler] AndroidRingerModeChangedEvent(string eventMsg): " + eventMsg);
		UpdateAudioRingerMode(eventMsg);
	}

	void OnApplicationPause(bool isPaused)
	{
		if ( !isPaused )
		{
			AndroidAppEventsBinding.RegisterAudioMgrEventsHandler(name, strRingerModeChangedEventName);
			SyncAudioToOSSoundMode();
		}
		else {
			AndroidAppEventsBinding.UnregisterAudioMgrEventsHandler();
		}
	}
	
	void OnApplicationQuit()
	{
		AndroidAppEventsBinding.UnregisterAudioMgrEventsHandler();
	}
			
	// Try to fix a weird Unity 3.4.2f3 bug that leaks GO that are marked as DontDestroyOnLoad.
//#if UNITY_EDITOR
//	void OnDestroy()
//	{
//		DestroyImmediate(gameObject);
//	}
//#endif
}
