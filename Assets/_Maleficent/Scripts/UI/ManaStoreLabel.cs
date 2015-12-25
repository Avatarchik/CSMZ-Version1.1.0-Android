using UnityEngine;
using System.Collections;

public class ManaStoreLabel : MonoBehaviour {

	UILabel label;

	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel>();
		TokensSystem.OnManaModified += HandleOnManaModified;
		HandleOnManaModified(0);
	}

	void HandleOnManaModified (int amount) {
		label.text = TokensSystem.Instance.ManaPoints.ToString();
	}

	void OnDestroy()
	{
		TokensSystem.OnManaModified -= HandleOnManaModified;
	}
}
