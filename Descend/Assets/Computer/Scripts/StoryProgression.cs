using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryProgression
{
    public static void UpdateProgression()
    {
        SaveData saveData = SaveData.Get();

        if(!saveData.hasMiningTutorialEmail)
        {
            saveData.AddEmail(MiningTutorialEmail());
            saveData.hasMiningTutorialEmail = true;
        }
    }

    private static Email MiningTutorialEmail()
    {
        Email e = new Email();
        e.sender = GameConfig.gemBuyerName;
        e.senderEmail = GameConfig.gemBuyerEmail;
        e.subject = "Sign-up confirmation";

        string mainBody = "Hello {0},\r\nThank you for signing up with the {1} Rewards Program! " +
                            "Your starter shovel has been sent to the address on record.\r\n" +
                            "We'll buy each precious metal and gem you find -- simply fill out the form on our website, and the funds will automatically be transferred to your bank account. " +
                            "For example, {2} is currently running at ${3} apiece." +
                            "We hope you are excited to start your mining journey with us!";
                            
        e.body = string.Format(mainBody, GameConfig.playerName, GameConfig.gemBuyerName, ItemType.IRON, ItemType.IRON.GetValue());

        return e;
    }
}
