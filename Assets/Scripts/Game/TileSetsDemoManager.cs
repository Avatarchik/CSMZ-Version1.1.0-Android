using UnityEngine;
using System.Collections.Generic;

public class TileSetsDemoManager : MonoBehaviour {
	protected float guiWidth = 150;
	protected float buttonsHeight = 45f;
	
	public Material[] materials;
	public List<TileSet> tileSets;
		
	public bool showTileSets = false;
	
	void OnGUI() 
	{
		if (showTileSets) {
			GUILayout.BeginArea(new Rect(0, Screen.height - (buttonsHeight + 3) * (tileSets.Count + 1), guiWidth, (buttonsHeight + 3) * (tileSets.Count + 1))); {
				if (GUILayout.Button("Hide tile sets", GUILayout.Height(buttonsHeight))) {
					showTileSets = false;
				}
			
				for(int i = 0; i < tileSets.Count; i++) { 
					if ( GUILayout.Button("TileSet " + (i + 1), GUILayout.Height(buttonsHeight)) ) {
						for(int j = 0; j < materials.Length; j++) { 
							materials[j].mainTexture = tileSets[i].textures[j];
						}
					}
				}
				
				GUILayout.EndArea();
			}		
		}
		else {
			GUILayout.BeginArea(new Rect(0, Screen.height - buttonsHeight - 3, guiWidth, buttonsHeight)); {
				if (GUILayout.Button("Show tile sets", GUILayout.Height(buttonsHeight))) {
					showTileSets = true;
				}
				
				GUILayout.EndArea();
			}
		}
	}
}

[System.Serializable]
public class TileSet {
	public Texture[] textures;
}