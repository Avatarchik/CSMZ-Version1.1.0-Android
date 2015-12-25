using UnityEngine;
using System.Collections;
using UnityEditor;

public class ModelsProcessor : AssetPostprocessor {
	
	void OnPreprocessModel() {
		ModelImporter modelImporter = assetImporter as ModelImporter;
		modelImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
		
		if(assetPath.Contains("@")) {
			modelImporter.importMaterials = false;
		}
	}
}
