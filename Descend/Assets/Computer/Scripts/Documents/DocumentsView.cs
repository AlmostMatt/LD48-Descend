using System.Collections;
using System.Collections.Generic;
using UnityBaseCode.Rendering;
using UnityEngine;

public class DocumentsView : MonoBehaviour
{
    public Transform docContainer;
    public Transform docMainView;

    private RenderableGroup<Document> mDocumentGroup;
    private Document mSelectedDocument;

    // Start is called before the first frame update
    void Start()
    {
        mDocumentGroup = new RenderableGroup<Document>(docContainer, Document.RenderToListOfDocs);
        RefreshDocs();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshDocs();
    }

    public void RefreshDocs()
    {
        mDocumentGroup.UpdateRenderables(SaveData.Get().documents);
        if(mSelectedDocument != null)
        {
            docMainView.gameObject.SetActive(true);
            Document.RenderToDocMainView(mSelectedDocument, docMainView.gameObject);
        }
        else
        {
            docMainView.gameObject.SetActive(false);
        }
    }

    public void SelectDocument(DocumentButton button)
    {
        int itemIndex = button.transform.GetSiblingIndex();
        Document doc = SaveData.Get().documents[itemIndex];
        mSelectedDocument = doc;
        SaveData.Get().MarkDocumentRead(doc);
    }
}
