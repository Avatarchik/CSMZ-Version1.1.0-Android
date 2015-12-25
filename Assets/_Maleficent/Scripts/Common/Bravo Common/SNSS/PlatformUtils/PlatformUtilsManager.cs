using UnityEngine;
using System.Collections;

public class PlatformUtilsManager {

	private static IPlatformUtils platformUtils = null;
	private static IPlatformUtils PlatformUtils {
		get {
			if (platformUtils == null) {
#if (UNITY_IPHONE) && (!UNITY_EDITOR)
				platformUtils = new IOSPlatformUtils();
#elif (UNITY_ANDROID) && (!UNITY_EDITOR)
				platformUtils = new AndroidPlatformUtils();
#else
				platformUtils = new FakePlatformUtils();
#endif
			}
			return platformUtils;
		}	
	}

	public static string GetBundleVersion() {
		return PlatformUtils.BundleVersion();
	}
	
	public static string GetBundleID() {
		return PlatformUtils.BundleID();
	}
}
