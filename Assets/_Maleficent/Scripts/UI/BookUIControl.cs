using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BookUIControl : UIControl {

	public Camera cam;
	public CameraLimits cameraLimits;
	
	public float maxH;
	public float minH;

	class TouchInfo {
		public Vector3 reference;
		public bool moved;
	}
	
	private TouchInfo[] touchesColls;
	private Vector3 speed;
	private float speedH;
	public float thresholdH = 0.06f;

	public enum SwipeDirection
	{
		up,
		down,
		left,
		right
	}

	// minimum speed to be considered as a direction
	public float swipeThreshold = 0.2f;
	public static event System.Action<SwipeDirection> OnSwipe;

	BravoInputManager.TouchCollision collisionOnDown;
	float firstX;

	public void Start() {
		touchesColls = new TouchInfo[InputHandler.MAX_TOUCHES];
		for(int i = 0; i < touchesColls.Length; ++i) {
			touchesColls[i] = new TouchInfo();
		}

		cameraLimits.onMovementLimited += RemapTouches;
	}

	public void RemapTouches() {
		for(int i = 0; i < touchesIds.Count; ++i) {
			touchesColls[touchesIds[i]].reference = GetTouchColl(touchesIds[i]);
		}
	}
	
	public Vector3 GetTouchColl(int _idx) {
		Ray r = cam.ScreenPointToRay(new Vector3(InputHandler.TouchesById[_idx].position.x, InputHandler.TouchesById[_idx].position.y, 0.0f));
		
		RaycastHit hit;
		GetComponent<Collider>().Raycast(r, out hit, Mathf.Infinity);
		
		return r.GetPoint(hit.distance);
	}
	
	private Vector3 ProjectPoint(Vector3 _pRef, Vector3 _p) {
		Plane plane1 = new Plane(Vector3.up, _pRef);
		Ray r = new Ray(cam.transform.position, _p - cam.transform.position);
		float distance;
		plane1.Raycast(r, out distance);
		return r.GetPoint(distance);
	}

	public void Update() {
		speed = Vector3.zero;
		speedH = 0.0f;

		//Manage zoom
		bool needsRemap = false;
		if(touchesIds.Count > 1) {
			speedH = 0.0f;
			if(touchesColls[touchesIds[0]].moved || touchesColls[touchesIds[1]].moved) {
				//Get point2 collision on plane1
				Vector3 p2RefOnPlane1 = ProjectPoint(touchesColls[touchesIds[0]].reference, touchesColls[touchesIds[1]].reference);
				Vector3 p1onPlane1    = ProjectPoint(touchesColls[touchesIds[0]].reference, GetTouchColl(touchesIds[0]));
				Vector3 p2onPlane1    = ProjectPoint(touchesColls[touchesIds[0]].reference, GetTouchColl(touchesIds[1]));
				
				float dWanted = (p2RefOnPlane1 - touchesColls[touchesIds[0]].reference).magnitude;
				float d = (p2onPlane1 - p1onPlane1).magnitude;
				float factor =  d / dWanted;
				
				float h = (cam.transform.position.y - touchesColls[touchesIds[0]].reference.y) /  factor;
				speedH = ((touchesColls[touchesIds[0]].reference + Vector3.up * h) - cam.transform.position).y;
			}
		}
		if(speedH != 0.0f) {
			cam.transform.Translate(Vector3.up * speedH, Space.World);
			
			float clampedH = Mathf.Clamp(cam.transform.position.y, minH, maxH);
			if(clampedH != cam.transform.position.y) {
				cam.transform.position = new Vector3(transform.position.x, clampedH, transform.position.z);
				needsRemap = true; //Do remap after repositioning
			}
		}
		
		//Manage movement
		if(touchesIds.Count > 0) {
			speed = Vector3.zero;
			if(touchesColls[touchesIds[0]].moved || speedH != 0.0f) {
				Vector3 p1onPlane1 = ProjectPoint(touchesColls[touchesIds[0]].reference, GetTouchColl(touchesIds[0]));
				speed = touchesColls[touchesIds[0]].reference - p1onPlane1;
			}
		}

		if(Input.GetKey(KeyCode.A))
			speed = Vector3.down / 50f;
		if(Input.GetKey(KeyCode.B))
			speed = Vector3.up / 50f;

		if(speed.sqrMagnitude > 0.0f) {
			cam.transform.Translate(speed, Space.World);
		}

		if(needsRemap) {
			RemapTouches();
		}
		
		//Clear for next frame
		if(touchesColls != null)
			foreach(TouchInfo touchInfo in touchesColls)
				touchInfo.moved = false;
	}

	#region UIControl overrides
	public override void OnTouchDown(BravoInputManager.TouchCollision _collisionInfo) {
		//Debug.Log("down: "+_collisionInfo.touchInfo.collPoint.x);
		collisionOnDown = new BravoInputManager.TouchCollision(_collisionInfo.touchInfo,_collisionInfo.touchRay);
		firstX = _collisionInfo.touchInfo.collPoint.x;
		touchesColls[_collisionInfo.touchInfo.id].reference = _collisionInfo.touchInfo.collPoint;
		touchesColls[_collisionInfo.touchInfo.id].moved = false;
	}
	
	public override void OnTouchMoved(BravoInputManager.TouchCollision _collisionInfo) {
		touchesColls[_collisionInfo.touchInfo.id].moved = true;
	}

	public override void OnTouchUp(BravoInputManager.TouchCollision _collisionInfo){
		float incX = Mathf.Abs(firstX-_collisionInfo.touchInfo.collPoint.x);
		if(incX>thresholdH)
		{
			if(OnSwipe != null && cameraLimits.GetIsOutOfBounds(CameraLimits.LimitType.right))
				OnSwipe(SwipeDirection.right);
			if(OnSwipe != null && cameraLimits.GetIsOutOfBounds(CameraLimits.LimitType.left))
				OnSwipe(SwipeDirection.left);
		}
		RemapTouches();
	}
	
	public override void OnTouchCancelled() {
		RemapTouches();
	}
	#endregion
}
