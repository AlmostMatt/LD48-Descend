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

    public const int PICKAXE_COST_1 = 1000;
    public const int PICKAXE_COST_2 = 4000;
    public const int PICKAXE_COST_3 = 8000;
    public const int PICKAXE_COST_4 = 12000;
    public const int PICKAXE_COST_5 = 16000;

    // Start is called before the first frame update
    void Start()
    {
        mPurchaseMessage = transform.Find("ShopResult/Message").gameObject;
        mBuyButton = transform.Find("ShopResult/BuyButton").gameObject;
        // gray
        shopItemList.Add(new ShopItem("Iron pickaxe", "Strong enough to break stone!", "pickaxeImage1", PICKAXE_COST_1, "DigSkill", 1));
        // black
        shopItemList.Add(new ShopItem("Steel pickaxe", "Strong enough to break frozen dirt!", "pickaxeImage2", PICKAXE_COST_2, "DigSkill", 2));
        // red
        shopItemList.Add(new ShopItem("Reinforced pickaxe", "Strong enough to break frozen stone!", "pickaxeImage3", PICKAXE_COST_3, "DigSkill", 3));
        // gold
        shopItemList.Add(new ShopItem("Golden pickaxe", "Strong enough to break molten dirt!", "pickaxeImage4", PICKAXE_COST_4, "DigSkill", 4));
        // diamond
        shopItemList.Add(new ShopItem("Diamond-tipped pickaxe", "Strong enough to break anymolten stone!", "pickaxeImage5", PICKAXE_COST_5, "DigSkill", 5));
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
                if (item.effectValue == 1)
                {
                    NewsProgression.AddNewsForNextDay(NewsProgression.DebtNews());
                }
                SaveData.Get().digSkill = Mathf.Max(SaveData.Get().digSkill, item.effectValue);
                break;
            default:
                Debug.LogError("Purchase an item with unknown effect type: " + item.effectType);
                break;
        }
    }
}
