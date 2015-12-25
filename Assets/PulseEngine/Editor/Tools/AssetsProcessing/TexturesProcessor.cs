using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Textures processor overrides texture import settings for alpha textures with the "Automatic Compressed" on the Android ETC format.
/// TODO: If don't want a texture to be overriden by this script add the following Asset Label to it: "NoOverride".
/// If the texture has Overriden settings for the current platform, those settings will be used and no preprocessing will be done on it.
/// </summary>
public class TexturesProcessor : AssetPostprocessor {
    void OnPreprocessTexture() {
		// Override texture import settings for Android if we're doing a build target
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
			// Note: All ETC textures that have alpha and are set to compressed must be converted to "Automatic Truecolor" because Unity sets them by default
			// to RGBA16 which in most cases it looks horrible. 
			TextureImporter importer = assetImporter as TextureImporter;			
			
			if (EditorUserBuildSettings.androidBuildSubtarget == AndroidBuildSubtarget.ETC && importer.textureFormat == TextureImporterFormat.AutomaticCompressed && 
				importer.DoesSourceTextureHaveAlpha()) {
				importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				Debug.Log("Preprocessing modified: " + importer.assetPath + " => " + importer.textureFormat);
			}
		}	
    }
}