using UnityEngine;
using System.Collections;
using System;

public class MapTextureSetter : MonoBehaviour {

	bool isBackground = false;

	// Use this for initialization
	void Start () {
		isBackground = transform.name.Contains("background");

		string strChapter = transform.parent.name.Substring(0,2);
		int numChapter = Convert.ToInt32(strChapter);

		// clear up unused resources
		int numTextures = GetComponent<MeshRenderer>().materials.Length;
		for(int t=1;t<=numTextures;++t)
		{
			string textureId = "Map/Textures/chapter_"+strChapter;
			textureId += isBackground?"_alpha_0":"_element_0";
			textureId += t;
			Texture tex = Resources.Load(textureId) as Texture;
			GetComponent<MeshRenderer>().materials[t-1].SetTexture("_MainTex",tex);
		}
	}
}
