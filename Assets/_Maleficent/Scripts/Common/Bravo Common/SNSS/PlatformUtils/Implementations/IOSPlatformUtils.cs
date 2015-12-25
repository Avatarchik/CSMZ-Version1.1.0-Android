using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


#if (UNITY_IPHONE) && (!UNITY_EDITOR)
public class IOSPlatformUtils : IPlatformUtils {

	[DllImport ("__Internal")]
	private static extern string BundleManager_getBundleVersion();
	[DllImport ("__Internal")]
	private static extern string BundleManager_getBundleID();

	public override string BundleVersion() {
		return BundleManager_getBundleVersion();
	}

	public override string BundleID() {
		return BundleManager_getBundleID();
	}
}
#endif
