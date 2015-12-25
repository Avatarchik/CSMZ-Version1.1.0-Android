using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class IPodPlayerEventsBinding
{
		
#region iOS native interface
#if !UNITY_EDITOR && UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern bool iPodPlayerEvents_IsIPodPlaying();
	
	[DllImport ("__Internal")]
	private static extern float iPodPlayerEvents_HardwareVolume();
#endif	
#endregion
	
	public static bool IsIPodPlaying() 
	{
		#if !UNITY_EDITOR && UNITY_IPHONE
			return iPodPlayerEvents_IsIPodPlaying();
		#else
			return false;
		#endif
	}
	
	public static float HardwareVolume() 
	{
		#if !UNITY_EDITOR && UNITY_IPHONE
			return iPodPlayerEvents_HardwareVolume();
		#else
			return 0.0f;
		#endif
	}
}