using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalPage : MonoBehaviour
{
    public int journalNum; // maybe enum?
    public string image;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            SaveData saveData = SaveData.Get();
            // update save data flags
            // add document
            string journalDesc = "A page from a journal!"; // TODO: change depending on journal num?
            DiggingUIOverlay.ShowPopup(journalDesc, image);
            AddDocument(saveData, journalNum);
            Destroy(gameObject);
        }
    }

    private void AddDocument(SaveData saveData, int journalNum)
    {
        Document d = null;
        switch(journalNum)
        {
            case 1:
                d = JournalDocumentOne();
                break;
            case 2:
                d = JournalDocumentTwo();
                break;
        }

        if(d != null)
        {
            saveData.AddDocument(d);
        }
    }

    // fill in other content here...
    private Document JournalDocumentOne()
    {
        Document d = new Document();
        d.name = "[Journal] June 1-5";
        d.body = "June 1:\r\n" +
            "I received the welcome email from the company yesterday and it looks like I just have to make the minimum payment each day to get by.\r\n" +
            "I found iron today, making some money...\r\n" +
            "\r\n" +
            "June 2:\r\n" +
            "I bought a better pickaxe from the store today and was able to dig deeper.It seems that I am lucky today and found some silver pieces. The prospects of paying off the debt looks better!\r\n" +
            "\r\n" +
            "June 3:\r\n" +
            "More precious metals were found, and I also found this strange green-ish item.I’ve never seen this before, looks pretty out of this world to me.Too bad the company won’t buy it.\r\n" +
            "\r\n" +
            "June 4:\r\n" +
            "Going pretty deep today. The deeper I go, the better things I get!I also kept digging up these weird green items. Oh well.\r\n" +
            "\r\n" +
            "June 5:\r\n" +
            "Unfruitful digging today, I am going to go deeper tomorrow.\r\n";
        return d;
    }

    // fill in other content here...
    private Document JournalDocumentTwo()
    {
        Document d = new Document();
        d.name = "[Journal] June 9-23";
        d.body = "June 9:\r\n" +
            "I still don’t seem to remember much after the incident, my memory draws a blank...but I guess I will keep digging.\r\n" +
            "\r\n" +
            "June 10 - June 20:\r\n" +
            "I'm still recovering…\r\n" +
            "\r\n" +
            "June 21:\r\n" +
            "I finally remembered my name today...it’s PLAYERNAME, I guess that’s progress?\r\n" +
            "\r\n" +
            "June 22:\r\n" +
            "I continued my digging today, and found some precious gems!This is getting really deep though.\r\n" +
            "\r\n" +
            "June 23:\r\n" +
            "I will start bright and early today and work a long day!\r\n";

        return d;
    }

}
