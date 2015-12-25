using UnityEngine;
using System.Collections;

public class KochavaBlackboardConfig : MonoBehaviour {

	[System.Serializable]
	public class KochavaPlatformConfig {
		public string appId;
	}

	public KochavaPlatformConfig iosConfig;
	public KochavaPlatformConfig googlePlayConfig;
	public KochavaPlatformConfig amazonConfig;
}
