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
        shopItemList.Add(new ShopItem("Fancy pickaxe", "Gives dig skill 1", "pickaxeImage", 100, "DigSkill", 1));
        shopItemList.Add(new ShopItem("Fancier pickaxe", "Gives DigSkill 2", "pickaxeImage", 300, "DigSkill", 2));
        shopItemList.Add(new ShopItem("Fanciest pickaxe", "Gives DigSkill 3", "pickaxeImage", 500, "DigSkill", 3));
        shopItemList.Add(new ShopItem("Fancy pickaxe", "Gives dig skill 1", "pickaxeImage", 100, "DigSkill", 1));
        shopItemList.Add(new ShopItem("Fancier pickaxe", "Gives DigSkill 2", "pickaxeImage", 300, "DigSkill", 2));
        shopItemList.Add(new ShopItem("Fanciest pickaxe", "Gives DigSkill 3", "pickaxeImage", 500, "DigSkill", 3));
        shopItemList.Add(new ShopItem("Fancy pickaxe", "Gives dig skill 1", "pickaxeImage", 100, "DigSkill", 1));
        shopItemList.Add(new ShopItem("Fancier pickaxe", "Gives DigSkill 2", "pickaxeImage", 300, "DigSkill", 2));
        shopItemList.Add(new ShopItem("Fanciest pickaxe", "Gives DigSkill 3", "pickaxeImage", 500, "DigSkill", 3));
        shopItemList.Add(new ShopItem("Fancy pickaxe", "Gives dig skill 1", "pickaxeImage", 100, "DigSkill", 1));
        shopItemList.Add(new ShopItem("Fancier pickaxe", "Gives DigSkill 2", "pickaxeImage", 300, "DigSkill", 2));
        shopItemList.Add(new ShopItem("Fanciest pickaxe", "Gives DigSkill 3", "pickaxeImage", 500, "DigSkill", 3));
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
        shopItemList[itemIndex] = new ShopItem(item.name, item.description, item.imageName, item.priceInDollars, item.effectType, item.effectValue, true);
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
