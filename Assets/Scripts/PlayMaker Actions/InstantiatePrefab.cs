// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Creates a Game Object from the Prefab specified by the path.")]
	public class InstantiatePrefab : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Path to the prefab, relative to Resources folder.")]
		public FsmString prefabPath;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		public override void Reset()
		{
			prefabPath = null;
			storeObject = null;
		}

		public override void OnEnter()
		{
			var go = Resources.Load(prefabPath.Value) as GameObject;
			
			if (go != null)
			{
				GameObject newObject = GameObject.Instantiate(go) as GameObject;
                storeObject.Value = newObject;
			}
			
			Finish();
		}
	}
}
