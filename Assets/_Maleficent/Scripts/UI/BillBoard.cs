using UnityEngine;
using System.Collections;

public class BillBoard : MonoBehaviour {
	
	public Transform cam;
	
	// Update is called once per frame
	//void Update () {
	///	transform.rotation = cam.rotation;
	//}
	
	void Update()
	{
		transform.LookAt(cam.transform);
	}
}
