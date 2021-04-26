using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankTab : MonoBehaviour
{
    GameObject mNotifications;

    void Start()
    {
        mNotifications = transform.Find("Notifications").gameObject;
    }

    void Update()
    {
        mNotifications.SetActive(!SaveData.Get().madePaymentToday);
    }
}
