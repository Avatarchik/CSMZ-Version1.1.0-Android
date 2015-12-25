using UnityEngine;
using System.Collections;

public class ManaStoreButton : UIControl {

	public PlayMakerFSM fsm;
	public string sendEvent = "ManaStore";

	void OnClick()
	{
		if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;
		if(ExchangeUIFlag.getInstance().m_bShow==false)
		{
			return;
		}
		ShopUINotifyer.getInstance ().IndexUI = 1;
		fsm.SendEvent(sendEvent);
	}
}
