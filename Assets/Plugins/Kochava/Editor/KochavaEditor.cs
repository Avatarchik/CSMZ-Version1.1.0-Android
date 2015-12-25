#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class KochavaEditor
{
	
	[MenuItem("GameObject/Create Other/Kochava Integration Object")]
	[MenuItem("Help/Kochava/Create Integration Object")]
	static void KochavaIntegration ()
	{
		Object integrationObj = AssetDatabase.LoadAssetAtPath("Assets/Plugins/Kochava/_Kochava Analytics.prefab", typeof(GameObject));
		PrefabUtility.InstantiatePrefab(integrationObj);		
	}
	
	[MenuItem("Help/Kochava/Documentation")]
	static void KochavaDocumentation ()
	{
		//DRAGONHERE: Need Doc URL
		Help.BrowseURL("http://kochava.com");
	}
	
}

#endif