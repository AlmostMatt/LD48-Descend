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

    public int currentDay = 0;

    public int digSkill = 0;

    public int[] inventory = new int[(int)ItemType.NUM_TYPES];

    public bool madePaymentToday = false;
    public int debt = 10000;
    private int mMinimumPayment = 100;
    public int GetMinimumPayment()
    {
        return mMinimumPayment;
    }
    public void IncreaseMinimumPayment(int amt)
    {
        mMinimumPayment += amt;
    }

    private int mCash = 1000;
    public int GetCash()
    {
        return mCash;
    }
    public void DecreaseCash(int amount)
    {
        // TODO: money spending sound effect
        mCash -= amount;
    }
    public void IncreaseCash(int amount)
    {
        // TODO: money earned sound effect
        mCash += amount;
    }

    private bool[] mFoundItemType = new bool[(int)ItemType.NUM_TYPES];
    public bool FoundItemType(ItemType itemType)
    {
        return mFoundItemType[(int)itemType];
    }
    public void SetFoundItemType(ItemType itemType)
    {
        mFoundItemType[(int)itemType] = true;
    }

    public int activeShopItem = -1;

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

    public List<Document> documents = new List<Document>();
    public int unreadDocuments = 0;
    public void AddDocument(Document d)
    {
        documents.Add(d);
        if(!d.read)
        {
            unreadDocuments++;
        }
    }
    public void MarkDocumentRead(Document d)
    {
        if(!d.read)
        {
            d.read = true;
            --unreadDocuments;
        }
    }

    public int musicStage = 0;

    public int missedDebtPayments = 0;
}
