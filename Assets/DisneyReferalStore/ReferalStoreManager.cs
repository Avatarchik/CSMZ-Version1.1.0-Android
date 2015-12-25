using UnityEngine;
using System.Collections;

public class ReferalStoreManager 
{
	private static ReferalStoreManager instance = null;

	private ReferalStore _referalStore = null;

	public static ReferalStoreManager Instance {
		get {
			if (instance == null) {
				instance = new ReferalStoreManager();
			}
			return instance;
		}
	}

	public ReferalStore referalStore {
		get{
			if (_referalStore == null) 
			{
				GameObject g = new GameObject("ReferalStore");
#if UNITY_IPHONE && !UNIYT_EDITOR
				_referalStore =  g.AddComponent< IOSReferalStore >();
#elif UNITY_ANDROID && !UNIYT_EDITOR
				_referalStore =  g.AddComponent< AndroidReferalStore >();
#else
				_referalStore =  g.AddComponent< FakeReferalStore >();
#endif

			}
			return _referalStore;
		}
	}
}
