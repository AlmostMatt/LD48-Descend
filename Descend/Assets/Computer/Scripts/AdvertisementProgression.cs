using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvertisementProgression
{
    public static void CheckEmails()
    {
        SaveData saveData = SaveData.Get();
        if(saveData.dirtTypeAttempted > saveData.lastAdvertisementEmail)
        {
            Email adEmail = GenerateAdEmail(saveData.dirtTypeAttempted);
            saveData.AddEmail(adEmail);
            saveData.lastAdvertisementEmail = saveData.dirtTypeAttempted;
        }
    }

    private static Email GenerateAdEmail(int dirtType)
    {
        Email e = new Email();
        e.emailType = EmailType.ADVERTISEMENT;
        e.sender = "Prospector's World";

        switch(dirtType)
        {
            default:
                e.subject = "New Pickaxe ON SALE NOW!!";
                e.body = "Stone giving you a hard time? Try the new PICKAXE, available now for order online!";
                e.linkText = "CLICK HERE TO ORDER NOW!";
                // e.image = "pickaxe";
                break;
        }
        
        return e;
    }
}
