using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchResultButton : MonoBehaviour
{
    public void OnClick()
    {
        GetComponentInParent<SearchView>().SelectResult(this);
    }
}
