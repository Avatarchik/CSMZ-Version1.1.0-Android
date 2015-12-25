using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ProductInfoReceivedEventArgs : EventArgs
{
	public bool Result {get; set; }
	public string Message {get; set;}
	public string Currency {get; set;}
	public string Locale {get; set;}
}

public class StoreKitUtilsBinding : MonoBehaviour {
	
	#region iOS Interface
	#if !UNITY_EDITOR && UNITY_IPHONE
	
	[DllImport ("__Internal")]
	private static extern void RequestProductsDataWithIdentifier(string identifier);

	#endif	
	#endregion
	
	public delegate void ProductLocaleDelegate(string locale);
	
	public event ProductLocaleDelegate OnProductLocaleReceived;
		
	protected static StoreKitUtilsBinding instance;
	protected static bool dontReload = false;
	
	public static StoreKitUtilsBinding Instance {
		get {
			if (instance == null && !dontReload) {
				GameObject container = GameObject.Instantiate(Resources.Load("StoreKitUtilsBinding") as GameObject) as GameObject;
				container.name = container.name.Replace("(Clone)", "");
				DontDestroyOnLoad(container);
			}
			
			return instance;
		}
	}
	
	void Awake ()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
	
	public void RequestProductInformation(string identifier)
	{
#if !UNITY_EDITOR && UNITY_IPHONE
		RequestProductsDataWithIdentifier(identifier);
#else
		ProductLocaleReceived("error");
#endif
	}
	
	public void ProductLocaleReceived(string locale) 
	{
		Debug.Log("PRODUCT LOCALE RECEIVED: " + locale);
		
		if (locale == "error") {
			locale = "US";
		}
		
		if (OnProductLocaleReceived != null)
			OnProductLocaleReceived(locale);
	}
}
