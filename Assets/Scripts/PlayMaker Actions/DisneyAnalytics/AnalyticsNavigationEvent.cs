using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("DisneyAnalytics")]
	public class AnalyticsNavigationEvent : FsmStateAction
	{	
		public FsmString buttonPressed;
		public FsmString fromLocation;
		public FsmString toLocation;
		public FsmString targetUrl;

			
		public override void Reset()
		{
			buttonPressed = "";
			fromLocation = "";
			toLocation = "";
			targetUrl = "";
		}
	
		public override void OnEnter()
		{
			AnalyticsBinding.LogEventNavigationAction(buttonPressed.Value, fromLocation.Value, toLocation.Value, targetUrl.Value);

			Finish();
		}
	}
}