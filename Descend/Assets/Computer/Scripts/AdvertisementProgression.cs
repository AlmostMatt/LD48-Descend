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
            saveData.activeShopItem = saveData.dirtTypeAttempted - 1;
        }
    }

    private static Email GenerateAdEmail(int dirtType)
    {
        Email e = new Email();
        e.emailType = EmailType.ADVERTISEMENT;
        e.sender = GameConfig.shopName;

        string rockType = "Stone";
        string pickaxeType = "IRON";
        switch (dirtType)
        {
            case 1: // The first time a player needs a tool upgrade
                rockType = "Stone";
                pickaxeType = "IRON";
                break;
            case 2:
                rockType = "Frozen dirt";
                pickaxeType = "STEEL";
                break;
            case 3:
                rockType = "Frozen stone";
                pickaxeType = "REINFORCED";
                break;
            case 4:
                rockType = "Molten dirt";
                pickaxeType = "UNBREAKABLE";
                break;
            case 5:
                rockType = "Molten stone";
                pickaxeType = "DIAMOND-TIPPED";
                break;
            default:
                break;
        }
        e.subject = "New " + pickaxeType + " PICKAXE - ON SALE NOW!!";
        e.body = rockType + " giving you a hard time?\r\n" +
            "Try the new " + pickaxeType + " PICKAXE, available now from our onine <b>shop</b>!\r\n\r\n" +
            "Short of cash? No worries!\r\n" +
            "Just hit the purchase button, and the remaining price will be added to your company loan.";
        e.linkText = "CLICK HERE TO ORDER NOW!";
        return e;
    }
}
