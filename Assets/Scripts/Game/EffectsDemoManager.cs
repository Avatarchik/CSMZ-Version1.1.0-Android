using UnityEngine;
using System.Collections.Generic;

public class EffectsDemoManager : MonoBehaviour {
	public Match3BoardGameLogic gameLogic;
	
	protected float guiWidth = 220;
	protected float buttonsHeight = 45f;
	
	public GameObject[] effectsPrefabs;
	protected List<GameObject> effectInstances = new List<GameObject>(20);
	
	protected bool showEffects = false;
	
	protected Vector2 scrollPos = new Vector2(0f, 0f);
	
	
	// Use this for initialization
	void Start () {
	}
	
	void OnGUI() {
		
		GUILayout.BeginArea(new Rect(Screen.width - guiWidth, 0, guiWidth, Screen.height)); {
			GUILayout.BeginVertical(); {
				if ( GUILayout.Button("MatchesCheck", GUILayout.Height(buttonsHeight)) ) {
					gameLogic.matchesFinder.FindMatches();
					gameLogic.DestroyLastFoundMatches();
				}
				
				if (showEffects) {
					scrollPos = GUILayout.BeginScrollView(scrollPos); {
						if (GUILayout.Button("Hide effects", GUILayout.Height(buttonsHeight))) {
							showEffects = false;
						}
					
						for(int i = 0; i < effectsPrefabs.Length; i++) { 
							if ( GUILayout.Button(effectsPrefabs[i].name, GUILayout.Height(buttonsHeight)) ) {
								GameObject newEffect = Instantiate(effectsPrefabs[i]) as GameObject;
								if ( !newEffect.activeInHierarchy ) {
									newEffect.SetActive(true);
								}
								newEffect.name = newEffect.name.Substring(0, newEffect.name.IndexOf("(Clone)"));
								effectInstances.Add(newEffect);
							}
						}
						
						GUILayout.Space(10f);
						if ( GUILayout.Button("Clear All", GUILayout.Height(buttonsHeight)) ) {
							for(int i = 0; i < effectInstances.Count; i++) {
								Destroy(effectInstances[i]);
							}
							effectInstances.Clear();
						}
						
						for(int i = 0; i < effectInstances.Count; i++) {
							if (effectInstances[i] != null) {
								if ( GUILayout.Button("Destroy: " + effectInstances[i].name, GUILayout.Height(buttonsHeight)) ) {
									GameObject toDestroy = effectInstances[i];
									effectInstances.Remove(toDestroy);
									Destroy(toDestroy);
								}
							} else {
								effectInstances.RemoveAt(i);
								i--;
							}
						}
					}
					GUILayout.EndScrollView();
				}
				else {
					if (GUILayout.Button("Show effects", GUILayout.Height(buttonsHeight))) {
						showEffects = true;
					}
				}
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
		}
	}
}
