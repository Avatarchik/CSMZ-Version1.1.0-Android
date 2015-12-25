using UnityEngine;
using System.Collections;

public class MaleficentTutorialLabel : MonoBehaviour
{
	public GameObject[] items;
	public AudioClip playSoundIn;
	public AudioClip playSoundOut;
	
	protected UILabel myLabel;
	protected GameObject daddy;
//	protected ManaItem designatedItem;
	protected MaleficentTutorialsSystem currentTutorialSystem {
		get {
			return MaleficentTutorialsManager.Instance.currentTutorialSystem;
		}
	}


	void Awake()
	{
		myLabel = GetComponent<UILabel>();
	}


	void Start()
	{
		MaleficentTutorialsManager tutorialManager = MaleficentTutorialsManager.Instance;
		if (tutorialManager != null) {
			tutorialManager.OnTutorialShow += HandleOnTutorialShow;
			tutorialManager.OnTutorialHide += HandleOnTutorialHide;
		}

		daddy = transform.parent.gameObject;
	}

	#region Private

	void _ConfigureTutorial()
	{
		_ConfigureLabel();
		_CheckPlayerPrefs();
		_ConfigureItems();


		daddy.transform.parent.localPosition = currentTutorialSystem.messagePosition;
		currentTutorialSystem.animationTime = daddy.GetComponent<Animation>()["ScaleDown"].length + 0.01f;
	}

	void _ConfigureLabel()
	{
		string text = Language.Get (currentTutorialSystem.textKey);

		if (currentTutorialSystem.justTap) {
			//text += "\n" + Language.Get("TUTORIAL_TAP");
		}

		if (currentTutorialSystem.giveManaAmount > 0) {
			text = text.Replace("<points>", currentTutorialSystem.giveManaAmount.ToString());
		}

		myLabel = GetComponent<UILabel>();
		myLabel.text = text;


	}


	void _CheckPlayerPrefs()
	{
		//TODO:...
	}


	void _ConfigureItems()
	{
		MaleficentTutorialsSystem tutorialSystem = currentTutorialSystem;
		int highlightItemIdx = tutorialSystem.highlightItemIdx;

		if (tutorialSystem.isHighlightItemTutorial && highlightItemIdx < items.Length) {

			if (tutorialSystem.highlightObjects[0] == null) {
				tutorialSystem.highlightObjects[0] = items[highlightItemIdx];
			}
			else {
				tutorialSystem.highlightObjects[1] = items[highlightItemIdx];
			}
//			designatedItem = items[highlightItemIdx].GetComponentsInChildren<ManaItem>(true)[0];
		}
	}


	void _ShowTutorialPanel()
	{
		daddy.GetComponent<Animation>().Play("ScaleUp");
	}


	void _HideTutorialPanel()
	{
		daddy.GetComponent<Animation>().Play("ScaleDown");
	}


//	void _SetItemsActivated(bool activated)
//	{
//		for (int i = 0; i < items.Length; ++i) {
//			if (i != currentTutorialSystem.highlightItemIdx) {
//				if (items[i].activeInHierarchy) {
//					items[i].GetComponentInChildren<BoxCollider>().enabled = activated;
//				}
//			}
//		}
//	}

	#endregion

	#region MaleficentTutorialManager delegate

	void HandleOnTutorialHide ()
	{
		_HideTutorialPanel();

		if (MaleficentTutorialsManager.Instance.IsLastTutorial()) {
			float waitTime = daddy.GetComponent<Animation>()["ScaleDown"].length + 0.01f;
			Destroy(daddy, waitTime);
		}
	}

	void HandleOnTutorialShow ()
	{
		_ConfigureTutorial();
		_ShowTutorialPanel();
	}

	#endregion
	
//	void Start()
//	{
//		TutorialsSystem tutorialSystem = TutorialsSystem.Instance;
//		if (tutorialSystem == null) {
//			Debug.LogWarning("No tutorial system");
//			return;
//		}
//		
//		//ConfigureTutorial();
//		
//		if (tutorialSystem.itemIdx >= 0 && tutorialSystem.itemIdx < items.Length) 
//		{
//			if (tutorialSystem.highlightObjects[0] == null) {
//				tutorialSystem.highlightObjects[0] = items[tutorialSystem.itemIdx];
//			}
//			else {
//				tutorialSystem.highlightObjects[1] = items[tutorialSystem.itemIdx];
//			}
//			
//			StartCoroutine(WaitForSetItem());
//		}
//		TutorialsSystem.OnTutorialShow += ShowTutorial;
//	}
	
//	IEnumerator WaitForSetItem()
//	{
//		yield return null;
//		
//		designatedItem = items[TutorialsSystem.Instance.itemIdx].GetComponentsInChildren<ItemHolder>(true)[0];
//		TutorialsSystem.Instance.GiveFreeItems(designatedItem);
//	}
	
//	void ActivateItems(bool activate)
//	{
//		for (int i = 0; i < items.Length; ++i) {
//			if (i != TutorialsSystem.Instance.itemIdx) {
//				if (items[i].activeInHierarchy) {
//					items[i].GetComponentInChildren<BoxCollider>().enabled = activate;
//				}
//			}
//		}
//	}
	
//	public void ShowTutorial()
//	{
//		ConfigureTutorial();
//		
//		if (TutorialsSystem.Instance.justTap) {
//			myLabel.text = myLabel.text + "\n" + Language.Get("TUTORIAL_TAP");
//		}
//		
//		ActivateItems(false);
//		
//		daddy.animation.Play("ScaleUp");
//		if (playSoundIn != null) {
//			NGUITools.PlaySound(playSoundIn);
//		}
//		
//		if (TutorialsSystem.Instance.itemIdx >= 0) 
//		{
//			designatedItem.OnItemClick += HideTutorial;
//			TutorialsSystem.OnTutorialHide += HideTutorial;
//		}
//		else {
//			TutorialsSystem.OnTutorialHide += HideTutorial;
//		}
//	}
	
//	void HideTutorial()
//	{
//		TutorialsSystem.OnTutorialHide -= HideTutorial;
//
//		if (designatedItem != null) {
//			designatedItem.OnItemClick -= HideTutorial;
//		}
//		
//		ActivateItems(true);
//		
//		if (TutorialsSystem.Instance.itemIdx >= 0) {
//			TutorialsSystem.Instance.disableTutorial = true;
//		}
//		
//		daddy.animation.Play("ScaleDown");
//		if (playSoundOut != null) {
//			NGUITools.PlaySound(playSoundOut);
//		}
//		
//		StartCoroutine(WaitForAnimation());
//	}
	
//	void ConfigureTutorial()
//	{
//		TutorialsSystem tutorialSystem = TutorialsSystem.Instance;
//		myLabel = GetComponent<UILabel>();
//		myLabel.text = Language.Get(tutorialSystem.textKey);
//		
//		if (PlayerPrefs.GetInt(tutorialSystem.textKey, 0) != 0 || 
//		    MaleficentBlackboard.Instance.level < LoadLevelButton.lastUnlockedLevel) 
//		{ 
//			//already shown tutorial
//			myLabel.text = myLabel.text.Replace("\n<TOKEN>", "");
//			myLabel.text = myLabel.text.Replace("<TOKEN>", "");
//			myLabel.text = myLabel.text.Replace("\n<FREE_ICE_PICKS>", "");
//			myLabel.text = myLabel.text.Replace("<FREE_ICE_PICKS>", "");
//			myLabel.text = myLabel.text.Replace("\n<FREE_SNOWBALLS>", "");
//			myLabel.text = myLabel.text.Replace("<FREE_SNOWBALLS>", "");
//			myLabel.text = myLabel.text.Replace("\n<FREE_HOURGLASSES>", "");
//			myLabel.text = myLabel.text.Replace("<FREE_HOURGLASSES>", "");
//		}
//		else {
//			myLabel.text = myLabel.text.Replace("<TOKEN>", Language.Get("TUTORIAL_ITEM_TOKENS"));
//			myLabel.text = myLabel.text.Replace("<FREE_ICE_PICKS>", Language.Get("TUTORIAL_FREE_ICE_PICKS"));
//			myLabel.text = myLabel.text.Replace("<FREE_SNOWBALLS>", Language.Get("TUTORIAL_FREE_SNOWBALLS"));
//			myLabel.text = myLabel.text.Replace("<FREE_HOURGLASSES>", Language.Get("TUTORIAL_FREE_HOURGLASSES"));
//		}
//
//
//		daddy = transform.parent.gameObject;
//		daddy.transform.parent.localPosition = tutorialSystem.messagePosition;
//
//		tutorialSystem.animationTime = daddy.animation["ScaleDown"].length + 0.01f;
//	}
	
//	IEnumerator WaitForAnimation()
//	{
//		bool shouldDestroy = false ;
//		if (MaleficentTutorialsManager.Instance.IsLastTutorial()) {
//			shouldDestroy = true;
//		}
//
//
//		yield return new WaitForSeconds(daddy.animation["ScaleDown"].length + 0.01f);
		
//		if (shouldDestroy) {
//			Destroy(daddy);
//		}
//	}
	
//	void OnDestroy()
//	{
//		TutorialsSystem.OnTutorialShow -= ShowTutorial;
//		TutorialsSystem.OnTutorialHide -= HideTutorial;;
//	}

}
