using UnityEngine;
using System.Collections;


[AddComponentMenu("")]
public class jTestBed : MonoBehaviour {
	
	string documentResultString = "";
	string dataResultString = "";
	
	// Just an exhaustive list of all usable functions with simple text monitoring for returns
	void OnGUI() {
		float width = Screen.width;
		float height = Screen.height/19;
		
		if (GUI.Button(new Rect(0.0f, 0.0f, width * 0.5f, height), "Write All Bytes To Test File")) {
				StartCoroutine(FileWriteAllBytes());
		}
		
		if (GUI.Button(new Rect(0.0f, height, width * 0.5f, height), "Read All Bytes From Test File")) {
				StartCoroutine(FileReadAllBytes());
		}
		
		if (GUI.Button(new Rect(0.0f, height*2, width * 0.5f, height), "Delete Test File")) {
				StartCoroutine(FileDelete());
		}

		if (GUI.Button(new Rect(0.0f, height*3, width * 0.5f, height), "Get Test File Modification Date")) {
				StartCoroutine(FileModificationDate());
		}

		if (GUI.Button(new Rect(0.0f, height*4, width * 0.5f, height), "Check Test File Exists")) {
				StartCoroutine(FileExists());
		}
		
		if (GUI.Button(new Rect(0.0f, height*5, width * 0.5f, height), "Copy Test File")) {
				StartCoroutine(FileCopy());
		}

		if (GUI.Button(new Rect(0.0f, height*6, width * 0.5f, height), "Write All Bytes To Test File (Conflict)")) {
				StartCoroutine(FileWriteAllBytesConflict());
		}

		if (GUI.Button(new Rect(0.0f, height*7, width * 0.5f, height), "Check Test File Has Conflict Versions")) {
			StartCoroutine(FileHasConflictVersions());
		}

		if (GUI.Button(new Rect(0.0f, height*8, width * 0.5f, height), "Fetch File Fetch All Versions")) {
			StartCoroutine(FileFetchAllVersions());
		}

		if (GUI.Button(new Rect(0.0f, height*9, width * 0.5f, height), "Read Conflict Version Bytes From Test File")) {
			StartCoroutine(FileReadConflictVersionBytes());
		}

		if (GUI.Button(new Rect(width * 0.5f, height*9, width * 0.5f, height), "Pick Conflict Version From Test File")) {
			StartCoroutine(FilePickConflictVersion());
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height*8, width * 0.5f, height), "Pick Current Version From Test File")) {
			StartCoroutine(FilePickCurrentVersion());
		}

		if (GUI.Button(new Rect(0.0f, height*10, width * 0.5f, height), "Poll Cloud Document Availability")) {
			bool availability = JCloudDocument.PollCloudDocumentAvailability();
			documentResultString = "cloud document " + (availability ? "available" : "unavailable");
		}
						
		if (GUI.Button(new Rect(width * 0.5f, 0.0f, width * 0.5f, height), "Create Test Directory")) {
				StartCoroutine(DirectoryCreate());
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height, width * 0.5f, height), "Check Test Directory Exists")) {
				StartCoroutine(DirectoryExists());
		}

		if (GUI.Button(new Rect(width * 0.5f, height*2, width * 0.5f, height), "Delete Test Directory")) {
				StartCoroutine(DirectoryDelete());
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height*3, width * 0.5f, height), "Get Test Directory Modification Date")) {
				StartCoroutine(DirectoryModificationDate());
		}

		if (GUI.Button(new Rect(width * 0.5f, height*4, width * 0.5f, height), "List Top Directory Files")) {
				StartCoroutine(DirectoryGetFiles());
		}

		if (GUI.Button(new Rect(width * 0.5f, height*5, width * 0.5f, height), "List Top Directory Folders")) {
				StartCoroutine(DirectoryGetDirectories());
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height*6, width * 0.5f, height), "Copy Test Directory")) {
				StartCoroutine(DirectoryCopy());
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height*7, width * 0.5f, height), "Register Cloud Document changes")) {
			documentResultString = "now watching cloud document changes";
			JCloudDocument.RegisterCloudDocumentExternalChanges(this);
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height*10, width * 0.5f, height), "Clear Resultats")) {
			documentResultString = "";
		}

		GUI.Label(new Rect(0.0f, height*11, width, height*1.5f), "Document Result : " + documentResultString);
		
		if (GUI.Button(new Rect(0.0f, height*11.5f, width * 0.5f, height), "Data Set \"TestKey\" Int")) {
			JCloudData.SetInt("TestKey", 15);
			dataResultString = "data set int return is void";
		}
		
		if (GUI.Button(new Rect(0.0f, height*12.5f, width * 0.5f, height), "Data Get \"TestKey\" Int")) {
			int keyValue = JCloudData.GetInt("TestKey");
			dataResultString = "data get int return is : " + keyValue;
		}

		if (GUI.Button(new Rect(0.0f, height*13.5f, width * 0.5f, height), "Data Set \"TestKey\" Float")) {
			JCloudData.SetFloat("TestKey", 13.37f);
			dataResultString = "data set float return is void";
		}

		if (GUI.Button(new Rect(0.0f, height*14.5f, width * 0.5f, height), "Data Get \"TestKey\" Float")) {
			float keyValue = JCloudData.GetFloat("TestKey");
			dataResultString = "data get float return is : " + keyValue;
		}
		
		if (GUI.Button(new Rect(0.0f, height*15.5f, width * 0.5f, height), "Register Cloud Data Changes")) {
			dataResultString = "now watching cloud data changes";
			JCloudData.RegisterCloudDataExternalChanges(this);
		}
		
		if (GUI.Button(new Rect(0.0f, height*16.5f, width * 0.5f, height), "Poll Cloud Data Availability")) {
			bool availability = JCloudData.PollCloudDataAvailability();
			dataResultString = "cloud data " + (availability ? "available" : "unavailable");
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height*11.5f, width * 0.5f, height), "Data Set \"TestKey\" String")) {
			JCloudData.SetString("TestKey", "this is a test string");
			dataResultString = "data set string return is void";
		}

		if (GUI.Button(new Rect(width * 0.5f, height*12.5f, width * 0.5f, height), "Data Get \"TestKey\" String")) {
			string keyValue = JCloudData.GetString("TestKey");
			dataResultString = "data get string return is : " + keyValue;
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height*13.5f, width * 0.5f, height), "Data Has \"TestKey\" Key")) {
			bool exists = JCloudData.HasKey("TestKey");
			dataResultString = "data has TestKey key return is : " + (exists ? "yes" : "no");
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height*14.5f, width * 0.5f, height), "Data Delete \"TestKey\" Key")) {
			JCloudData.DeleteKey("TestKey");
			dataResultString = "data delete TestKey key return is void";
		}
		
		if (GUI.Button(new Rect(width * 0.5f, height*15.5f, width * 0.5f, height), "Data Delete All Keys")) {
			JCloudData.DeleteAll();
			dataResultString = "data delete all keys return is void";
		}

		if (GUI.Button(new Rect(width * 0.5f, height*16.5f, width * 0.5f, height), "Data Save")) {
			JCloudData.Save();
			dataResultString = "data save return is ok";
		}
		
		if (GUI.Button(new Rect(0.0f, height*17.5f, width, height), "Clear Resultats")) {
			dataResultString = "";
		}
		
		GUI.Label(new Rect(0.0f, height*18.5f, width, height*1.5f), "Dict/Data Result : " + dataResultString);
	}
	
	void JCloudDataDidChangeExternally(JCloudDataExternalChange change) {
		dataResultString = "cloud data changed ; reason : ";
		switch (change.Reason)
		{
		case JCloudDataChangeReason.JCloudDataAccountChange:
			dataResultString += "account change";
			break;
		case JCloudDataChangeReason.JCloudDataInitialSyncChange:
			dataResultString += "initial sync";
			break;
		case JCloudDataChangeReason.JCloudDataQuotaViolationChange:
			dataResultString += "quota violation";
			break;
		case JCloudDataChangeReason.JCloudDataServerChange:
			dataResultString += "server change";
			break;
		default:
			dataResultString += "nope";
			break;
		}
		
		foreach (JCloudKeyValueChange keyValueChange in change.ChangedKeyValues)
			dataResultString += " ; " + keyValueChange.Key + " (old : " + ((keyValueChange.OldValue == null) ? "(null)" : keyValueChange.OldValue) + " ; new : " + ((keyValueChange.NewValue == null) ? "(null)" : keyValueChange.NewValue) + ")";
	}
	
	
	
	IEnumerator FileWriteAllBytes() {
		System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
		JCloudDocumentOperation operation = JCloudDocument.FileWriteAllBytes("testfile.txt", encoder.GetBytes("this is a test content"));
		while (!operation.finished)
			yield return null;
		if (operation.success)
			documentResultString = "cloud document did write bytes with : " + ((bool)operation.result ? "success" : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
		else
			documentResultString = "cloud document write all bytes failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator FileReadAllBytes() {
		JCloudDocumentOperation operation = JCloudDocument.FileReadAllBytes("testfile.txt");
		while (!operation.finished) {
			documentResultString = "cloud document read bytes progress : " + operation.progress;
			yield return null;
		}
		System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
		if (operation.success)
			documentResultString = "cloud document did read bytes ; read this : " + ((operation.result == null) ? "(null)" : encoder.GetString(operation.result as byte[])) + (operation.error != null ? (" ; error : " + operation.error) : "");
		else
			documentResultString = "cloud document read bytes : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator FileDelete() {
		JCloudDocumentOperation operation = JCloudDocument.FileDelete("testfile.txt");
		while (!operation.finished)
			yield return null;
		if (operation.success && (bool)operation.result)
			documentResultString = "cloud document did delete" + (operation.error != null ? (" ; error : " + operation.error) : "");
		else
			documentResultString = "cloud document did not delete (may not exist?)" + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator FileModificationDate() {
		JCloudDocumentOperation operation = JCloudDocument.FileModificationDate("testfile.txt");
		while (!operation.finished)
			yield return null;
		documentResultString = "cloud document test file modification date : " + (operation.success ? ((System.DateTime)operation.result).ToString("MM-dd-yyyy HH:mm:ss") : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator FileExists() {
		JCloudDocumentOperation operation = JCloudDocument.FileExists("testfile.txt");
		while (!operation.finished)
			yield return null;
		documentResultString = "cloud document test file exists : " + ((bool)operation.result ? "exists" : "does not exist") + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator FileCopy() {
		JCloudDocumentOperation operation = JCloudDocument.FileCopy("testfile.txt", "testfile copy.txt", true);
		while (!operation.finished) {
			documentResultString = "cloud document test file copy progress : " + operation.progress;
			yield return null;
		}
		if (operation.success)
			documentResultString = "cloud document test file copy : " + ((bool)operation.result ? "success" : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
		else
			documentResultString = "cloud document test file copy : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator FileWriteAllBytesConflict() {
		System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
		JCloudDocumentOperation operation = JCloudDocument.FileWriteAllBytes("testfile.txt", encoder.GetBytes("this is a conflict test content"));
		while (!operation.finished)
			yield return null;
		if (operation.success)
			documentResultString = "cloud document did write bytes with : " + ((bool)operation.result ? "success" : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
		else
			documentResultString = "cloud document write all bytes failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
	}

	IEnumerator FileHasConflictVersions() {
		JCloudDocumentOperation operation = JCloudDocument.FileHasConflictVersions("testfile.txt");
		while (!operation.finished)
			yield return null;
		
		if (operation.success)
			documentResultString = "cloud document test file has conflict versions : " + ((bool)operation.result ? "yes" : "no") + (operation.error != null ? (" ; error : " + operation.error) : "");
		else
			documentResultString = "cloud document test file has conflict versions : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator FileFetchAllVersions() {
		JCloudDocumentOperation operation = JCloudDocument.FileFetchAllVersions("testfile.txt");
		while (!operation.finished)
			yield return null;
		
		if (operation.success && operation.result != null) {
			JCloudDocumentVersions versions = (JCloudDocumentVersions)operation.result;
			documentResultString = "cloud document test file versions :";
			int offset = 1;
			foreach (JCloudDocumentVersionMetadata metadata in versions.versionsMetadata) {
				documentResultString += " " + offset + ". " + metadata.modificationDate + (metadata.isCurrent ? " (current)" : "");
				offset++;
			}
			documentResultString += " (hash : " + versions.versionsHash + ")" + (operation.error != null ? (" ; error : " + operation.error) : "");
		} else {
			documentResultString = "cloud document test file versions : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
		}
	}
	
	IEnumerator FileReadConflictVersionBytes() {
		JCloudDocumentOperation operation;
		
		operation = JCloudDocument.FileFetchAllVersions("testfile.txt");
		while (!operation.finished)
			yield return null;
		
		if (operation.success && operation.result != null) {
			JCloudDocumentVersions versions = (JCloudDocumentVersions)operation.result;
			JCloudDocumentVersionMetadata? conflictVersionMetadata = null;
			foreach (JCloudDocumentVersionMetadata metadata in versions.versionsMetadata) {
				if (metadata.isCurrent == false) {
					conflictVersionMetadata = metadata;
					break;
				}
			}
			
			if (conflictVersionMetadata != null) {
				operation = JCloudDocument.FileReadVersionBytes("testfile.txt", conflictVersionMetadata.Value.uniqueIdentifier);
				while (!operation.finished) {
					documentResultString = "cloud document read conflict version bytes progress : " + operation.progress;
					yield return null;
				}
				System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
				if (operation.success)
					documentResultString = "cloud document did read conflict version bytes ; read this : " + ((operation.result == null) ? "(null)" : encoder.GetString(operation.result as byte[])) + (operation.error != null ? (" ; error : " + operation.error) : "");
				else
					documentResultString = "cloud document read conflict version bytes : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
			} else {
				documentResultString = "cloud document read conflict version bytes : failure" + (operation.error != null ? (" ; error : " + operation.error) : "") +  " (found no conflict version)";
			}
		} else {
			documentResultString = "cloud document read conflict version bytes : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
		}
	}
	
	IEnumerator FilePickConflictVersion() {
		JCloudDocumentOperation operation;
		
		operation = JCloudDocument.FileFetchAllVersions("testfile.txt");
		while (!operation.finished)
			yield return null;
		
		if (operation.success && operation.result != null) {
			JCloudDocumentVersions versions = (JCloudDocumentVersions)operation.result;
			JCloudDocumentVersionMetadata? conflictVersionMetadata = null;
			foreach (JCloudDocumentVersionMetadata metadata in versions.versionsMetadata) {
				if (metadata.isCurrent == false) {
					conflictVersionMetadata = metadata;
					break;
				}
			}
			
			if (conflictVersionMetadata != null) {
				operation = JCloudDocument.FilePickVersion("testfile.txt", conflictVersionMetadata.Value.uniqueIdentifier, versions.versionsHash);
				while (!operation.finished) {
					yield return null;
				}
				
				if (operation.success)
					documentResultString = "cloud document did pick conflict version : " + (((bool)operation.result == true) ? "success" : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
				else
					documentResultString = "cloud document did pick conflict version : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
			} else {
				documentResultString = "cloud document did pick conflict version : failure" + (operation.error != null ? (" ; error : " + operation.error) : "") +  " (found no conflict version)";
			}
		} else {
			documentResultString = "cloud document did pick conflict version : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
		}
	}
	
	IEnumerator FilePickCurrentVersion() {
		JCloudDocumentOperation operation;
		
		operation = JCloudDocument.FileFetchAllVersions("testfile.txt");
		while (!operation.finished)
			yield return null;
		
		if (operation.success && operation.result != null) {
			JCloudDocumentVersions versions = (JCloudDocumentVersions)operation.result;
			JCloudDocumentVersionMetadata? currentVersionMetadata = null;
			foreach (JCloudDocumentVersionMetadata metadata in versions.versionsMetadata) {
				if (metadata.isCurrent == true) {
					currentVersionMetadata = metadata;
					break;
				}
			}
			
			if (currentVersionMetadata != null) {
				operation = JCloudDocument.FilePickVersion("testfile.txt", currentVersionMetadata.Value.uniqueIdentifier, versions.versionsHash);
				while (!operation.finished) {
					yield return null;
				}
				
				if (operation.success)
					documentResultString = "cloud document did pick current version : " + (((bool)operation.result == true) ? "success" : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
				else
					documentResultString = "cloud document did pick current version : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
			} else {
				documentResultString = "cloud document did pick current version : failure" + (operation.error != null ? (" ; error : " + operation.error) : "") +  " (found no current version)";
			}
		} else {
			documentResultString = "cloud document did pick current version : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
		}
	}
	
	IEnumerator DirectoryCreate() {
		JCloudDocumentOperation operation = JCloudDocument.DirectoryCreate("Test");
		while (!operation.finished)
			yield return null;
		documentResultString = "cloud document did create test directory with " + ((bool)operation.result ? "success" : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator DirectoryExists() {
		JCloudDocumentOperation operation = JCloudDocument.DirectoryExists("Test");
		while (!operation.finished)
			yield return null;
		documentResultString = "cloud document test directory exists : " + ((bool)operation.result ? "exists" : "does not exist") + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator DirectoryDelete() {
		JCloudDocumentOperation operation = JCloudDocument.DirectoryDelete("Test");
		while (!operation.finished)
			yield return null;
		if (operation.success)
			documentResultString = "cloud document test directory deleted : " + ((bool)operation.result ? "success" : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
		else 
			documentResultString = "cloud document test directory deleted : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator DirectoryModificationDate() {
		JCloudDocumentOperation operation = JCloudDocument.DirectoryModificationDate("Test");
		while (!operation.finished)
			yield return null;
		documentResultString = "cloud document test directory modification date : " + (operation.success ? ((System.DateTime)operation.result).ToString("MM-dd-yyyy HH:mm:ss") : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	IEnumerator DirectoryGetFiles() {
		JCloudDocumentOperation operation = JCloudDocument.DirectoryGetFiles("");
		while (!operation.finished)
			yield return null;
		
		if (operation.success)
		{
			documentResultString = "cloud directory files list : " + (operation.result as string[]).Length;
			for (int i = 0; i < (operation.result as string[]).Length; i++)
				documentResultString += " ; " + (operation.result as string[])[i];
			documentResultString += (operation.error != null ? (" ; error : " + operation.error) : "");
		} else {
			documentResultString = "cloud directory files list : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
		}
	}
	
	IEnumerator DirectoryGetDirectories() {
		JCloudDocumentOperation operation = JCloudDocument.DirectoryGetDirectories("");
		while (!operation.finished)
			yield return null;
		
		if (operation.success)
		{
			documentResultString = "cloud directory directories list : " + (operation.result as string[]).Length;
			for (int i = 0; i < (operation.result as string[]).Length; i++)
				documentResultString += " ; " + (operation.result as string[])[i];
			documentResultString += (operation.error != null ? (" ; error : " + operation.error) : "");
		} else {
			documentResultString = "cloud directory directories list : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
		}
	}
	
	IEnumerator DirectoryCopy() {
		JCloudDocumentOperation operation = JCloudDocument.DirectoryCopy("Test", "Test Copy", true);
		while (!operation.finished) {
			documentResultString = "cloud document test directory copy progress : " + operation.progress;
			yield return null;
		}
		if (operation.success)
			documentResultString = "cloud document test directory copy : " + ((bool)operation.result ? "success" : "failure") + (operation.error != null ? (" ; error : " + operation.error) : "");
		else
			documentResultString = "cloud document test directory copy : failure" + (operation.error != null ? (" ; error : " + operation.error) : "");
	}
	
	public void JCloudDocumentDidChangeExternally(JCloudDocumentExternalChange[] changes) {
		documentResultString = "cloud document did change externally :";
		for (int i = 0; i < changes.Length; i++) {
			documentResultString += " " + i + ". " + changes[i].path;
			
			switch (changes[i].change) {
			case JCloudDocumentChangeType.Added:
				documentResultString += " (Added)";
				break;
			case JCloudDocumentChangeType.Changed:
				documentResultString += " (Changed)";
				break;
			case JCloudDocumentChangeType.Removed:
				documentResultString += " (Removed)";
				break;
			default:
				documentResultString += " (Unknown)";
				break;
			}
		}
	}
	
}