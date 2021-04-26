using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentButton : MonoBehaviour
{
    public void OnClick()
    {
        GetComponentInParent<DocumentsView>().SelectDocument(this);
    }
}
