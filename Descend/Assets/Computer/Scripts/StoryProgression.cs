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
        e.sender = GameConfig.HRname;
        e.senderEmail = GameConfig.HRemail;
        e.subject = "[IMPORTANT] Welcome to " + GameConfig.playerAddress;

        string mainBody = "Hello {0},\r\n\r\n" +
                            "You have been relocated to Mining Site: {1}!\r\n\r\n" +
                            "We hope you are settling in and are ready to make progress in paying down your debt.\r\n" +
                            "We'll buy any precious metal and gems that you find -- simply fill out the form on the <b>{2}</b> page of our website, and the funds will be transferred automatically.\r\n\r\n" +
                            "Remember, your daily minimal payments commence immediately. <b>It is ESSENTIAL that you have enough cash to pay this every day</b>. Please see the details of your financial status in the <b>bank tab</b>.\r\n" +
                            "We look forward to continuously helping you to improve your financial situation!\r\n\r\n" +
                            "Yours Truly,\r\n" +
                            "{3}";

        e.body = string.Format(mainBody, GameConfig.playerName, GameConfig.playerAddress, GameConfig.gemBuyerTabName, GameConfig.HRname);
        return e;
    }
}
