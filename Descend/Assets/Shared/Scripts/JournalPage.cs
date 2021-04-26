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
        d.name = "Journal Entry #1";
        d.body = "digging every day";
        return d;
    }

}
