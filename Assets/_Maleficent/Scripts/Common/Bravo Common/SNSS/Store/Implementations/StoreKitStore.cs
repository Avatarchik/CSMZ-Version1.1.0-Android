using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_IPHONE
public class StoreKitStore : IStore {
	private delegate void OnInitializationDone();
	
	OnInitializationDone 				onInitializationDone;
	IStore.OnRestorePurchasesFinished 	onRestorePurchasesFinished;
	IStore.OnProductPurchased 			onProductPurchased;
	IStore.OnPurchaseFailed 			onPurchaseFailed;
	
	class StoreKitProductInfo : IStore.ProductInfo {
		public string storeKitId;
	}
	
	public StoreKitStore(StoreConfig _config) {
		//Initialize
		foreach(StoreConfig.Product product in _config.products) {
			StoreKitProductInfo productInfo = new StoreKitProductInfo();
			productInfo.consumable = product.consumable;
			productInfo.storeKitId = product.iosStoreKitId;
			
			products[product.id] = productInfo;
		}
	
		//Event subscription	
		StoreKitManager.productListReceivedEvent 	  	+= ProductListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent 	+= ProductListRequestFailedEvent;
		StoreKitManager.purchaseSuccessfulEvent 		+= PurchaseSuccesfulEvent;
		StoreKitManager.purchaseFailedEvent 			+= PurchaseFailedEvent;
		StoreKitManager.purchaseCancelledEvent			+= PurchaseFailedEvent;
		StoreKitManager.restoreTransactionsFinishedEvent+= RestoreTransactionfinishedEvent;
		StoreKitManager.restoreTransactionsFailedEvent	+= RestoreTransactionsFailedEvent;
	}
	
	public override void _Initialize(IStore.OnInitializationDone _onInitializationDone, IStore.OnInitializationFailed _onInitializationFailed) {
		if(StoreKitBinding.canMakePayments()) {
			onInitializationDone = ( 
				() => {
					_onInitializationDone();
				}
			);
			
			onPurchaseFailed = (
				(_msg) => {
					_onInitializationFailed(_msg);
				}
			);
			
			string[] productsData = new string[products.Count];
			int i = 0;
			foreach(string productId in products.Keys) {
				productsData[i ++] = ((StoreKitProductInfo)products[productId]).storeKitId;
			}
			
			//Apple says it is mandatory doing this before anything else
			StoreKitBinding.requestProductData(productsData);
		} else {
			_onInitializationFailed("Payments are disabled by parental controls");
		}
	}
	
	public override void _PurchaseProduct(string _id, IStore.OnProductPurchased _onProductPurchased, IStore.OnPurchaseFailed _onPurchaseFailed) {
		onProductPurchased = _onProductPurchased;
		onPurchaseFailed = _onPurchaseFailed;
	
		StoreKitBinding.purchaseProduct(((StoreKitProductInfo)products[_id]).storeKitId, 1);
	}
	
	public override void _RestorePurchases(	IStore.OnRestorePurchasesFinished _onRestorePurchasesFinished, 
											IStore.OnProductPurchased _onProductPurchased, 
											IStore.OnPurchaseFailed _onPurchaseFailed) {
		onRestorePurchasesFinished = _onRestorePurchasesFinished;
		onProductPurchased = _onProductPurchased;
		onPurchaseFailed = _onPurchaseFailed;
											
		StoreKitBinding.restoreCompletedTransactions();	
	}
	
#region delegates
	void ProductListReceivedEvent(List< StoreKitProduct > allProducts) {
		//Debug.Log( "received total products: " + allProducts.Count );
		for(int j = 0; j < allProducts.Count; ++ j) {
			foreach(IStore.ProductInfo product in products.Values) {
				if(((StoreKitProductInfo)product).storeKitId == allProducts[j].productIdentifier) {
					product.productInfo.price = allProducts[j].formattedPrice;
					product.productInfo.currencyCode = allProducts[j].currencyCode;
					break;
				}
			}
		
			//((StoreKitProductInfo)products[allProducts[j].]).price = allProducts[j].price;
			//Debug.Log("product:" + allProducts[j] + "\n");	
		}
		
		
		if(allProducts.Count == 0) {
			onPurchaseFailed("No products received on initialization... are you using a bad provisioning profile? Try removing the app from the device and reinstalling again");
		} else {
			onInitializationDone();
		}
	}
	
	void ProductListRequestFailedEvent (string _error) {
		onPurchaseFailed(_error);
	}

	void PurchaseSuccesfulEvent(StoreKitTransaction _transaction) {
		string storeKitId = _transaction.productIdentifier;
		foreach(KeyValuePair< string, IStore.ProductInfo > data in products) {
			if(((StoreKitProductInfo)data.Value).storeKitId == storeKitId) {
				onProductPurchased(data.Key);
				break;
			}
		}
	}
	
	void PurchaseFailedEvent (string _msg) {
		onPurchaseFailed(_msg);
	}
	
	void RestoreTransactionfinishedEvent () {	
		onRestorePurchasesFinished();
	}
	
	void RestoreTransactionsFailedEvent (string _msg) {
		onPurchaseFailed(_msg);
	}
#endregion
}
#endif
