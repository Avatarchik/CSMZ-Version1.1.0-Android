
/** 
 *  How To Use
 * ==============
 * You can setup the import properties of a texture.
 * To apply the settings of that texture to another set of textures, select the manually configured texture and then with SHIFT select
 * the other textures below/above it to which the settings will be applied.
 */


using UnityEngine;
using System.Collections;
using UnityEditor;


public class MultipleTexturesSetup : ScriptableObject {
	/// <summary>
	/// This tool allows you to apply the import settings of a texture to other multiple selected textures.
	/// You can setup the import properties of a texture.
 	/// To apply the settings of that texture to another set of textures, select the manually configured texture and then with SHIFT select
 	/// the other textures below/above it to which the settings will be applied.
	/// </summary>
	[MenuItem ("Mobility Games/Textures/Apply Settings To All &%t")]
	public static void SetTexSettingsToAllSelected() {
		// get all the selected textures in the editor
		
		Object[] selectedTex = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
		
		// read the texture settings from the first texture in the selection list
		if (Selection.activeObject.GetType() != typeof(Texture2D)) {
			Debug.Log("Your first selection must be a texture asset!");
			return;
		}
		
		string firstAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		Debug.Log("MultipleTexturesSetup: Applying texture settings from: " + firstAssetPath);
		
		TextureImporter texImporter = AssetImporter.GetAtPath(firstAssetPath) as TextureImporter;
		TextureImporterSettings texSettings = new TextureImporterSettings();
		texImporter.ReadTextureSettings(texSettings);
		TextureImporterType texType = texImporter.textureType;
		
		// apply the texture settings to all the other selected textures
		Debug.Log("MultipleTexturesSetup: Applied texture settings to " + (selectedTex.Length - 1) + " textures:");
		for(int i = 0; i < selectedTex.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selectedTex[i]);
			// avoid the first selected asset path (this is because the assets returned by Selection.GetFiltered are not in a known order
			if (firstAssetPath == newAssetPath)
				continue;
	
			texImporter = AssetImporter.GetAtPath(newAssetPath) as TextureImporter;	
			texImporter.SetTextureSettings(texSettings);
			texImporter.textureType = texType;
			
			Debug.Log("MultipleTexturesSetup: " + newAssetPath);
			
			// re-import the texture with the new settings
			AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
		}
	}
	
	/// <summary>
	/// Removes the Android Override texture flag from all selected textures that are of Texture type. (not Advanced, GUI or anything else)
	/// </summary>
	[MenuItem ("Mobility Games/Textures/Remove Texture AndroideOverride")]
	public static void RemoveAndroidOverrideFromAllSelectedTextureTypes() {
		
		Object[] selectedTex = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
		int skippedFiles = 0;
		int processedFiles = 0;
		
		TextureImporter texImporter;
		for(int i = 0; i < selectedTex.Length; i++) {
			string assetPathName = AssetDatabase.GetAssetPath(selectedTex[i]);
			texImporter = AssetImporter.GetAtPath(assetPathName) as TextureImporter;
			// If this texture type is not of Image type (Texture type) or Advanced Texture type, we skip it.
			bool isValidTextureType = texImporter.textureType == TextureImporterType.Image ||
				(texImporter.textureType == TextureImporterType.Advanced && !texImporter.lightmap && !texImporter.normalmap);
			
			if ( !isValidTextureType ) {
				skippedFiles++;
//				Debug.Log("Skipping " + texImporter.assetPath);
				continue;
			}
			
			// Read the current platform texture settings to see the override texture settings for Android.
			int maxTexSize = 0;
			TextureImporterFormat androidImpFormat;
			texImporter.GetPlatformTextureSettings(BuildTarget.Android.ToString(), out maxTexSize, out androidImpFormat);
			// If this texture has overriden android settings disable them.
			if (maxTexSize > 0) {
				texImporter.ClearPlatformTextureSettings(BuildTarget.Android.ToString());
				EditorUtility.SetDirty(selectedTex[i]);
				
				AssetDatabase.ImportAsset(assetPathName, ImportAssetOptions.ForceSynchronousImport);
//				Debug.Log("Disabled Android Override for: " + texImporter.assetPath);
				processedFiles++;
			} else {
				skippedFiles++;
			}
		}
		
		Debug.Log("Total files: " + selectedTex.Length);
		Debug.Log("Skipped files: " + skippedFiles);
		Debug.Log("Modified settings for: " + processedFiles +" files: ");
	}	
}
