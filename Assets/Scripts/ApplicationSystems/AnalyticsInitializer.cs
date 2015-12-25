using UnityEngine;
using System.Collections;

public class AnalyticsInitializer : MonoBehaviour {

	void Awake()
	{
		AnalyticsBinding.Initialize();
	}
}
