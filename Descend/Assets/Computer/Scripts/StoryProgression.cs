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

        if(!saveData.hasDebtTutorialEmail)
        {
            saveData.AddEmail(DebtEmail());
            saveData.hasDebtTutorialEmail = true;
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

    private static Email DebtEmail()
    {
        SaveData saveData = SaveData.Get();
        Email e = new Email();
        e.sender = GameConfig.debtCollectorName;
        e.senderEmail = GameConfig.debtCollectorEmail;
        e.subject = "Your account statement is available";

        string mainBody = "Hello {0}, your account statement can now be viewed online.\r\n\r\n" +
                            "You are currently overdrawn for ${1}. " +
                            "You must make a minimum payment of ${2} immediately.\r\n\r\n" +
                            "If you fail to do so, you will be reported to the {3} for debt evasion, and you may face jail time.";
        e.body = string.Format(mainBody, GameConfig.playerName, saveData.debt, saveData.GetMinimumPayment(), GameConfig.financialRegulatoryBody);

        return e;
    }
}
