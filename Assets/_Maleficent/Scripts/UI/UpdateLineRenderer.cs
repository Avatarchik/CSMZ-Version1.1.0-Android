using UnityEngine;
using System.Collections;

public class UpdateLineRenderer : MonoBehaviour
{
	private GameObject srcGO;
	private GameObject dstGO;
	private LineRenderer lineRenderer;

	// Use this for initialization
	public void Init (GameObject srcGO,GameObject dstGO)
	{
		this.srcGO = srcGO;
		this.dstGO = dstGO;
		lineRenderer = this.transform.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(lineRenderer == null || srcGO == null || dstGO == null)
			return;

		lineRenderer.SetPosition(0,new Vector3(srcGO.transform.position.x,srcGO.transform.position.y,srcGO.transform.position.z+0.01f));
		lineRenderer.SetPosition(1,new Vector3(dstGO.transform.position.x,dstGO.transform.position.y,dstGO.transform.position.z+0.01f));
	}
}
