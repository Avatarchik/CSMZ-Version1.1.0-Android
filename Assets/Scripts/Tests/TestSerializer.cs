using UnityEngine;
using System.Collections;

public class TestSerializer : MonoBehaviour {
	public Match3Tile targetTile;
		
	// Use this for initialization
	void Start () {
		targetTile.InitComponent();
		
		SerializerUtils.WriteToBinaryStream(targetTile, null);
	}
}
