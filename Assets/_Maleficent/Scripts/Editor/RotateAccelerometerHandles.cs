using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RotateAccelerometer2))]
public class RotateAccelerometerHandles : Editor {

	public void OnSceneGUI() {
		RotateAccelerometer2 t = ((RotateAccelerometer2)target);

		UpdateXLimit(ref t.leftLimit);
		UpdateXLimit(ref t.rightLimit);
	}

	private void UpdateXLimit(ref Vector2 _limit) {
		RotateAccelerometer2 t = ((RotateAccelerometer2)target);

		Vector3 newPos = Handles.PositionHandle(new Vector3(_limit.x, t.transform.position.y, _limit.y), Quaternion.identity);
		_limit.x = newPos.x;
		_limit.y = newPos.z;
	}
}
