using UnityEngine;
using System.Collections;

public class IPodPlayerEventsHandler : MonoBehaviour
{
	public delegate void OnIPodPlayStateChangedDelegate(bool isPlaying);
	
	public static event OnIPodPlayStateChangedDelegate OnIPodPlayStateChanged;
	public static event System.Action OnIPodPlayStatusChecked;
	
	protected static IPodPlayerEventsHandler instance;
	
	public bool useEditorDebugMode = false;
	public float updateDelay = 5f;
		
	private bool isIPodPlaying = false;


	void Awake()
	{
		// Destroy this game object on other platforms different from iOS
		if (Application.platform != RuntimePlatform.IPhonePlayer && !Application.isEditor) {
			Destroy(this.gameObject);
			
			return;
		}
			
		if (instance != null)
		{
			Debug.LogWarning("[IPodPlayerEventsHandler] Multiple instances of this singleton found! In scene: " + Application.loadedLevelName);
			
			Destroy(gameObject);
			return;
		}

		instance = this;
		Object.DontDestroyOnLoad(gameObject);
	}
	
	public static IPodPlayerEventsHandler Instance
	{
		get 
		{
			if (instance == null) 
			{
				GameObject container = new GameObject("IPodPlayerEventsHandler", typeof(IPodPlayerEventsHandler));
				DontDestroyOnLoad(container);
			}

			return instance;
		}
	}
	
	void Start()
	{
		CheckIPodState();
		StartCoroutine( UpdateIPodState() );
	}
	
	void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			// Check after a safe delay after resuming the app because the OS might have the iPod temporarilly stopped after a recent phone call or voice record/command.
			StopCoroutine("CheckIPodStateAfterDelay");
			StartCoroutine("CheckIPodStateAfterDelay", 2f);
//			CheckIPodState();
		} 
	}
	
	public IEnumerator UpdateIPodState()
	{
		WaitForSeconds waitTime = new WaitForSeconds(updateDelay);
		
		while(true)
		{
			yield return waitTime;
			
			CheckIPodState();			
		}
	}
	
	public bool IsIPodPlaying
	{
		get
		{
			return isIPodPlaying;
		}
		set 
		{
			bool oldValue = isIPodPlaying;
			isIPodPlaying = value;
			
			if (oldValue != isIPodPlaying)
			{
				if (isIPodPlaying)
				{
					RaiseOnIPodStartedPlaying();
				}
				else
				{
					RaiseOnIPodStoppedPlaying();
				}
			}
		}
	}
	
	IEnumerator CheckIPodStateAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		CheckIPodState();
	}
		
	public void CheckIPodState()
	{
		IsIPodPlaying = IPodPlayerEventsBinding.IsIPodPlaying();
		
		if (OnIPodPlayStatusChecked != null) {
			OnIPodPlayStatusChecked();
		}
	}
	
	public void RaiseOnIPodStartedPlaying()
	{
		if (OnIPodPlayStateChanged != null) 
		{
			OnIPodPlayStateChanged(true);
		}
	}

	public void RaiseOnIPodStoppedPlaying()
	{
		if (OnIPodPlayStateChanged != null)
		{
			OnIPodPlayStateChanged(false);
		}
	}
	
#if UNITY_EDITOR
	void OnGUI() 
	{
		if ( !useEditorDebugMode ) {
			return;
		}
			
		GUILayout.BeginVertical();
		{
			if ( GUILayout.Button("Raise iPod Playing", GUILayout.Height(50f)) ) {
				RaiseOnIPodStartedPlaying();
			}
			
			if ( GUILayout.Button("Raise iPod Stopped", GUILayout.Height(50f)) ) {
				RaiseOnIPodStoppedPlaying();
			}
		}
		GUILayout.EndVertical();
	}
	
#endif
	
}

