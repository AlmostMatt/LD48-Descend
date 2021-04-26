using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Document
{
    public string name;
    public string body;
    public bool read = false;

    public static void RenderToListOfDocs(Document doc, GameObject uiObject)
    {
        uiObject.transform.Find("Name").GetComponent<Text>().text = doc.name;
        uiObject.transform.Find("Body").GetComponent<Text>().text = doc.body;
        uiObject.transform.Find("Unread").gameObject.SetActive(!doc.read);
    }

    public static void RenderToDocMainView(Document doc, GameObject uiObject)
    {
        uiObject.transform.Find("Text").GetComponent<Text>().text = doc.body;
    }
}
