using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


//TODO: Revise the code for creating LODS. The LODGroup API has changed in Unity 4.0 since 3.x and
// now adding multiple LODS can be done in a cleaner way. 
// Code should be updated for future Unity compatibility issues.

#region FBX LODs processing tool
/// <summary>
/// Builds the lod groups on model import (AssetPreprocessor) or for the currently selected GameObject(tool).
/// In order to succesfully build the lodGroups, the model hierarchy must respect the following:
/// 
/// ModelName             
///  [->someName_LOD1]    
///  [->someName_LOD2]
///  
///  - the parent should contain the mesh with the most detail (LOD0)
///  - the children should be suffixed with _LOD<X> (not case sensitive)
/// </summary>
public class BuildLodGroupTool {

	const int NR_OF_LOD_LEVELS = 3;	
	const int RENDERS_PER_LEVEL = 5;
	
	public static bool singleLod = true;
	
	[MenuItem ("Mobility Games/LOD/Build LOD Groups")]
	public static void BuildLodGroup() {
		singleLod = true;
		BuildLodGroup(Selection.activeTransform, null);
	}
	
	/// <summary>
	/// Create a single LODGroup on the parrent Renderer for the entire hierarchy. This LODGroup contains the meshes from all the hierarchy renderers.
	/// </summary>
	[MenuItem ("Mobility Games/LOD/Build Single LOD Group")]	
	public static void BuildSingleLodGroup() {
		
		LODGroup[] lodGroups = Selection.activeTransform.GetComponentsInChildren<LODGroup>();
		foreach(LODGroup lodGroup in lodGroups) {
			GameObject.Destroy(lodGroup);
		}
		
		singleLod = false;
		Dictionary<int,  List<Renderer>> lodRenders = new Dictionary<int, List<Renderer>>();
		BuildLodGroup(Selection.activeTransform, lodRenders);
		addLods(Selection.activeTransform, lodRenders);
	}
	
	public static void  BuildLodGroup(Transform parent, Dictionary<int, List<Renderer>> dic) {
		
		//Debug.Log("[BuildLodGroup] Imported:" + parent.name);
		int childCount = parent.childCount;
		bool canBuildLod = false;
		Transform child = null;
		
		Dictionary<int,  List<Renderer>> lodRenders;
		
		if(dic != null) {
			lodRenders = dic;
		}
		else {
			lodRenders = new Dictionary<int, List<Renderer>>();
		}
		
		//If the parent gameObject contains a renderer, than this will be assigned as: LOD Level 0
		if(parent.renderer != null) {
			if(lodRenders.ContainsKey(0) == false) {
				lodRenders[0] = new List<Renderer>();
			}
			lodRenders[0].Add(parent.renderer);
			//Debug.Log("[BuildLodGroup]" + parent.name + "has mesh");
		}
		
		//Iterate trought each child, and check if it contains a renderer.
		//If so, then we store the renderer in a list that will be passed
		//as a parameter to addLods() which will create the lod group, and
		//add them accordingly.
		//Otherwise apply the same procedure to the child.
		for(int i = 0; i < childCount; i++) {
			child = parent.GetChild(i);
			
			int lodLevel = isLod(child);
			
			if(child.renderer != null && lodLevel != -1 ) {
				if(!lodRenders.ContainsKey(lodLevel)) {
					
					//Can build Lod
					if(lodLevel > 0 )  {
						canBuildLod = true;
					}
					
					//Create a list of meshes for this LOD_LVL
					lodRenders[lodLevel] = new List<Renderer>();
					//Debug.Log("[Created] LOD_LVL: " + lodLevel);
				}
	
				//Add this renderer to the current LOD_LVL
				if(dic == null) {
					lodRenders[lodLevel].Add(child.renderer);
				}
				else {
					dic[lodLevel].Add(child.renderer);
				}
				
			}

			//Apply LODGroup tool recursively
			else if(child.childCount != 0) {
				BuildLodGroup(child, dic);
			}
		}
		
		//Call addLods which will actually build the lodGroup on the parent,
		//based on the dictionary built above.
		
		if(canBuildLod && singleLod)
		addLods(parent, lodRenders);
	}
	
	private static void addLods(Transform parent, Dictionary<int, List<Renderer>> lods) {	
		Debug.Log("[BuildLodGroup] <<ADDING LODS FOR " + parent.name + ">>");
		//Getting the attached lod group
		LODGroup lodGroup =  parent.gameObject.GetComponent<LODGroup>();
	
		//If the parent doesn't have LodGroup component attached, attach a new one.
		if(lodGroup != null ) {
			 GameObject.DestroyImmediate(lodGroup);
		}
		
		lodGroup = parent.gameObject.AddComponent<LODGroup>();
	
		//SerializedObject and Serialized property are used for editing an objects properties
		//in a completely generic way
		SerializedObject lodObj = new SerializedObject(lodGroup);
		SerializedProperty lodLevels = lodObj.FindProperty("m_LODs");
		SerializedProperty lodRendererArray = null;
		SerializedProperty lodRenderer = null;
	
		//DeleteArrayElementAtIndex repositions the remaining array elements
		//hence we need to store the times we call it, to keep index consistency
		int offset = 0;

		for(int i = 0; i < lodGroup.lodCount; i++) {
			
			lodRendererArray = lodObj.FindProperty("m_LODs.Array.data[" + i.ToString() + "].renderers");
			
			//If we don't have any renderers for this lod lvl continue
			if(!lods.ContainsKey(i)) {
				Debug.Log("No key for " + i);
				lodLevels.DeleteArrayElementAtIndex(i-offset++);
				continue;
			}
			
			lodRendererArray.arraySize = RENDERS_PER_LEVEL;
			
			if(lodRendererArray != null) {	
					//Set the apropriate renderer to the
					for(int j = 0; j < lods[i].Count; j++) {
						Debug.Log("Set to LOD[0]["+ j + "]");
						lodRenderer = lodRendererArray.GetArrayElementAtIndex(j).FindPropertyRelative("renderer");
						lodRenderer.objectReferenceValue = lods[i][j];
					}
				}
		}
		lodObj.ApplyModifiedProperties();
	}
	
	/// <summary>
	/// Check if object name ends with _LODX  (X - digit)
	/// </summary>
	/// <returns>
	/// -1 if name convention is bad, and X otherwise
	/// </returns>
	private static int isLod(Transform obj) {
		//Obtain last digit
		char lastDigit = obj.name[obj.name.Length-1];
		
		if(Char.IsDigit(lastDigit) && obj.name.ToUpper().EndsWith("_LOD"+lastDigit)) 
			return Int32.Parse(lastDigit.ToString());
		else
			return -1;
	}
}
#endregion

#region FBX Postprocessor
/// <summary>
/// Postprocesses the FBX models and creates and sets up the corresponding LODs.
/// </summary>
public class LodPostProcess : AssetPostprocessor {
	
	private void OnPostprocessModel(GameObject g) {
        BuildLodGroupTool.BuildLodGroup(g.transform, null);
    }
	
	/*
	public override int GetPostprocessOrder() {
		return 100000;
	}
	*/
}

#endregion