using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
	
[CustomEditor(typeof(MeshRenderer))]
public class MeshRendererAABB : Editor {
	private const string STR_ENABLE_MESH_AABB = "Enable AABB";
	private const string STR_KEEP_VISIBLE = "Keep AABB Visible";
	private const string STR_CLEAR_ALL_VISIBLE = "Clear All AABBs";
	
	public static Dictionary<int, Renderer> storedSelections = new Dictionary<int, Renderer>();
	private static List<int> cleanupList = new List<int>();
	// The currently selected MeshRenderer
	private static MeshRenderer currentSelection = null;
		
	public static bool isAABBenabled;
	public static bool canSelectChildren = false;
	
	private static bool isKeepAABBVisible = false;
	private static bool foldout = true;
	
	
	public override void OnInspectorGUI ()
	{
		this.DrawDefaultInspector();
		
		EditorGUI.indentLevel = 0;
		foldout = EditorGUILayout.Foldout(foldout, "MeshRenderer AABB"); 
		if (foldout) {
			EditorGUI.indentLevel = 2;
			bool newIsAABBenabled = EditorGUILayout.Toggle(STR_ENABLE_MESH_AABB,isAABBenabled);
			if (newIsAABBenabled != isAABBenabled) {
				isAABBenabled = newIsAABBenabled;
				EditorUtility.SetDirty(this.target);
			}
			
			// Update status of the "Keep AABB visible" checkbox (optimize this by doing the search in storedSelections dictionary only when the current selection changes)
			if (currentSelection != this.target) {
				currentSelection = this.target as MeshRenderer;
				
				// check if this MeshRenderer is already in the stored selections
				isKeepAABBVisible = storedSelections.ContainsKey(currentSelection.GetInstanceID());
				//Debug.Log("isKeepAABBVisible current status: " + isKeepAABBVisible);
			}
			
			bool newIsKeepAABBVisible = EditorGUILayout.Toggle(STR_KEEP_VISIBLE, isKeepAABBVisible);
			if (newIsKeepAABBVisible != isKeepAABBVisible) {
				if (newIsKeepAABBVisible) {
					Transform[] multiSelection = Selection.transforms;
					
					// check for multi selection
					if (multiSelection.Length > 1) {
						for(int i = 0; i < multiSelection.Length; i++) {
							if ( multiSelection[i].renderer && !storedSelections.ContainsKey((multiSelection[i].renderer as MeshRenderer).GetInstanceID()) ) {
								storedSelections.Add( (multiSelection[i].renderer as MeshRenderer).GetInstanceID(), multiSelection[i].renderer as MeshRenderer);
							}
						}
					} else {
						// add current mesh renderer to the stored selection dictionary to have the AABB always highlighted
						storedSelections.Add(currentSelection.GetInstanceID(), currentSelection);
					}
				} else {
					Transform[] multiSelection = Selection.transforms;
					
					// check for multi-selection
					if (multiSelection.Length > 1) {
						for(int i = 0; i < multiSelection.Length; i++) {
							if ( multiSelection[i].renderer && storedSelections.ContainsKey((multiSelection[i].renderer as MeshRenderer).GetInstanceID()) ) {
								storedSelections.Remove( (multiSelection[i].renderer as MeshRenderer).GetInstanceID() );
							}
						}
					} else {
						// remove current MeshRenderer from the storeSelections dictionary
						storedSelections.Remove(currentSelection.GetInstanceID());
					}
				}
				EditorUtility.SetDirty(this.target);
				// force refresh of the "Keep AABB visible" checkbox status
				currentSelection = null;
			}
			
			// Display current AABB bounds size
			if (currentSelection != null) {
				EditorGUILayout.SelectableLabel("Bounds Size:" + currentSelection.bounds.size.ToString());
			}
							
			EditorGUILayout.Separator();
			GUILayout.BeginVertical();
				if ( GUILayout.Button(STR_CLEAR_ALL_VISIBLE, GUILayout.MaxWidth(200)) ) {
					storedSelections.Clear();
					cleanupList.Clear();
					EditorUtility.SetDirty(this.target);
				}
			GUILayout.EndVertical();
		}
	}
	
	/// <summary>
	/// Handles drawing of the mesh renderer AABB gizmo. This is only called for a selected GameObject with a MeshRenderer on it
	/// and for all the MeshRenderer's the user selected to keep highlighted.
	/// </summary>
	/// <param name="renderer">
	/// A <see cref="MeshRenderer"/>
	/// </param>
	/// <param name="gizmoType">
	/// A <see cref="GizmoType"/>
	/// </param>
	[DrawGizmo(GizmoType.Active | GizmoType.NotSelected | GizmoType.Selected | GizmoType.Pickable | GizmoType.SelectedOrChild)]
	static void MeshRendererAABBGizmo(MeshRenderer renderer, GizmoType gizmoType) {
		if (!isAABBenabled) {
			return;
		}
		
		// Draw selected renderer AABB and stored selections as well
		Transform[] selection = Selection.GetTransforms(canSelectChildren ? SelectionMode.Deep : SelectionMode.Unfiltered);
		
		for (int i = 0; i < selection.Length; i++) {
			Renderer curRenderer = selection[i].renderer;
			if (curRenderer) {
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(curRenderer.bounds.center, curRenderer.bounds.size);
			}
		}
		
		// Draw AABB for stored selections specified by user to remain highlighted
		bool mustCleanupStoredSel = false;
		foreach(KeyValuePair<int, Renderer> pair in storedSelections) {
			// if this renderer is still available
			if (pair.Value) {
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(pair.Value.bounds.center, pair.Value.bounds.size);
			} else {
				cleanupList.Add(pair.Key);
				mustCleanupStoredSel = true;
			}
		}
		
		// Check if we need to cleanup the stored selections (if any of the renderers was deleted from the scene)
		if (mustCleanupStoredSel) {
			for(int i = 0; i < cleanupList.Count; i++) {
				storedSelections.Remove(cleanupList[i]);
			}
			cleanupList.Clear();
		}
	}
}