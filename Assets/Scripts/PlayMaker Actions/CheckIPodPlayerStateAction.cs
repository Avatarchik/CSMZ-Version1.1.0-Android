using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("Application")]
public class CheckIPodPlayerStateAction : FsmStateAction {
	
	public override void OnEnter()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
		{
			if (IPodPlayerEventsHandler.Instance == null) {
				Debug.LogWarning("[Playmaker->IPodPlayerEventsAction] IPodPlayerEventsHandler instance not found...");
			}
			
			IPodPlayerEventsHandler.Instance.CheckIPodState();
		}
		
		Finish();
	}
}
