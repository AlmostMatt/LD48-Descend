using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemButton : MonoBehaviour
{
    public void OnClick()
    {
        GetComponentInParent<ShopView>().PurchaseItem(this);
    }
}
