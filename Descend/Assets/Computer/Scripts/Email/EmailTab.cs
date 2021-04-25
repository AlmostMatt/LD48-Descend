using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailTab : MonoBehaviour
{    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(SaveData.Get().unreadEmails > 0)
        {
            Transform notifications = transform.Find("Notifications");
            notifications.gameObject.SetActive(true);
            notifications.GetComponentInChildren<Text>().text = SaveData.Get().unreadEmails.ToString();
        }
        else
        {
            transform.Find("Notifications").gameObject.SetActive(false);
        }
    }
}
