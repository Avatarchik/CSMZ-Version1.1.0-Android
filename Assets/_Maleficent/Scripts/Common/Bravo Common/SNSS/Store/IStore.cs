using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public abstract class IStore {

	public static string persistentDataPath() {
		#if UNITY_EDITOR
			return "StoredData";
		#else
			return Application.persistentDataPath;
		#endif	
	}

	
	public delegate void OnInitializationDone();
	public delegate void OnInitializationFailed(string _msg);
	public delegate void OnProductPurchased(string _id);
	public delegate void OnPurchaseFailed(string _msg);
	public delegate void OnRestorePurchasesFinished();
	public delegate void OnProductRetrieved(string _id, PublicProductInfo _info);
	public delegate void OnProductRetrieveFailed(string _id, string _msg);
	
	
	public class PublicProductInfo {
		public string price = "?";
		public string currencyCode = "?";
	}
	
	protected class ProductInfo {
		public bool consumable;
		public bool purchased = false;
		
		public PublicProductInfo  productInfo = new PublicProductInfo();
	}
	
	protected Dictionary< string, ProductInfo > products = new Dictionary<string, ProductInfo>();
	
	private bool initialized = false;
	
	private int purchasesCount;
	public int PurchasesCount {
		get {
			return purchasesCount;
		}
	}
	
	private void SaveStoreData() {
		using (FileStream stream = new FileStream(persistentDataPath() + "/store.sav", FileMode.Create)) {
			using (BinaryWriter writer = new BinaryWriter(stream)) {
				
				//Save non consumable items only
				int nConsumables = 0;
				foreach(string productId in products.Keys) {
					if(!products[productId].consumable) {
						nConsumables ++;
					}
				}
				writer.Write(nConsumables);
				writer.Write(purchasesCount);
				
				foreach(string productId in products.Keys) {
					if(!products[productId].consumable) {
						writer.Write(productId);
						writer.Write(products[productId].purchased);	
					}
				}
				
				
				writer.Close();
			}
		}
		
	}
	
	public void LoadStoreData() {
		string _path = persistentDataPath() + "/store.sav";
		if ((new FileInfo(_path)).Exists) {
			using (FileStream stream = new FileStream(_path, FileMode.Open)) {
				using (BinaryReader reader = new BinaryReader(stream)) {
					
					int nProducts = reader.ReadInt32();
					purchasesCount = reader.ReadInt32();
					for(int i = 0; i < nProducts; ++i) {
						string productId =  reader.ReadString();
						ProductInfo product = products[productId];
						if(product != null) {
							product.purchased = reader.ReadBoolean();
						} else {
							Debug.Log("Product " + productId + " not found");
						}
					}
			        reader.Close();
			    }
			}

		}
	}
	
	public void ProductPurchased(string _id) {
		if(products.ContainsKey(_id)) {
			if(!products[_id].consumable) {
				products[_id].purchased = true;
			}
			purchasesCount ++;
			SaveStoreData();
		} else {
			Debug.LogError("Product " + _id + " not found");
		}
	}
	
	public bool IsProductPurchased(string _id) {
		ProductInfo product = products[_id];
		if(product != null) {
			return products[_id].purchased;
		} 
		return false;
	}
	
	public string[] GetProducts() {
		string[] ret = new string[products.Keys.Count];
		int i = 0;
		foreach(string key in products.Keys) {
			ret[i ++] = key;
		}
		return ret;
	}
	
	public static void GetCurrencySymbolAndPrice(string _str, out float price, out string currency) {
		string priceTxt = "";
		currency = "";
		
		for(int i = 0; i < _str.Length; ++i) {
			if((_str[i] >= 48 && _str[i] < 58) || _str[i] == '.') {
				priceTxt = priceTxt + _str[i];
			} else if ( _str[i] == ',') {
				priceTxt = priceTxt + '.';
			} else if (_str[i] != ' '){
				currency = currency + _str[i];
			}
		}
		
		price = float.Parse(priceTxt);
	}
	
	/*public static string GetLocaleFromCurrencySymbol(string _symbol){
		string ret = "";
		CultureInfo[] cultures = CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures);
		foreach(CultureInfo culture in cultures) {
			string name = culture.DisplayName;
			try {
				System.Globalization.RegionInfo regionInfo = new System.Globalization.RegionInfo(name);
				if(regionInfo.CurrencySymbol == _symbol) {
					ret = _symbol;
					break;
				}
			} catch {
				
			}
		}
		return ret;
	}*/
	
	
	public void Initialize(OnInitializationDone _onInitializationDone, OnInitializationFailed _onInitializationFailed) {
		if(!initialized) {
			_Initialize(
				() => {
					initialized = true;
					_onInitializationDone();
				},
				_onInitializationFailed
			);
		} else {
			_onInitializationDone();
		}
	}
	
	public void PurchaseProduct(string _id, OnProductPurchased _onProductPurchased, OnPurchaseFailed _onPurchaseFailed) {
		if(initialized) {
			if(!products.ContainsKey(_id)) {
				_onPurchaseFailed("Product " + _id + " not registered on the Blackboard");
			} else {
				_PurchaseProduct(
					_id,
					(_pid) => {
						ProductPurchased(_pid);
						_onProductPurchased(_pid);
					},
					_onPurchaseFailed
				);
			}
		} else {
			_onPurchaseFailed("Shop not initialized");
		}
	}
	
	public void RestorePurchases(OnRestorePurchasesFinished _onRestorePurchasesFinished, OnProductPurchased _onProductPurchased, OnPurchaseFailed _onPurchaseFailed) {
		if(initialized) {
			_RestorePurchases(
				_onRestorePurchasesFinished, 
				(_id) => {
					ProductPurchased(_id);
					if(_onProductPurchased != null)
						_onProductPurchased(_id);
				},
				_onPurchaseFailed
			);
		} else {
			_onPurchaseFailed("Shop not initialized");
		}
	}
	
	public void GetProductInfo(string _id, OnProductRetrieved _onProductRetrieved, OnProductRetrieveFailed _onProductRetrieveFailed) {
		if(initialized){
			if(!products.ContainsKey(_id)) {
				_onProductRetrieveFailed(_id, "Product " + _id + " not registered on the Blackboard");
			} else {
				_onProductRetrieved(_id, products[_id].productInfo);
			}
		} else {
			_onProductRetrieveFailed(_id, "Shop not initialized");
		}
	}
	
	public abstract void _Initialize(OnInitializationDone _onInitializationDone, OnInitializationFailed _onInitializationFailed);
	public abstract void _PurchaseProduct(string _id, OnProductPurchased _onProductPurchased, OnPurchaseFailed _onPurchaseFailed);
	public abstract void _RestorePurchases(OnRestorePurchasesFinished _onRestorePurchasesFinished, OnProductPurchased _onProductPurchased, OnPurchaseFailed _onPurchaseFailed);
}
