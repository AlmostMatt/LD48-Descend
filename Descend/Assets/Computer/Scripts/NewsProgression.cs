using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsProgression
{
    // could do this dynamically but the conditions vary

    public static void AddNews(Document doc)
    {
        SaveData.Get().AddDocument(doc);
    }

    public static Document NewsOne()
    {
        Document d = new Document();
        d.name = "[News] News Entry #1";
        d.body = "digging every day";
        return d;
    }
}
