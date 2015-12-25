using UnityEngine;
using System.Collections;

public class VersionLabel : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		string bundleVersion = PlatformUtilsManager.GetBundleVersion();
		string systemVersion = Language.Get("ABOUT_VERSION").Replace ("1.0.0", bundleVersion);
		gameObject.GetComponent<UILabel>().text = systemVersion;
	}
}
