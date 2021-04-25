using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public struct ShopItem
{
    public ShopItem(
        string name,
        string description,
        string imageName,
        int priceInDollars,
        string effectType,
        int effectValue,
        bool isOwned=false)
    {
        this.name = name;
        this.description = description;
        this.imageName = imageName;
        this.priceInDollars = priceInDollars;
        this.isOwned = isOwned;
        this.effectType = effectType;
        this.effectValue = effectValue;
    }
    public string name;
    public string description;
    public string imageName;
    public int priceInDollars;
    public bool isOwned;
    public string effectType;
    public int effectValue;

    public static void RenderToListOfShopItems(ShopItem shopItem, GameObject uiObject)
    {
        uiObject.GetComponent<Button>().interactable = (!shopItem.isOwned);
        uiObject.transform.Find("OwnedOverlay").gameObject.SetActive(shopItem.isOwned);

        uiObject.transform.Find("V/Name").GetComponent<Text>().text = shopItem.name;
        uiObject.transform.Find("V/Description").GetComponent<Text>().text = shopItem.description;
        // TODO: sprite manager to get image. (maybe in the constructor)
        // uiObject.transform.Find("V/Image").GetComponent<Image>();
        uiObject.transform.Find("V/Price").GetComponent<Text>().text = "$" + shopItem.priceInDollars;
    }
}
