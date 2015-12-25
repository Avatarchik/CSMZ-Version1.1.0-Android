//
//  JCloudData.cs
//  iCloud for Unity
//
//  Copyright (c) 2011-2013 jemast software.
//

// DO NOT EDIT OR PLUGIN WILL STOP USING ICLOUD
#define JCLOUDPLUGIN_IOS
#define JCLOUDPLUGIN_OSX


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public enum JCloudDataChangeReason {
	JCloudDataServerChange,
	JCloudDataInitialSyncChange,
	JCloudDataQuotaViolationChange,
	JCloudDataAccountChange
}

public enum JCloudDataValueType {
	JCloudDataNull,
	JCloudDataInt,
	JCloudDataFloat,
	JCloudDataString
}

public struct JCloudDataExternalChange {
	public JCloudDataChangeReason Reason;
	
	public JCloudKeyValueChange[] ChangedKeyValues;
}

public struct JCloudKeyValueChange {
	public string Key;
	
	public JCloudDataValueType OldValueType;
	public object OldValue;
	
	public JCloudDataValueType NewValueType;
	public object NewValue;
}

[AddComponentMenu("")]
public class JCloudData : MonoBehaviour {
	
	public static bool AcceptJailbrokenDevices = true;
	
	public static void SetInt(string key, int value) {
		// Set int
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			JCloudExtern.CloudDataSetInt(key, value);
		} else
#endif
			PlayerPrefs.SetInt(key, value);
	}
		
	public static int GetInt(string key, int defaultValue) {
		// Get int, if key doesn't exist, value will keep the defaultValue
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			int value = defaultValue;
			JCloudExtern.CloudDataGetInt(key, ref value);
			return value;
		} else
#endif
			return PlayerPrefs.GetInt(key, defaultValue);
	}
	
	public static int GetInt(string key) {
		// Get int, if key doesn't exist, will return 0
		return JCloudData.GetInt(key, 0);
	}

	public static void SetFloat(string key, float value) {
		// Set float
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			JCloudExtern.CloudDataSetFloat(key, value);
		} else
#endif
			PlayerPrefs.SetFloat(key, value);
	}
	
	public static float GetFloat(string key, float defaultValue) {
		// Get float, if key doesn't exist, value will keep the defaultValue
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			float value = defaultValue;
			JCloudExtern.CloudDataGetFloat(key, ref value);
			return value;
		} else
#endif
			return PlayerPrefs.GetFloat(key, defaultValue);
	}
	
	public static float GetFloat(string key) {
		// Get float, if key doesn't exist, will return 0.0f
		return JCloudData.GetFloat(key, 0.0f);
	}
	
	public static void SetString(string key, string value) {
		// Set string
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			JCloudExtern.CloudDataSetString(key, value);
		} else
#endif
			PlayerPrefs.SetString(key, value);
	}
	
	public static string GetString(string key, string defaultValue) {
		// Get string, if key doesn't exist, value will keep the defaultValue
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			string value = defaultValue;
			System.IntPtr valuePtr;
			if (JCloudExtern.CloudDataGetString(key, out valuePtr)) {
				value = Marshal.PtrToStringAnsi(valuePtr);
				JCloudExtern.FreeMemory(valuePtr);
			}
			
			return value;
		} else
#endif
			return PlayerPrefs.GetString(key, defaultValue);
	}
	
	public static string GetString(string key) {
		// Get string, if key doesn't exist, will return ""
		return JCloudData.GetString(key, "");
	}
	
	public static bool HasKey(string key) {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			return JCloudExtern.CloudDataHasKey(key);
		} else
#endif
			return PlayerPrefs.HasKey(key);
	}
	
	public static void DeleteKey(string key) {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			JCloudExtern.CloudDataDeleteKey(key);
		} else
#endif
			PlayerPrefs.DeleteKey(key);
	}
	
	public static void DeleteAll() {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			JCloudExtern.CloudDataDeleteAll();
		} else
#endif
			PlayerPrefs.DeleteAll();
	}
	
	public static void Save() {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			JCloudExtern.CloudDataSave();
		} else
#endif
			PlayerPrefs.Save();
	}
	
	public static bool PollCloudDataAvailability() {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			return JCloudExtern.GetUbiquitousStoreAvailability();
		} else
#endif
			return false;
	}
	
	//////////////////////////////////////////////
	// KEY-VALUE STORE CHANGES MONITORING
	
	public static bool RegisterCloudDataExternalChanges(Component componentOrGameObject) {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Failsafe
			if (componentOrGameObject == null)
				return false;
			
			// Check manager status
			JCloudManager.CheckManagerStatus();
			
			// Add
			JCloudManager.AddKeyValueStoreRegisteredComponent(componentOrGameObject);
			
			return true;
		}
#endif
	
		return false;
	}
	
	public static bool UnregisterCloudDataExternalChanges(Component componentOrGameObject) {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Failsafe
			if (componentOrGameObject == null)
				return false;
	
			// Check manager status
			JCloudManager.CheckManagerStatus();
			
			// Remove
			JCloudManager.RemoveKeyValueStoreRegisteredComponent(componentOrGameObject);
		
			return true;
		}
#endif
	
		return false;
	}
}
