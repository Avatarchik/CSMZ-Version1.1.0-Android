using UnityEngine;
using System.Collections;

public class TestBanners : MonoBehaviour 
{
	public GameObject banners;
	
	// Use this for initialization
	IEnumerator Start ()
	{
		yield return new WaitForSeconds(1f);
		
		banners.SetActive(true);
	}
	
}
