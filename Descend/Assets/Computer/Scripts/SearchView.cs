using System.Collections;
using System.Collections.Generic;
using UnityBaseCode.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class SearchView : MonoBehaviour
{
    public Transform searchResultContainer;

    private RenderableGroup<SearchResult> mSearchResultGroup;

    public void Start()
    {
        mSearchResultGroup = new RenderableGroup<SearchResult>(searchResultContainer, SearchResult.RenderToListOfResults);
        PerformSearch("");
    }

    public void PerformSearch(string text)
    {
        Debug.Log("Searched for: " + text);
        List<SearchResult> results = new List<SearchResult>();
        if (text != "")
        {
            results.Add(new SearchResult(text + " title", text + " Content content content content content"));
            results.Add(new SearchResult(text + " whoaaa", text + " Content content content content content content content"));
            results.Add(new SearchResult(text + " again?", text + " Content content content"));
        } else
        {
            results.Add(new SearchResult("", "No results."));
        }
        mSearchResultGroup.UpdateRenderables(results);
    }

    public void SelectResult(SearchResultButton searchResultButton)
    {
        // TODO: find the corresponding SearchResult struct
        // TODO: render the corresponding detailed info.
    }
}
