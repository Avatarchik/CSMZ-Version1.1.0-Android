using UnityEngine;
using System.Collections;

public class Checker : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	float count = 0;
	
	// Update is called once per frame
	void Update () {
		count += Time.deltaTime;
		
		if (count > 3)
		{
			Debug.Log("============================================================");
			Debug.Log(" Alive " + UserManagerCloud.Instance.gameObject.activeSelf);
			Debug.Log("============================================================");
			count = 0;
		}
	}
}
