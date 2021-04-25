using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityBaseCode.Rendering;

public struct InventoryEntry
{
    public InventoryEntry(ItemType itemType, int itemCount)
    {
        this.itemType = itemType;
        this.itemCount = itemCount;
        this.totalValue = itemCount * itemType.GetValue();
    }
    public ItemType itemType;
    public int itemCount;
    public int totalValue;

    public static void RenderToGemSaleList(InventoryEntry entry, GameObject uiObject)
    {
        // TODO: don't add rows for undiscovered item types
        // uiObject.SetActive(GameData.singleton.potionsUnlocked[(int)report.PotionType] || report.numSold > 0);

        uiObject.transform.Find("H/Name").GetComponent<Text>().text = string.Format("{0}\nPotion", entry.itemType.GetName());
        uiObject.transform.Find("H/Icon").GetComponent<Image>().sprite = SpriteDictionary.GetSprite(entry.itemType.GetImage());
        uiObject.transform.Find("H/Icon").GetComponent<Image>().color = entry.itemType.GetColor();

        uiObject.transform.Find("H/Sales").GetComponent<Text>().text = string.Format("{0}x", entry.itemCount);
        uiObject.transform.Find("H/Profit").GetComponent<Text>().text = string.Format("${0}", entry.totalValue);
        // TODO: Exclude reports that have 0 sales (so that they are not even rendered)
        uiObject.transform.Find("H/Profit").GetComponent<CanvasGroup>().alpha = (entry.itemCount == 0 ? 0f : 1f);
    }
}
