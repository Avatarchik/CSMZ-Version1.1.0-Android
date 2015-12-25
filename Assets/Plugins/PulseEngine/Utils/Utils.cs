#define USE_OLD_METHOD_ON_IOS

using UnityEngine;
using System.Collections;

namespace PulseEngine
{
	public class Utils 
	{
		public const string deviceGUIDKey = "DeviceGUID";
		public static System.DateTime baseDate = new System.DateTime(2013, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		
	
		public static long TimeSeconds()
		{
			return (long)System.DateTime.Now.Subtract(baseDate).TotalSeconds;
		}	
		
		/// <summary>
		/// Generates and stores a unique GUID for the current device.
		/// </summary>
		/// <returns>
		/// The device GUI.
		/// </returns>
		public static string GetDeviceGUID()
		{
#if USE_OLD_METHOD_ON_IOS && UNITY_IPHONE
			return SystemInfo.deviceUniqueIdentifier;
#else
			string referenceGuid = System.Guid.Empty.ToString().Replace("-", "");
			string resultGuid = null;
			bool generateNewGuid = false;
			
			if (PlayerPrefs.HasKey(deviceGUIDKey))
			{
				string currentGuid = PlayerPrefs.GetString(deviceGUIDKey);
				if (currentGuid.Length != referenceGuid.Length) 
				{
					Debug.LogWarning("Last cached GUID has invalid format: " + currentGuid);
					generateNewGuid = true;
				}
				else {
					resultGuid = currentGuid;
				}
			}
			else {
				generateNewGuid = true;
			}
			
			if (generateNewGuid)
			{
				resultGuid = System.Guid.NewGuid().ToString().ToUpper().Replace("-", "");
				PlayerPrefs.SetString(deviceGUIDKey, resultGuid);
				PlayerPrefs.Save();
			}
			
			return resultGuid;
#endif
		}
		
		public static bool EventContainsMethod(System.Delegate _event, string methodName, Object target)
		{
			System.Delegate[] calls = _event.GetInvocationList();

			foreach (System.Delegate call in calls)
			{
				Debug.Log(call.Method.Name + "   " + call.Target);
				if (call.Target == target && call.Method.Name == methodName)
				{
					return true;
				}
			}
			
			return false;
		}
	}
}