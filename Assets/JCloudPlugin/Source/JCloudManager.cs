//
//  JCloudManager.cs
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

// Document states
public enum JCloudDocumentState { JCloudDocumentStateClosed = 0, JCloudDocumentStateOpening, JCloudDocumentStateNormal, JCloudDocumentStateInConflict, JCloudDocumentStateEditingDisabled };


// FOR INTERNAL USE ONLY -- DO NOT CALL THESE METHODS DIRECTLY
[AddComponentMenu("")]
public class JCloudManager : MonoBehaviour {
	
	// This should detect only valid platforms with upward compatibility unless they break the pattern
#if ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
	protected static bool platformCheck = false;
	protected static bool cloudCompatible = false;
	public static bool PlatformIsCloudCompatible() {
		if (platformCheck == false) {
			cloudCompatible = (SystemInfo.operatingSystem.Contains("iPhone OS") && !SystemInfo.operatingSystem.Contains("iPhone OS 3")  && !SystemInfo.operatingSystem.Contains("iPhone OS 4")) || (SystemInfo.operatingSystem.Contains("Mac OS X") && !SystemInfo.operatingSystem.Contains("Mac OS X 10.4") && !SystemInfo.operatingSystem.Contains("Mac OS X 10.5") && !SystemInfo.operatingSystem.Contains("Mac OS X 10.6"));
			platformCheck = true;
		}
		
		return cloudCompatible;
	}
#else
	public static bool PlatformIsCloudCompatible() {
		return false;
	}
#endif
	
	// Fallback path for local storage
	static public string JCloudDocumentFallbackPath = Application.persistentDataPath + "/";
	
#if UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX
	public static bool persistentDataPathIsSet = false;
#endif
	
	public struct DocumentStatus {
		public bool success;
		public JCloudDocumentError? error;
	}
	
	struct DocumentLock {
		public int index, count;
		public DocumentLock(int index, int count) {
	    	this.index = index;
	    	this.count = count;
	    }
	}
	
	public static Dictionary<int, JCloudDocumentState> documentState = new Dictionary<int, JCloudDocumentState>();
	public static Dictionary<int, DocumentStatus> documentStatus = new Dictionary<int, DocumentStatus>();
	public static Dictionary<int, float> documentProgress = new Dictionary<int, float>();
	public static Dictionary<int, int> documentAccess = new Dictionary<int, int>();
	static Dictionary<int, DocumentLock> documentLock = new Dictionary<int, DocumentLock>();
	
	// This will ensure our shared manager exists
	protected static JCloudManager sharedManager = null;
	protected static System.Object checkLock = new System.Object();
	public static void CheckManagerStatus() {
			if (sharedManager == null) {
				// Actually we recheck the null status inside the lock, so if it's not null we avoided the lock
				lock (checkLock) {
					if (sharedManager == null) {
						GameObject sharedManagerObject = new GameObject("JCloudManager", typeof(JCloudManager)) as GameObject;
						sharedManager = sharedManagerObject.GetComponent<JCloudManager>();
						DontDestroyOnLoad(sharedManagerObject);
					}
				}
			}
	}
	
	// Get our singleton
	public static JCloudManager GetSharedManager() {
		if (sharedManager == null) // This will avoid a function call most of the time even if the function will check itself
			CheckManagerStatus();
		return sharedManager;
	}
	
	// Internal method to monitor document access & status
	public static void WatchDocument(int uid) {
		lock (documentState) {
			if (!documentAccess.ContainsKey(uid))
				documentAccess.Add(uid, 1);
			else
				documentAccess[uid]++;
			
			if (!documentState.ContainsKey(uid))
				documentState.Add(uid, JCloudDocumentState.JCloudDocumentStateOpening);
		}
	}
	
	// Internal method to retrieve document status
	public static JCloudDocumentState GetDocumentState(int uid) {
		lock (documentState) {
			if (documentState.ContainsKey(uid))
				return documentState[uid];
			else
				return JCloudDocumentState.JCloudDocumentStateClosed;
		}
	}
	
	// Internal method to stop monitoring document status & update access
	public static void UnwatchDocument(int uid) {
		lock (documentState) {
			if (documentAccess.ContainsKey(uid)) {
				documentAccess[uid]--;
			
				if (documentStatus.ContainsKey(uid))
					documentStatus.Remove(uid);
				
				if (documentProgress.ContainsKey(uid))
					documentProgress.Remove(uid);
				
				if (documentAccess[uid] == 0) {
					if (documentState.ContainsKey(uid))
						documentState.Remove(uid);
					
					documentAccess.Remove(uid);
				}
			} else {
				if (documentState.ContainsKey(uid))
					documentState.Remove(uid);
				if (documentStatus.ContainsKey(uid))
					documentStatus.Remove(uid);
				if (documentProgress.ContainsKey(uid))
					documentProgress.Remove(uid);
			}
		}
	}
	
	// Internal method to get a status
	public static bool GetDocumentStatus(int uid, out DocumentStatus? status) {
		lock (documentState) {
			if (documentStatus.ContainsKey(uid)) {
				status = documentStatus[uid];
				return true;
			}
		}
			
		status = null;
		return false;
	}
	
	// Internal method to get a progress
	public static bool GetDocumentProgress(int uid, out float progress) {
		lock (documentState) {
			if (documentProgress.ContainsKey(uid)) {
				progress = documentProgress[uid];
				return true;
			}
		}
			
		progress = 0f;
		return false;
	}
	
	// Internal method to get a lock access on a document
	public static int GetDocumentLock(int uid) {
		lock (documentLock) {
			if (documentLock.ContainsKey(uid)) {
				DocumentLock dLock = documentLock[uid];
				int value = dLock.count++;
				documentLock[uid] = dLock;
				return value;
			}
			
			documentLock.Add(uid, new DocumentLock(0, 1) );
			return 0;
		}
	}
	
	// Internal method to check a lock access on a document
	public static bool CheckDocumentLock(int uid, int lockId) {
		lock (documentLock) {
			if (documentLock.ContainsKey(uid))
				return (lockId == documentLock[uid].index);
			else
				return false;
		}		
	}
	
	// Internal method to release lock access on a document
	public static void ReleaseDocumentLock(int uid) {
		lock (documentLock) {
			if (documentLock.ContainsKey(uid)) {
				DocumentLock dLock = documentLock[uid];
				dLock.index++;
				
				if (dLock.index == dLock.count)
					documentLock.Remove(uid);
				else
					documentLock[uid] = dLock;
			}
		}
	}
	
// TRICK FOR UNMANAGED TO MANAGED CALLBACK --- DESKTOP ONLY
#if UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX
	public delegate void DocumentStateDidChangeDelegate(int uid, JCloudDocumentState state);
	protected GCHandle stateChangeCallbackHandle;
	protected System.IntPtr stateChangeCallbackPointer;
	
	public delegate void DocumentStatusDidChangeDelegate(int uid, int success, int error);
	protected GCHandle statusChangeCallbackHandle;
	protected System.IntPtr statusChangeCallbackPointer;
	
	public delegate void DocumentProgressDidChangeDelegate(int uid, float progress);
	protected GCHandle progressChangeCallbackHandle;
	protected System.IntPtr progressChangeCallbackPointer;
	
	public delegate void KeyValueStoreDidChangeExternallyDelegate(string message);
	protected GCHandle keyValueStoreChangeCallbackHandle;
	protected System.IntPtr keyValueStoreChangeCallbackPointer;
	
	public delegate void FileDidChangeDelegate(string message);
	protected GCHandle fileChangeChangeCallbackHandle;
	protected System.IntPtr fileChangeCallbackPointer;
	
	// Constructor
	public JCloudManager() {
		// Make sure state change callback pointer was set
		if (stateChangeCallbackPointer == System.IntPtr.Zero) {
			DocumentStateDidChangeDelegate stateChangeCallback = DocumentStateDidChange;
			stateChangeCallbackHandle = GCHandle.Alloc(stateChangeCallback);
			stateChangeCallbackPointer = Marshal.GetFunctionPointerForDelegate(stateChangeCallback);
			JCloudExtern.SetStateChangeCallbackPointer(stateChangeCallbackPointer);
		}
		
		// Make sure status change callback pointer was set
		if (statusChangeCallbackPointer == System.IntPtr.Zero) {
			DocumentStatusDidChangeDelegate statusChangeCallback = DocumentStatusDidChange;
			statusChangeCallbackHandle = GCHandle.Alloc(statusChangeCallback);
			statusChangeCallbackPointer = Marshal.GetFunctionPointerForDelegate(statusChangeCallback);
			JCloudExtern.SetStatusChangeCallbackPointer(statusChangeCallbackPointer);
		}
		
		// Make sure progress change callback pointer was set
		if (progressChangeCallbackPointer == System.IntPtr.Zero) {
			DocumentProgressDidChangeDelegate progressChangeCallback = DocumentProgressDidChange;
			progressChangeCallbackHandle = GCHandle.Alloc(progressChangeCallback);
			progressChangeCallbackPointer = Marshal.GetFunctionPointerForDelegate(progressChangeCallback);
			JCloudExtern.SetProgressChangeCallbackPointer(progressChangeCallbackPointer);
		}
		
		// Make sure key-value store change callback pointer was set
		if (keyValueStoreChangeCallbackPointer == System.IntPtr.Zero) {
			KeyValueStoreDidChangeExternallyDelegate keyValueStoreChangeCallback = KeyValueStoreDidChangeExternally;
			keyValueStoreChangeCallbackHandle = GCHandle.Alloc(keyValueStoreChangeCallback);
			keyValueStoreChangeCallbackPointer = Marshal.GetFunctionPointerForDelegate(keyValueStoreChangeCallback);
			JCloudExtern.CloudDataSetCallbackPointer(keyValueStoreChangeCallbackPointer);
		}
		
		// Make sure file change callback pointer was set
		if (fileChangeCallbackPointer == System.IntPtr.Zero) {
			FileDidChangeDelegate fileChangeCallback = FileDidChange;
			fileChangeChangeCallbackHandle = GCHandle.Alloc(fileChangeCallback);
			fileChangeCallbackPointer = Marshal.GetFunctionPointerForDelegate(fileChangeCallback);
			JCloudExtern.CloudMetadataSetCallbackPointer(fileChangeCallbackPointer);
		}
		
		// First run will set persistent data path
		if (JCloudManager.persistentDataPathIsSet == false) {
			JCloudExtern.SetPersistentDataPath(JCloudManager.JCloudDocumentFallbackPath);
			JCloudManager.persistentDataPathIsSet = true;
		}
	}
	
	// Destructor
	~JCloudManager() {
		stateChangeCallbackHandle.Free();
		statusChangeCallbackHandle.Free();
		progressChangeCallbackHandle.Free();
		keyValueStoreChangeCallbackHandle.Free();
		fileChangeChangeCallbackHandle.Free();
	}
#endif
	
	
#if UNITY_IPHONE && JCLOUDPLUGIN_IOS
	void DocumentStateDidChange(string message) {
		lock (documentState) {
			// Split message for data mining
			string[] splitMessage = message.Split('.');
			
			// Set the new document state
			int documentUID = int.Parse(splitMessage[0]);
			if (documentState.ContainsKey(documentUID))			
				documentState[documentUID] = (JCloudDocumentState)int.Parse(splitMessage[1]);
		}
	}
	
	void DocumentStatusDidChange(string message) {
		lock (documentState) {
			// Split message for data mining
			string[] splitMessage = message.Split('.');
			
			// Set the new document status
			int documentUID = int.Parse(splitMessage[0]);
			int successValue = int.Parse(splitMessage[1]);
			int errorValue = int.Parse(splitMessage[2]);
			if (documentState.ContainsKey(documentUID)) {
				if (documentStatus.ContainsKey(documentUID)) {
					documentStatus[documentUID] = new DocumentStatus { success = successValue != 0, error = errorValue != -1 ? (JCloudDocumentError)errorValue : (JCloudDocumentError?)null };
				} else {
					documentStatus.Add(documentUID, new DocumentStatus { success = successValue != 0, error = errorValue != -1 ? (JCloudDocumentError)errorValue : (JCloudDocumentError?)null });
				}
			}
		}
	}
	
	void DocumentProgressDidChange(string message)
	{
		lock (documentState) {
			// Split message for data mining
			string[] splitMessage = message.Split(';');
			
			// Set the new document progress
			int documentUID = int.Parse(splitMessage[0]);
			if (documentState.ContainsKey(documentUID)) {
				if (documentProgress.ContainsKey(documentUID))
					documentProgress[documentUID] = float.Parse(splitMessage[1]);
				else
					documentProgress.Add(documentUID, float.Parse(splitMessage[1]));
			}
		}
	}
#endif
	
#if UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX
	void DocumentStateDidChange(int uid, JCloudDocumentState state) {		
		lock (documentState) {
			// Set the new document state
			if (documentState.ContainsKey(uid))			
				documentState[uid] = state;
		}
	}
	
	void DocumentStatusDidChange(int uid, int successValue, int errorValue) {		
		lock (documentState) {
			// Set the new document status
			if (documentState.ContainsKey(uid)) {
				if (documentStatus.ContainsKey(uid)) {
					documentStatus[uid] = new DocumentStatus { success = successValue != 0, error = errorValue != -1 ? (JCloudDocumentError)errorValue : (JCloudDocumentError?)null };
				} else {
					documentStatus.Add(uid, new DocumentStatus { success = successValue != 0, error = errorValue != -1 ? (JCloudDocumentError)errorValue : (JCloudDocumentError?)null });
				}
			}
		}
	}
	
	void DocumentProgressDidChange(int uid, float progress) {		
		lock (documentState) {
			// Set the new document progress
			if (documentState.ContainsKey(uid)) {
				if (documentProgress.ContainsKey(uid))			
					documentProgress[uid] = progress;
				else
					documentProgress.Add(uid, progress);
			}
		}
	}
#endif
	
	
	
	
	// Internal helper for converting bytes array from unmanaged code to string array
	public static string[] PathListFromBytes(byte[] bytes, bool directories) {
		if (bytes == null)
			return null;
		
		uint itemCount = System.BitConverter.ToUInt32(bytes, 0);
		System.Collections.Generic.List<string> pathList = new System.Collections.Generic.List<string>();
		
		// Process each item
		uint offset = sizeof(System.UInt32);
		for (uint i = 0; i < itemCount; i++) {
			// Get if it is a directory
			bool itemIsDirectory = (bool)System.BitConverter.ToBoolean(bytes, (int)offset);
			offset += sizeof(bool);
			
			// Get the path length
			uint pathLength = System.BitConverter.ToUInt32(bytes, (int)offset);
			offset += sizeof(System.UInt32);
			
			// Add to list if item is of the category we desire
			if (itemIsDirectory == directories) {
				System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
				pathList.Add(encoder.GetString(bytes, (int)offset, (int)pathLength));
			}
			offset += pathLength;
		}
		
		
		// Return path list array
		return pathList.ToArray();
	}
	

#if !UNITY_WEBPLAYER && !UNITY_METRO
	// Copy directory structure recursively -- code from http://www.codeproject.com/Articles/3210/Function-to-copy-a-directory-to-another-place-noth
    public static void CopyDirectory(string src, string dst) {
		if (dst[dst.Length-1] != System.IO.Path.DirectorySeparatorChar) 
		    dst += System.IO.Path.DirectorySeparatorChar;
		
		if (!System.IO.Directory.Exists(dst))
			System.IO.Directory.CreateDirectory(dst);
		
		string[] files = System.IO.Directory.GetFileSystemEntries(src);
		
		for (int i = 0; i < files.Length; i++) {
		    if(System.IO.Directory.Exists(files[i])) // Sub directories
		        CopyDirectory(files[i], dst + System.IO.Path.GetFileName(files[i]));
		    else // Files in directory
		        System.IO.File.Copy(files[i], dst + System.IO.Path.GetFileName(files[i]), true);
	    }
	}
#endif

#if ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))	
	private static List<Component> keyValueStoreDataRegisteredComponents = new List<Component>();
	
	public static void AddKeyValueStoreRegisteredComponent(Component component)
	{
		lock (keyValueStoreDataRegisteredComponents)
		{
			if (!keyValueStoreDataRegisteredComponents.Contains(component))
				keyValueStoreDataRegisteredComponents.Add(component);
			
			if (keyValueStoreDataRegisteredComponents.Count == 1)
				JCloudExtern.CloudDataSetShouldMessage(true);
		}
	}
	
	public static void RemoveKeyValueStoreRegisteredComponent(Component component)
	{
		lock (keyValueStoreDataRegisteredComponents)
		{
			if (keyValueStoreDataRegisteredComponents.Contains(component))
				keyValueStoreDataRegisteredComponents.Remove(component);
			
			if (keyValueStoreDataRegisteredComponents.Count == 0)
				JCloudExtern.CloudDataSetShouldMessage(false);
		}
	}

	// This is our callback for monitoring key-value store changes
	void KeyValueStoreDidChangeExternally(string keyValues) {
		// No need to keep processing if no one is watching
		lock (keyValueStoreDataRegisteredComponents)
		{
			if (keyValueStoreDataRegisteredComponents.Count == 0)
				return;
		}
		
		string[] keyComponents = keyValues.Split('.');
		
		// Get heading parameters
		int reason = int.Parse(keyComponents[0]);
		int keyCount = int.Parse(keyComponents[1]);
		int[] keyLength = new int[keyCount];
		int[] oldValueType = new int[keyCount];
		int[] oldValueLength = new int[keyCount];
		int[] newValueType = new int[keyCount];
		int[] newValueLength = new int[keyCount];
		
		int offset = keyComponents[0].Length + keyComponents[1].Length + 2;
		
		// Get types & lengthes
		for (int i = 0; i < keyCount; i++)
		{
			keyLength[i] = int.Parse(keyComponents[i*5 + 2]);
			oldValueType[i] = int.Parse(keyComponents[i*5 + 3]);
			oldValueLength[i] = int.Parse(keyComponents[i*5 + 4]);
			newValueType[i] = int.Parse(keyComponents[i*5 + 5]);
			newValueLength[i] = int.Parse(keyComponents[i*5 + 6]);
			offset += keyComponents[i*5 + 2].Length + keyComponents[i*5 + 3].Length + keyComponents[i*5 + 4].Length + keyComponents[i*5 + 5].Length + keyComponents[i*5 + 6].Length + 5;
		}
		
		// Get keys & values
		JCloudKeyValueChange[] consolidatedKeyValueChangeArray = new JCloudKeyValueChange[keyCount];
		for (int i = 0; i < keyCount; i++) {
			consolidatedKeyValueChangeArray[i] = new JCloudKeyValueChange();
			
			consolidatedKeyValueChangeArray[i].Key = keyValues.Substring(offset, keyLength[i]);
			offset += keyLength[i];
			
			consolidatedKeyValueChangeArray[i].OldValueType = (JCloudDataValueType)oldValueType[i];
			switch (consolidatedKeyValueChangeArray[i].OldValueType) {
			case JCloudDataValueType.JCloudDataInt:
				consolidatedKeyValueChangeArray[i].OldValue = int.Parse(keyValues.Substring(offset, oldValueLength[i]));
				break;
			case JCloudDataValueType.JCloudDataFloat:
				consolidatedKeyValueChangeArray[i].OldValue = float.Parse(keyValues.Substring(offset, oldValueLength[i]));
				break;
			case JCloudDataValueType.JCloudDataString:
				consolidatedKeyValueChangeArray[i].OldValue = keyValues.Substring(offset, oldValueLength[i]);
				break;
			default:
				consolidatedKeyValueChangeArray[i].OldValue = null;
				break;
			}
			offset += (oldValueType[i] != 0) ? oldValueLength[i] : 0;
			
			consolidatedKeyValueChangeArray[i].NewValueType = (JCloudDataValueType)newValueType[i];
			switch (consolidatedKeyValueChangeArray[i].NewValueType) {
			case JCloudDataValueType.JCloudDataInt:
				consolidatedKeyValueChangeArray[i].NewValue = int.Parse(keyValues.Substring(offset, newValueLength[i]));
				break;
			case JCloudDataValueType.JCloudDataFloat:
				consolidatedKeyValueChangeArray[i].NewValue = float.Parse(keyValues.Substring(offset, newValueLength[i]));
				break;
			case JCloudDataValueType.JCloudDataString:
				consolidatedKeyValueChangeArray[i].NewValue = keyValues.Substring(offset, newValueLength[i]);
				break;
			default:
				consolidatedKeyValueChangeArray[i].NewValue = null;
				break;
			}
			offset += (newValueType[i] != 0) ? newValueLength[i] : 0;
		}
		
		// Broadcast to all registered components
		lock (keyValueStoreDataRegisteredComponents) {
			for (int i = 0; i < keyValueStoreDataRegisteredComponents.Count; i++) {
				if (keyValueStoreDataRegisteredComponents[i] != null)
					keyValueStoreDataRegisteredComponents[i].SendMessage("JCloudDataDidChangeExternally", new JCloudDataExternalChange { Reason = (JCloudDataChangeReason)reason, ChangedKeyValues = consolidatedKeyValueChangeArray });
			}
		}
	}
	
	
	
	private static List<Component> fileChangeRegisteredComponents = new List<Component>();
	
	public static void AddFileChangeRegisteredComponent(Component component)
	{
		lock (fileChangeRegisteredComponents)
		{
			if (!fileChangeRegisteredComponents.Contains(component))
				fileChangeRegisteredComponents.Add(component);
			
			if (fileChangeRegisteredComponents.Count == 1)
				JCloudExtern.CloudMetadataSetShouldMessage(true);
		}
	}
	
	public static void RemoveFileChangeRegisteredComponent(Component component)
	{
		lock (fileChangeRegisteredComponents)
		{
			if (fileChangeRegisteredComponents.Contains(component))
				fileChangeRegisteredComponents.Remove(component);
			
			if (fileChangeRegisteredComponents.Count == 0)
				JCloudExtern.CloudMetadataSetShouldMessage(false);
		}
	}
	
	void FileDidChange(string message) {
		// Split message for data mining
		string[] splitMessage = message.Split(':');
		
		int changeCount = int.Parse(splitMessage[0]);
		
		JCloudDocumentExternalChange[] changes = new JCloudDocumentExternalChange[changeCount];
		for (int i = 0; i < changeCount; i++) {
			changes[i] = new JCloudDocumentExternalChange();
			changes[i].path = splitMessage[i*2+1];
			changes[i].change = (JCloudDocumentChangeType)int.Parse(splitMessage[i*2+2]);
		}
	
		lock (fileChangeRegisteredComponents)
		{
			for (int i = 0; i < fileChangeRegisteredComponents.Count; i++) {
				if (fileChangeRegisteredComponents[i] != null)
					fileChangeRegisteredComponents[i].SendMessage("JCloudDocumentDidChangeExternally", changes);
			}			
		}
	}
#endif
	
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
	void OnApplicationFocus(bool focus) {
		if (focus && JCloudManager.PlatformIsCloudCompatible()) {
			JCloudExtern.ResetUbiquityStatus();
		}
	}
	
	void OnApplicationPause(bool pause) {
		if (!pause && JCloudManager.PlatformIsCloudCompatible()) {
			JCloudExtern.ResetUbiquityStatus();
		}
	}
#endif
}
