using System.Collections;
using System.Collections.Generic;
using UnityBaseCode.Rendering;
using UnityEngine;

public class ShopView2 : MonoBehaviour
{
    public GameObject shopItemDisplay;
    public GameObject noStockDisplay;

    private GameObject mPurchaseMessage;
    private GameObject mBuyButton;

    private List<ShopItem> shopItemList = new List<ShopItem>();
    private int mActiveItem = 0;

    // Start is called before the first frame update
    void Start()
    {
        mPurchaseMessage = transform.Find("ShopResult/Message").gameObject;
        mBuyButton = transform.Find("ShopResult/BuyButton").gameObject;
        shopItemList.Add(new ShopItem("Iron pickaxe", "Strong enough to break any stone!", "pickaxeImage", 15000, "DigSkill", 1));
    }

    // Update is called once per frame
    void Update()
    {
        SaveData saveData = SaveData.Get();
        mActiveItem = saveData.activeShopItem;

        if(mActiveItem >= 0)
        {
            noStockDisplay.SetActive(false);
            shopItemDisplay.SetActive(true);

            ShopItem item = shopItemList[mActiveItem];
            ShopItem.RenderToSingleShopItem(item, shopItemDisplay);
            mPurchaseMessage.SetActive(item.isOwned);
            mBuyButton.SetActive(!item.isOwned);
        }
        else
        {
            shopItemDisplay.SetActive(false);
            noStockDisplay.SetActive(true);
        }
    }

    public void PurchaseItem()
    {
        ShopItem item = shopItemList[mActiveItem];
        SaveData.Get().debt += item.priceInDollars;
        SaveData.Get().IncreaseMinimumPayment(100);
        shopItemList[mActiveItem] = new ShopItem(item.name, item.description, item.imageName, item.priceInDollars, item.effectType, item.effectValue, true);
        // The effect of the item
        switch (item.effectType)
        {
            case "DigSkill":
                SaveData.Get().digSkill = Mathf.Max(SaveData.Get().digSkill, item.effectValue);
                break;
            default:
                Debug.LogError("Purchase an item with unknown effect type: " + item.effectType);
                break;
        }
    }
}
