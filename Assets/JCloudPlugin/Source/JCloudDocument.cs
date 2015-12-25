//
//  JCloudDocument.cs
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
using System;
using System.IO;

public class JCloudDocumentOperation {
	public float progress;
	public bool success;
	public object result;
	public bool finished;
	public JCloudDocumentError? error;
}

public struct JCloudDocumentVersions {
	public string versionsHash;
	public JCloudDocumentVersionMetadata[] versionsMetadata;
}

public struct JCloudDocumentVersionMetadata {
	public byte[] uniqueIdentifier;
	public System.DateTime modificationDate;
	public bool isCurrent;
}

public enum JCloudDocumentError {
	PluginError = 0,
	InvalidArguments,
	DocumentNotFound,
	OverwriteError,
	NativeError,
	CloudUnavailable,
	InvalidVersionIdentifier,
	InvalidVersionsHash,
	DownloadTimeout,
	InvalidPlatform
}

public struct JCloudDocumentExternalChange {
	public string path;
	public JCloudDocumentChangeType change;
}

public enum JCloudDocumentChangeType {
	Added = 0,
	Changed,
	Removed
}

[AddComponentMenu("")]
public class JCloudDocument : MonoBehaviour {
	
	
	public static bool AcceptJailbrokenDevices = true;
	
	// Helper function for generic cloud document creating & dismissing
#if ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
	protected static int NewCloudDocument(string path, bool isDirectory) {
		// Check manager status
		JCloudManager.CheckManagerStatus();
		
		// Prepare a cloud file and get its unique identifier
		int newDocumentUID = JCloudExtern.PrepareCloudItem(path, isDirectory);
		
		// Add this UID to our document state watch dictionary
		JCloudManager.WatchDocument(newDocumentUID);
		
		return newDocumentUID;
	}
	
	protected static void DismissCloudDocument(int uid) {
		// Remove this document from our state watch dictionary
		JCloudManager.UnwatchDocument(uid);
		
		// We can now unlock the document
		JCloudManager.ReleaseDocumentLock(uid);
		
		// Ask plugin to dismiss
		JCloudExtern.DismissCloudItem(uid);
	}
#endif
	
	// Determines if a file exists at path
	public static JCloudDocumentOperation FileExists(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileExistsOperation(path, operation));
		return operation;
	}
	
	// Deletes file at path
	public static JCloudDocumentOperation FileDelete(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileDeleteOperation(path, operation));
		return operation;
	}
	
	// Writes bytes in the file at path
	public static JCloudDocumentOperation FileWriteAllBytes(string path, byte[] bytes) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileWriteAllBytesOperation(path, bytes, operation));
		return operation;
	}
	
	// Reads bytes from file at path
	public static JCloudDocumentOperation FileReadAllBytes(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileReadAllBytesOperation(path, operation));
		return operation;
	}
	
	// Get a file modification date at path
	public static JCloudDocumentOperation FileModificationDate(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileModificationDateOperation(path, operation));
		return operation;
	}
	
	// Copy a file
	public static JCloudDocumentOperation FileCopy(string sourcePath, string destinationPath, bool overwrite) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileCopyOperation(sourcePath, destinationPath, overwrite, operation));
		return operation;
	}
	
	// Move a file
	public static JCloudDocumentOperation FileMove(string sourcePath, string destinationPath, bool overwrite) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileMoveOperation(sourcePath, destinationPath, overwrite, operation));
		return operation;
	}
	
	// Determines if a file has versions 
	public static JCloudDocumentOperation FileHasConflictVersions(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileHasConflictVersionsOperation(path, operation));
		return operation;
	}
	
	// Fetch metadata about all conflict versions of a file (plus its current live version) 
	public static JCloudDocumentOperation FileFetchAllVersions(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileFetchAllVersionsOperation(path, operation));
		return operation;
	}
	
	// Reads the byte of a specific file version
	public static JCloudDocumentOperation FileReadVersionBytes(string path, byte[] uniqueIdentifier) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FileReadVersionBytesOperation(path, uniqueIdentifier, operation));
		return operation;
	}
	
	// Pick a specific file version, make it current and remove all other versions
	public static JCloudDocumentOperation FilePickVersion(string path, byte[] uniqueIdentifier, string versionsHash) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(FilePickVersionOperation(path, uniqueIdentifier, versionsHash, operation));
		return operation;
	}
	
	
	// Creates a directory at path
	public static JCloudDocumentOperation DirectoryCreate(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(DirectoryCreateOperation(path, operation));
		return operation;
	}
	
	// Determines if a directory exists at path
	public static JCloudDocumentOperation DirectoryExists(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(DirectoryExistsOperation(path, operation));
		return operation;
	}
	
	// Deletes a directory at path
	public static JCloudDocumentOperation DirectoryDelete(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(DirectoryDeleteOperation(path, operation));
		return operation;
	}
	
	// Gets a list of files at path
	public static JCloudDocumentOperation DirectoryGetFiles(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(DirectoryGetFilesOperation(path, operation));
		return operation;
	}
	
	// Gets a list of directories at path.
	public static JCloudDocumentOperation DirectoryGetDirectories(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(DirectoryGetDirectoriesOperation(path, operation));
		return operation;
	}
	
	// Get a directory modification date at path
	public static JCloudDocumentOperation DirectoryModificationDate(string path) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(DirectoryModificationDateOperation(path, operation));
		return operation;
	}
	
	// Copy a directory
	public static JCloudDocumentOperation DirectoryCopy(string sourcePath, string destinationPath, bool overwrite) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(DirectoryCopyOperation(sourcePath, destinationPath, overwrite, operation));
		return operation;
	}
	
	// Move a directory
	public static JCloudDocumentOperation DirectoryMove(string sourcePath, string destinationPath, bool overwrite) {
		// Prepare a new operation, start doing stuff and return operation so user can yield
		JCloudDocumentOperation operation = new JCloudDocumentOperation();
		JCloudManager.GetSharedManager().StartCoroutine(DirectoryMoveOperation(sourcePath, destinationPath, overwrite, operation));
		return operation;
	}
	
	
	
	// Determines if a file exists at path
	protected static IEnumerator FileExistsOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, false);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// Check for existence
			byte exists = 0;
			if (JCloudExtern.GetCloudItemExistence(documentUID, ref exists)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				operation.success = status.Value.success;
				operation.result = (exists != 0);
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Dismiss document handling
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				operation.result = System.IO.File.Exists(JCloudManager.JCloudDocumentFallbackPath + path);
			}
			catch {
				// File.Exists doesn't raise any exception anyway but we have to give an error just in case
				operation.error = JCloudDocumentError.PluginError;
				operation.finished = true;
				yield break;
			}
			
			// We have a result, no need to let user yield any longer
			operation.success = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Deletes file at path
	protected static IEnumerator FileDeleteOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}
		
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, false);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// We can now attempt delete the cloud file regardless of its current state
			byte deleted = 0;
			if (JCloudExtern.DeleteCloudItem(documentUID, ref deleted)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				operation.success = status.Value.success;
				operation.result = (deleted != 0);
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Dismiss document handling
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				if (System.IO.File.Exists(JCloudManager.JCloudDocumentFallbackPath + path))
					System.IO.File.Delete(JCloudManager.JCloudDocumentFallbackPath + path);
				else {
					operation.error = JCloudDocumentError.DocumentNotFound;
					operation.finished = true;
					yield break;
				}
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is DirectoryNotFoundException || e is FileNotFoundException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException || e is IOException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.result = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Writes bytes in the file at path
	protected static IEnumerator FileWriteAllBytesOperation(string path, byte[] bytes, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}
				
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, false);
	
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
			
			// We can now create or open the cloud file
			if (JCloudExtern.CreateCloudItem(documentUID)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				// Attempt to write contents if file was created/opened
				if (status.Value.success) {
					operation.success = JCloudExtern.WriteCloudItemContents(bytes, bytes.Length, documentUID);
					operation.result = operation.success;
				}
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				System.IO.File.WriteAllBytes(JCloudManager.JCloudDocumentFallbackPath + path, bytes);
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is UnauthorizedAccessException || e is IOException || e.GetType().FullName.Equals("System.Security.SecurityException") || e is DirectoryNotFoundException || e is FileNotFoundException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.result = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Reads bytes from file at path
	protected static IEnumerator FileReadAllBytesOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}
				
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, false);
	
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// We can now open the cloud file
			if (JCloudExtern.OpenCloudItem(documentUID)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status)) {
					JCloudManager.GetDocumentProgress(documentUID, out operation.progress);
					yield return null;
				}
				
				// Attempt to read bytes if file was opened
				if (status.Value.success) {
					System.IntPtr bytesPtr;
					int length = JCloudExtern.ReadCloudItemContents(documentUID, out bytesPtr);
					
					if (length != -1) {
						// Success, read bytes from pointer
						operation.success = true;
						operation.result = new byte[length];
						Marshal.Copy(bytesPtr, operation.result as byte[], 0, length);
						JCloudExtern.FreeMemory(bytesPtr);
					} else
						operation.error = JCloudDocumentError.NativeError;
				}
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
				
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				operation.result = System.IO.File.ReadAllBytes(JCloudManager.JCloudDocumentFallbackPath + path);
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is DirectoryNotFoundException || e is FileNotFoundException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException || e is IOException || e.GetType().FullName.Equals("System.Security.SecurityException"))
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Get a file modification date at path
	protected static IEnumerator FileModificationDateOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, false);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// Get modification date
			long modificationDate = 0;
			if (JCloudExtern.GetCloudItemModificationDate(documentUID, ref modificationDate)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				// Check if we did get a correct date
				if (status.Value.success) {
					operation.success = true;
					operation.result = new System.DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(modificationDate).ToLocalTime();
				}
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;

			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				if (System.IO.File.Exists(JCloudManager.JCloudDocumentFallbackPath + path))
					operation.result = System.IO.File.GetLastWriteTime(JCloudManager.JCloudDocumentFallbackPath + path);
				else {
					operation.error = JCloudDocumentError.DocumentNotFound;
					operation.finished = true;
					yield break;
				}
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is IOException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Copy a file
	protected static IEnumerator FileCopyOperation(string sourcePath, string destinationPath, bool overwrite, JCloudDocumentOperation operation) {
		// Failsafe
		if ((sourcePath == null) || (sourcePath == "") || (destinationPath == null) || (destinationPath == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(sourcePath, false);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// Attempt to copy to destination
			if (JCloudExtern.CopyCloudItem(documentUID, destinationPath, overwrite)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status)) {
					JCloudManager.GetDocumentProgress(documentUID, out operation.progress);
					yield return null;
				}
				
				operation.success = status.Value.success;
				operation.result = operation.success;
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				if (System.IO.File.Exists(JCloudManager.JCloudDocumentFallbackPath + destinationPath)) {
					if (overwrite)
						System.IO.File.Copy(JCloudManager.JCloudDocumentFallbackPath + sourcePath, JCloudManager.JCloudDocumentFallbackPath + destinationPath, overwrite);
					else {
						operation.error = JCloudDocumentError.OverwriteError;
						operation.finished = true;
						yield break;
					}
				} else
					System.IO.File.Copy(JCloudManager.JCloudDocumentFallbackPath + sourcePath, JCloudManager.JCloudDocumentFallbackPath + destinationPath, overwrite);
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is FileNotFoundException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException || e is DirectoryNotFoundException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.result = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Move a file
	protected static IEnumerator FileMoveOperation(string sourcePath, string destinationPath, bool overwrite, JCloudDocumentOperation operation) {
		// Failsafe
		if ((sourcePath == null) || (sourcePath == "") || (destinationPath == null) || (destinationPath == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(sourcePath, false);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// Attempt to move to destination
			if (JCloudExtern.MoveCloudItem(documentUID, destinationPath, overwrite)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status)) {
					JCloudManager.GetDocumentProgress(documentUID, out operation.progress);
					yield return null;
				}
				
				operation.success = status.Value.success;
				operation.result = operation.success;
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				if (System.IO.File.Exists(JCloudManager.JCloudDocumentFallbackPath + destinationPath)) {
					if (overwrite)
						System.IO.File.Delete(JCloudManager.JCloudDocumentFallbackPath + destinationPath);
					else {
						operation.finished = true;
						yield break;
					}
				}
				
				System.IO.File.Move(JCloudManager.JCloudDocumentFallbackPath + sourcePath, JCloudManager.JCloudDocumentFallbackPath + destinationPath);
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is FileNotFoundException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException || e is DirectoryNotFoundException|| e is IOException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.result = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Determines if a file has cloud versions (conflicts) at path
	protected static IEnumerator FileHasConflictVersionsOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, false);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// Check for existence
			byte conflict = 0;
			if (JCloudExtern.CloudItemHasConflictVersions(documentUID, ref conflict)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				operation.success = status.Value.success;
				operation.result = (conflict != 0);
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Dismiss document handling
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			operation.error = JCloudDocumentError.CloudUnavailable;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Fetch conflict versions metadata + current live version metadata at path
	protected static IEnumerator FileFetchAllVersionsOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, false);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// Check for existence
			JCloudExtern.JCloudDocumentVersionsInternal versionsInternal = new JCloudExtern.JCloudDocumentVersionsInternal();
			if (JCloudExtern.CloudItemFetchAllVersions(documentUID, ref versionsInternal)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				if (status.Value.success) {
					// Spawn data in public struct
					JCloudDocumentVersions versions = new JCloudDocumentVersions();
					versions.versionsHash = Marshal.PtrToStringAnsi(versionsInternal.versionsHash);
					versions.versionsMetadata = new JCloudDocumentVersionMetadata[versionsInternal.versionsCount];
					
					System.IntPtr[] uniqueIdentifiersPointers = new System.IntPtr[versionsInternal.versionsCount];
					int[] uniqueIdentifiersLengthes = new int[versionsInternal.versionsCount];
					long[] modificationDates = new long[versionsInternal.versionsCount];
					byte[] isCurrent = new byte[versionsInternal.versionsCount];
					
					Marshal.Copy(versionsInternal.versionsUniqueIdentifiers, uniqueIdentifiersPointers, 0, versionsInternal.versionsCount);
					Marshal.Copy(versionsInternal.versionsUniqueIdentifiersLengthes, uniqueIdentifiersLengthes, 0, versionsInternal.versionsCount);
					Marshal.Copy(versionsInternal.versionsModificationDates, modificationDates, 0, versionsInternal.versionsCount);
					Marshal.Copy(versionsInternal.versionsIsCurrent, isCurrent, 0, versionsInternal.versionsCount);
					
					for (int i = 0; i < versionsInternal.versionsCount; i++) {
						JCloudDocumentVersionMetadata metadata = new JCloudDocumentVersionMetadata();
						
						metadata.uniqueIdentifier = new byte[uniqueIdentifiersLengthes[i]];
						Marshal.Copy(uniqueIdentifiersPointers[i], metadata.uniqueIdentifier, 0, uniqueIdentifiersLengthes[i]);
						metadata.modificationDate = new System.DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(modificationDates[i]).ToLocalTime();
						metadata.isCurrent = (isCurrent[i] != 0);
							
						versions.versionsMetadata[i] = metadata;
					}
					
					// Free memory
					JCloudExtern.FreeMemory(versionsInternal.versionsHash);
					for (int i = 0; i < versionsInternal.versionsCount; i++) {
						JCloudExtern.FreeMemory(uniqueIdentifiersPointers[i]);
					}
					JCloudExtern.FreeMemory(versionsInternal.versionsUniqueIdentifiers);
					JCloudExtern.FreeMemory(versionsInternal.versionsUniqueIdentifiersLengthes);
					JCloudExtern.FreeMemory(versionsInternal.versionsModificationDates);
					JCloudExtern.FreeMemory(versionsInternal.versionsIsCurrent);
					
					operation.success = true;
					operation.result = versions;
				}
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Dismiss document handling
			DismissCloudDocument(documentUID);
		} else {
#endif

#if !UNITY_WEBPLAYER && !UNITY_METRO
			operation.error = JCloudDocumentError.CloudUnavailable;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Reads bytes from file specific version at path
	protected static IEnumerator FileReadVersionBytesOperation(string path, byte[] uniqueIdentifier, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "") || uniqueIdentifier == null || uniqueIdentifier.Length == 0) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}
				
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, false);
	
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// We can now open the cloud file
			if (JCloudExtern.OpenCloudItemWithVersionIdentifier(documentUID, uniqueIdentifier, uniqueIdentifier.Length)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status)) {
					JCloudManager.GetDocumentProgress(documentUID, out operation.progress);
					yield return null;
				}
				
				// Attempt to read bytes if file was opened
				if (status.Value.success) {
					System.IntPtr bytesPtr;
					int length = JCloudExtern.ReadCloudItemContents(documentUID, out bytesPtr);
					
					if (length != -1) {
						// Success, read bytes from pointer
						operation.success = true;
						operation.result = new byte[length];
						Marshal.Copy(bytesPtr, operation.result as byte[], 0, length);
						JCloudExtern.FreeMemory(bytesPtr);
					} else
						operation.error = JCloudDocumentError.NativeError;
				}
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
				
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			operation.error = JCloudDocumentError.CloudUnavailable;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Pick a specific file version, make it current and remove all other versions
	protected static IEnumerator FilePickVersionOperation(string path, byte[] uniqueIdentifier, string versionsHash, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "") || uniqueIdentifier == null || uniqueIdentifier.Length == 0 || versionsHash == null || versionsHash.Length == 0) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, false);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
					
			// Check for existence
			if (JCloudExtern.PickCloudItemWithVersionIdentifier(documentUID, uniqueIdentifier, uniqueIdentifier.Length, versionsHash)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				operation.success = status.Value.success;
				operation.result = status.Value.success;
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Dismiss document handling
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			operation.error = JCloudDocumentError.CloudUnavailable;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	
	// Creates a directory at path
	protected static IEnumerator DirectoryCreateOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}
		
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, true);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
			
			// We can now create or open the cloud directory
			if (JCloudExtern.CreateCloudItem(documentUID)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				operation.success = status.Value.success;
				operation.result = operation.success;
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				System.IO.Directory.CreateDirectory(JCloudManager.JCloudDocumentFallbackPath + path);
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is UnauthorizedAccessException || e is IOException || e.GetType().FullName.Equals("System.Security.SecurityException") || e is DirectoryNotFoundException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.result = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Determines if a directory exists at path
	protected static IEnumerator DirectoryExistsOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}
		
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, true);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
			
			// Check for existence
			byte exists = 0;
			if (JCloudExtern.GetCloudItemExistence(documentUID, ref exists)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				operation.success = status.Value.success;
				operation.result = (exists != 0);
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				operation.result = System.IO.Directory.Exists(JCloudManager.JCloudDocumentFallbackPath + path);
			}
			catch {
				// Directory.Exists doesn't raise any exception anyway but we have to give an error just in case
				operation.error = JCloudDocumentError.PluginError;
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Deletes a directory at path
	protected static IEnumerator DirectoryDeleteOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}
		
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, true);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
			
			// We can now attempt delete the cloud file regardless of its current state
			byte deleted = 0;
			if (JCloudExtern.DeleteCloudItem(documentUID, ref deleted)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				operation.success = status.Value.success;
				operation.result = (deleted != 0);
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				if (System.IO.Directory.Exists(JCloudManager.JCloudDocumentFallbackPath + path))
					System.IO.Directory.Delete(JCloudManager.JCloudDocumentFallbackPath + path, true);
				else {
					operation.error = JCloudDocumentError.DocumentNotFound;
					operation.finished = true;
					yield break;
				}
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is DirectoryNotFoundException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException || e is IOException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.result = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Gets a list of files at path
	protected static IEnumerator DirectoryGetFilesOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if (path == null) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}
		
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, true);
	
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
			
			// We can now open the cloud file
			if (JCloudExtern.OpenCloudItem(documentUID)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				if (status.Value.success) {
					// Attempt to read contents
					System.IntPtr bytesPtr;
					int length = JCloudExtern.ReadCloudItemContents(documentUID, out bytesPtr);
					
					if (length != -1) {
						// Success, read bytes from pointer
						operation.success = true;
						byte[] bytes = new byte[length];
						Marshal.Copy(bytesPtr, bytes, 0, length);
						JCloudExtern.FreeMemory(bytesPtr);
						
						// Post-process read data
						operation.result = JCloudManager.PathListFromBytes(bytes, false);
					}
				}
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				operation.result = System.IO.Directory.GetFiles(JCloudManager.JCloudDocumentFallbackPath + path);
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is DirectoryNotFoundException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException || e is IOException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			// Trim path from filenames
			System.Collections.Generic.List<string> trimmedFiles = new System.Collections.Generic.List<string>();
			string[] filenames = operation.result as string[];
			for (int i = 0; i < filenames.Length; i++)
				trimmedFiles.Add(filenames[i].Remove(0, (JCloudManager.JCloudDocumentFallbackPath + path).Length));
			
			operation.success = true;
			operation.result = trimmedFiles.ToArray();
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Gets a list of directories at path.
	protected static IEnumerator DirectoryGetDirectoriesOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if (path == null) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}
		
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, true);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
			
			// We can now open the cloud file
			if (JCloudExtern.OpenCloudItem(documentUID)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				if (status.Value.success) {
					// Attempt to read contents
					System.IntPtr bytesPtr;
					int length = JCloudExtern.ReadCloudItemContents(documentUID, out bytesPtr);
					
					if (length != -1) {
						// Success, read bytes from pointer		
						operation.success = true;
						byte[] bytes = new byte[length];
						Marshal.Copy(bytesPtr, bytes, 0, length);
						JCloudExtern.FreeMemory(bytesPtr);
						
						// Post-process read data
						operation.result = JCloudManager.PathListFromBytes(bytes, true);
					}
				}
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				operation.result = System.IO.Directory.GetDirectories(JCloudManager.JCloudDocumentFallbackPath + path);
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is DirectoryNotFoundException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException || e is IOException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			// Trim path from directories
			System.Collections.Generic.List<string> trimmedDirectories = new System.Collections.Generic.List<string>();
			string[] directories = operation.result as string[];
			for (int i = 0; i < directories.Length; i++)
				trimmedDirectories.Add(directories[i].Remove(0, (JCloudManager.JCloudDocumentFallbackPath + path).Length));
			
			operation.success = true;
			operation.result = trimmedDirectories.ToArray();
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Get a directory modification date at path
	protected static IEnumerator DirectoryModificationDateOperation(string path, JCloudDocumentOperation operation) {
		// Failsafe
		if ((path == null) || (path == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(path, true);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
			
			// Get modification date
			long modificationDate = 0;
			if (JCloudExtern.GetCloudItemModificationDate(documentUID, ref modificationDate)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status))
					yield return null;
				
				// Check if we did get a correct date
				if (status.Value.success) {
					operation.success = true;
					operation.result = new System.DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(modificationDate).ToLocalTime();
				}
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;

			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				if (System.IO.Directory.Exists(JCloudManager.JCloudDocumentFallbackPath + path))
					operation.result = System.IO.Directory.GetLastWriteTime(JCloudManager.JCloudDocumentFallbackPath + path);
				else {
					operation.error = JCloudDocumentError.DocumentNotFound;
					operation.finished = true;
					yield break;
				}
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is IOException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Copy a directory
	protected static IEnumerator DirectoryCopyOperation(string sourcePath, string destinationPath, bool overwrite, JCloudDocumentOperation operation) {
		// Failsafe
		if ((sourcePath == null) || (sourcePath == "") || (destinationPath == null) || (destinationPath == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(sourcePath, true);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
			
			// Attempt to copy to destination
			if (JCloudExtern.CopyCloudItem(documentUID, destinationPath, overwrite)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status)) {
					JCloudManager.GetDocumentProgress(documentUID, out operation.progress);
					yield return null;
				}
				
				operation.success = status.Value.success;
				operation.result = operation.success;
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				if (!System.IO.Directory.Exists(JCloudManager.JCloudDocumentFallbackPath + sourcePath)) {
						operation.error = JCloudDocumentError.DocumentNotFound;
						operation.finished = true;
						yield break;
				}
				
				if (System.IO.Directory.Exists(JCloudManager.JCloudDocumentFallbackPath + destinationPath)) {
					if (overwrite)
						System.IO.Directory.Delete(JCloudManager.JCloudDocumentFallbackPath + destinationPath);
					else {
						operation.error = JCloudDocumentError.OverwriteError;
						operation.finished = true;
						yield break;
					}
				}
				
				JCloudManager.CopyDirectory(JCloudManager.JCloudDocumentFallbackPath + sourcePath, JCloudManager.JCloudDocumentFallbackPath + destinationPath);
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is UnauthorizedAccessException || e is DirectoryNotFoundException || e is FileNotFoundException || e is IOException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.result = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	// Move a directory
	protected static IEnumerator DirectoryMoveOperation(string sourcePath, string destinationPath, bool overwrite, JCloudDocumentOperation operation) {
		// Failsafe
		if ((sourcePath == null) || (sourcePath == "") || (destinationPath == null) || (destinationPath == "")) {
			operation.error = JCloudDocumentError.InvalidArguments;
			operation.finished = true;
			yield break;
		}

#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		// Make sure our platfom is compatible
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Prepare a new cloud document
			int documentUID = JCloudDocument.NewCloudDocument(sourcePath, true);
			
			// Any current operation on that document? Just wait yield until they end
			int lockId = JCloudManager.GetDocumentLock(documentUID);
			while (!JCloudManager.CheckDocumentLock(documentUID, lockId))
				yield return null;
			
			// Attempt to move to destination
			if (JCloudExtern.MoveCloudItem(documentUID, destinationPath, overwrite)) {
				JCloudManager.DocumentStatus? status;
				while (!JCloudManager.GetDocumentStatus(documentUID, out status)) {
					JCloudManager.GetDocumentProgress(documentUID, out operation.progress);
					yield return null;
				}
				
				operation.success = status.Value.success;
				operation.result = operation.success;
				operation.error = status.Value.error;
			} else
				operation.error = JCloudDocumentError.PluginError;
			
			// We have a result, no need to let user yield any longer
			operation.finished = true;
			
			// Wether successfull or not, we don't need document anymore
			DismissCloudDocument(documentUID);
		} else {
#endif
#if !UNITY_WEBPLAYER && !UNITY_METRO
			// C# fallback
			try {
				if (System.IO.Directory.Exists(JCloudManager.JCloudDocumentFallbackPath + destinationPath)) {
					if (overwrite)
						System.IO.Directory.Delete(JCloudManager.JCloudDocumentFallbackPath + destinationPath);
					else {
						operation.error = JCloudDocumentError.OverwriteError;
						operation.finished = true;
						yield break;
					}
				}
				
				System.IO.Directory.Move(JCloudManager.JCloudDocumentFallbackPath + sourcePath, JCloudManager.JCloudDocumentFallbackPath + destinationPath);
			}
			catch (System.Exception e) {
				if (e is ArgumentException || e is ArgumentNullException || e is NotSupportedException || e is PathTooLongException)
					operation.error = JCloudDocumentError.InvalidArguments;
				else if (e is DirectoryNotFoundException)
					operation.error = JCloudDocumentError.DocumentNotFound;
				else if (e is UnauthorizedAccessException || e is IOException)
					operation.error = JCloudDocumentError.NativeError;
				else
					operation.error = JCloudDocumentError.PluginError;
				
				operation.finished = true;
				yield break;
			}
			
			operation.success = true;
			operation.result = true;
			operation.finished = true;
#else
			operation.error = JCloudDocumentError.InvalidPlatform;
			operation.finished = true;
#endif
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		}
#endif
	}
	
	
	//Determines if iCloud document storage is available
	public static bool PollCloudDocumentAvailability() {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Check manager status
			JCloudManager.CheckManagerStatus();
			
			return JCloudExtern.GetUbiquitousContainerAvailability();
		} else
#endif
			return false;
	}
	
	//////////////////////////////////////////////
	// FILE STORE CHANGES MONITORING
	
	public static bool RegisterCloudDocumentExternalChanges(Component componentOrGameObject) {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Failsafe
			if (componentOrGameObject == null)
				return false;
			
			// Check manager status
			JCloudManager.CheckManagerStatus();
			
			// Add
			JCloudManager.AddFileChangeRegisteredComponent(componentOrGameObject);
			
			return true;
		}
#endif
	
		return false;
	}
	
	public static bool UnregisterCloudDocumentExternalChanges(Component componentOrGameObject) {
#if !UNITY_EDITOR && ((UNITY_IPHONE && JCLOUDPLUGIN_IOS) || (UNITY_STANDALONE_OSX && JCLOUDPLUGIN_OSX))
		if (JCloudManager.PlatformIsCloudCompatible() && (AcceptJailbrokenDevices || JCloudExtern.IsJailbroken() == false)) {
			// Failsafe
			if (componentOrGameObject == null)
				return false;
	
			// Check manager status
			JCloudManager.CheckManagerStatus();
			
			// Remove
			JCloudManager.RemoveFileChangeRegisteredComponent(componentOrGameObject);
		
			return true;
		}
#endif
	
		return false;
	}
	
}
