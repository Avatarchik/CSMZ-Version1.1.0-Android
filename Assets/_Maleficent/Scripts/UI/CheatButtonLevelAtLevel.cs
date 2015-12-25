using UnityEngine;
using System.Collections;

public class CheatButtonLevelAtLevel : UIControl 
{
	void Awake()
	{
		transform.gameObject.SetActive(MaleficentTools.IsDebugBuild);
	}

	void OnClick()
	{
		int idxChapter = LoadLevelButton.lastLevelPlayedIdx + 1;
		ChaptersManager.Instance.Cheat(idxChapter);
	}
}
