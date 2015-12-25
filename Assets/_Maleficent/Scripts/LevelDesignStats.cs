using UnityEngine;
using System.Collections;

public class LevelDesignStats : MonoBehaviour
{

	bool powerupUsed = false;
 
	void Start ()
	{
		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
		gameLogic.winConditions.OnWinChecked += HandleOnWinChecked;
		gameLogic.loseConditions.OnLoseChecked += HandleOnLoseChecked;
		BasicItem.OnActuallyUsingAnyItem += HandleOnActuallyUsingAnyItem;
	}

	void Unsubscribe ()
	{
		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
		gameLogic.winConditions.OnWinChecked -= HandleOnWinChecked;
		gameLogic.loseConditions.OnLoseChecked -= HandleOnLoseChecked;
		BasicItem.OnStartUsingAnyItem -= HandleOnActuallyUsingAnyItem;
	}

	void HandleOnLoseChecked ()
	{
		Unsubscribe ();
		if (!powerupUsed) {
			StartCoroutine (SendStats ("Lose"));
		}
	}

	void HandleOnWinChecked ()
	{
		Unsubscribe ();
		if (!powerupUsed) {
			StartCoroutine (SendStats ("Win"));
		}
	}

	void HandleOnActuallyUsingAnyItem (BasicItem item)
	{
		powerupUsed = true;
	}

	IEnumerator SendStats (string result)
	{
		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;

		LoseMoves loseMoves = gameLogic.loseConditions as LoseMoves;
		WinScore winScore = gameLogic.winConditions as WinScore;

		int startMoves = loseMoves.StartMoves;
		int remainingMoves = loseMoves.RemainingMoves;
		int playerScore = ScoreSystem.Instance.Score;
		int target1Star = winScore.targetScore;
		int target2Stars = winScore.targetScore2Stars;
		int target3Stars = winScore.targetScore3Stars;

		//int level = MaleficentBlackboard.Instance.level;
		string level = winScore.transform.parent.name; 
		string levelParameter = "";
		
		if (winScore.GetType () == typeof(WinDestroyLightPieces)) {
			WinDestroyLightPieces winLightPieces = winScore as WinDestroyLightPieces;
			levelParameter += winLightPieces.notLightenPieces.ToString();
		}

		if (winScore.GetType () == typeof(WinDestroyLockedTiles)) {
			WinDestroyLockedTiles winLockedPieces = winScore as WinDestroyLockedTiles;	 
			int sum =0;
			foreach (DestroyTilesPair destroy in winLockedPieces.destroyTiles) {
				sum += destroy.number - destroy.current;
			}
			levelParameter += sum.ToString();
		}

		string identifer = SystemInfo.deviceName.Replace(" ","");
		
		string url = "http://apppush.interactivecdn.com:8080/Push/get?t="+ identifer +"&bd=Maleficent&id=level,startMoves,remainingMoves,playerScore,target1Star,target2Stars,target3Stars,result,levelParameter&t=Maleficent&p1="+ level + "," + startMoves + "," + remainingMoves + "," + playerScore + "," + target1Star + "," + target2Stars + "," + target3Stars + "," + result + ","+levelParameter+"&p2=BETA3";

		Debug.Log ("=================== " + url);
		WWW www = new WWW (url);

		yield return www;

		Debug.Log (www.text);

	}
		

}
