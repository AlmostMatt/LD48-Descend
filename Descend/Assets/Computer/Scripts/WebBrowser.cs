using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WebBrowser : MonoBehaviour
{
    public Text urlBox;
    public Transform contentViewsParent;

    private string[] tabUrls = new string[] {
        "www.email.com",
        "www.search.com",
        "www.shopping.com"
    };
    private TabButton mSelectedTab = null;

    public void Start()
    {
        // Disable all but the first content-view
        for (int i=1; i< contentViewsParent.childCount; i++)
        {
            contentViewsParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SelectTab(TabButton tab)
    {
        if (tab == mSelectedTab) { return; }
        // Hide the old content view
        if (mSelectedTab != null)
        {
            int oldTabIndex = mSelectedTab.transform.GetSiblingIndex();
            contentViewsParent.GetChild(oldTabIndex).gameObject.SetActive(false);
        }
        // Show the new content view
        mSelectedTab = tab;
        Debug.Log("Selected tab: " + tab.GetComponentInChildren<Text>().text);
        int tabIndex = tab.transform.GetSiblingIndex();
        contentViewsParent.GetChild(tabIndex).gameObject.SetActive(true);
        // Update the URL text
        urlBox.text = tabUrls[tabIndex];
        // When switching to the search view, give focus to the search box
        InputField inputBox = contentViewsParent.GetChild(tabIndex).GetComponentInChildren<InputField>();
        if (inputBox != null)
        {
            inputBox.Select();
            inputBox.ActivateInputField();
            // EventSystem.current.SetSelectedGameObject(inputBox.gameObject, null);
            // inputBox.OnPointerClick(null);
        }
    }
}
