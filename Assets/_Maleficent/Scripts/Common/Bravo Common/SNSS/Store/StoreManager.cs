using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreManager {
	
	private static IStore store = null;
	private static IStore Store {
		get {
			if(store == null) {
				StoreConfig storeConfig = Blackboard.Instance.GetComponent< StoreConfig >();
				if(storeConfig == null) {
					Debug.LogError("StoreConfig not found on BravoTools, please add it");
					return null;
				}
				
#if (UNITY_IPHONE) && (!UNITY_EDITOR)
				SetStore(new StoreKitStore(storeConfig));
#elif (UNITY_ANDROID) && (!UNITY_EDITOR)
				SetStore (new GooglePlayStore(storeConfig));
#else
				SetStore (new FakeStore(storeConfig));
#endif
			}
			return store;	
		}
	}
	
	private static void SetStore(IStore _store){
		store = _store;
		store.LoadStoreData();
	}
	
	public static void Initialize(	IStore.OnInitializationDone _onInitializationDone,
									IStore.OnInitializationFailed _onInitializationFailed) {
		if(Store != null){
			Store.Initialize(_onInitializationDone, _onInitializationFailed);
		} else {
			_onInitializationFailed("No store registered for this device");
		}
	}
	
	public static void PurchaseProduct(	string _productId, 
										IStore.OnProductPurchased _onProductPurchased, 
										IStore.OnPurchaseFailed _onPurchaseFailed) {
		if(Store != null){
			Store.PurchaseProduct(_productId, _onProductPurchased, _onPurchaseFailed);
		} else {
			_onPurchaseFailed("No store registered for this device");
		}
	}
	
	public static void RestorePurchases(	
									IStore.OnRestorePurchasesFinished _onRestorePurchasesFinished, 
									IStore.OnProductPurchased _onProductPurchased, 
									IStore.OnPurchaseFailed _onPurchaseFailed) {
		if(Store != null) {
			Store.RestorePurchases(	_onRestorePurchasesFinished, _onProductPurchased, _onPurchaseFailed);
		} else {
			_onPurchaseFailed("No store registered for this device");
		}
	}
	
	public static void GetProductInfo(string _id,
								IStore.OnProductRetrieved _onProductRetrieved, 
								IStore.OnProductRetrieveFailed _onProductRetrieveFailed) {
		if(Store != null) {
			Store.GetProductInfo(_id, _onProductRetrieved, _onProductRetrieveFailed);
		} else {
			_onProductRetrieveFailed(_id, "No store registered for this device");
		}
	}
	
	public static bool IsProductPurchased(string _productId) {
		if(Store != null) {
			return Store.IsProductPurchased(_productId);
		}
		return false;
	}
	
	public static string[] GetProducts() {
		if(Store != null) {
			return Store.GetProducts();
		}
		return new string[] {};
	}
	
	public static int GetPurchasesCount() {
		if(Store != null) {
			return Store.PurchasesCount;
		}
		return 0;
	}
}
