using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelEditorData {
	public System.Type defaultBoardPieceType = typeof(Match3BoardPiece);
	
	public Match3BoardPiece defaultBoardPiece = null;
	
	public Match3BoardRenderer boardRenderer = null;
	public BoardData boardData = null;
	
	public int boardNumRows;
	public int boardNumCols;
	
	/// <summary>
	/// The currently selected board tool prefab which can be the prefab of a board piece or a tile.
	/// </summary>
	public GameObject selectedBoardTool;


	/// <summary>
	/// Board grid info.
	/// </summary>
	public Match3BoardPiece[,] boardGrid = null;
	
	
	public LevelEditorData() { }

	public bool Init(bool allocateNewGrid = true) {
		return Init(allocateNewGrid, null);
	}
	
	public bool Init(bool allocateNewGrid, Match3BoardPiece[,] oldBoardGrid) {
		boardRenderer = GameObject.FindObjectOfType(typeof(Match3BoardRenderer)) as Match3BoardRenderer;
		
		if (boardRenderer != null) 
		{
			boardData = boardRenderer.GetComponent<BoardData>();
		} 
		else 
		{
//			EditorUtility.DisplayDialog("Level Editor", "No Match3BoardRenderer component found in the scene!", "Ok");
			return false;
		}
		
		// Init default board piece to use when no board piece is found in a certain grid position
		defaultBoardPiece = LevelEditorUtils.GetPrefabFromArray<Match3BoardPiece>(defaultBoardPieceType, boardRenderer.prefabsPieces);
		if (defaultBoardPiece == null) {
			EditorUtility.DisplayDialog("Level Editor", "Default board piece of type: " + defaultBoardPieceType.Name + 
				" not found in the board pieces prefabs list on the Match3BoardRenderer component!", "Ok");
			
			return false;
		}

		if (boardData != null) {
			boardNumRows = boardData.NumRows;
			boardNumCols = boardData.NumColumns;
			
			if (allocateNewGrid) {
		 		boardGrid = new Match3BoardPiece[boardNumRows, boardNumCols];
			}
			
//			// Copy old grid to new grid (boardGrid)
//			if (oldBoardGrid != null) {
//				int oldBoardNumRows = oldBoardGrid.GetLength(0);
//				int oldBoardNumCols = oldBoardGrid.GetLength(1);
//				
//				for(int rowIdx = 0; rowIdx < boardNumRows; rowIdx++) {
//					for(int colIdx = 0; colIdx < boardNumCols; colIdx++) {
//						if (rowIdx < oldBoardNumRows && colIdx < oldBoardNumCols) {
//							boardGrid[rowIdx, colIdx] = oldBoardGrid[rowIdx, colIdx];
//						} else {
//							// If the new grid is bigger than the old grid, add default board pieces
//							Match3BoardPiece newPiece = SpawnBoardPieceAt(new BoardCoord(rowIdx, colIdx), defaultBoardPiece.gameObject);
//							UpdateBoardPieceGridTransform(newPiece, new BoardCoord(rowIdx, colIdx));
//						}
//					}
//				}
//
//				// Destroy extra board pieces if the new grid is smaller than the old grid
//				for(int rowIdx = boardNumRows - 1; rowIdx < oldBoardNumRows; rowIdx++) {
//					for(int colIdx = boardNumCols - 1; colIdx < oldBoardNumCols; colIdx++) {
//						DestroyBoardPiece(oldBoardGrid[rowIdx, colIdx]);
//					}
//				}
//			} else {
//				// Init board with default board piece.
//				for(int rowIdx = 0; rowIdx < boardNumRows; rowIdx++) {
//					for(int colIdx = 0; colIdx < boardNumCols; colIdx++) {
//						// If the new grid is bigger than the old grid, add default board pieces
//						Match3BoardPiece newPiece = SpawnBoardPieceAt(new BoardCoord(rowIdx, colIdx), defaultBoardPiece.gameObject);
//						UpdateBoardPieceGridTransform(newPiece, new BoardCoord(rowIdx, colIdx));
//					}
//				}				
//			}
			
		} else {
			EditorUtility.DisplayDialog("Level Editor", "No BoardData component found on the Match3BoardRenderer!", "Ok");
			return false;
		}
		
		return true;		
	}
	
	/// <summary>
	/// Determines whether the specified board tool is assigned to the grid. (Match3BoardPiece or Match3Tile)
	/// </summary>
	/// <returns>
	/// <c>true</c>
	/// </returns>
	/// <param name='boardTool'>
	/// </param>
	public bool IsBoardToolAssignedToBoardGrid(MonoBehaviour boardTool) {
		for(int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++) 
		{
			for(int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) 
			{
				if (boardTool is Match3BoardPiece) {
					if (boardGrid[rowIdx, colIdx] == boardTool) 
					{
						return true;
					}
				} else if (boardTool is Match3Tile) {
					if (boardGrid[rowIdx, colIdx].Tile == boardTool) 
					{
						return true;
					}
				}
			}				
		}
		
		return false;
	}
	
	/// <summary>
	/// Finds and optionally selects invalid board tools (board pieces or board tiles).
	/// An invalid board piece/tile is a board tool that is present in the level prefab but not assigned in the board grid or to a board piece. 
	/// (it's an unused extra board piece/tile let in the prefab by mistake)
	/// </summary>
	/// <param name='boardPieces'>
	/// Board pieces.
	/// </param>
	/// <returns>
	/// The list of invalid board tools found.
	/// </returns>
	public List<GameObject> FindInvalidBoardTools<T>(T[] boardTools, bool selectInHierarchy = true) where T: MonoBehaviour {
		List<GameObject> invalidBoardTools = new List<GameObject>();
		for(int i = 0; i < boardTools.Length; i++) 
		{
			// If we didn't find the current board piece on the grid then select it in the hierarchy.
			if ( !IsBoardToolAssignedToBoardGrid(boardTools[i]) ) 
			{
				invalidBoardTools.Add(boardTools[i].gameObject);
			}
		}
		
		// Select the extra board pieces found in the hierarchy
		if (invalidBoardTools.Count > 0 && selectInHierarchy) {
			Selection.objects = invalidBoardTools.ToArray();
		}
		
		return invalidBoardTools;
	}
	
	public void LoadLevelSetupFromHierarchy() {
		if (boardRenderer == null) {
			// Try to re-initialize the editor data without re-allocating the board grid.
			if ( !Init(false) ) 
			{
				// In-case re-initializing failed, abort.
				return;
			}
		}
	
		Match3BoardPiece[] childBoardPieces = boardRenderer.GetComponentsInChildren<Match3BoardPiece>(true);
		for(int i = 0; i < childBoardPieces.Length; i++) {
			childBoardPieces[i].gameObject.SetActive(true);
		}
		
		Match3Tile[] childTiles = boardRenderer.GetComponentsInChildren<Match3Tile>(true);
		for(int i = 0; i < childTiles.Length; i++) {
			childTiles[i].gameObject.SetActive(true);
		}
		
		// Re-initialize the level editor data with the newly loaded board renderer.
		Init(true);

		boardRenderer.EditorLoadBoardSetupFromHierarchy(boardGrid);
		
		// Some safety checks.
		// Make sure the level prefab doesn't contain more board pieces than the maximum available on the board size NumRows X NumColumns.
		if (FindInvalidBoardTools<Match3BoardPiece>(childBoardPieces).Count > 0) {
			EditorUtility.DisplayDialog("Level Editor - Error", "This level contains invalid board pieces!", "Ok");
		}

		// Make sure there aren't more tiles than board pieces (meaning some tile might be for some reason without a board piece).
		if (FindInvalidBoardTools<Match3Tile>(childTiles).Count > 0) {
			EditorUtility.DisplayDialog("Level Editor - Error", "This level contains invalid! (tiles without owner board pieces)", "Ok");
		}
	}
	
	public void SaveLevelSetupFromHierarchy() {
		if (boardRenderer == null) 
		{
			return;
		}
		
		if (boardRenderer.winConditions == null || boardRenderer.loseConditions == null) 
		{
			EditorUtility.DisplayDialog("Level Editor - Warning", "You didn't set a Win Condition or Lose Condition on this level!", "Ok");
		}
			
		Match3BoardPiece[] childBoardPieces = boardRenderer.GetComponentsInChildren<Match3BoardPiece>(true);
		for(int i = 0; i < childBoardPieces.Length; i++) {
			childBoardPieces[i].gameObject.SetActive(false);
		}
		
		Match3Tile[] childTiles = boardRenderer.GetComponentsInChildren<Match3Tile>(true);
		for(int i = 0; i < childTiles.Length; i++) {
			childTiles[i].gameObject.SetActive(false);
		}	
	}

	public Match3BoardPiece SpawnBoardPieceAt(BoardCoord boardPos, GameObject piecePrefab) {
		// Destroy previous board piece that is on the "boardPos".
		if (boardGrid[boardPos.row, boardPos.col] != null) {
			DestroyBoardPiece(boardGrid[boardPos.row, boardPos.col]);
		}

		GameObject newBoardPieceGO = PrefabUtility.InstantiatePrefab(piecePrefab) as GameObject;
		PrefabUtility.DisconnectPrefabInstance(newBoardPieceGO);
		
		Match3BoardPiece newPiece = newBoardPieceGO.GetComponent<Match3BoardPiece>();
		newPiece.transform.parent = boardRenderer.transform;
		newPiece.name = string.Format("[{0},{1}] {2}", boardPos.row, boardPos.col, newPiece.name);
		
		// Apply the new board piece to the editor grid
		boardGrid[boardPos.row, boardPos.col] = newPiece;
		newPiece.editorBoardPos = boardPos;
		
		return newPiece;
	}
	
	/// <summary>
	/// Destroies the specified board piece and it's tile if it has one.
	/// </summary>
	/// <param name='piece'>
	/// Piece.
	/// </param>
	public void DestroyBoardPiece(Match3BoardPiece piece) {
		if (piece != null) {
			// Make sure we destroy it's tile if it has one.
			if (piece.EditorTile != null) {
				GameObject.DestroyImmediate(piece.EditorTile.gameObject);
			}
			// Destroy the board piece.
			GameObject.DestroyImmediate(piece.gameObject);
		}
	}
	
	public Match3Tile SpawnTileAt(Match3BoardPiece boardPiece, BoardCoord boardPos, GameObject tilePrefab) {
		// Destroy previous tile from board piece (if present).
		if (boardPiece.EditorTile != null) {
			// Remove the first spawn rule entry for this tile if it corresponds to its type.
			SetBoardPieceSpawnRuleForTile(boardPiece, boardPiece.EditorTile, true);
			GameObject.DestroyImmediate(boardPiece.EditorTile.gameObject);
		}

		GameObject newTileGO = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
		PrefabUtility.DisconnectPrefabInstance(newTileGO);
		
		Match3Tile newTile = newTileGO.GetComponent<Match3Tile>();
		newTile.name = string.Format("[{0},{1}] {2}", boardPos.row, boardPos.col, newTile.name);
		newTile.transform.parent = boardRenderer.transform;
		newTile.transform.localPosition = boardPiece.transform.localPosition - boardRenderer.transform.forward * 2f;
		boardPiece.EditorTile = newTile;

		// Add the first spawn rule entry for with this tile's type.
		SetBoardPieceSpawnRuleForTile(boardPiece, boardPiece.EditorTile);

		return newTile;
	}
	
	public void SetBoardPieceSpawnRuleForTile(Match3BoardPiece boardPiece, Match3Tile tile, bool removeRule = false) 
	{
		if (boardPiece == null || tile == null) 
		{
			return;
		}
		
		// Check if we have at least one RuleEntry in the board piece initial TileSpawnRule
		if (boardPiece.initialTileSpawnRule != null) 
		{
			if (boardPiece.initialTileSpawnRule.ruleEntries == null)
			{
				boardPiece.initialTileSpawnRule.ruleEntries = new List<RuleEntry>();
			}
			
			// Make sure there's at least one rule entry in the list if we're adding a tile.
			if (boardPiece.initialTileSpawnRule.ruleEntries.Count == 0 && !removeRule)
			{
				boardPiece.initialTileSpawnRule.ruleEntries.Add(new RuleEntry());
			}
			
			if ( !removeRule )
			{
				// Change only the first RuleEntry in the entries list. (is it ok?! too sleepy to think)
				boardPiece.initialTileSpawnRule.ruleEntries[0] = tile.DefaultRuleEntry;
			}
			else
			{
				if (boardPiece.initialTileSpawnRule.ruleEntries.Count > 0) {
					// Reset the first rule entry to default.
					boardPiece.initialTileSpawnRule.ruleEntries[0] = new RuleEntry();
				}
			}

		}
	}
		
	public void ApplyCurrentBoardToolToAll() 
	{
		ApplyActionToAll((piece, gridOffset, boardPos) => 
		{
			ApplyCurrentBoardToolToPos(boardPos);
		});
	}
	
	/// <summary>
	/// Applies the currently board tool (board piece or tile) to the specified board position.
	/// </summary>
	/// <param name='boardPos'>
	/// Board position.
	/// </param>
	public void ApplyCurrentBoardToolToPos(BoardCoord boardPos) 
	{
		if (selectedBoardTool != null)
		{
			if ( selectedBoardTool.GetComponent<Match3BoardPiece>() )
			{
				SpawnBoardPieceAt(boardPos, selectedBoardTool);
			} 
			else if ( selectedBoardTool.GetComponent<Match3Tile>() )
			{
				Match3BoardPiece piece = boardGrid[boardPos.row, boardPos.col];

				if (piece == null)
				{
					piece = SpawnBoardPieceAt(boardPos, defaultBoardPiece.gameObject);
					UpdateBoardPieceGridTransform(piece, boardPos);
				}
				SpawnTileAt(piece, boardPos, selectedBoardTool);
			}
		}
	}
	
	public void ApplyActionToRow(int rowIdx, System.Action<Match3BoardPiece, BoardCoord> action) {
		for(int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) {
			if (action != null) {
				action(boardGrid[rowIdx, colIdx], new BoardCoord(rowIdx, colIdx));
			}
		}
	}
	
	public void ApplyActionToColumn(int colIdx, System.Action<Match3BoardPiece, BoardCoord> action) {
		for(int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++) {
			if (action != null) {
				action(boardGrid[rowIdx, colIdx], new BoardCoord(rowIdx, colIdx));
			}
		}
	}
	
	public void UpdateBoardPieceGridTransform(Match3BoardPiece boardPiece, BoardCoord newGridPos) {
		Vector3 gridOffset = Vector3.zero;
		gridOffset.x = newGridPos.col * boardRenderer.horizontalTileDistance + boardRenderer.horizontalTileOffset;
		gridOffset.y = -newGridPos.row * boardRenderer.verticalTileDistance + boardRenderer.verticalTileOffset;
		boardPiece.transform.localPosition = gridOffset;
	}
	
	public void ApplyActionToAll(System.Action<Match3BoardPiece, Vector3, BoardCoord> action) {
		Vector3 gridOffset = Vector3.zero;
		for(int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++) {
			gridOffset.y = -rowIdx * boardRenderer.verticalTileDistance + boardRenderer.verticalTileOffset;
			
			for(int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) {
				gridOffset.x = colIdx * boardRenderer.horizontalTileDistance + boardRenderer.horizontalTileOffset;
				
				if (action != null) {
					action(boardGrid[rowIdx, colIdx], gridOffset, new BoardCoord(rowIdx, colIdx));
				}
			}
		}
	}
}
