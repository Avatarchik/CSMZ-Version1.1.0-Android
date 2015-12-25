using UnityEngine;
using System.Collections;

public class CameraTransparencyConfig : MonoBehaviour {

	public enum TransparencySortModeType
	{
		Default,
		Ortographic,
		Perspective
	}

	public TransparencySortModeType transparencySortMode;

	// Use this for initialization
	void Start () {
		if(transparencySortMode == TransparencySortModeType.Default)
		{
			transform.GetComponent<Camera>().GetComponent<Camera>().transparencySortMode = TransparencySortMode.Default;
		}
		if(transparencySortMode == TransparencySortModeType.Ortographic)
		{
			transform.GetComponent<Camera>().GetComponent<Camera>().transparencySortMode = TransparencySortMode.Orthographic;
		}
		if(transparencySortMode == TransparencySortModeType.Perspective)
		{
			transform.GetComponent<Camera>().GetComponent<Camera>().transparencySortMode = TransparencySortMode.Perspective;
		}
	}
}
