using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsProgression
{
    // could do this dynamically but the conditions vary
    private static List<Document> queuedNews = new List<Document>();

    public static void UpdateNews()
    {
        foreach (Document doc in queuedNews)
        {
            SaveData.Get().AddDocument(doc);
        }
        queuedNews.Clear();
    }

    // the added news will show up the next time the player stops digging
    public static void AddNewsForNextDay(Document doc)
    {
        queuedNews.Add(doc);
    }

    // triggered by buying a digskill=1 pickaxe
    public static Document DebtNews()
    {
        Document d = new Document();
        d.name = "[News] Unethical Loan Programs - Beware!";
        d.body = "There have been recent reports regarding unethical company loan programs.\r\n" +
            "Multiple prominent companies in the mining industry have been issuing private loans to their employees, requiring the employee to make small daily payments. If the payment is not met, the employee’s loan will default and must work at minimum wage to slowly repay their debt.\r\n\r\n";
        return d;
    }

    // triggered by discovering the dead body
    public static Document EarthquakeNews()
    {
        Document d = new Document();
        d.name = "[Archived News] Earthquake near " + GameConfig.playerAddress;
        d.body = "There was a magnitude 3 earthquake yesterday at 6 p.m. near " + GameConfig.playerAddress + ".\r\n"
            + "We advise everyone mining in the region to proceed with caution.\r\n"
            + "\r\n"
            + "(It's dated June 24)\r\n";
        return d;
    }
}
