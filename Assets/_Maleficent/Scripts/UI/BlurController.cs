using UnityEngine;
using System.Collections;

public class BlurController : MonoBehaviour {

	public RenderBufferSize renderBuffer;
	public GameObject background;
	public float amount;

	private Material mat;
	private float previousAmount;

	// Use this for initialization
	void Start () {
		mat = background.GetComponent<Renderer>().sharedMaterial;
	}
	
	// Update is called once per frame
	void Update () {
		if(amount != previousAmount) {
			mat.SetFloat("_Size", amount);
			renderBuffer.Size = amount;

			previousAmount = amount;
		}
	}
}
