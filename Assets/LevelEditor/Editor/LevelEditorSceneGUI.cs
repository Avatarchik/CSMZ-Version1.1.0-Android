using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelEditorSceneGUI  {
	public const string StrHintBoardPieceButton = null;//"Apply currently selected board tool to this board piece slot.";
	public const string StrHintSelectInHierarchy = null;//"Select the current board piece in the hierarchy.";
	public const string StrHintDeleteButton = null;//"Delete this tile/boardpiece. First deletes the tile if it exists.";
	
	public LevelEditorData editor;
	
	public LevelEditorSceneGUI(LevelEditorData _editor) {
		editor = _editor;
	}
	
	public void Init() { }
	
	/// <summary>
	/// Raises the select in hierarchy button pressed event.
	/// If the CONTROL key is pressed while clicking this button then all the board pieces on the same row will be selected.
	/// If the ALT key is pressed, all on the same column will be selected.
	/// 
	/// If the SHIFT key is pressed together with one of the above keys, only the tiles will be selected.
	/// 
	/// If the SHIFT key is pressed alone it will do a multi-select of the clicked board pieces.
	/// </summary>
	/// <param name='boardPiece'>
	/// Board piece.
	/// </param>
	public void OnSelectInHierarchyButtonPressed(Match3BoardPiece boardPiece) 
	{
		if (Event.current.control) 
		{
			// Modifier key to select entire row of the selected board piece was pressed.
			List<GameObject> selectedRow = new List<GameObject>();
			for(int i = 0; i < editor.boardNumCols; i++) {
				// If Alt is also pressed, select tiles only
				if (Event.current.shift) 
				{
					Match3Tile tile = editor.boardGrid[boardPiece.editorBoardPos.row, i].EditorTile;
					if (tile != null) 
					{
						selectedRow.Add(tile.gameObject);
					}
				} 
				else 
				{
					selectedRow.Add(editor.boardGrid[boardPiece.editorBoardPos.row, i].gameObject);
				}
			}
			
			Selection.objects = selectedRow.ToArray();
			selectedRow.Clear();
		} 
		else if (Event.current.alt) 
		{
			// Select the entire column of the selected board piece.
			List<GameObject> selectedColumn = new List<GameObject>();
			for(int i = 0; i < editor.boardNumRows; i++) {
				// If Alt is also pressed, select tiles only
				if (Event.current.shift) 
				{
					Match3Tile tile = editor.boardGrid[i, boardPiece.editorBoardPos.col].EditorTile;
					if (tile != null) 
					{
						selectedColumn.Add(tile.gameObject);
					}
				} 
				else 
				{
					selectedColumn.Add(editor.boardGrid[i, boardPiece.editorBoardPos.col].gameObject);
				}
			}

			Selection.objects = selectedColumn.ToArray();
			selectedColumn.Clear();			
		}
		else if (Event.current.shift)  
		{
			Object[] currentSelection = Selection.objects;
			Object[] newSelection = null;
			int selectedIndex = System.Array.IndexOf<Object>(currentSelection, boardPiece.gameObject);
			if (selectedIndex >= 0) 
			{
				// The currently selected board piece is already present in the current selection group, so we need to remove it.
				currentSelection[selectedIndex] = null;
				newSelection = currentSelection;
			}
			else 
			{
				// Add currently clicked board piece to the current selection
				newSelection = new Object[currentSelection.Length + 1];
				System.Array.Copy(currentSelection, newSelection, currentSelection.Length);
				newSelection[newSelection.Length - 1] = boardPiece.gameObject;
			}
			
			// Update the current selection
			Selection.objects = newSelection;
		}
		else
		{
			// Select the current board piece in the hierarchy (if there is one).
			Selection.activeGameObject = boardPiece.gameObject;
			EditorGUIUtility.PingObject(Selection.activeGameObject);
		}
	}
	
	public void OnDeleteButtonPressed(Match3BoardPiece boardPiece)
	{
		if (boardPiece != null && boardPiece.EditorTile != null) 
		{
			// If there's a tile on this board piece, destroy the tile first
			editor.SetBoardPieceSpawnRuleForTile(boardPiece, boardPiece.EditorTile, true);
			GameObject.DestroyImmediate(boardPiece.EditorTile.gameObject);
		} else if (boardPiece != null) 
		{
			// If there's only a board piece, destroy it.
			GameObject.DestroyImmediate(boardPiece.gameObject);
			editor.boardGrid[boardPiece.editorBoardPos.row, boardPiece.editorBoardPos.col] = null;
		}
	}
	
	/// <summary>
	/// Draws the board GUI and updates board pieces position according to their position in the board grid.
	/// </summary>
	public void Draw() {
		if (editor.boardRenderer == null || editor.boardGrid == null) 
		{
			return;
		}

		Vector2 guiCoord;
		
		Handles.BeginGUI(); 
		{
			// Draw board GUI in the scene
			editor.ApplyActionToAll((boardPiece, gridOffset, boardPos) =>
			{
				if (boardPiece != null) 
				{
					boardPiece.transform.localPosition = gridOffset; 
					Vector3 worldPos = boardPiece.transform.position - new Vector3(1f, -1f, 0f) * editor.boardRenderer.horizontalTileDistance * 0.5f;
					guiCoord = HandleUtility.WorldToGUIPoint(worldPos);
				} 
				else 
				{
					Vector3 worldPos = editor.boardRenderer.transform.TransformPoint(gridOffset 
												- new Vector3(1f, -1f, 0f) * editor.boardRenderer.horizontalTileDistance * 0.5f);

					guiCoord = HandleUtility.WorldToGUIPoint(worldPos);
				}
				
				// Draw the gui button for the current board piece
				GUILayout.BeginArea(new Rect(guiCoord.x, guiCoord.y, 80f, 80f)); 
				{
					GUILayout.BeginHorizontal(); 
					{
						// Draw board piece button
						if ( GUILayout.Button(new GUIContent(string.Format("[{0},{1}]", boardPos.row, boardPos.col), StrHintBoardPieceButton), 
												GUILayout.MaxWidth(42f)) ) 
						{
							OnBoardPieceButtonPressed(ref editor.boardGrid[boardPos.row, boardPos.col], boardPos);
						}

						
						// Draw Delete button if there's at least a board piece here.
						if (boardPiece != null && GUILayout.Button(new GUIContent("X", StrHintDeleteButton), GUILayout.MaxWidth(25f))) 
						{
							OnDeleteButtonPressed(boardPiece);
						}
					}
					GUILayout.EndHorizontal();
					
					// Draw "Select in Hierarchy" button
					if ( boardPiece != null && GUILayout.Button(new GUIContent("=", StrHintSelectInHierarchy), GUILayout.MaxWidth(25f)) ) 
					{
						OnSelectInHierarchyButtonPressed(boardPiece);
					}

					// Draw board piece editor label (if it has one).
					if (boardPiece != null) 
					{
						if (boardPiece.editorSceneLabel.Trim().Length > 0) 
						{
							//TODO: easiest way to draw a visible label (it's not meant to be a functional button)
							GUILayout.Button(boardPiece.editorSceneLabel, GUILayout.MaxWidth(45f));
						}
					}
							
				}
				GUILayout.EndArea();
			});
		}
		Handles.EndGUI();
	}

	/// <summary>
	/// Raises the board piece button pressed event when pressing a level editor button in the Unity scene view.
	/// </summary>
	/// <param name='refBoardPiece'>
	/// Board piece.
	/// </param>
	/// <param name='rowIdx'>
	/// Row index.
	/// </param>
	/// <param name='colIdx'>
	/// Col index.
	/// </param>
	void OnBoardPieceButtonPressed(ref Match3BoardPiece refBoardPiece, BoardCoord boardPos) 
	{
		// Validate the current board tool selection (the currently selected prefab)
//		editor.selectedBoardTool = Selection.activeGameObject;
		object selectedToolObj = null;
		if (editor.selectedBoardTool != null) {
			if (editor.selectedBoardTool.GetComponent<Match3BoardPiece>()) {
				selectedToolObj = editor.selectedBoardTool.GetComponent<Match3BoardPiece>();
			} if (editor.selectedBoardTool.GetComponent<Match3Tile>()) {
				selectedToolObj = editor.selectedBoardTool.GetComponent<Match3Tile>();
			}
		}
		
		if ( selectedToolObj is Match3BoardPiece ) {
			// Get the currently selected board tool type
			if ( !LevelEditorUtils.IsPrefabInArray(editor.boardRenderer.prefabsPieces, editor.selectedBoardTool) ) {
				EditorUtility.DisplayDialog("Level Editor", "The selected board prefab is not added to the Match3BoardRenderer pieces prefab list!", "Ok");
				return;
			}

			// Spawn a new board piece
			editor.SpawnBoardPieceAt(boardPos, editor.selectedBoardTool);
		} else if ( selectedToolObj is Match3Tile ) {
			// Get the currently selected board tool type
			if ( !LevelEditorUtils.IsPrefabInArray(editor.boardRenderer.tilesPrefabs, editor.selectedBoardTool) ) {
				EditorUtility.DisplayDialog("Level Editor", "The selected board prefab is not added to the Match3BoardRenderer tiles prefab list!", "Ok");
				return;
			} else if (refBoardPiece == null) {
				// Spawn a default board piece if none found at the current board position.
				editor.SpawnBoardPieceAt(boardPos, editor.defaultBoardPiece.gameObject);
				editor.UpdateBoardPieceGridTransform(refBoardPiece, boardPos);
			}
			
			// Spawn a new tile on the selected board piece.
			editor.SpawnTileAt(refBoardPiece, boardPos, editor.selectedBoardTool);
		} else {
			EditorUtility.DisplayDialog("Level Editor", "The selected board prefab type is not supported!", "Ok");
			return;
		}
	}
}
