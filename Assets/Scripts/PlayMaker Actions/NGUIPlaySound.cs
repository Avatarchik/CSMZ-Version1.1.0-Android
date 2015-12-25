// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("NGUI")]
	[Tooltip("Plays a specified sound using the NGUI framework")]
	public class NGUIPlaySound : FsmStateAction
	{	
		[RequiredField]
		[ObjectType(typeof(AudioClip))]
		[Title("Audio Clip")]
		public FsmObject audioClip;
		
		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume;
		
		[HasFloatSlider(-3f, 3f)]
		public FsmFloat pitch;
			
	
		public override void Reset()
		{
			audioClip = null;
			volume = 1f;
			pitch = 1f;
		}
	
		public override void OnEnter()
		{
			if (audioClip.Value == null)
			{
				LogWarning("Missing Audio Clip!");
				return;
			}
			else {
				NGUITools.PlaySound(audioClip.Value as AudioClip, volume.Value, pitch.Value);
			}
			
			Finish();
		}
	}
}