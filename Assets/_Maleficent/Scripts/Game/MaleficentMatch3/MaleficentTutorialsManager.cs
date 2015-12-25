using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaleficentTutorialsManager : MonoBehaviour {

	public event System.Action OnTutorialShow;
	public event System.Action OnTutorialHide;

	public float delayBetweenTutorials = 0.5f;
	public GameObject tutorialPrefab;
	[HideInInspector]
	public bool tutorialFinished = false;

	public static MaleficentTutorialsManager Instance {
		get {
			return instance;
		}
	}

	public MaleficentTutorialsSystem currentTutorialSystem {
		get {
			return (currentTutorialIdx != -1 && currentTutorialIdx < tutorials.Length)? tutorials[currentTutorialIdx] : null;
		}
	}
	protected MaleficentTutorialsSystem[] tutorials;
	protected static MaleficentTutorialsManager instance;
	protected int currentTutorialIdx = -1;

	void Awake()
	{
		instance = this;

		MaleficentTutorialsSystem[] tutorialList;
		GameObject tutorial = gameObject;
		if (tutorialPrefab != null) {
			//FOR retrocompatibility
			tutorial = GameObject.Instantiate(tutorialPrefab) as GameObject;
		}


		tutorialList = tutorial.GetComponents<MaleficentTutorialsSystem>();
		tutorials = new MaleficentTutorialsSystem[tutorialList.Length];
//		foreach (MaleficentTutorialsSystem t in tutorialList) {
//			Debug.Log(t);
//			if (t.tutorialIdx >= 0) {
//				tutorials[t.tutorialIdx] = t;
//			}
//		}
		tutorials = tutorialList;

		foreach (MaleficentTutorialsSystem tutorialSystem in tutorials) {
			tutorialSystem.OnTutorialSystemShow += HandleOnTutorialSystemShow;
			tutorialSystem.OnTutorialSystemHide += HandleOnTutorialSystemHide;
		}
	}


	void Start()
	{
		Match3BoardGameLogic.OnStartGame += HandleOnStartGame;
	}


	void OnDestroy()
	{
		foreach (MaleficentTutorialsSystem tutorialSystem in tutorials) {
			tutorialSystem.OnTutorialSystemShow -= HandleOnTutorialSystemShow;
			tutorialSystem.OnTutorialSystemHide -= HandleOnTutorialSystemHide;
		}

		Match3BoardGameLogic.OnStartGame -= HandleOnStartGame;
		instance = null;
	}


	#region Public

	public bool IsLastTutorial()
	{
		return (currentTutorialIdx == -1 || currentTutorialIdx == (tutorials.Length - 1));
	}

	#endregion



	#region Private

	void _InitTutorials()
	{
		currentTutorialIdx = -1;
		if (tutorials != null && tutorials.Length > 0) {
			tutorialFinished = false;
			_NextTutorial();
		}
	}


	void _NextTutorial()
	{
		currentTutorialIdx++;

		while (currentTutorialSystem != null && !_ShouldShowNextTutorial()) {
			currentTutorialIdx++;
		}

		if (currentTutorialSystem != null) {

			if(currentTutorialSystem.isFirstTimeOnlyTutorial) {
				PlayerPrefs.SetInt(_TutorialPlayerPrefsKey(currentTutorialSystem), 1);
			}

			if (OnTutorialShow != null) {
				OnTutorialShow();
			}

			currentTutorialSystem.lastTutorial = IsLastTutorial();
			currentTutorialSystem.ShowTutorialSystem();

		} else {
			tutorialFinished = true;
		}
	}


	IEnumerator _NextTutorialDelayed(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_NextTutorial();
	}


	bool _ShouldShowNextTutorial()
	{
		MaleficentTutorialsSystem nextTutorial = tutorials[currentTutorialIdx];
		bool alreadyShown = (PlayerPrefs.GetInt(_TutorialPlayerPrefsKey(nextTutorial), 0) != 0);

		return !alreadyShown;
	}

	string _TutorialPlayerPrefsKey(MaleficentTutorialsSystem _tutorialSystem)
	{
		string key =  string.Format("tutorial_{0}-{1}_already_shown", MaleficentBlackboard.Instance.level, currentTutorialSystem.textKey);
		return key;
	}

	#endregion

	#region Match3BoardGameLogic delegate

	void HandleOnStartGame()
	{
		_InitTutorials();
	}

	#endregion


	#region MaleficentTutorialSystem delegate

	void HandleOnTutorialSystemShow ()
	{

	}

	void HandleOnTutorialSystemHide ()
	{
		if (OnTutorialHide != null) {
			OnTutorialHide();
		}

		StartCoroutine(_NextTutorialDelayed(delayBetweenTutorials));
	}

	#endregion

}
