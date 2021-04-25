using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryProgression
{
    public static void UpdateProgression()
    {
        SaveData saveData = SaveData.Get();

        if(!saveData.hasTutorialEmail)
        {
            saveData.AddEmail(IntroEmail());
            saveData.hasTutorialEmail = true;
        }
    }

    private static Email IntroEmail()
    {
        Email e = new Email();
        e.sender = GameConfig.gemBuyerName;
        e.senderEmail = GameConfig.gemBuyerEmail;
        e.subject = "Sign-up confirmation";

        string mainBody = "Hello {0}, Thank you for signing up with the {1} Rewards Program! " +
                            "Your starter shovel has been sent to the address on record." +
                            "We'll buy each precious metal and gem you find -- simply fill out the form on our website, and the funds will automatically be transferred to your bank account. " +
                            "For example, {2} is currently running at ${3} apiece." +
                            "We hope you are excited to start your mining journey with us!";
                            
        e.subject = string.Format(mainBody, GameConfig.playerName, GameConfig.gemBuyerName, ItemType.IRON, ItemType.IRON.GetValue());

        return e;
    }
}
