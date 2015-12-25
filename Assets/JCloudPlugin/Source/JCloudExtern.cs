//
//  JCloudExtern.cs
//  iCloud for Unity
//
//  Copyright (c) 2011-2013 jemast software.
//

// DO NOT EDIT OR PLUGIN WILL STOP USING ICLOUD
#define JCLOUDPLUGIN_IOS
#define JCLOUDPLUGIN_OSX


using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


// FOR INTERNAL USE ONLY -- DO NOT CALL THESE METHODS DIRECTLY
[AddComponentMenu("")]
public class JCloudExtern {
	
	// Extern structs
	public struct JCloudDocumentVersionsInternal {
		public System.IntPtr versionsHash;
		public System.IntPtr versionsUniqueIdentifiers;
		public System.IntPtr versionsUniqueIdentifiersLengthes;
		public System.IntPtr versionsModificationDates;
		public System.IntPtr versionsIsCurrent;
		public int versionsCount;
	}
	
	// Trick for DllImport
#if UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX
	private const string importString = "jCloudPlugin_MacOS";
#elif UNITY_IPHONE && JCLOUDPLUGIN_IOS
	private const string importString = "__Internal";
#endif
	
#if ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))	
	// CLOUD DOCUMENT EXTERN FUNCTIONS
	[DllImport (importString)]
	public static extern int PrepareCloudItem(string path, bool directory);
	[DllImport (importString)]
	public static extern int DismissCloudItem(int uid);
	[DllImport (importString)]
	public static extern JCloudDocumentState GetCloudItemState(int uid);
	[DllImport (importString)]
	public static extern bool CreateOrOpenCloudItem(int uid);
	[DllImport (importString)]
	public static extern bool CreateCloudItem(int uid);
	[DllImport (importString)]
	public static extern bool OpenCloudItem(int uid);
	[DllImport (importString)]
	public static extern bool DeleteCloudItem(int uid, ref byte deleted);
	[DllImport (importString)]
	public static extern bool WriteCloudItemContents(byte[] contents, int length, int uid);
	[DllImport (importString)]
	public static extern int ReadCloudItemContents(int uid, out System.IntPtr bytes);
	[DllImport (importString)]
	public static extern bool SetPersistentDataPath(string path);
	[DllImport (importString)]
	public static extern bool GetCloudDirectoryPath(out System.IntPtr value);
	[DllImport (importString)]
	public static extern bool GetCloudItemExistence(int uid, ref byte exists);
	[DllImport (importString)]
	public static extern bool GetCloudItemModificationDate(int uid, ref long modificationDate);
	[DllImport (importString)]
	public static extern bool CopyCloudItem(int uid, string destinationPath, bool overwrite);
	[DllImport (importString)]
	public static extern bool MoveCloudItem(int uid, string destinationPath, bool overwrite);
	[DllImport (importString)]
	public static extern bool CloudItemHasConflictVersions(int uid, ref byte conflict);
	[DllImport (importString)]
	public static extern bool CloudItemFetchAllVersions(int uid, ref JCloudDocumentVersionsInternal versions);
	[DllImport (importString)]
	public static extern bool OpenCloudItemWithVersionIdentifier(int uid, byte[] versionIdentifier, int versionIdentifierLength);
	[DllImport (importString)]
	public static extern bool PickCloudItemWithVersionIdentifier(int uid, byte[] versionIdentifier, int versionIdentifierLength, string versionsHash);
	[DllImport (importString)]
	public static extern void SetStateChangeCallbackPointer(System.IntPtr pointer);
	[DllImport (importString)]
	public static extern void SetStatusChangeCallbackPointer(System.IntPtr pointer);
	[DllImport (importString)]
	public static extern void SetProgressChangeCallbackPointer(System.IntPtr pointer);
	[DllImport (importString)]
	public static extern void CloudMetadataSetCallbackPointer(System.IntPtr pointer);
	[DllImport (importString)]
	public static extern void CloudMetadataSetShouldMessage(bool message);
	
	// CLOUD DATA EXTERN FUNCTIONS
	[DllImport (importString)]
	public static extern void CloudDataSetInt(string key, int value);
	[DllImport (importString)]
	public static extern bool CloudDataGetInt(string key, ref int value);
	[DllImport (importString)]
	public static extern void CloudDataSetFloat(string key, float value);
	[DllImport (importString)]
	public static extern bool CloudDataGetFloat(string key, ref float value);
	[DllImport (importString)]
	public static extern void CloudDataSetString(string key, string value);
	[DllImport (importString)]
	public static extern bool CloudDataGetString(string key, out System.IntPtr value);
	[DllImport (importString)]
	public static extern bool CloudDataHasKey(string key);
	[DllImport (importString)]
	public static extern void CloudDataDeleteKey(string key);
	[DllImport (importString)]
	public static extern void CloudDataDeleteAll();
	[DllImport (importString)]
	public static extern void CloudDataSave();
	[DllImport (importString)]
	public static extern void CloudDataSetShouldMessage(bool message);
	[DllImport (importString)]
	public static extern void CloudDataSetCallbackPointer(System.IntPtr pointer);

	
	// CLOUD POLLING EXTERN FUNCTIONS
	[DllImport (importString)]
	public static extern bool GetUbiquitousContainerAvailability();
	[DllImport (importString)]
	public static extern bool GetUbiquitousStoreAvailability();
	[DllImport (importString)]
	public static extern void ResetUbiquityStatus();

	// MEMORY MANAGEMENT FUNCTIONS
	[DllImport (importString)]
	public static extern void FreeMemory(System.IntPtr pointer);
	
	// VARIOUS STUFF
	[DllImport (importString)]
	public static extern bool IsJailbroken();
#endif

}
