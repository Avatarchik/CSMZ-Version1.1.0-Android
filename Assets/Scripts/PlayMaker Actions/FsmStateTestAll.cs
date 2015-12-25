using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on whether all the specified FSMs matched their States.")]
	public class FsmStateTestAll : FsmStateAction
	{
        [CompoundArray("Tests", "Game Objects", "Compare States")]
		public FsmGameObject[] gameObjects;
		public FsmString[] compareTos;
		
		[Tooltip("All true event.")]
		public FsmEvent allTrueEvent;
		[Tooltip("Not all true event.")]
		public FsmEvent notAllTrueEvent;
		
        [Tooltip("Repeat every frame. Useful if you're waiting for a particular result.")]
		public bool everyFrame;
				
		// cach the fsm component since that's an expensive operation
        private PlayMakerFSM[] fsms;
		
		public override void Reset()
		{
			gameObjects = new FsmGameObject[1];
			compareTos = new FsmString[1];
			everyFrame = false;
			allTrueEvent = null;
			notAllTrueEvent = null;
		}

		public override void OnEnter()
		{
			DoFsmStatesTest();
			
			if (!everyFrame)
			{
			    Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFsmStatesTest();
		}
		
		void DoFsmStatesTest()
		{
			if (fsms == null) {
				fsms = new PlayMakerFSM[gameObjects.Length];
				
				for (var i = 0; i < gameObjects.Length; i++) 
				{
					fsms[i] = ActionHelpers.GetGameObjectFsm(gameObjects[i].Value, "FSM");
				}
			}
			
			bool allTrue = true;
			
			for (var i = 0; i < gameObjects.Length; i++) 
			{
				if (fsms[i].ActiveStateName != compareTos[i].Value)
				{
					allTrue = false;
					break;
				}
			}
			
			if (allTrue) {
				Fsm.Event(allTrueEvent);
			}
			else {
				Fsm.Event(notAllTrueEvent);
			}
		}
	}
}
