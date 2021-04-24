using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<Toggle>().group = GetComponentInParent<ToggleGroup>();
    }

    // Called by Toggle's OnClick
    public void OnToggle()
    {
        if (GetComponent<Toggle>().isOn) {
            GetComponent<Toggle>().Select(); // This line is to handle an edge-case in the first frame.
            if (GetComponentInParent<WebBrowser>() != null) // This line is to handle an edge-case when the game ends
            {
                GetComponentInParent<WebBrowser>().SelectTab(this);
            }
        }
    }
}
