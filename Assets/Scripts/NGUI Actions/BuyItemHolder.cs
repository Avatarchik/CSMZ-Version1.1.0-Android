using UnityEngine;
using System.Collections;

public class BuyItemHolder : MonoBehaviour
{
	public delegate void SelectedItemChanged();
	
	public static event SelectedItemChanged OnSelectedItemChanged;
	public static System.Action<int, int, InAppPurchasesSystem.InAppPurchase> OnItemBuyPressed;
	
	public  static ItemHolder item;
	public static bool endGame = false;
	
	public UISprite icon;
	public UISprite iconBg;
	public UISprite buttonBg;
	public UILabel priceLabel;
	public UILabel packLabel;
	public UILabel itemCountLabel;
	public UILabel itemDescriptionLabel;
	public UILabel titleLabel;
	public int packIndex;
	
	public PlayMakerFSM buyFSM;
	
	public UISprite iconSale;
	public UILabel saleLabel;
	
	protected int itemCount;
	protected int freeItemCount;
	protected InAppPurchasesSystem.InAppPurchase purchaseId;
	
	public static void SetSelectedItem(ItemHolder newItem, bool _endGame = false)
	{
		item = newItem;
		endGame = _endGame;
		
		if (OnSelectedItemChanged != null) {
			OnSelectedItemChanged();
		}
	}
	
	void Start()
	{
		OnSelectedItemChanged += UpdateStats;
	}
	
	void ChangeScaleX(Transform xForm, float newX)
	{
		Vector3 newScale = xForm.localScale;
		newScale.x = newX;
		xForm.localScale = newScale;
	}
	
	void UpdateStats()
	{
		BasicItem itemComponent = item.itemPrefab.GetComponent<BasicItem>();
		
		icon.enabled = true;
		iconBg.enabled = true;
		icon.spriteName = itemComponent.iconName;
		ChangeScaleX(buttonBg.transform, 352f);
				
		if (packIndex == 0) {
			itemDescriptionLabel.text = Language.Get(itemComponent.NameSingular.Replace(" ", "_").ToUpper() + "_DESCRIPTION");
			titleLabel.text = Language.Get("BUY_ITEMS");
		}
		
		saleLabel.transform.localPosition = new Vector3(-224f, saleLabel.transform.localPosition.y, saleLabel.transform.localPosition.z);
		iconSale.transform.localPosition = new Vector3(-222f, iconSale.transform.localPosition.y, iconSale.transform.localPosition.z);
		
		if (itemComponent.GetType() == typeof(IcePick)) 
		{
			itemCount = TweaksSystem.Instance.intValues["IcePickPack" + packIndex];
			freeItemCount = 0;
		}
		else if (itemComponent.GetType() == typeof(Snowball)) 
		{
			itemCount = TweaksSystem.Instance.intValues["SnowballPack" + packIndex];
			freeItemCount = endGame ? 0 : TweaksSystem.Instance.intValues["SnowballFreePack" + packIndex];
		}
		else if (itemComponent.GetType() == typeof(Hourglass)) 
		{
			itemCount = TweaksSystem.Instance.intValues["HourglassPack" + packIndex];
			freeItemCount = endGame ? 0 : TweaksSystem.Instance.intValues["HourglassFreePack" + packIndex];
		}
		else {
			itemCount = TweaksSystem.Instance.intValues["ItemsPack" + packIndex];
			freeItemCount = 0;
			
			icon.enabled = false;
			iconBg.enabled = false;
			icon.spriteName = "coin_gold";
			ChangeScaleX(buttonBg.transform, 310f);
			
			if (packIndex == 0) {
				itemDescriptionLabel.text = Language.Get("ITEM_TOKEN_DESCRIPTION");
				titleLabel.text = Language.Get("GET_MORE_TOKENS");
			}
			
			saleLabel.transform.localPosition = new Vector3(-178f, saleLabel.transform.localPosition.y, saleLabel.transform.localPosition.z);
			iconSale.transform.localPosition = new Vector3(-176f, iconSale.transform.localPosition.y, iconSale.transform.localPosition.z);
		}
		
		if ((endGame && itemCount > TweaksSystem.Instance.intValues["SaleEndDefaultPack" + packIndex]) || 
			(!endGame && itemCount + freeItemCount > TweaksSystem.Instance.intValues["SaleDefaultPack" + packIndex])) 
		{
			saleLabel.enabled = true;
			iconSale.enabled = true;
		}
		else {
			saleLabel.enabled = false;
			iconSale.enabled = false;
		}
		
		itemCountLabel.text = (itemCount + freeItemCount).ToString();// + " " + ((itemCount == 1) ? itemComponent.NameSingular : itemComponent.NamePlural);

		/*
		if (itemComponent is Snowball) {
			purchaseId = (InAppPurchasesSystem.InAppPurchase)((int)InAppPurchasesSystem.InAppPurchase.SnowballSmallPack + packIndex);
		}
		else if (itemComponent is Hourglass) {
			purchaseId = (InAppPurchasesSystem.InAppPurchase)((int)InAppPurchasesSystem.InAppPurchase.HourglassSmallPack + packIndex);
		}
		else if (itemComponent is IcePick) {
			purchaseId = (InAppPurchasesSystem.InAppPurchase)((int)InAppPurchasesSystem.InAppPurchase.IcePickSmallPack + packIndex);
		}
		else {
			purchaseId = (InAppPurchasesSystem.InAppPurchase)((int)InAppPurchasesSystem.InAppPurchase.TokenSmallPack + packIndex);
		}*/
	}
	
	void OnClick()
	{
		if (buyFSM.ActiveStateName != "In") {
			return;
		}
		
		if (OnItemBuyPressed != null) {
			OnItemBuyPressed(itemCount + freeItemCount, packIndex, purchaseId);
		}
		
		buyFSM.SendEvent("BuyFinished");
	}
	
	void OnDestroy()
	{
		OnSelectedItemChanged -= UpdateStats;
	}
}

