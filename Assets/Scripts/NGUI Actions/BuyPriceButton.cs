using UnityEngine;
using System.Collections;

public class BuyPriceButton : MonoBehaviour
{
	public UILabel priceLabel;
	public UISprite itemIcon;
	public UILabel countLabel;
	public UILabel titleLabel;
	public UILabel saleLabel;
	public UISprite saleIcon;
	public PlayMakerFSM myFSM;
	
	protected InAppPurchasesSystem.InAppPurchase purchaseId;
	protected int count;
	protected int packIndex;
	
	protected static bool purchasing = false;
	
	void Start ()
	{
		BuyItemHolder.OnItemBuyPressed += UpdateStats;
	}
	
	void UpdateStats(int _count, int _packIndex, InAppPurchasesSystem.InAppPurchase _purchaseId)
	{
		purchaseId = _purchaseId;
		packIndex = _packIndex;
		count = _count;
		
		BasicItem itemComponent = BuyItemHolder.item.itemPrefab.GetComponent<BasicItem>();
		string purchaseIdString = InAppPurchasesSystem.Instance.GetPurchaseId(purchaseId);
		
		itemIcon.spriteName = itemComponent.iconName;
		titleLabel.text = purchaseIdString.Contains("token") ? Language.Get ("POWER_UPS") : itemComponent.NamePlural;
		countLabel.text = count.ToString();
		
		InAppProduct product = InAppPurchasesSystem.Instance.GetProduct(purchaseIdString);
		if (product == null) {
			priceLabel.text = "$" + (packIndex == 0 ? "0.99" : (packIndex == 1 ? "1.99" : "2.99"));
		}
		else {
			priceLabel.text = product.formattedPrice;
		}
		
		int defaultCount = BuyItemHolder.endGame ? TweaksSystem.Instance.intValues["SaleEndDefaultPack" + packIndex] :
			TweaksSystem.Instance.intValues["SaleDefaultPack" + packIndex];
		
		if (count > defaultCount) 
		{
			saleLabel.enabled = true;
			saleIcon.enabled = true;
			saleLabel.text = Language.Get("MORE_FREE").Replace("<percentage>",  (int)((float)(count - defaultCount) / defaultCount * 100) + "%");
		}
		else {
			saleLabel.enabled = false;
			saleIcon.enabled = false;
		}
	}
	
	void OnClick()
	{
		Debug.Log("purchasing: "+purchasing);
		Debug.Log("in: "+myFSM.ActiveStateName != "In");
		if (purchasing || myFSM.ActiveStateName != "In") {
			return;
		}
		
		purchasing = true;
		
		InAppPurchasesSystem.OnPurchaseSuccess += OnPurchaseSuccess;
		InAppPurchasesSystem.OnPurchaseFail += OnPurchaseFail;
		InAppPurchasesSystem.OnPurchaseCancel += OnPurchaseFail;
		InAppPurchasesSystem.Instance.PurchaseProduct(purchaseId);
	}

	void OnPurchaseFail (string id)
	{
		purchasing = false;
		
		InAppPurchasesSystem.OnPurchaseSuccess -= OnPurchaseSuccess;
		InAppPurchasesSystem.OnPurchaseFail -= OnPurchaseFail;
		InAppPurchasesSystem.OnPurchaseCancel -= OnPurchaseFail;
	}

	void OnPurchaseSuccess (string id)
	{
		purchasing = false;
		
		InAppPurchasesSystem.OnPurchaseSuccess -= OnPurchaseSuccess;
		InAppPurchasesSystem.OnPurchaseFail -= OnPurchaseFail;
		InAppPurchasesSystem.OnPurchaseCancel -= OnPurchaseFail;
		
		InAppProduct product = InAppPurchasesSystem.Instance.GetProduct(id);
		if (product != null) 
		{
			BasicItem itemComponent = BuyItemHolder.item.itemPrefab.GetComponent<BasicItem>();

			/*
			// commented temporaly in order to avoid the warning 'subtype' is assigned but its value its never used
			string subtype = "small";
			if (packIndex == 1) {
				subtype = "medium";
			}
			else if (packIndex == 2) {
				subtype = "large";
			}
			*/

			float price;
			if (!float.TryParse(product.price , out price)) {
				price = 0.99f + packIndex * 1f;
			}
			
			AnalyticsBinding.LogEventPaymentAction(product.currencyCode, InAppPurchasesSystem.locale, -price, id, 1, itemComponent.ItemName, 
				"consumable", BuyItemHolder.endGame ? "postgame" : "ingame", MaleficentBlackboard.Instance.level);

		}
		
		BuyItemHolder.item.AddItems(count);
		myFSM.SendEvent("BuyFinished");
	}
	
	void OnDestroy()
	{
		BuyItemHolder.OnItemBuyPressed -= UpdateStats;
		
		InAppPurchasesSystem.OnPurchaseSuccess -= OnPurchaseSuccess;
		InAppPurchasesSystem.OnPurchaseFail -= OnPurchaseFail;
		InAppPurchasesSystem.OnPurchaseCancel -= OnPurchaseFail;
	}
}

