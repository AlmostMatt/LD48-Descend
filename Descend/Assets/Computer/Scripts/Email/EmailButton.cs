using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailButton : MonoBehaviour
{
    public void OnClick()
    {
        GetComponentInParent<EmailView>().SelectEmail(this);
    }
}
