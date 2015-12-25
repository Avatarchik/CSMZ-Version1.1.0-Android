using UnityEngine;
using System.Collections;

public class QualityDownMaterials : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(QualitySettings.GetQualityLevel() == 0) {
			Shader unlitTexture = Shader.Find("Unlit/Texture");
			Shader unlitTransparent = Shader.Find("Unlit/Transparent");
			Shader unlitTransparentCutOut = Shader.Find("Unlit/Transparent Cutout");

			Renderer[] renderers = gameObject.GetComponentsInChildren< Renderer >();
			foreach(Renderer r in renderers) {
				for(int i = 0; i < r.materials.Length; ++i) {
					Material m = r.materials[i]; //Material duplicated here
					if (m.shader.name == "Self-Illumin/Diffuse") {
						m.shader = unlitTexture;
					} else if (m.shader.name == "_Maleficent/HalfLambert") {
						m.shader = unlitTexture;
					} else if (m.shader.name == "_Maleficent/HalfLambert Transparent") {
						m.shader = unlitTransparent;
					} else if (m.shader.name == "Diffuse" || m.shader.name == "Mobile/Diffuse") {
						m.shader = unlitTexture;
					} else if (m.shader.name == "Transparent/Cutout/Diffuse") {
						m.shader = unlitTransparentCutOut;
					}
				}
			}
		}
	}
}
