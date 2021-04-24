using System.Collections;
using System.Collections.Generic;
using UnityBaseCode.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour
{
    public Transform shopItemsContainer;
    public Text balanceText;

    private RenderableGroup<ShopItem> mShopItemsGroup;
    private List<ShopItem> shopItemList = new List<ShopItem>();

    public void Start()
    {
        mShopItemsGroup = new RenderableGroup<ShopItem>(shopItemsContainer, ShopItem.RenderToListOfShopItems);
        InitializeShopItems();
    }

    private void InitializeShopItems()
    {
        shopItemList.Add(new ShopItem("Fancy pickaxe", "Lets you dig 25% faster", "pickaxeImage", 100));
        shopItemList.Add(new ShopItem("Fancier pickaxe", "Lets you dig 50% faster", "pickaxeImage", 300));
        shopItemList.Add(new ShopItem("Fanciest pickaxe", "Lets you dig 100% faster", "pickaxeImage", 500));
        shopItemList.Add(new ShopItem("Fancy pickaxe", "Lets you dig 25% faster", "pickaxeImage", 100));
        shopItemList.Add(new ShopItem("Fancier pickaxe", "Lets you dig 50% faster", "pickaxeImage", 300));
        shopItemList.Add(new ShopItem("Fanciest pickaxe", "Lets you dig 100% faster", "pickaxeImage", 500));
        shopItemList.Add(new ShopItem("Fancy pickaxe", "Lets you dig 25% faster", "pickaxeImage", 100));
        shopItemList.Add(new ShopItem("Fancier pickaxe", "Lets you dig 50% faster", "pickaxeImage", 300));
        shopItemList.Add(new ShopItem("Fanciest pickaxe", "Lets you dig 100% faster", "pickaxeImage", 500));
        UpdateForSale();
    }

    public void Update()
    {
        // Update the current balance
        balanceText.text = "Balance: $" + PlayerState.Get().GetBalance(); 

        // Update the list of items
        UpdateForSale();
    }

    public void UpdateForSale()
    {
        mShopItemsGroup.UpdateRenderables(shopItemList);
    }

    public void PurchaseItem(ShopItemButton shopItemButton)
    {
        int itemIndex = shopItemButton.transform.GetSiblingIndex();
        ShopItem item = shopItemList[itemIndex];
        PlayerState.Get().DecreaseBalance(item.priceInDollars);
        shopItemList[itemIndex] = new ShopItem(item.name, item.description, item.imageName, item.priceInDollars, true);
        // TODO: find the corresponding ShopItemButton struct
        // TODO: purchase the corresponding item
    }
}
