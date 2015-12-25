using UnityEngine;
using UnityEditor;

public class FBXImportTools {	
	[MenuItem ("Mobility Games/FBX/Disable Normals&Tangents")]
	public static void DisableFbxNormalsAndTangents() {
		Object[] selected = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
		ModelImporter fbxImporter;
		
		for(int i = 0; i < selected.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selected[i]);
	
			fbxImporter = AssetImporter.GetAtPath(newAssetPath) as ModelImporter;
			if (fbxImporter && fbxImporter.normalImportMode != ModelImporterTangentSpaceMode.None) {
				fbxImporter.normalImportMode = ModelImporterTangentSpaceMode.None;
				// re-import the texture with the new settings
				AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
			}			
		}
	}
	
	[MenuItem ("Mobility Games/FBX/Disable Tangents")]
	public static void DisableFbxTangents() {
		Object[] selected = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
		ModelImporter fbxImporter;
		
		for(int i = 0; i < selected.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selected[i]);
	
			fbxImporter = AssetImporter.GetAtPath(newAssetPath) as ModelImporter;
			if (fbxImporter && fbxImporter.tangentImportMode != ModelImporterTangentSpaceMode.None) {
				fbxImporter.tangentImportMode = ModelImporterTangentSpaceMode.None;
				// re-import the texture with the new settings
				AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
			}			
		}
	}
	
	[MenuItem ("Mobility Games/FBX/Scale Factor 1")]
	public static void SetFbxScaleFactor1() {
		Object[] selected = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
		ModelImporter fbxImporter;
		
		for(int i = 0; i < selected.Length; i++) {
			string newAssetPath = AssetDatabase.GetAssetPath(selected[i]);
	
			fbxImporter = AssetImporter.GetAtPath(newAssetPath) as ModelImporter;
			if (fbxImporter && fbxImporter.globalScale != 1.0f) {
				fbxImporter.globalScale = 1.0f;
				// re-import the texture with the new settings
				AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceSynchronousImport);
			}			
		}
	}
}