using UnityEngine;
using System.Collections;

public class OrientationMaterialChanger : MonoBehaviour 
{
	public Material materialLandscape;
	public Material materialPortrait;
	public Renderer objectRenderer;
	public int materialIndex;
	
	Material[] materials;
	
	void Awake()
	{
		if (Screen.width >= Screen.height) {
			OrientationChanged(ScreenOrientation.Landscape);
		}
		else {
			OrientationChanged(ScreenOrientation.Portrait);
		}
	}
	
	void Start()
	{
		OrientationListener.Instance.OnOrientationChanged += OrientationChanged;
	}
	
	void OrientationChanged(ScreenOrientation newOrientation)
	{
//		Debug.LogWarning("new orientation: " + newOrientation + " material: " + materialIndex);
		if (newOrientation == ScreenOrientation.Landscape) {
			ChangeMaterial(materialLandscape, materialIndex);
		}
		else if (newOrientation == ScreenOrientation.Portrait) {
			ChangeMaterial(materialPortrait, materialIndex);
		}
	}
	
	void ChangeMaterial(Material newMaterial, int index)
	{
		materials = objectRenderer.materials;
		materials[index] = newMaterial;
		objectRenderer.materials = materials;
	}
}
