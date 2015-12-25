using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class DataCheater : MonoBehaviour{

	private static DataCheater instance;

	public static DataCheater Instance {
		get {
			if(instance == null) {
				instance = new DataCheater();
			}
			
			return instance;
		}
	}

	public bool allLevelsUnlocked;

	[System.Serializable]
	public class LevelResult
	{
		public int score = 5000;
		public int stars = 3;
	}

	public int currentLevel;
	//public List<LevelResult> levelResults;

	public static void DoReset()
	{
		if(!Application.isPlaying)
		{
			Debug.Log("Reset will be done when starting");
			PlayerPrefs.SetInt("reset",1);
		}
		else
		{
			UserManagerCloud.Instance.DeleteUserFromCloud();
			UserManagerCloud.Instance.ResetLocalUser();
			TokensSystem.Instance.Reset();
			Debug.Log("reset done successfully!");
		}
	}

	// Use this for initialization
	void Awake ()
	{
		if(Application.loadedLevelName == "MapBook")
		{
			int resetEnabled = PlayerPrefs.GetInt("reset",0);
			if(resetEnabled == 1)
			{
				PlayerPrefs.DeleteAll();
				UserManagerCloud.Instance.DeleteUserFromCloud();
				UserManagerCloud.Instance.ResetLocalUser();
				TokensSystem.Instance.Reset();

				Debug.Log("reset done successfully!");
			}

			int levelsUnlocked = PlayerPrefs.GetInt("unlockLevels",0);
			if(levelsUnlocked != 0)
			{
				for(int i=1;i<levelsUnlocked;i++)
				{
					UserManagerCloud.Instance.SetScoreForLevel(i, 5000, 3);
				}
				
				Debug.Log("reset done successfully!");
			}
			PlayerPrefs.SetInt("reset",0);
			PlayerPrefs.SetInt("unlockLevels",0);
			PlayerPrefs.Save();
		}
	}
}