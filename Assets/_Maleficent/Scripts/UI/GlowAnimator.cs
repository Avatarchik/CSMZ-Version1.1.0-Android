using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class GlowAnimator : MonoBehaviour {

	Vector3 originalScale;
	void Awake()
	{
		originalScale = transform.localScale;
	}

	// Use this for initialization
	void Start ()
	{
		HOTween.To(this.transform,1f, new TweenParms()
		           .Prop("localScale",originalScale*0.8f)
		           .Ease(EaseType.EaseInOutSine)
		           .Delay(0.3f)
		           .Loops(-1,LoopType.Yoyo)
		           );
	}
}