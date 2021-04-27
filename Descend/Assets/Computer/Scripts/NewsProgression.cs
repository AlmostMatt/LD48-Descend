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
        d.body = "There have been recent reports regarding unethical company loan programs.\r\n\r\n" +
            "Multiple prominent companies in the mining industry have been issuing private loans to their employees, requiring the employee to make small daily payments. If the payment is not met, the employee’s loan will default and must work at minimum wage to slowly repay their debt.\r\n\r\n";
        return d;
    }

    // triggered by discovering the dead body
    public static Document EarthquakeNews()
    {
        Document d = new Document();
        d.name = "[Archived News] Earthquake near " + GameConfig.playerAddress;
        d.body = "There was a magnitude 3 earthquake yesterday at 6 p.m. near " + GameConfig.playerAddress + ".\r\n\r\n"
            + "We advise everyone mining in the region to proceed with caution.\r\n"
            + "\r\n"
            + "(It's dated June 24)\r\n";
        return d;
    }

    // triggered by touching an artifacts
    public static Document ParallelResearchNews()
    {

        Document d = new Document();
        d.name = "[News] Experimental Research Projects Paused";
        d.body = string.Format("{0}'s experimental research projects have been paused.\r\n" +
         "Their laboratory on {1} was rebuilt into a mining facility earlier this year, and the land has been restored.\r\n" +
         "It is currently being occupied by {0}’s employees to conduct daily mining procedures.\r\n",
            GameConfig.gemBuyerName, GameConfig.playerAddress);
        return d;
    }

    // triggered by journal 3
    public static Document AddSpaceTearNews()
    {
        Document d = new Document();
        d.name = "[News] Dangerous Research Project Into Space Channels";
        d.body = string.Format("It has been revealed by an insider today that {0}’s cancelled research project was on the creation of channels through space. \r\n\r\n"
            + "The company has allegedly discovered a method to transfer items between parallel worlds. "
            + "However, this method is widely regarded as dangerous and unreliable by scientists. "
            + "Potential side effects of such trials include permanent space tears, de-stabilized landmass and "
            + " random teleportation around trail regions.", GameConfig.gemBuyerName);
        return d;
    }

    // triggered by journal 4
    public static Document AddEquilibriumNews()
    {
        Document d = new Document();
        d.name = "[News] Law of Equilibrium on Space Channels";
        d.body = "The space scientists have established the Law of Equilibrium on Space Channels:\r\n"
            + "If something is teleported through a space channel, something of similar properties "
            + "will be teleported back, eventually. The timing of the equilibrium transfer has proven to be"
            + " unpredictable and can occur many days later.";
        return d;
    }
}
