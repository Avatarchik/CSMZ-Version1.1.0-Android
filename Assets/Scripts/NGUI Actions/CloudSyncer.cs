using UnityEngine;
using System.Collections;

public class CloudSyncer : MonoBehaviour 
{
	protected float updateTimeInterval = 4f;
	protected float lastUpdateTime;
	
	// Use this for initialization
	void Awake()
	{
		TweaksSystemManager.Instance.SynchTweaks();
		updateTimeInterval = TweaksSystem.Instance.floatValues["ResyncTime"];
		lastUpdateTime = Time.realtimeSinceStartup;
		
		StartCoroutine(CheckTime());
	}
	
	IEnumerator CheckTime()
	{
		WaitForSeconds oneSecond = new WaitForSeconds(1f);
		
		while (true) 
		{
			if (Time.realtimeSinceStartup - lastUpdateTime >= updateTimeInterval) {
				Debug.Log("Should update cloud");
				if (UserManagerCloud.Instance != null) {
					Debug.Log("Updating cloud");
					UserManagerCloud.Instance.LoadUserFromCloud();
				}
				lastUpdateTime = Time.realtimeSinceStartup;
			}
			
			yield return oneSecond;
		}
	}
}
