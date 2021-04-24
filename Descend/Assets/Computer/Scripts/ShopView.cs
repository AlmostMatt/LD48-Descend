using System.Collections;
using System.Collections.Generic;
using UnityBaseCode.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour
{
    public Transform shopItemsContainer;

    private RenderableGroup<ShopItem> mShopItemsGroup;

    public void Start()
    {
        mShopItemsGroup = new RenderableGroup<ShopItem>(shopItemsContainer, ShopItem.RenderToListOfShopItems);
    }

    public void Update()
    {
        UpdateForSale();
    }

    public void UpdateForSale()
    {
        List<ShopItem> results = new List<ShopItem>();
        results.Add(new ShopItem("Fancy pickaxe", "Lets you dig 25% faster", "pickaxeImage", 100));
        results.Add(new ShopItem("Fancier pickaxe", "Lets you dig 50% faster", "pickaxeImage", 300));
        results.Add(new ShopItem("Fanciest pickaxe", "Lets you dig 100% faster", "pickaxeImage", 500));
        results.Add(new ShopItem("Fancy pickaxe", "Lets you dig 25% faster", "pickaxeImage", 100));
        results.Add(new ShopItem("Fancier pickaxe", "Lets you dig 50% faster", "pickaxeImage", 300));
        results.Add(new ShopItem("Fanciest pickaxe", "Lets you dig 100% faster", "pickaxeImage", 500));
        results.Add(new ShopItem("Fancy pickaxe", "Lets you dig 25% faster", "pickaxeImage", 100));
        results.Add(new ShopItem("Fancier pickaxe", "Lets you dig 50% faster", "pickaxeImage", 300));
        results.Add(new ShopItem("Fanciest pickaxe", "Lets you dig 100% faster", "pickaxeImage", 500));
        mShopItemsGroup.UpdateRenderables(results);
    }
}
