using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreConfig : MonoBehaviour {
	
	public string googlePlayAppId;
	
	[System.Serializable]
	public class Product {
		public string id;
		public bool consumable;
		public string iosStoreKitId;
		public string googlePlayId;
	}
	public Product[] products;
}
