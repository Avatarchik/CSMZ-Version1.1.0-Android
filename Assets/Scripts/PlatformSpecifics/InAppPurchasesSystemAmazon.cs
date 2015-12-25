using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InAppPurchasesSystemAmazon : InAppPurchasesSystem {

	private bool sdkAvailable = true;

#if AMAZON_ANDROID
	private static AmazonIAPManager amazonManager = null;
	private List<AmazonItem> availableItems;
#endif

	#region Overrides

	protected override void Awake ()
	{
#if AMAZON_ANDROID
		AmazonIAPManager.onSdkAvailableEvent += (bool _testmode) => {
			sdkAvailable = true;
			Debug.Log("SDKAvailable, testmode:" + _testmode);
		};

		if(amazonManager == null) {
			GameObject managerObj = new GameObject();
			amazonManager = managerObj.AddComponent<AmazonIAPManager>();
			DontDestroyOnLoad(managerObj);
		}

		showDialogs = false;
#endif

		base.Awake ();
	}
	
	 protected override void PlatformRequestProductList(string[] products)
	{
#if AMAZON_ANDROID
		if(sdkAvailable && !requestingProducts) {
			requestingProducts = true;

			AmazonIAPManager.itemDataRequestFinishedEvent += HandleitemDataRequestFinishedEvent;
			AmazonIAPManager.itemDataRequestFailedEvent += HandleitemDataRequestFailedEvent;

			AmazonIAP.initiateItemDataRequest(GetProducts());
		}else if(!sdkAvailable) {
			Debug.LogError("AmazonIAP: SDK NOT available");
		}
#endif
	}
	
	 public override InAppProduct GetProduct(string productId)
	{
		InAppProduct res = null;

#if AMAZON_ANDROID
		if (availableItems == null) {
			res = base.GetProduct(productId);
		}else {
			foreach (AmazonItem product in availableItems) 
			{
				if (product.sku == productId) 
				{
					InAppProduct myProduct = new InAppProduct();
					
					myProduct.id = productId;
					myProduct.formattedPrice = product.price;
					myProduct.price = product.price;
					myProduct.currencyCode = "NUL";

					res = myProduct;
					break;
				}
			}
		}
#endif
		
		return res;
	}
	
	public override void PurchaseProduct(InAppPurchase purchase)
	{
		#if UNITY_EDITOR
		base.PurchaseProduct(purchase);
		#else
#if AMAZON_ANDROID
		if (sdkAvailable) {
			purchasingProduct = purchase;
			
			RequestProductList();

			AmazonIAPManager.purchaseSuccessfulEvent += HandlePurchaseSuccesful;
			AmazonIAPManager.purchaseFailedEvent += HandlePurchaseFailed;
			AmazonIAP.initiatePurchaseRequest(GetPurchaseId(purchase));

		}
		else {
			OnInAppDisabled(purchase);
		}
		#endif
#endif
	}

	#endregion

			
	#region Event Handlers

	 protected void HandleitemDataRequestFailedEvent ()
	 {
#if AMAZON_ANDROID
		AmazonIAPManager.itemDataRequestFinishedEvent -= HandleitemDataRequestFinishedEvent;
		AmazonIAPManager.itemDataRequestFailedEvent -= HandleitemDataRequestFailedEvent;

		Debug.LogError("AmazonIAP: item data request failed");

		requestingProducts = false;
#endif
	 }

	protected void HandleitemDataRequestFinishedEvent (List<string> _unavailableSkus, List<AmazonItem> _availableItems)
	 {
#if AMAZON_ANDROID
		AmazonIAPManager.itemDataRequestFinishedEvent -= HandleitemDataRequestFinishedEvent;
		AmazonIAPManager.itemDataRequestFailedEvent -= HandleitemDataRequestFailedEvent;

		if (_availableItems != null && _availableItems.Count > 0) {
			availableItems = _availableItems;
			receivedProductList = true;
		}else {
			Debug.Log("AmazonIAP: Can NOT find any available item");
		}

		if(_unavailableSkus != null) {
			foreach (string sku in _unavailableSkus) {
				Debug.LogError("Can NOT find product with sku "+ sku);
			}
		}
	
		requestingProducts = false;
#endif
	 }

	protected void HandlePurchaseSuccesful (AmazonReceipt receipt) 
	{
#if AMAZON_ANDROID
		AmazonIAPManager.purchaseSuccessfulEvent -= HandlePurchaseSuccesful;
		AmazonIAPManager.purchaseFailedEvent -= HandlePurchaseFailed;

		base.OnProductAwaitingVerification();

		if (receipt.sku == GetPurchaseId(purchasingProduct)) {
			base.OnVerificationSuccess();
		}else {
			base.OnProductCanceled("Verification error");
		}
#endif
	}

	protected void HandlePurchaseFailed (string error)
	{
#if AMAZON_ANDROID
		AmazonIAPManager.purchaseSuccessfulEvent -= HandlePurchaseSuccesful;
		AmazonIAPManager.purchaseFailedEvent -= HandlePurchaseFailed;

		base.OnProductFailed(error);
#endif
	}

	#endregion
}
