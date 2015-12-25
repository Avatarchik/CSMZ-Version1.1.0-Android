using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class PhysicsController : MonoBehaviour {
	public Transform destination;
	
	public float accel = 15f;
	public float maxVel = 15f;
	private float vel = 0f;
	private Vector3 dir;
	private Vector3 heading;
	
	// Use this for initialization
	void Start () {
//		float testDot = Vector3.Dot(Vector3.up * 15f, Vector3.right * 25f);
//		Debug.Log(testDot);
		StartCoroutine(UpdateFall());
	}
	
	IEnumerator UpdateFall() {
		dir = (destination.position - transform.position).normalized;
		while(true) {
			// Limit maximum speed.
			vel = Mathf.Min(vel + accel * Time.smoothDeltaTime, maxVel);
			
			// Calculate and apply displacement.
			transform.position += dir * vel * Time.deltaTime;
			// Calculate current direction to the target
			heading = destination.position - transform.position;
			
			if (Vector3.Dot(heading, dir) <= 0f) {
				transform.position = destination.position;
				Debug.LogWarning("Reached destination!");
				yield break;
			}
			
			yield return null;
		}
	}
}
