using UnityEngine;
using System.Collections;

public abstract class ReferalStore : MonoBehaviour
{
	public delegate void ReferalStoreDelegate(ReferalStore rs);
	public event ReferalStoreDelegate OnReferalStoreClosed;

	public string appID{
		set{
			string aplicationID = value;
			_SetAplicationID(aplicationID);
		}
	}


	public ReferalStore()
	{

	}

	public virtual void ShowReferalView()
	{

	}

	protected virtual void _SetAplicationID(string appID)
	{

	}


	protected void _CallOnReferalStoreClosed()
	{
		if(OnReferalStoreClosed != null){
			OnReferalStoreClosed(this);
		}
	}
}
