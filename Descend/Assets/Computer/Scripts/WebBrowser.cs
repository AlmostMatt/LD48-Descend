using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WebBrowser : MonoBehaviour
{
    public Text urlBox;
    public Transform contentViewsParent;
    public Transform tabContainer;

    private string[] tabUrls = new string[] {
        "www.gemstonemarket.com",
        "www.email.com",
        "www.search.com",
        "www.shopping.com"
    };
    private TabButton mSelectedTab = null;

    public void Start()
    {
        Debug.Log("Started WebBrowser");
    }

    public void SetTab(int tabIndex)
    {
        // Initially disable all other views
        for (int i = 0; i < contentViewsParent.childCount; i++)
        {
            contentViewsParent.GetChild(i).gameObject.SetActive(i == tabIndex);
        }
        // Then select the first tab
        tabContainer.GetChild(tabIndex).GetComponent<Toggle>().isOn = true;
        tabContainer.GetChild(tabIndex).GetComponent<Toggle>().Select();
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
