using UnityEngine;
using System.Collections;

public class ARScaler : MonoBehaviour {
	float originalYScale;

	// Use this for initialization
	void Start () {
		originalYScale = transform.localScale.y;	
	}
	
	// Update is called once per frame
	void Update () {
		float ar = (float)Screen.width/(float)Screen.height;
		Debug.Log("ar: "+ar);
		//transform.localScale = new Vector3(transform.localScale.x,1700f/(transform.localScale.y*ar),transform.localScale.z);
		//float myScaleFactor = screenWidth / referenceWidth;
		//cachedTransform.localScale = originalScale * Mathf.Clamp(dpiScaleFactor / myScaleFactor, minScaleFactor, maxScaleFactor);
	}
}
