using UnityEngine;
using System.Collections;

public class BuyManaEffect : MonoBehaviour {

	public GameObject prefabEffect;
	Transform effectTransform;

	// Use this for initialization
	void SpawnEffect () {
		//effectTransform = (Transform.Instantiate(prefabEffect) as GameObject).transform;
	}
}
