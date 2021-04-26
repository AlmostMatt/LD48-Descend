using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTab : MonoBehaviour
{
    GameObject mNotifications;

    // Start is called before the first frame update
    void Start()
    {
        mNotifications = transform.Find("Notifications").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        SaveData saveData = SaveData.Get();
        mNotifications.SetActive(saveData.activeShopItem+1 > saveData.digSkill);
    }
}
