
using JsonFx.Json;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Disney.DMOAnalytics.Framework
{
	public class DMOAnalyticsHelper
	{
		
		public static bool isDebugEnvLog = false;
		public static void Log(string msg) {
				if(isDebugEnvLog) {
					Debug.Log(msg);
				}
			}
		public static bool ICanUseNetwork = true;
		public static bool RestrictedTracking = false;
		
		public static string GetStringFromDictionary (Dictionary<string, object> dictData)
		{
			string jsonStr = "";
			if (dictData != null)
			{
				jsonStr = JsonWriter.Serialize(dictData);
				Debug.Log("JsonStr "+ jsonStr);
			}
			
			return jsonStr;
		}
	}
}