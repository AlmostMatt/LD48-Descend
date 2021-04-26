using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebtProgression
{
    public static void UpdateProgression()
    {
        SaveData saveData = SaveData.Get();
        if(!saveData.hasDebtTutorialEmail)
        {
            saveData.AddEmail(DebtTutorialEmail());
            saveData.hasDebtTutorialEmail = true;
        }

        if(GameLoopController.IsComputerScene())
        {
            if(!saveData.madePaymentToday)
            {
                saveData.missedDebtPayments += 1;
                if(saveData.missedDebtPayments == 1)
                {
                    saveData.AddEmail(DebtWarningEmail());
                }
                else
                {
                    GameLoopController.GameOver();
                }
            }
        }
    }

    private static Email DebtTutorialEmail()
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

    private static Email DebtWarningEmail()
    {
        Email e = new Email();
        e.sender = GameConfig.debtCollectorName;
        e.senderEmail = GameConfig.debtCollectorEmail;
        e.subject = "FINAL NOTICE -- PAST DUE";

        string mainBody = "Hello {0}, we did not receive a payment towards your outstanding balance.\r\n" +
                    "The {1} has been notified. If you miss another payment, YOU WILL BE ARRESTED.";
        e.body = string.Format(mainBody, GameConfig.playerName, GameConfig.financialRegulatoryBody);

        return e;
    }
}
