using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public struct SearchResult
{
    public SearchResult(string title, string content)
    {
        this.title = title;
        this.content = content;
    }
    public string title;
    public string content;

    public static void RenderToListOfResults(SearchResult result, GameObject uiObject)
    {
        uiObject.GetComponent<Button>().interactable = (result.title != "");
        uiObject.transform.Find("Title").GetComponent<Text>().text = result.title;
        uiObject.transform.Find("Content").GetComponent<Text>().text = result.content;
    }
}
