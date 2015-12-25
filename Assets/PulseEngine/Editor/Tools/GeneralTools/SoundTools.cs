using UnityEngine;
using UnityEditor;

public class SoundTools {
	[MenuItem ("Mobility Games/Sounds/Sounds To DecompressOnLoad")]
	public static void SetSoundsToDecompressOnLoad() {
		Object[] selectedSounds = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		AudioImporter audioImporter;
		
		for(int i = 0; i < selectedSounds.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selectedSounds[i]);
	
			audioImporter = AssetImporter.GetAtPath(newAssetPath) as AudioImporter;	
			if (audioImporter.format == AudioImporterFormat.Native) {
				audioImporter.loadType = AudioImporterLoadType.DecompressOnLoad;
			} else {
				Debug.Log("WARNING: Ignoring audio file: " + newAssetPath + " because it's set to be Compressed not Native!");
			}
			
			// re-import the texture with the new settings
			AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
		}
	}
		
	[MenuItem ("Mobility Games/Sounds/Disable 3D")]
	public static void Disable3DSounds() {
		Object[] selectedSounds = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		AudioImporter audioImporter;
		
		for(int i = 0; i < selectedSounds.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selectedSounds[i]);
	
			audioImporter = AssetImporter.GetAtPath(newAssetPath) as AudioImporter;	
			if (audioImporter.format == AudioImporterFormat.Native) {
				audioImporter.threeD = false;
			} else {
				Debug.Log("WARNING: Ignoring audio file: " + newAssetPath + " because it's set to be Compressed not Native!");
			}
			
			// re-import the texture with the new settings
			AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
		}
	}
	
	[MenuItem ("Mobility Games/Sounds/Set to compressed")]
	public static void SetCompressed() {
		Object[] selectedSounds = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		AudioImporter audioImporter;
		
		for(int i = 0; i < selectedSounds.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selectedSounds[i]);
	
			audioImporter = AssetImporter.GetAtPath(newAssetPath) as AudioImporter;	
			audioImporter.format = AudioImporterFormat.Compressed;
			audioImporter.compressionBitrate = 96000;
			
			// re-import the texture with the new settings
			AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
		}
	}

	[MenuItem ("Mobility Games/Sounds/Set to compressed load type")]
	public static void SetCompressedLoadType() {
		Object[] selectedSounds = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		AudioImporter audioImporter;
		
		for(int i = 0; i < selectedSounds.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selectedSounds[i]);
	
			audioImporter = AssetImporter.GetAtPath(newAssetPath) as AudioImporter;	
			audioImporter.loadType = AudioImporterLoadType.CompressedInMemory;
			
			AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
		}
	}

	[MenuItem ("Mobility Games/Sounds/Set to compressed voice")]
	public static void SetCompressedVoices() {
		Object[] selectedSounds = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		AudioImporter audioImporter;
		
		for(int i = 0; i < selectedSounds.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selectedSounds[i]);
	
			audioImporter = AssetImporter.GetAtPath(newAssetPath) as AudioImporter;	
			audioImporter.format = AudioImporterFormat.Compressed;
			audioImporter.compressionBitrate = 96000;
			audioImporter.threeD = false;
			audioImporter.hardware = true;
			audioImporter.loadType = AudioImporterLoadType.CompressedInMemory;
			
			// re-import the texture with the new settings
			AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
		}
	}
	
	[MenuItem ("Mobility Games/Sounds/Set to compressed music")]
	public static void SetCompressedMusic() {
		Object[] selectedSounds = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		AudioImporter audioImporter;
		
		for(int i = 0; i < selectedSounds.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selectedSounds[i]);
	
			audioImporter = AssetImporter.GetAtPath(newAssetPath) as AudioImporter;	
			audioImporter.format = AudioImporterFormat.Compressed;
			audioImporter.loadType = AudioImporterLoadType.StreamFromDisc;
			audioImporter.compressionBitrate = 96000;
			audioImporter.threeD = false;
			audioImporter.hardware = true;
			audioImporter.loopable = true;
			
			// re-import the texture with the new settings
			AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
		}
	}
}