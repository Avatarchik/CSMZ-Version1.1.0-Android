using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SelectionChange : ProjectManager.ProjectManagerWindow {
	
	private const int STACK_SIZE = 20;
	
	private LinkedList< Object > gameObjects = new LinkedList< Object >();
	private LinkedListNode< Object > selectedGameObject = null;
	private bool ignoreSelectionChange = false;
	
	public override void OnGUI ()
	{
		GUILayout.BeginHorizontal("box");
		if(GUILayout.Button((Texture)Resources.LoadAssetAtPath("Assets/_Maleficent/Scripts/Editor Tools/Editor/previows.png", typeof(Texture)), GUILayout.Width(31)))
		{
			if(selectedGameObject != null && selectedGameObject.Previous != null)	
			{
				selectedGameObject = selectedGameObject.Previous;
				Selection.activeObject = selectedGameObject.Value;
				ignoreSelectionChange = true;
			}
		}
		if(GUILayout.Button( (Texture)Resources.LoadAssetAtPath("Assets/_Maleficent/Scripts/Editor Tools/Editor/next.png", typeof(Texture)), GUILayout.Width(31)))
		{
			if(selectedGameObject != null && selectedGameObject.Next != null)	
			{
				selectedGameObject = selectedGameObject.Next;
				Selection.activeObject = selectedGameObject.Value;
				ignoreSelectionChange = true;
			}
		}
		GUILayout.EndHorizontal();
		
		
		/*string text = "";
		foreach(GameObject gameObject in gameObjects)
		{
			if(gameObject == selectedGameObject.Value)
				text += "->";
			text += gameObject.name + "\n";
		}
		GUILayout.Label(text);*/
	}
	
	public override void OnSelectionChange() {	
		if(ignoreSelectionChange)
		{
			//Selection was changed pressed "previous" or "next", ignore it but restore selections for next time
			ignoreSelectionChange = false;
		}
		else
		{
			//Remove all objects behind the selected one on the stack
			while(this.selectedGameObject != gameObjects.Last)
			{
				gameObjects.RemoveLast();
			}
			
			
			Object selectedGameObject = Selection.activeObject;
			if(selectedGameObject != null)
			{
				gameObjects.AddLast(selectedGameObject);
				while(gameObjects.Count > STACK_SIZE)
					gameObjects.RemoveFirst();
				
				Repaint();
				this.selectedGameObject = gameObjects.Last;
			}
		}
    }
}
