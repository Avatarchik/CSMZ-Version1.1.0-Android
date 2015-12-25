using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on whether any of the specified FSMs matches their States.")]
	public class FsmStateTestAny : FsmStateAction
	{
        [CompoundArray("Tests", "Game Objects", "Compare States")]
		public FsmGameObject[] gameObjects;
		public FsmString[] compareTos;
		
		[Tooltip("All true event.")]
		public FsmEvent allTrueEvent;
		[Tooltip("Any true event.")]
		public FsmEvent anyTrueEvent;
		[Tooltip("None true event.")]
		public FsmEvent noneTrueEvent;
		
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
			anyTrueEvent = null;
			noneTrueEvent = null;
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
			bool anyTrue = false;
			
			for (var i = 0; i < gameObjects.Length; i++) 
			{
				if (fsms[i].ActiveStateName != compareTos[i].Value)
				{
					allTrue = false;
				} 
				else {
					anyTrue = true;
				}
				
				if (!allTrue && anyTrue) {
					break;
				}
			}
			
			if (allTrue) {
				Fsm.Event(allTrueEvent);
			}
			else if (anyTrue) {
				Fsm.Event(anyTrueEvent);
			}
			else {
				Fsm.Event(noneTrueEvent);
			}
		}
	}
}
