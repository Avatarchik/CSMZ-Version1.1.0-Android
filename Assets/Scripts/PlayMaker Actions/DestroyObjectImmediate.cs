using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]

public class DestroyObjectImmediate : DestroyObject 
{
	
	public FsmBool immediate;
	
	public override void OnEnter()
	{
		if (immediate.Value) {
			GameObject.DestroyImmediate(gameObject.Value);
		}
		else {
			base.OnEnter();
		}
	}
}
