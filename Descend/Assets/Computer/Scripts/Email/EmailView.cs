using System.Collections;
using System.Collections.Generic;
using UnityBaseCode.Rendering;
using UnityEngine;

public class EmailView : MonoBehaviour
{
    public Transform emailContainer;
    public Transform emailMainView;

    private RenderableGroup<Email> mEmailGroup;
    private Email mSelectedEmail;

    // Start is called before the first frame update
    void Start()
    {
        mEmailGroup = new RenderableGroup<Email>(emailContainer, Email.RenderToListOfEmails);
        RefreshEmails();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshEmails();
    }

    public void RefreshEmails()
    {
        mEmailGroup.UpdateRenderables(SaveData.Get().emails);
        if(mSelectedEmail != null)
        {
            emailMainView.gameObject.SetActive(true);
            Email.RenderToEmailMainView(mSelectedEmail, emailMainView.gameObject);
        }
        else
        {
            emailMainView.gameObject.SetActive(false);
        }
    }

    public void SelectEmail(EmailButton button)
    {
        int itemIndex = button.transform.GetSiblingIndex();
        Email email = SaveData.Get().emails[itemIndex];
        mSelectedEmail = email;
        SaveData.Get().MarkEmailRead(email);
    }

    public void FollowEmailLink()
    {

    }
}
