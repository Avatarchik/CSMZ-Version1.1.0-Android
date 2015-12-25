using UnityEngine;
using System.Collections;

public class Kochava_Demo_Cam : MonoBehaviour {
	
	public float spinSpeed = 20;
	public Color particleColorHealthy;
	public Color particleColorAiling;
	public Color particleColorIdle;
	public Color particleColorDead;
	
	public float speedHealthy;
	public float speedAiling;
	
	public ParticleSystem particleVisualizer;
	
	public int postBurst = 20;
	float lastQueueLength = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Kochava.eventQueueLength == 0)
			particleVisualizer.startColor = particleColorIdle;
		
		else if(Kochava.eventPostingTime == -1)
			particleVisualizer.startColor = particleColorDead;
		
		else
			particleVisualizer.startColor = Color.Lerp(particleColorAiling, particleColorHealthy, Mathf.Lerp(speedAiling, speedHealthy, Kochava.eventPostingTime));
		
		if(Kochava.eventQueueLength != lastQueueLength)
		{
			if(Kochava.eventQueueLength < lastQueueLength)
			{
				particleVisualizer.Emit (postBurst);
				particleVisualizer.Play ();
			}
			lastQueueLength = Kochava.eventQueueLength;
		}
		
	}
	
	void FixedUpdate ()
	{
		 transform.RotateAround (Vector3.zero, Vector3.up, spinSpeed * Time.deltaTime);
	}
}
