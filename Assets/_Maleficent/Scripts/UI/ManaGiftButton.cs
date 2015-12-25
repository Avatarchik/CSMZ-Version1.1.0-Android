using UnityEngine;
using System.Collections;

public class ManaGiftButton : UIControl
{

	public PlayMakerFSM fsm;
	public string sendEvent = "GameGiftBegin";

	void OnClick()
	{
		if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;

		fsm.SendEvent(sendEvent);
	}
}
