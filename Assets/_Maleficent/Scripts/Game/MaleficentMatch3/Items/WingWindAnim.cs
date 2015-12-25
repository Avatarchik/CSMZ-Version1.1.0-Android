using UnityEngine;
using System.Collections;

public class WingWindAnim : MonoBehaviour {

	public MeshRenderer[] mr;
	
	public float t;
	private Material[] mat;
	
	
	// Use this for initialization
	void Start () {
		mat = new Material[mr.Length];
		for(int i = 0; i < mat.Length; ++i) {
			mat[i] = mr[i].GetComponent<Renderer>().material;
		}
		Update();
	}
	
	// Update is called once per frame
	void Update () {
		foreach(Material m in mat) {
			m.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, t));
		}
	}
}
