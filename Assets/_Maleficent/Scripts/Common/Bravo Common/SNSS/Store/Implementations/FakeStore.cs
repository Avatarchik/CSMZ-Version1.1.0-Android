#define SIMULATE_SUCCESS

using UnityEngine;
using System.Collections;

public class FakeStore : IStore {
	public FakeStore(StoreConfig _config) {
		//Initialize
		foreach(StoreConfig.Product product in _config.products) {
			IStore.ProductInfo productInfo = new IStore.ProductInfo();
			
			productInfo.consumable = product.consumable;
			products[product.id] = productInfo;
		}
	}
	
	public override void _Initialize(IStore.OnInitializationDone _onInitializationDone, IStore.OnInitializationFailed _onInitializationFailed) {
		//TRN2Utils.DoAfterSeconds(Blackboard.Instance, 2.0f, () => {
			#if SIMULATE_SUCCESS
				_onInitializationDone();
			#else
				_onInitializationFailed("Couldn't initialize shop");
			#endif
		//});
	}

	public override void _PurchaseProduct(	string _id,
										 	IStore.OnProductPurchased _onProductPurchased, 
										 	IStore.OnPurchaseFailed _onPurchaseFailed) {
		//TRN2Utils.DoAfterSeconds(Blackboard.Instance, 2.0f, () => {
			#if SIMULATE_SUCCESS
				_onProductPurchased(_id);
			#else
				_onPurchaseFailed("Purchase failed");
			#endif
		//});
	}
	
	public override void _RestorePurchases(	IStore.OnRestorePurchasesFinished _onRestorePurchasesFinished, 
											IStore.OnProductPurchased _onProductPurchased, 
											IStore.OnPurchaseFailed _onPurchaseFailed) {
		//TRN2Utils.DoAfterSeconds(Blackboard.Instance, 2.0f, () => {
			#if SIMULATE_SUCCESS
				_onRestorePurchasesFinished();
			#else
				_onPurchaseFailed("Restore purchases failed");
			#endif
		//});
	}
}
