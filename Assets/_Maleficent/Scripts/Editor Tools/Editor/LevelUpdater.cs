using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelUpdater : MonoBehaviour {

	//[MenuItem ("Project Tools/Refresh Level")]
	static void  RefreshLevel() {
		Match3BoardRenderer board = GameObject.FindObjectOfType< Match3BoardRenderer >();
		RefreshLevel(board);
	}

	static void  RefreshLevel(Match3BoardRenderer _board) {
		//Cache all children first (we are going to remove while iterating, so we need a copy)
		Transform[] cachedChildren = new Transform[_board.transform.childCount];
		for(int i = 0; i < cachedChildren.Length; ++ i) {
			cachedChildren[i] = _board.transform.GetChild(i);
		}

		foreach(Transform child in cachedChildren){
			//check name with board pieces
			foreach(GameObject piece in _board.prefabsPieces) {
				if(child.name.Contains(piece.name)) {
					RefreshPiece(child.gameObject, piece);
					break;
				}
			}
		}

		foreach(Transform child in cachedChildren){
			//check name with tile pieces
			foreach(GameObject tile in _board.tilesPrefabs) {
				if(child.name.Contains(tile.name)) {
					RefreshPiece(child.gameObject, tile);
					break;
				}
			}
		}
	}

	static void RefreshPiece(GameObject _piece, GameObject _prefab) {
		GameObject newPiece = GameObject.Instantiate(_prefab) as GameObject;
		newPiece.name = _piece.name;
		newPiece.transform.parent = _piece.transform.parent;
		newPiece.transform.localPosition = _piece.transform.localPosition;
		newPiece.transform.localRotation = _piece.transform.localRotation;
		newPiece.transform.localScale = _piece.transform.localScale;

		MonoBehaviour[] components = _piece.GetComponents< MonoBehaviour >();
		foreach(MonoBehaviour component in components) { 
			CopyComponent(component, newPiece);
		}

		GameObject.DestroyImmediate(_piece);
	}

	static T CopyComponent< T >(T _source, GameObject _target) where T : Component {
		System.Type type = _source.GetType();
		T t = _target.GetComponent< T >();
		if(t == null) {
			t = _target.AddComponent< T >();
		}

		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields) {
			if(field.IsPublic && !field.IsNotSerialized) {
				field.SetValue(t, field.GetValue(_source));
			}
		}

		return t;
	}
}
