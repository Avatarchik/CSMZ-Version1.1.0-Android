using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("DisneyAnalytics")]
	public class AnalyticsPageViewEvent : FsmStateAction
	{	
		public FsmString location;
		public FsmString pageUrl;
		public FsmString message;
			
		public override void Reset()
		{
			location = "";
			pageUrl = "";
			message = "";
		}
	
		public override void OnEnter()
		{
			AnalyticsBinding.LogEventPageView(location.Value, pageUrl.Value, message.Value);

			Finish();
		}
	}
}