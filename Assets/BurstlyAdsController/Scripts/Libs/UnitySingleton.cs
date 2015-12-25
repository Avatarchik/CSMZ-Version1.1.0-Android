using UnityEngine;
using System.Collections;

/// <summary>
/// Unity singleton.
/// 
/// Extend a MonoBehavior from this class when you need a singleton MonoBehavior.
/// IMPORTANT NOTE: You must override Awake() method and after calling the base.Awake()
/// check the "hasDuplicateInstance" flag to stop the Awake() method immediatelly in case you have
/// any more initialization logic after that:
/// Example:
/// protected override Awake()
/// {
/// 	base.Awake();
/// 	if (hasDuplicateInstances)
/// 	{
/// 		return;
/// 	}
/// 	// -- continue intialization logic here or make gameobject not destroyable.
/// }
/// 
/// </summary>
public class UnitySingleton<T>: MonoBehaviour where T : Component
{
	private static T instance;
	public static bool canCreate = true;
	
	// Indicates if the current singleton instance has duplicates.
	protected bool hasDuplicateInstances = false;


	// Use this for initialization
	protected virtual void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("Multiple instances of " + name + " found in scene: " + Application.loadedLevelName);
			Destroy(gameObject);
			
			hasDuplicateInstances = true;
			
			return;
		}
		
		instance = this as T;
	}
	
	public static T Instance
	{
		get 
		{
			if (instance == null && canCreate)
			{
				if (instance == null) {
					new GameObject(typeof(T).Name, typeof(T));
				}
			}
			
			return instance;
		}
	}
	
	protected virtual void OnDestroy()
	{
		canCreate = false;
	}
	
	protected virtual void OnApplicationQuit()
	{
		canCreate = false;
	}
}
