using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelEditorWindowGUI {
	public const string StrBtnSaveLevel = "Unload Current Level";
	
	public const string StrBtnLoadLevel = "Load Current Level";
		
	public const string StrBtnFillBoard = "Fill Board";
	public const string StrHintFillBoard = "Fill the board with the board tool selected below (board piece or tile)";
	
	public const string StrToolBtnBoardPiece = "Board Pieces: ";
	public const string StrHintToolBtnBoardPiece = "Choose board tool as a board piece.";
	
	public const string StrToolBtnBoardTile = "Board Tiles: ";
	public const string StrHintToolBtnBoardTile = "Choose board tool as a board tile.";
	
	public const string StrBtnInvalidBoardPieces = "Select Invalid BoardPieces";
	public const string StrHintInvalidBoardPieces = "Selects all board pieces in the hierarchy that are not assigned to the board (orphan).\n" +
													"This can happen because of a bug or because the board pieces were not added through the level editor system.";
	
	public const string StrBtnInvalidTiles = "Select Invalid Tiles";
	public const string StrHintInvalidTiles = "Selects all tiles in the hierarchy that are not assigned to any board piece.";
	
	public const string StrBtnApplyToSelection = "Apply To Selection";
	public const string StrHintBtnApplyToSelection = "Apply the above selected board tool to all currently selected board pieces";		
		
	public LevelEditorWindow window;
	public LevelEditorData editor;
	
//	private string strBoardNumRows;
//	private string strBoardNumCols;
	
	private List<GUIContent> listBtns = new List<GUIContent>(50);
	

	public LevelEditorWindowGUI(LevelEditorWindow ownerWindow, LevelEditorData _editor) {
		window = ownerWindow;
		editor = _editor;
	}
	
	public void Init() {
//		strBoardNumRows = editor.boardNumRows.ToString();
//		strBoardNumCols = editor.boardNumCols.ToString();		
	}
	
	public int BoardNumRows {
		get {
			return editor.boardNumRows;
		}
		set {
			editor.boardNumRows = value;
//			strBoardNumRows = editor.boardNumRows.ToString();
		}
	}
	
	public int BoardNumCols {
		get {
			return editor.boardNumCols;
		}
		set {
			editor.boardNumCols = value;
//			strBoardNumCols = editor.boardNumCols.ToString();
		}
	}
	
	protected void DrawBoardToolButtonsSet(string label, string tooltip, GameObject[] toolsList, params GUILayoutOption[] layoutParams) {
		if (toolsList != null) {
			// Render toggle buttons
			GUILayout.Space(10f);
			GUILayout.Label(label);
			
			listBtns.Clear();
			int selectedIdx = 0;
			listBtns.Add(new GUIContent("None Selected", tooltip));
			
			// Create the options popup list
			for(int i = 0; i < toolsList.Length; i++) {
				if (toolsList[i] != null) {
					listBtns.Add(new GUIContent(toolsList[i].name, tooltip));

					if (editor.selectedBoardTool == toolsList[i]) {
						selectedIdx = i + 1;
					}		
				}
			}

			// Draw the popup control
			int newIndex = EditorGUILayout.Popup(selectedIdx, listBtns.ToArray(), layoutParams);
			// If the selection changed, change the currently selected board tool prefab
			if (newIndex != selectedIdx) {
				editor.selectedBoardTool = toolsList[newIndex - 1];
			}
		}
	}
	
	public void OnLoadButtonPressed() 
	{
		editor.LoadLevelSetupFromHierarchy();
		SceneView.lastActiveSceneView.Repaint();
	}

	public void OnSaveButtonPressed() 
	{
		editor.SaveLevelSetupFromHierarchy();
//		// We close the level editor so the level editor won't crash after removing the board renderer from the hierarchy.
//		// No time for a more elegant method at the moment.
//		window.Close();
	}
		
	public void OnFillBoardButtonPressed() 
	{
		editor.ApplyCurrentBoardToolToAll();
	}
	
	public void OnApplyToSelectionButtonPressed() 
	{
		GameObject[] selectedGOs = Selection.gameObjects;
		for(int i = 0; i < selectedGOs.Length; i++)
		{
			Match3BoardPiece selectedBoardPiece = selectedGOs[i].GetComponent<Match3BoardPiece>();
			if (selectedBoardPiece != null) {
				editor.ApplyCurrentBoardToolToPos(selectedBoardPiece.editorBoardPos);
			}
		}
	}
	
	public void OnSelectInvalidBoardPiecesButtonPressed() 
	{
		editor.FindInvalidBoardTools<Match3BoardPiece>(editor.boardRenderer.GetComponentsInChildren<Match3BoardPiece>(true));
	}
	
	public void OnSelectInvalidTilesButtonPressed() 
	{
		editor.FindInvalidBoardTools<Match3Tile>(editor.boardRenderer.GetComponentsInChildren<Match3Tile>(true));
	}
	
	/// <summary>
	/// Render all the window GUI.
	/// </summary>
	public void Draw() {
		GUILayout.BeginVertical(); 
		{
//			GUILayout.Label("Board Rows:");
//			strBoardNumRows = EditorGUILayout.TextField(strBoardNumRows, GUILayout.MaxWidth(50f));
//			GUILayout.Label("Board Columns:");
//			strBoardNumCols = EditorGUILayout.TextField(strBoardNumCols, GUILayout.MaxWidth(50f));
//			int.TryParse(strBoardNumRows, out editor.boardNumRows);
//			int.TryParse(strBoardNumCols, out editor.boardNumCols);
						
			GUILayout.Space(10f);

			GUILayout.BeginHorizontal();
			{
				if ( GUILayout.Button(StrBtnLoadLevel, GUILayout.MaxWidth(150f)) ) 
				{
					OnLoadButtonPressed();
				}
				
				GUILayout.Space(10f);
				if ( GUILayout.Button(new GUIContent(StrBtnInvalidBoardPieces, StrHintInvalidBoardPieces), 
										GUILayout.MaxWidth(190f)) && editor.boardRenderer != null ) {
					OnSelectInvalidBoardPiecesButtonPressed();
				}
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			{
				if ( GUILayout.Button(StrBtnSaveLevel, GUILayout.MaxWidth(150f)) && editor.boardRenderer != null ) 
				{
					OnSaveButtonPressed();
				}
				
				GUILayout.Space(10f);
				if ( GUILayout.Button(new GUIContent(StrBtnInvalidTiles, StrHintInvalidTiles), GUILayout.MaxWidth(190f)) && editor.boardRenderer != null ) {
					OnSelectInvalidTilesButtonPressed();
				}
			}
			GUILayout.EndHorizontal();
			
//			if (GUILayout.Button("Focus Scene Camera")) {
//				LevelEditorWindow.FocusSceneCameraOn(boardRenderer.transform);
//			}
				
//			if (GUILayout.Button("Apply", GUILayout.MaxWidth(50f))) {
//				// Re-allocate the board matrix and re-load the current setup from hierarchy
//				if (editor.boardNumRows < 4 || editor.boardNumRows > 8 || editor.boardNumCols < 4 && editor.boardNumCols > 8) {
//					EditorUtility.DisplayDialog("Level Editor", "Invalid board size! Expected at least 4x4 and max 8x8.", "Ok");
//					BoardNumRows = Mathf.Clamp(BoardNumRows, 4, 8);
//					BoardNumCols = Mathf.Clamp(BoardNumCols, 4, 8);
//				} else {
//					editor.boardData.NumRows = BoardNumRows;
//					editor.boardData.NumColumns = BoardNumCols;
//					SceneView.lastActiveSceneView.Repaint();
//				}
//				editor.Init(editor.boardGrid);
//			}
			
			// Draw level editor board tools
			GUILayout.Space(10f);
			if (editor.boardRenderer) 
			{
				DrawBoardToolButtonsSet(StrToolBtnBoardPiece, StrHintToolBtnBoardPiece, editor.boardRenderer.prefabsPieces, GUILayout.MaxWidth(250f));
			}

			if (editor.boardRenderer)
			{
				DrawBoardToolButtonsSet(StrToolBtnBoardTile, StrHintToolBtnBoardTile, editor.boardRenderer.tilesPrefabs, GUILayout.MaxWidth(250f));
			}
			
			GUILayout.Space(5f);
			if ( GUILayout.Button(new GUIContent(StrBtnApplyToSelection, StrHintBtnApplyToSelection), GUILayout.MaxWidth(150f)) &&
				 editor.boardRenderer != null )
			{				
				OnApplyToSelectionButtonPressed();
			}
			
			if ( GUILayout.Button(new GUIContent(StrBtnFillBoard, StrHintFillBoard), GUILayout.MaxWidth(150f)) &&
				editor.boardRenderer != null ) 
			{
				OnFillBoardButtonPressed();
			}

//			GUILayout.Space(20f);

		}
		GUILayout.EndVertical();
	}
}
