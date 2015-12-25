// RigidbodyFPSWalker
// http://www.unifycommunity.com/wiki/index.php?title=RigidbodyFPSWalker
// Don't know who to credit
// Slightly modified

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
[AddComponentMenu("")]
public class jRigidbodyFPSController : MonoBehaviour
{
	public float speed = 10.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	public float rotationSensitivity = 15f;
	public jMobileJoystick leftJoystick;
	public jMobileJoystick rightJoystick;
	public Collider groundCollider;

	private bool grounded = false;
	private bool desktopPlatform = false;
	private float mass = 0.0f;
	private Vector3 gravityVector;
	private Vector3 targetVelocity;

	void Start ()
	{
		GetComponent<Rigidbody>().freezeRotation = true;
		GetComponent<Rigidbody>().useGravity = false;
		mass = GetComponent<Rigidbody>().mass;
		gravityVector = new Vector3 (0.0f, -gravity * mass, 0.0f);
		targetVelocity = new Vector3(0.0f, 0.0f, 0.0f);
		desktopPlatform = (Application.platform != RuntimePlatform.IPhonePlayer) && (Application.platform != RuntimePlatform.Android);
	}

	void FixedUpdate ()
	{
		if (grounded) {
			// Calculate how fast we should be moving
			if (desktopPlatform) {
				targetVelocity[0] = Input.GetAxis ("Horizontal");
				targetVelocity[1] = 0.0f;
				targetVelocity[2] = Input.GetAxis ("Vertical");
			} else {
				targetVelocity[0] = leftJoystick.position.x;
				targetVelocity[1] = 0.0f;
				targetVelocity[2] = leftJoystick.position.y;
			}
			
			targetVelocity = transform.TransformDirection (targetVelocity);
			targetVelocity *= speed;
			
			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = GetComponent<Rigidbody>().velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			GetComponent<Rigidbody>().AddForce (velocityChange, ForceMode.VelocityChange);
			
			// Jump
			if (canJump && Input.GetButton ("Jump")) {
				GetComponent<Rigidbody>().velocity = new Vector3 (velocity.x, Mathf.Sqrt (2.0f * jumpHeight * gravity), velocity.z);
			}
			
			// X-Axis rotation
			if (desktopPlatform)
				transform.Rotate (0, Input.GetAxis ("Mouse X") * rotationSensitivity, 0);
			else
				transform.Rotate (0, rightJoystick.position.x * rotationSensitivity, 0);
		}
		
		// We apply gravity manually for more tuning control
		GetComponent<Rigidbody>().AddForce (gravityVector, ForceMode.Force);
	}

	void OnCollisionEnter (Collision collision)
	{
		if (collision.collider == groundCollider)
			grounded = true;
	}
	
	void OnCollisionExit(Collision collision) {
		if (collision.collider == groundCollider)
			grounded = false;
	}
}
