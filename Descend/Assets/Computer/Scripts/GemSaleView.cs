using System.Collections;
using System.Collections.Generic;
using UnityBaseCode.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class GemSaleView : MonoBehaviour
{
    public Transform gemSaleRowContainer;
    public Text totalPriceText;
    public Button sellButton;
    public Text currentBalanceText;

    private RenderableGroup<InventoryEntry> mSaleRowGroup;
    private List<InventoryEntry> invList = new List<InventoryEntry>();

    public void Start()
    {
        mSaleRowGroup = new RenderableGroup<InventoryEntry>(gemSaleRowContainer, InventoryEntry.RenderToGemSaleList);
    }

    public void Update()
    {
        invList.Clear();
        for (int i=0; i<(int)ItemType.NUM_TYPES; i++)
        {
            invList.Add(new InventoryEntry((ItemType) i, SaveData.Get().inventory[i]));
        }
        mSaleRowGroup.UpdateRenderables(invList);
        int totalPrice = GetTotalInventoryValue();
        totalPriceText.text = "$" + totalPrice.ToString();
        sellButton.interactable = (totalPrice > 0);
        // current balance
        currentBalanceText.text = "Balance: $" + SaveData.Get().GetCash();
    }

    public void SellGems()
    {
        // increase balance
        SaveData.Get().IncreaseCash(GetTotalInventoryValue());
        // clear out inventory
        for (int i = 0; i < (int)ItemType.NUM_TYPES; i++)
        {
            SaveData.Get().inventory[i] = 0;
        }
    }

    private int GetTotalInventoryValue()
    {
        int total = 0;
        for (int i = 0; i < (int)ItemType.NUM_TYPES; i++)
        {
            total += SaveData.Get().inventory[i] * ((ItemType)i).GetValue();
        }
        return total;
    }
}
