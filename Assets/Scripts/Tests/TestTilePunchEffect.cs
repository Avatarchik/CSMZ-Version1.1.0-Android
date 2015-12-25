using UnityEngine;
using System.Collections;

public class TestTilePunchEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		yield return new WaitForSeconds(1f);
		
		NormalTile tile = GetComponent<NormalTile>();
		tile.ApplyLateUpdatePunchEffect(Vector3.up - Vector3.right);
	}	
}
