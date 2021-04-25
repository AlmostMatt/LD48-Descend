using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    private static SaveData sSingleton;
    public static SaveData Get()
    {
        if(sSingleton == null)
        {
            sSingleton = new SaveData();
        }
        return sSingleton;
    }

    public int digSkill = 0;

    public int[] inventory = new int[(int)ItemType.NUM_TYPES];

    public int debt = 1000;
    public int GetMinimumPayment()
    {
        return 100;
    }

    public List<Email> emails = new List<Email>();
    public int unreadEmails = 0;

    // progression for emails
    public bool hasMiningTutorialEmail = false;
    public bool hasDebtTutorialEmail = false;
    public int dirtTypeAttempted = 0;
    public int lastAdvertisementEmail = 0;

    public void AddEmail(Email e)
    {
        emails.Add(e);
        if(!e.read)
        {
            unreadEmails++;
        }
    }

    public void MarkEmailRead(Email e)
    {
        if(!e.read)
        {
            e.read = true;
            --unreadEmails;
        }
    }
}
