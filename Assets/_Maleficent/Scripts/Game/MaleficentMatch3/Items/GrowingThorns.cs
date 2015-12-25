using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class GrowingThorns : ManaItem {

	public GameObject prefabSelectionEffect;
	public GameObject prefabGrowingThorns; //Use normal effect for this!! (it was used for the torch, but now we can)

	public float selectionAnimationSpeed = 1f;
	
	public int maxNumberOfSelections = 10;

	public float selectionEffectDestroyDelay = 1.0f;

	protected int critNumberOfSelections;
	
	protected BoardPieceTouchController touchController;
	
	protected List<AbstractBoardPiece> selectionList;
	
	protected int selectionCount = 0;
	
	protected List<Transform> selectionEffectList;
	
	private List< GameObject > thorns = new List< GameObject >();
	
	protected override void Awake()
	{
		base.Awake();
		
		critNumberOfSelections = maxNumberOfSelections * 2;
		
		selectionList = new List<AbstractBoardPiece>(10);
		selectionEffectList = new List<Transform>();
		selectionCount = 0;
		
		touchController = GetComponent<BoardPieceTouchController>();
		touchController.OnNewBoardPieceSelected += OnNewBoardPieceSelected;
	}
	
	public override string ItemName {
		get {
			return "GrowingThorns";
		}
	}
	
	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		
		PossibleMatchesController.Instance.EnabledCount--;
		TileSwitchInput.Instance.DisableInput();
	}
	
	public override void CancelUsingItem()
	{	
		RemoveAllSelections();
		
		PossibleMatchesController.Instance.EnabledCount++;
		TileSwitchInput.Instance.EnableInput();
		
		base.CancelUsingItem();
	}
	
	protected void DoItemCoroutine()
	{
		//Start animation
		PlayMakerFSM characterFSM = GameObject.FindObjectOfType< CharacterSpecialAnimations >().GetComponent< PlayMakerFSM >();
		characterFSM.SendEvent("PauseAnimation");
		characterFSM.SendEvent(animState);

		//Activate smoke
		((BackgroundLoader)GameObject.FindObjectOfType(typeof(BackgroundLoader))).powerUpSmoke.Show();
		
		ActuallyUsingItem();
		
		SoundManager.Instance.PlayOneShot("powerup_growingthorns_sfx");
		//AvatarParticles.Instance.ActivateEffect(AvatarParticlesEffectType.GrowingThorns);
		
		//Lock boardPieces
		Match3BoardGameLogic.Instance.boardData.ApplyActionToAll( (boardPiece) => {
			(boardPiece as Match3BoardPiece).BlockCount++;
		});

		//Destroy selection
		StartCoroutine(DestroySelectionDelayed(selectionEffectDestroyDelay));
		selectionCount = 0;

		//Destroy tile
		MaleficentTools.DoAfterSeconds(this, 1.0f, () => {
			StartCoroutine(GrowThorn(0));
		});
	}
	
	protected IEnumerator GrowThorn(int _idx)
	{
		AbstractTile tile = selectionList[_idx].Tile;
		
		
		//Grow thorn
		thorns.Add(GameObject.Instantiate(prefabGrowingThorns) as GameObject);
		Transform t = thorns[thorns.Count - 1].transform;
		t.position = MaleficentTools.ConvertPositionBetweenLayers(selectionList[_idx].transform.position, "Default", "Effects", 12.0f);
		t.Rotate(Vector3.forward, UnityEngine.Random.Range(0.0f, 360.0f), Space.World);

		if(tile != null) {
			Vector3 cachedPos = tile.transform.position;
			int cachedLayer = tile.gameObject.layer;
			//Change the tile to be destroyed into the effects layer so that the thorn goes throw it
			tile.transform.position = MaleficentTools.ConvertPositionBetweenLayers(tile.transform.position, LayerMask.LayerToName(tile.gameObject.layer), "Effects", 11.25f);
			MaleficentTools.ChangeLayerRecursively(tile.gameObject, "Effects");
			//tile.GetComponent< AbstractTile >().enabled = false;

			//Restore tile position if it exists (coroutine is added on it) after 2 seconds
			MaleficentTools.DoAfterSeconds( tile, 2.0f, () => {
				tile.transform.position = cachedPos;
				MaleficentTools.ChangeLayerRecursively(tile.gameObject, LayerMask.LayerToName(cachedLayer));
			});
		}


		MaleficentTools.DoAfterSeconds(this, 0.6f,
			() => {
				if(tile != null) {
					selectionList[_idx].Tile.Destroy();
				}
				
				if(_idx == selectionList.Count - 1)
				{	
					MaleficentTools.DoAfterSeconds(this, 0.5f, () => {
						PossibleMatchesController.Instance.EnabledCount++;
						
						Match3BoardGameLogic.Instance.boardData.ApplyActionToAll( (boardPiece) => {
							(boardPiece as Match3BoardPiece).BlockCount--;
						});
						
						Match3BoardGameLogic.Instance.boardData.ApplyActionToAll( (boardPiece) => { 
							(boardPiece as Match3BoardPiece).UpdateOrphanState();
						});
						
						//Destroy thorns
						foreach(GameObject thorn in thorns) {
							thorn.transform.GetChild(0).GetComponent<Animation>().Play("fadeout");
							Destroy(thorn, 0.5f);
						}
						
						BaseDoItem();
						TileSwitchInput.Instance.EnableInput();
						DoDestroy();

						//Deactivate smoke
						((BackgroundLoader)GameObject.FindObjectOfType(typeof(BackgroundLoader))).powerUpSmoke.Hide();
					});
				}
			}
		);


		if(_idx != selectionList.Count - 1)
		{
			yield return new WaitForSeconds(0.1f);
			StartCoroutine(GrowThorn(_idx + 1));
		}
	}
	
	private void BaseDoItem() {
		base.DoItem();
	}

	protected void OnNewBoardPieceSelected(AbstractBoardPiece boardPiece, CustomInput.TouchInfo touchInfo)
	{	
		if (IsRunning)
		{
			return;
		}
		
		if (BoardShuffleController.Instance.IsBoardReshuffling) 
		{
			touchController.ClearLastSelection();
			return;
		}
		
		bool isLinked = false;
		AbstractBoardPiece lastSelection = null;
		
		//Cache reference to the last selection
		if(selectionList.Count > 0)
		{
			lastSelection = selectionList[selectionList.Count-1];
		}
		
		//Undo selection
		if (selectionList.Count >= 2 && selectionList[selectionList.Count-2] == boardPiece)
		{	
			RemoveSelection(selectionList.Count-1);
			return;
		}
		
		//Check link between new and last selection
		if (lastSelection) {
			isLinked = lastSelection.IsAdjacentTo(boardPiece);
		}
		
		//Add current tile to the selection or start a new one
		if(selectionCount < maxNumberOfSelections) // && !selectionList.Contains(boardPiece)
		{
			if(!isLinked) 
			{
				if(selectionList.Contains(boardPiece))
				{
					return;
				}
				RemoveAllSelections();
			}

			AddSelection(boardPiece);
		}
		
		if(selectionCount == maxNumberOfSelections || selectionList.Count == critNumberOfSelections)
		{
			touchController.StopInputController();
			DoItemCoroutine();
			IsRunning = true;
		}
	}
	
	protected void AddSelection(AbstractBoardPiece boardPiece)
	{
		Transform selection = null;
		
		if(!selectionList.Contains(boardPiece))
		{
			selection = (GameObject.Instantiate(prefabSelectionEffect) as GameObject).transform;
			
			selection.parent = boardPiece.cachedTransform;
			selection.localPosition = new Vector3(0f, 0f, -1f);
			selection.GetComponent<Animation>()["effect_nextitem3"].speed = selectionAnimationSpeed;
			
			if ( !(boardPiece is EmptyBoardPiece) || boardPiece.Tile != null )
			{
				selectionCount++;
			}	
		}
		
		selectionEffectList.Add(selection);
		selectionList.Add(boardPiece);
		
	}
	
	protected void RemoveSelection(int targetIndex)
	{	
		if(selectionEffectList[targetIndex])
		{	
			GameObject.Destroy(selectionEffectList[targetIndex].gameObject);         

			if (!selectionList[targetIndex].IsEmpty) {
				selectionCount--;
			}
		}

		selectionList.RemoveAt(targetIndex);
		selectionEffectList.RemoveAt(targetIndex);
	}
	
	protected void RemoveAllSelections()
	{	
		for( int i = 0; i < selectionList.Count; i++)
		{			
			if(selectionEffectList[i])
			{
				GameObject.Destroy(selectionEffectList[i].gameObject);
			}
		}
		
		selectionEffectList.Clear();
		selectionList.Clear();
		selectionCount = 0;
	}

	private IEnumerator DestroySelectionDelayed(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);

		for(int i = 0; i < selectionEffectList.Count; i++)
		{
			if(selectionEffectList[i] != null)
			{
				GameObject.Destroy(selectionEffectList[i].gameObject);
			}
		}

		selectionEffectList.Clear();
	}
}
