using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DocumentsTab : MonoBehaviour
{
    void Update()
    {
        if(SaveData.Get().unreadDocuments > 0)
        {
            Transform notifications = transform.Find("Notifications");
            notifications.gameObject.SetActive(true);
            notifications.GetComponentInChildren<Text>().text = SaveData.Get().unreadDocuments.ToString();
        }
        else
        {
            transform.Find("Notifications").gameObject.SetActive(false);
        }
    }
}
