using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaleficentTutorialsSystem : MonoBehaviour {
	
	public event System.Action OnTutorialSystemShow;
	public event System.Action OnTutorialSystemHide;
	
	public static float waitTime = 10f;
	
	public GameObject[] highlightObjects;
	public GameObject highlightEffect;
	//public GameObject highlightEffectExtra;
	public string textKey;
	public Vector3 messagePosition = new Vector3(0f, -200f, 0f);
	public bool openManaBallOnShow = false;

	[HideInInspector]
	public bool lastTutorial = false;
	public int highlightItemIdx = -1;

	public bool isHighlightItemTutorial {
		get {
			return highlightItemIdx > -1;
		}
	}

	public bool isFirstTimeOnlyTutorial = false;
	public ManaItem giveManaFromItem = null;
	public int giveManaAmount {
		get {
			return (giveManaFromItem != null)?giveManaFromItem.ManaPointsCost:0;
		}
	}
	
	public Match3BoardPiece pieceToMove;
	public Match3BoardPiece pieceToMoveDestination;
	public float moveIndicatorAngle = 0f;

	public string selectionDismiss = "";
	public bool dismissOnSelectionEnd = false;
	public bool disableBoardInteraction;
	public bool disableManaBallInteraction;
	public bool disableOtherPowerUpsInteraction;

	[System.NonSerialized]
	public float animationTime = 0.3f;
	[System.NonSerialized]
	public bool disableTutorial = false;
	[System.NonSerialized]
	public bool gaveFreeItems = false;

	//[System.NonSerialized]
	public bool justTap {
		get {
			return (isHighlightItemTutorial || !(pieceToMove != null && pieceToMoveDestination != null)) && selectionDismiss == "";
			//return dismissOnTap;
		}
	}

	protected List<ManaPowerBallController> selectableManaBalls;
	protected List<ISelectableItem> selectableOtherPowerUpsItems;
	
	protected GameObject[] effectsObjs;
	protected GameObject moveArrow;


	void Awake()
	{
		if (isHighlightItemTutorial && (highlightObjects == null || highlightObjects.Length < 2)) {
			highlightObjects = new GameObject[2];
		}
	}

	#region Public

	public void ShowTutorialSystem()
	{
		_RegisterSelectableItems();

		PossibleMatchesController.Instance.EnabledCount--;
		_ShowHighlights();
		_DisableSelectedInteractions();

		if (openManaBallOnShow) {
			_OpenManaBall();
		}

		StartCoroutine(_WaitForTapOrTime());

		if (OnTutorialSystemShow != null) {
			OnTutorialSystemShow();
		}
	}


	public void HideTutorialSystem()
	{
		_HideHighlights();
		_EnableSelectedInteractions();

//		if(lastTutorial) {
//			_CloseManaBall();
//		}

		PossibleMatchesController.Instance.EnabledCount++;
		if (justTap) {
			PossibleMatchesController.Instance.OnStableBoardEvent();
		}

		_GiveManaIfNeeded();
		
		if (OnTutorialSystemHide != null) {
			OnTutorialSystemHide();
		}
	}

	#endregion



	#region Private

	void _RegisterSelectableItems()
	{
		selectableManaBalls = new List<ManaPowerBallController>();
		selectableOtherPowerUpsItems = new List<ISelectableItem>();
		
		GameObject[] selectableItems = GameObject.FindGameObjectsWithTag("SelectableItem");
		foreach (GameObject each in selectableItems) {
			ISelectableItem item = each.GetComponent(typeof(ISelectableItem)) as ISelectableItem;
			string name = item.SelectableItemName();

			if (name == "ManaPowerBall") {
				selectableManaBalls.Add(each.GetComponent<ManaPowerBallController>());
			} else if (selectionDismiss != name) {
				selectableOtherPowerUpsItems.Add(item);
			}
			
			if (name == selectionDismiss) {
				if (dismissOnSelectionEnd) {
					item.EndSelectionEvent += (ISelectableItem obj) => {
						disableTutorial = true;
					};
				} else {
					item.SelectionEvent += (ISelectableItem obj) => {
						disableTutorial = true;
					};

				}
			}
		}
	}


	void _GiveManaIfNeeded()
	{
		if (giveManaAmount > 0) {
			int manaToAdd = giveManaFromItem.ManaPointsCost;
			TokensSystem.Instance.AddMana(manaToAdd);
			SoundManager.Instance.PlayOneShot("mana_earn_sfx");

			AnalyticsBinding.LogEventGameAction(Match3BoardGameLogic.Instance.GetLevelType(), "earn_mana", manaToAdd.ToString(), 
			                                    "at tutorial", MaleficentBlackboard.Instance.level);
		}
	}

	IEnumerator _WaitForTapOrTime()
	{
		while (!disableTutorial) 
		{
			if (justTap && CustomInput.touchCount > 0) {
				disableTutorial = true;
			}
			else {
				yield return null;
			}
		}

		HideTutorialSystem();
	}


	private void _ShowHighlights()
	{
		_ShowArrow();
		_ShowItemHighlights();
	}


	private void _HideHighlights()
	{
		_HideArrow();
		_HideItemHighlights();
	}


	private void _ShowArrow()
	{
		if (pieceToMove != null && pieceToMoveDestination != null) {
			
			TileSwitchInput.Instance.InputFilter = InputFilter;
			
			moveArrow = GameObject.Instantiate(Resources.Load("Game/Prefabs/Movetile_Indicator") as GameObject) as GameObject;
			moveArrow.transform.parent = pieceToMove.transform;
			
			Vector3 rot = moveArrow.transform.localEulerAngles;
			rot.z = moveIndicatorAngle;
			moveArrow.transform.localEulerAngles = rot;
			
			Vector3 pos = moveArrow.transform.localPosition;
			pos.x = 0f;
			pos.y = 0f;
			moveArrow.transform.localPosition = pos;
		}
	}


	private void _OpenManaBall()
	{
		foreach(ManaPowerBallController manaBall in selectableManaBalls) {
			manaBall.Open(true, true);
		}
	}


	private void _HideArrow()
	{
		if (moveArrow) {
			Destroy(moveArrow);
		}
	}


	private void _ShowItemHighlights()
	{
		if (highlightObjects != null && highlightObjects.Length > 0 && highlightEffect != null) 
		{
			effectsObjs = new GameObject[highlightObjects.Length];
			
			for (int i = 0; i < highlightObjects.Length; ++i) 
			{
				GameObject effect = GameObject.Instantiate(highlightEffect) as GameObject;
				
				Vector3 newScale = effect.transform.localScale;
				effect.transform.parent = highlightObjects[i].transform;
				effect.transform.localPosition = Vector3.zero;
				
				if (effect.layer == LayerMask.NameToLayer("GameGUI")) {
					effect.transform.localScale = newScale;
				}
				
				effectsObjs[i] = effect;
			}
		}
	}


	private void _HideItemHighlights()
	{
		if (effectsObjs != null && effectsObjs.Length > 0) {
			for (int i = 0; i < effectsObjs.Length; ++i) 
			{
				Destroy(effectsObjs[i]);
			}
		}
	}


	private void _DisableSelectedInteractions()
	{
		bool isMovePieceTutorial = (pieceToMove != null && pieceToMoveDestination != null);

		_SetManaBallInteractionsActive(!disableManaBallInteraction && !isMovePieceTutorial);
		_SetOtherItemsInteractionsActive(!disableOtherPowerUpsInteraction && !isMovePieceTutorial);
		_SetBoardInteractionsActive(!disableBoardInteraction);
	}


	protected void _EnableSelectedInteractions()
	{
		_SetManaBallInteractionsActive(true);
		_SetOtherItemsInteractionsActive(true);
		_SetBoardInteractionsActive(true);
	}



	private void _SetManaBallInteractionsActive(bool active)
	{
		foreach (ManaPowerBallController ball in selectableManaBalls) {
			ball.EnableItem(active);
		}
	}


	private void _SetOtherItemsInteractionsActive(bool active)
	{
		foreach (ISelectableItem powerup in selectableOtherPowerUpsItems){
			powerup.EnableItem(active);
		}
	}


	private void _SetBoardInteractionsActive(bool active)
	{
		if (active) {
			TileSwitchInput.Instance.EnableInput();
		} else {
			TileSwitchInput.Instance.DisableInput();
		}
	}


	#endregion


	#region InputFilter

	bool InputFilter(AbstractTile selectedTile, AbstractTile destinationTile, TileMoveDirection moveDirection) 
	{
		if (pieceToMove == null || pieceToMoveDestination == null) {
			return true;
		}
		
		AbstractTile tileToMove = pieceToMove.Tile;
		AbstractTile tileToMoveDest = pieceToMoveDestination.Tile;
		
		if (destinationTile == null) 
		{
			Match3BoardGameLogic boardLogic = Match3BoardGameLogic.Instance;
			BoardCoord targetBoardPos = selectedTile.BoardPiece.BoardPosition;
			targetBoardPos.OffsetByAndClamp(boardLogic.tilesMoveDirections[(int)moveDirection], boardLogic.boardData.NumRows - 1, boardLogic.boardData.NumColumns - 1);
			destinationTile = boardLogic.boardData[targetBoardPos].Tile;
		}
		
		if (tileToMove == selectedTile && tileToMoveDest == destinationTile ||
		    tileToMove == destinationTile && tileToMoveDest == selectedTile)
		{
			disableTutorial = true;
			TileSwitchInput.Instance.InputFilter = null;
			return true;
		}
		
		return false;
	}

	#endregion

//	public static TutorialsSystem Instance {
//		get {
//			return instance;
//		}
//	}
	
//	void Awake()
//	{
//		if (itemIdx >= 0 && (highlightObjects == null || highlightObjects.Length < 2)) {
//			highlightObjects = new GameObject[2];
//		}
//	}
	
	// Use this for initialization
//	void Start()
//	{
//		Match3BoardGameLogic.OnStartGame += ShowTutorial;
//		PossibleMatchesController.Instance.EnabledCount--;
//	}
	
//	public void GiveFreeItems(ItemHolder item)
//	{
//		if (PlayerPrefs.GetInt(textKey, 0) == 0) 
//		{
//			PlayerPrefs.SetInt(textKey, 1);
//			
//			if (MaleficentBlackboard.Instance.level >= LoadLevelButton.lastUnlockedLevel) 
//			{
//				if (Language.Get(textKey).Contains("<TOKEN>")) {
//					item.AddItems(TweaksSystem.Instance.intValues["TutorialTokens"]);
//				}
//				else if (textKey == "TUTORIAL_ICE_PICK_ITEM") {
//					item.AddItems(TweaksSystem.Instance.intValues["TutorialIcePicks"]);
//				}
//				else if (textKey == "TUTORIAL_SNOWBALL_ITEM") {
//					item.AddItems(TweaksSystem.Instance.intValues["TutorialSnowballs"]);
//				}
//				else if (textKey == "TUTORIAL_HOURGLASS_ITEM") {
//					item.AddItems(TweaksSystem.Instance.intValues["TutorialHourglasses"]);
//				}
//				
//				gaveFreeItems = true;
//			}
//		}
//	}


//	public virtual void ShowTutorial()
//	{
//		Match3BoardGameLogic.OnStartGame -= ShowTutorial;
//		
//		if (itemIdx >= 0) {
//			if (gaveFreeItems) {
//				TileSwitchInput.Instance.DisableInput();
//			}
//			else {
//				justTap = true;
//			}
//		}
//		else if (pieceToMove != null && pieceToMoveDestination != null) {
//			TileSwitchInput.Instance.InputFilter = InputFilter;
//			moveArrow = GameObject.Instantiate(Resources.Load("Game/Prefabs/Movetile_Indicator") as GameObject) as GameObject;
//			moveArrow.transform.parent = pieceToMove.transform;
//			Vector3 rot = moveArrow.transform.localEulerAngles;
//			rot.z = moveIndicatorAngle;
//			moveArrow.transform.localEulerAngles = rot;
//			Vector3 pos = moveArrow.transform.localPosition;
//			pos.x = 0f;
//			pos.y = 0f;
//			moveArrow.transform.localPosition = pos;
//		}
//		else {
//			justTap = true;
//		}
//		
//		if (highlightObjects != null && highlightObjects.Length > 0 && highlightEffect != null) 
//		{
//			effectsObjs = new GameObject[highlightObjects.Length];
//			
//			for (int i = 0; i < highlightObjects.Length; ++i) 
//			{
//				GameObject effect;
//				if (i > 1 && itemIdx >= 0) {
//					effect = GameObject.Instantiate(highlightEffectExtra) as GameObject;
//				}
//				else {
//					effect = GameObject.Instantiate(highlightEffect) as GameObject;
//				}
//				Vector3 newScale = effect.transform.localScale;
//				effect.transform.parent = highlightObjects[i].transform;
//				effect.transform.localPosition = Vector3.zero;
//				
//				if (effect.layer == LayerMask.NameToLayer("GameGUI")) {
//					effect.transform.localScale = newScale;
//				}
//				
//				effectsObjs[i] = effect;
//			}
//		}
//		
//		if (OnTutorialShow != null) {
//			OnTutorialShow();
//		}
//		
//		StartCoroutine(WaitForTapOrTime());
//	}
//	
//	bool InputFilter(AbstractTile selectedTile, AbstractTile destinationTile, TileMoveDirection moveDirection) 
//	{
//		if (pieceToMove == null || pieceToMoveDestination == null) {
//			return true;
//		}
//		
//		AbstractTile tileToMove = pieceToMove.Tile;
//		AbstractTile tileToMoveDest = pieceToMoveDestination.Tile;
//		
//		if (destinationTile == null) 
//		{
//			Match3BoardGameLogic boardLogic = Match3BoardGameLogic.Instance;
//			BoardCoord targetBoardPos = selectedTile.BoardPiece.BoardPosition;
//			targetBoardPos.OffsetByAndClamp(boardLogic.tilesMoveDirections[(int)moveDirection], boardLogic.boardData.NumRows - 1, boardLogic.boardData.NumColumns - 1);
//			destinationTile = boardLogic.boardData[targetBoardPos].Tile;
//		}
//		
//		if (tileToMove == selectedTile && tileToMoveDest == destinationTile ||
//		    tileToMove == destinationTile && tileToMoveDest == selectedTile)
//		{
//			disableTutorial = true;
//			TileSwitchInput.Instance.InputFilter = null;
//			return true;
//		}
//		
//		return false;
//	}
//	
//	IEnumerator WaitForTapOrTime()
//	{
//		while (!disableTutorial) 
//		{
//			if (justTap && CustomInput.touchCount > 0) {
//				disableTutorial = true;
//			}
//			else {
//				yield return null;
//			}
//		}
//		
//		if (itemIdx >= 0) {
//			TileSwitchInput.Instance.EnableInput();
//		}
//		
//		HideHighlights();
//		
//		if (OnTutorialHide != null) {
//			OnTutorialHide();
//		}
//	}
//	
//	public virtual void HideHighlights()
//	{
//		if (effectsObjs != null && effectsObjs.Length > 0) {
//			for (int i = 0; i < effectsObjs.Length; ++i) 
//			{
//				Destroy(effectsObjs[i]);
//			}
//		}
//		
//		if (moveArrow) {
//			Destroy(moveArrow);
//		}
//		
//		PossibleMatchesController.Instance.EnabledCount++;
//		if (justTap) {
//			PossibleMatchesController.Instance.OnStableBoardEvent();
//		}
//	}
//	
//	void OnDestroy()
//	{
//		Match3BoardGameLogic.OnStartGame -= ShowTutorial;
//	}
}
