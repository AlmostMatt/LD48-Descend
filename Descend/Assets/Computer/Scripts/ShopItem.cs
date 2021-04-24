using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public struct ShopItem
{
    public ShopItem(string name, string description, string imageName, int priceInDollars)
    {
        this.name = name;
        this.description = description;
        this.imageName = imageName;
        this.priceInDollars = priceInDollars;
    }
    public string name;
    public string description;
    public string imageName;
    public int priceInDollars;

    public static void RenderToListOfShopItems(ShopItem shopItem, GameObject uiObject)
    {
        uiObject.transform.Find("Name").GetComponent<Text>().text = shopItem.name;
        uiObject.transform.Find("Description").GetComponent<Text>().text = shopItem.description;
        // TODO: sprite manager to get image. (maybe in the constructor)
        // uiObject.transform.Find("Image").GetComponent<Image>();
        uiObject.transform.Find("Price").GetComponent<Text>().text = "$" + shopItem.priceInDollars;
    }
}
