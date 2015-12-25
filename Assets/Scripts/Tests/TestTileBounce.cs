using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class TestTileBounce : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		NormalTile normalTile = GetComponent<NormalTile>();

//		normalTile.tileModelTransform.localPosition += Vector3.up * normalTile.fallBouncePower;
		HOTween.To(normalTile.tileModelTransform, normalTile.fallBounceStiffness, new TweenParms()
						 .Prop("localPosition", Vector3.up * normalTile.fallBouncePower)
						 .Ease(EaseType.EaseOutSine)//, amplit, period)
					  );
		
		yield return new WaitForSeconds(normalTile.fallBounceStiffness * 0.5f);
		
		Tweener fallBounceAnimTweener = HOTween.To(normalTile.tileModelTransform, normalTile.fallBounceStiffness, new TweenParms()
						 .Prop("localPosition", normalTile.tileModelLocalPos)
						 .Ease(EaseType.EaseOutBounce)//, amplit, period)
					  );
		
//		yield return new WaitForSeconds(0.1f);
//		fallBounceAnimTweener.Play();
//		fallBounceAnimTweener.Kill();
	}
	
}
