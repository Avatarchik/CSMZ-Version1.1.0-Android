using UnityEngine;
using System.Collections;


[AddComponentMenu("")]
public class jMouseLook : MonoBehaviour {

	public float sensitivityY = 5F;
	
	public jMobileJoystick rightJoystick;

	float rotationY = 0F;
	
	private bool desktopPlatform = false;
	
	void Start ()
	{
		desktopPlatform = (Application.platform != RuntimePlatform.IPhonePlayer) && (Application.platform != RuntimePlatform.Android);
	}
	
	void Update () {
		// Compute rotation Y delta
		float delta;
		
		if (desktopPlatform)
			delta = -Input.GetAxis("Mouse Y") * sensitivityY;
		else
			delta = -rightJoystick.position.y * sensitivityY;
		
		// Save previous rotation and apply delta
		float previousRotationY = rotationY;
		rotationY = transform.localEulerAngles.x + delta;
		
		// Poor man's clamping
		if (previousRotationY <= 60F && delta > 0F) {
			if (rotationY > 60F)
				rotationY = 60F;
		} else if (previousRotationY <= 60F && delta < 0F) {
			if (rotationY < 0F) {
				rotationY += 360F;
			
				if (rotationY < 300F)
					rotationY = 300F;
			}
		} else if (previousRotationY >= 300F && delta < 0F) {
			if (rotationY < 300F)
				rotationY = 300F;
		} else if (previousRotationY >= 300F && delta > 0F) {
			if (rotationY > 360F) {
				rotationY -= 360F;
			
				if (rotationY > 60F)
					rotationY = 60F;
			}
		}
		
		// Apply
		transform.localEulerAngles = new Vector3(rotationY, transform.localEulerAngles.y, 0);
	}
	
}