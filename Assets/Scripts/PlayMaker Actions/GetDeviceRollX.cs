// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets the rotation of the device from its x axis.")]
	public class GetDeviceRollX : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmFloat storeAngle;
		public FsmFloat limitAngle;
		public FsmFloat smoothing;
		public bool everyFrame;
		public bool lowPass;
		
		private float lastAngle;
		private float filteredAcceleration;
		
		public override void Reset()
		{
			storeAngle = null;
			limitAngle = new FsmFloat { UseVariable = true };
			smoothing = 5f;
			everyFrame = true;
			if (lowPass) {
				filteredAcceleration = 0f;
			}
		}
		
		public override void OnEnter()
		{
			if (lowPass && everyFrame) {
				filteredAcceleration = 0f;
			}
			
			DoGetDeviceRollX();
			
			if (!everyFrame)
				Finish();
		}
		

		public override void OnUpdate()
		{
			DoGetDeviceRollX();
		}
		
		public void LowPass(ref float oldPass, float newValue, float kFilteringFactor)
		{
			oldPass = (newValue * kFilteringFactor) + (oldPass * (1.0f - kFilteringFactor));
		}
		
		void DoGetDeviceRollX()
		{
			float angle = 0;
			float x;
			
			if (lowPass && everyFrame) {
				LowPass(ref filteredAcceleration, Input.acceleration.x, smoothing.Value * Time.deltaTime);
				x = filteredAcceleration;
			}
			else {
				x = Input.acceleration.x;
			}
			
			angle = Mathf.Asin(x);
//			Debug.Log(angle);
			
			if (!limitAngle.IsNone)
			{
				angle = Mathf.Clamp(Mathf.Rad2Deg * angle, -limitAngle.Value, limitAngle.Value);
			}
			
			if (!lowPass && smoothing.Value > 0)
			{
				angle = Mathf.LerpAngle(lastAngle, angle, smoothing.Value * Time.deltaTime);
			}
			
			lastAngle = angle;
			
			storeAngle.Value = angle;
		}
		
	}
}