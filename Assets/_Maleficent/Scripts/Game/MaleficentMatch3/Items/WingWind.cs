using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class WingWind : ManaItem {

	protected Transform shakeTarget;
	
	public int nrOfTilesPerTurn = 3;
	public float shakeDuration = 0.3f;
	
	public float[] delayBetweenTurns;
	
	public float postEffectWaitTime = 0.1f;
	public float preEffectWaitTime = 0.3f;

	protected int minColumn = 10000;
	protected int maxColumn = 0;
	protected Dictionary<int, List<NormalTile>> tileDictByColumns;
	protected Dictionary<int, List<NormalTile>> prioritizedTileDictByColumns;
	
	// Value between 1 and 100 (percentage)
	public int priorityListBias = 80;
	
	protected int randomTileIndex;
	
	public bool carrotBeginThumping = false;
	
	public override string ItemName
	{
		get {
			return "Wind";
		}
	}
	
	protected override void Awake()
	{
		base.Awake();
	}
	
	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);

		tileDictByColumns = new Dictionary<int, List<NormalTile>>();
		prioritizedTileDictByColumns = new Dictionary<int, List<NormalTile>>();

		shakeTarget = Match3BoardGameLogic.Instance.match3Board;
		TileSwitchInput.Instance.DisableInput();

		
		PopulateTileList();

		effectPosition = shakeTarget;
		StartItemEffects();
		DoItem();
	}
	
	public override void CancelUsingItem()
	{
		TileSwitchInput.Instance.EnableInput();	
		base.CancelUsingItem();
	}
	
	public override bool CanBeUsed()
	{
		return !BoardShuffleController.Instance.IsBoardReshuffling;
	}
	
	protected override void DoItem()
	{
		//ActuallyUsingItem();
		StartCoroutine(CarrotBehaviour());
		PlayCarrotThumpingSnd();
	}
	
	protected IEnumerator CarrotBehaviour()
	{	
		yield return new WaitForSeconds(preEffectWaitTime);
		int listBiasFactor;

		for(int turnIndex = minColumn; turnIndex <= maxColumn; turnIndex++)
		{
			if (!tileDictByColumns.ContainsKey(turnIndex))
				continue;

			float delay = delayBetweenTurns[delayBetweenTurns.Length - 1];
			if (turnIndex < delayBetweenTurns.Length) {
				delay = delayBetweenTurns[turnIndex];
			}

			yield return new WaitForSeconds(delay);
			
//			PlayCarrotThumpingSnd();
			//HOTween.Shake(shakeTarget, shakeDuration, new TweenParms().Prop("position", shakeTarget.position + Vector3.down * 0.25f), 1f, 1f);

			List<NormalTile> prioritizedTileList = null; 
			if (prioritizedTileDictByColumns.ContainsKey(turnIndex)) {
				prioritizedTileList = prioritizedTileDictByColumns[turnIndex];
			}

			List<NormalTile> tileList = null;
			if (tileDictByColumns.ContainsKey(turnIndex)) {
				tileList = tileDictByColumns[turnIndex];
			}
			
			for(int tileIndex = 0; tileIndex < nrOfTilesPerTurn; tileIndex++)
			{
				listBiasFactor = Random.Range(1, 101);

				if(prioritizedTileList != null && listBiasFactor < priorityListBias && prioritizedTileList.Count != 0)
				{
					randomTileIndex = Random.Range(0, prioritizedTileList.Count);
					prioritizedTileList[randomTileIndex].Destroy();
					prioritizedTileList.RemoveAt(randomTileIndex);
				}
				else if (tileList != null && tileList.Count > 0)
				{
					randomTileIndex = Random.Range(0, tileList.Count);
					tileList[randomTileIndex].Destroy();
					tileList.RemoveAt(randomTileIndex);
				} 
				else {
					break;
				}
			}	
		}
		
		yield return new WaitForSeconds(postEffectWaitTime);
		CharacterSpecialAnimations.instance.characterFSM.SendEvent("FinishedCustom");
		
		BaseDoItem();
		TileSwitchInput.Instance.EnableInput();
		
		DoDestroy();
	}
	
	void BaseDoItem() {
		base.DoItem();
	}
	
	protected void PopulateTileList()
	{
		Match3BoardGameLogic.Instance.boardData.ApplyActionToAll((boardPiece) => 
		                                                         {
			NormalTile tile = boardPiece.Tile as  NormalTile;

			if (tile) {

				int row = tile.BoardPiece.BoardPosition.row;
				int column = tile.BoardPiece.BoardPosition.col;
				
				minColumn = Mathf.Min(row, minColumn);
				maxColumn = Mathf.Max(column, maxColumn);


				if (IsPrioritizedTile(tile)) {
					List<NormalTile> priorTiles;

					if (!prioritizedTileDictByColumns.ContainsKey(column)) {
						priorTiles = new List<NormalTile>();
						prioritizedTileDictByColumns[column] = priorTiles;
					} else {
						priorTiles = prioritizedTileDictByColumns[column];
					}

					priorTiles.Add(tile);
				
				}

				if (tile.GetType() == typeof(NormalTile)) {
					List<NormalTile> tiles;
					if (!tileDictByColumns.ContainsKey(column)) {
						tiles = new List<NormalTile>();
						tileDictByColumns[column] = tiles;
					} else {
						tiles = tileDictByColumns[column];
					}

					tiles.Add(tile);
				}

			}
		});
	}
	
	protected bool IsPrioritizedTile(NormalTile tile)
	{
		if (tile is SnowTile || tile is FreezerTile || tile is LockedTile || tile is WolfTile || tile is SoldierTile)
		{
			return true;
		}
		
		return false;
	}
	
	protected void PlayCarrotThumpingSnd()
	{	
		if(carrotBeginThumping == false)
		{
			carrotBeginThumping = true;
			SoundManager.Instance.Play("powerup_wind_sfx");
		}
	}
}
