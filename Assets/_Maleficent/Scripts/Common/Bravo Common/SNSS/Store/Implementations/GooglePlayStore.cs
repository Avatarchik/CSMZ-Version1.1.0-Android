using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID
public class GooglePlayStore : IStore {
	
	private string publicKey;
	
	private List<GooglePurchase> purchases;
	
	OnInitializationDone		onInitializationDone;
	OnInitializationFailed		onInitializationFailed;
	OnProductPurchased 			onProductPurchased;
	OnPurchaseFailed 			onPurchaseFailed;
	
	class GooglePlayProductInfo : IStore.ProductInfo {
		public string googlePlayId;
	}
	
	public GooglePlayStore(StoreConfig _config) {
		//Initialize
		foreach(StoreConfig.Product product in _config.products) {
			GooglePlayProductInfo productInfo = new GooglePlayProductInfo();
			productInfo.consumable = product.consumable;
			productInfo.googlePlayId = product.googlePlayId;
			
			products[product.id] = productInfo;
		}
		
		
		GoogleIABManager.billingSupportedEvent 			+= BillingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent 		+= BillingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent 	+= QueryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent 		+= QueryInventoryFailedEvent;
		GoogleIABManager.purchaseSucceededEvent 		+= PurchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent 			+= PurchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent 	+= ConsumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent 	+= ConsumePurchaseFailedEvent;
		
		publicKey = _config.googlePlayAppId;
	}
	
	public override void _Initialize(IStore.OnInitializationDone _onInitializationDone, IStore.OnInitializationFailed _onInitializationFailed) {
		onInitializationDone = (
			() => {
				_onInitializationDone();
			}
		);
		onInitializationFailed = _onInitializationFailed;
		
		GoogleIAB.init(publicKey);
	}
	
	
	public override void _PurchaseProduct(	string _id, 
											IStore.OnProductPurchased _onProductPurchased, 
											IStore.OnPurchaseFailed _onPurchaseFailed) {
		if(products[_id].purchased) { //Prime31 throws an error if the item is already purchased...
			_onProductPurchased(_id);
		} else {
			onProductPurchased = _onProductPurchased;
			onPurchaseFailed = _onPurchaseFailed;
			
			GooglePlayProductInfo googlePlayProductInfo = (GooglePlayProductInfo)products[_id];
			if(googlePlayProductInfo != null) {
				GoogleIAB.purchaseProduct(googlePlayProductInfo.googlePlayId);
			} else {
				_onPurchaseFailed("Product " + _id + " not found");
			}
		}
	}
	
	public override void _RestorePurchases(	IStore.OnRestorePurchasesFinished _onRestorePurchasesFinished, 
											IStore.OnProductPurchased _onProductPurchased, 
											IStore.OnPurchaseFailed _onPurchaseFailed) {
		//Send product purchase to each purchased item
		foreach(GooglePurchase purchase in purchases) {
			if(purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased) {
				_onProductPurchased(FindProductById(purchase.productId));
			}
		}
		_onRestorePurchasesFinished();
	}

	
	private string FindProductById(string _id) {
		foreach(KeyValuePair< string, IStore.ProductInfo > data in products) {
			GooglePlayProductInfo googlePlayProduct = (GooglePlayProductInfo)data.Value;
			if(googlePlayProduct.googlePlayId == _id)
				return data.Key;
		}
		return null;
	}
	
#region Events	
	void BillingSupportedEvent() {
		//According to prime31 doccumentation we should now retrieve the full inventory (or we'll get some weird errors...)
		List< string > inventory = new List< string >();
		foreach(string productId in products.Keys) {
			string googlePlayId = ((GooglePlayProductInfo)products[productId]).googlePlayId;
			if(googlePlayId != "") {
				inventory.Add(googlePlayId);
			} else {
				Debug.Log("Google play id for " + productId + " is empty");
			}
		}
		GoogleIAB.queryInventory(inventory.ToArray());
	}


	void BillingNotSupportedEvent(string _error) {
		onInitializationFailed(_error);
	}


	void QueryInventorySucceededEvent( List<GooglePurchase> _purchases, List<GoogleSkuInfo> _skus ) {
		purchases = _purchases;
		
		
		for(int j = 0; j < _skus.Count; ++ j) {
			foreach(IStore.ProductInfo product in products.Values) {
				if(((GooglePlayProductInfo)product).googlePlayId == _skus[j].productId) {
					product.productInfo.price = _skus[j].price;
					//product.productInfo.currencyCode = System.Globalization.RegionInfo.CurrentRegion.ISOCurrencySymbol ; //Doesn't work
					product.productInfo.currencyCode = "USD";
					break;
				}
			}
		}
		
		onInitializationDone();
		
		Debug.Log("Query inventory succeded");
	}


	void QueryInventoryFailedEvent(string _error) {
		onInitializationFailed(_error);
	}

	void PurchaseSucceededEvent( GooglePurchase purchase ){
		//check if this is a consumable item and consume it in that case
		string id = FindProductById(purchase.productId);		
		if(products[id].consumable) {
			//Consume it
			GoogleIAB.consumeProduct(purchase.productId);
		} else {
			onProductPurchased(id);
		}
	}


	void PurchaseFailedEvent( string error ) {
		onPurchaseFailed(error);
	}


	void ConsumePurchaseSucceededEvent( GooglePurchase purchase ) {
		onProductPurchased(FindProductById(purchase.productId));
	}


	void ConsumePurchaseFailedEvent( string error ) {
		onPurchaseFailed(error);
	}
#endregion
}
#endif
