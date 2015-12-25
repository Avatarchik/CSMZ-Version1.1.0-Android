using UnityEngine;
using System.Collections;

public class jLoadSaveUI : MonoBehaviour {
	
	// We are keeping track of objects which state will be saved/loaded
	public GameObject player;
	public GameObject playerCamera;
	public GameObject redCube;
	public GameObject greenCube;
	public GameObject blueCube;
	
	private string lastPlayedPlatform;
	private string lastPlayedTime;
	private int playCount;
	
	private string error;
	
	void OnEnable() {
		// Get last played platform & Set us as the last played platform
		lastPlayedPlatform = JCloudData.GetString("Last Played Platform", Application.platform.ToString());
		JCloudData.SetString("Last Played Platform", Application.platform.ToString());

		// Get last played time & Set now as the last played time
		lastPlayedTime = JCloudData.GetString("Last Played Time", System.DateTime.Now.ToString());
		JCloudData.SetString("Last Played Time", System.DateTime.Now.ToString());

		// Get play count & Increment play count and save new value
		playCount = JCloudData.GetInt("Play Count");
		playCount++;
		JCloudData.SetInt("Play Count", playCount);

		// Finally save our dictionary
		JCloudData.Save();
	}
	
#if (UNITY_IPHONE || UNITY_ANDROID)
	// Only iPhone & Android have pause & resume of apps
	void OnApplicationPause(bool pause) {
		if (pause == false) {
			// Get last played platform & Set us as the last played platform
			lastPlayedPlatform = JCloudData.GetString("Last Played Platform", Application.platform.ToString());
			JCloudData.SetString("Last Played Platform", Application.platform.ToString());
	
			// Get last played time & Set now as the last played time
			lastPlayedTime = JCloudData.GetString("Last Played Time", System.DateTime.Now.ToString());
			JCloudData.SetString("Last Played Time", System.DateTime.Now.ToString());
	
			// Get play count & Increment play count and save new value
			playCount = JCloudData.GetInt("Play Count");
			playCount++;
			JCloudData.SetInt("Play Count", playCount);
	
			// Finally save our dictionary
			JCloudData.Save();
		}
	}
#endif

#if !(UNITY_IPHONE || UNITY_ANDROID)
	void Update() {
		// Keys for quick save & quick load
		if (Input.GetKeyUp(KeyCode.O))
			StartCoroutine(LoadGame());
		
		if (Input.GetKeyUp(KeyCode.P))
			StartCoroutine(SaveGame());
	}
#endif
	
	// Very basic load/save GUI
	void OnGUI() {
		// Quick save only on desktop
#if !(UNITY_IPHONE || UNITY_ANDROID)
		GUILayout.Label("Press 'o' to load game and 'p' to save game");
#endif
		
		// Load game command
		if (GUILayout.Button("Load Game", GUILayout.Width(320), GUILayout.Height(40)))
			StartCoroutine(LoadGame());
		
		// Save game command
		if (GUILayout.Button("Save Game", GUILayout.Width(320), GUILayout.Height(40)))
			StartCoroutine(SaveGame());
		
		if (error != null)
			GUILayout.Label("Error: " + error);
		
		GUILayout.Label("Last Played Platform: " + lastPlayedPlatform);
		GUILayout.Label("Last Played Time: " + lastPlayedTime);
		GUILayout.Label("Play Count: " + playCount);
	}
	
	IEnumerator LoadGame() {
		JCloudDocumentOperation operation = JCloudDocument.FileReadAllBytes("Savegames/My saved game.sav");
		while (!operation.finished)
			yield return null;
		
		// Look for error -- if any, handle & stop coroutine here
		if (operation.error.HasValue) {
			HandleDocumentError(operation.error.Value);
			yield break;
		}
		
		// Success
		error = null;
		byte[] gameBytes = operation.result as byte[];
		
		// No bytes, no savegame
		if (gameBytes != null) {
			System.IO.MemoryStream dataStream = new System.IO.MemoryStream(gameBytes);
			System.IO.BinaryReader reader = new System.IO.BinaryReader(dataStream);
			
			// Read player state
			DeserializeTransformFromReader(player.transform, reader);
			DeserializeRigidbodyFromReader(player.GetComponent<Rigidbody>(), reader);
			
			// Read player camera state
			DeserializeTransformFromReader(playerCamera.transform, reader);
			
			// Read red, green & blue cubes state
			DeserializeTransformFromReader(redCube.transform, reader);
			DeserializeRigidbodyFromReader(redCube.GetComponent<Rigidbody>(), reader);
			DeserializeTransformFromReader(greenCube.transform, reader);
			DeserializeRigidbodyFromReader(greenCube.GetComponent<Rigidbody>(), reader);
			DeserializeTransformFromReader(blueCube.transform, reader);
			DeserializeRigidbodyFromReader(blueCube.GetComponent<Rigidbody>(), reader);
		}
	}
	
	IEnumerator SaveGame() {
		// Failsafe
		if (player && playerCamera && redCube && greenCube && blueCube) {
			// Prepare a memory stream & a binary writer
			System.IO.MemoryStream dataStream = new System.IO.MemoryStream();
			System.IO.BinaryWriter writer = new System.IO.BinaryWriter(dataStream);
			
			// Write player state
			SerializeTransformToWriter(player.transform, writer);
			SerializeRigidbodyToWriter(player.GetComponent<Rigidbody>(), writer);
			
			// Write player camera state
			SerializeTransformToWriter(playerCamera.transform, writer);
			
			// Write red, green & blue cubes state
			SerializeTransformToWriter(redCube.transform, writer);
			SerializeRigidbodyToWriter(redCube.GetComponent<Rigidbody>(), writer);
			SerializeTransformToWriter(greenCube.transform, writer);
			SerializeRigidbodyToWriter(greenCube.GetComponent<Rigidbody>(), writer);
			SerializeTransformToWriter(blueCube.transform, writer);
			SerializeRigidbodyToWriter(blueCube.GetComponent<Rigidbody>(), writer);
			
			// Save game -- make sure directory exists
			JCloudDocumentOperation operation;
			operation = JCloudDocument.DirectoryExists("Savegames");
			while (!operation.finished)
				yield return null;
			
			// Look for error -- if any, handle & stop coroutine here
			if (operation.error.HasValue) {
				HandleDocumentError(operation.error.Value);
				yield break;
			}
			
			if (!(bool)operation.result) {
				operation = JCloudDocument.DirectoryCreate("Savegames");
				while (!operation.finished)
					yield return null;
				
				// Look for error -- if any, handle & stop coroutine here
				if (operation.error.HasValue) {
					HandleDocumentError(operation.error.Value);
					yield break;
				}
			}
					
			// Write file
			operation = JCloudDocument.FileWriteAllBytes("Savegames/My saved game.sav", dataStream.GetBuffer());
				while (!operation.finished)
					yield return null;
					
			// Look for error -- if any, handle & stop coroutine here
			if (operation.error.HasValue) {
				HandleDocumentError(operation.error.Value);
				yield break;
			}
		}
	}
	
	// Helper to serialize Transform directly into a BinaryWriter
	void SerializeTransformToWriter(Transform tr, System.IO.BinaryWriter writer) {
		writer.Write(tr.localPosition.x);
		writer.Write(tr.localPosition.y);
		writer.Write(tr.localPosition.z);
		writer.Write(tr.localEulerAngles.x);
		writer.Write(tr.localEulerAngles.y);
		writer.Write(tr.localEulerAngles.z);
		writer.Write(tr.localScale.x);
		writer.Write(tr.localScale.y);
		writer.Write(tr.localScale.z);
	}
	
	// Helper to deserialize Transform directly from a BinaryReader
	void DeserializeTransformFromReader(Transform tr, System.IO.BinaryReader reader) {
		tr.localPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		tr.localEulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		tr.localScale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
	}
	
	// Helper to serialize Rigidbody directly into a BinaryWriter
	void SerializeRigidbodyToWriter(Rigidbody body, System.IO.BinaryWriter writer) {
		writer.Write(body.velocity.x);
		writer.Write(body.velocity.y);
		writer.Write(body.velocity.z);
		writer.Write(body.angularVelocity.x);
		writer.Write(body.angularVelocity.y);
		writer.Write(body.angularVelocity.z);
	}
	
	// Helper to deserialize Rigidbody directly from a BinaryReader
	void DeserializeRigidbodyFromReader(Rigidbody body, System.IO.BinaryReader reader) {
		body.velocity = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		body.angularVelocity = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
	}
	
	// Single method for error handling
	private void HandleDocumentError(JCloudDocumentError documentError) {
		switch (documentError) {
			case JCloudDocumentError.InvalidPlatform: // Web player -- no file access. Do not use JCloudDocument.
				error = "No file access allowed on this platform.";
				break;
			case JCloudDocumentError.PluginError:
			case JCloudDocumentError.NativeError:
			case JCloudDocumentError.InvalidArguments: // Look out for this one as it means you passed invalid path
				// Offer the user to retry
				error = "An error ocurred while loading game data. Please retry. Error: " + documentError.ToString();
				break;
			case JCloudDocumentError.DocumentNotFound:
				error = "There is no saved game present on this device. Start a new game.";
				break;
			case JCloudDocumentError.DownloadTimeout:
				// Offer the user to retry
				error = "Could not download the save game data. Please retry.";
				break;
			case JCloudDocumentError.InvalidVersionIdentifier:
			case JCloudDocumentError.InvalidVersionsHash:
				// Offer the user to retry
				error = "An error occured while handling conflict versions of your save game data. Please retry.";
				break;
			default: // We should never get there
				break;
		}
	}
}
