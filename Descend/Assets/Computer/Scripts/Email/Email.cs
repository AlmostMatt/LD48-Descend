using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Email
{
    public string sender;
    public string senderEmail;
    public string subject;
    public string body;
    public string linkText;
    public string image;
    public bool read = false;

    public EmailType emailType;

    public static void RenderToListOfEmails(Email email, GameObject uiObject)
    {
        uiObject.transform.Find("Sender").GetComponent<Text>().text = email.sender;
        uiObject.transform.Find("Subject").GetComponent<Text>().text = email.subject;
        uiObject.transform.Find("Body").GetComponent<Text>().text = email.body;
        uiObject.transform.Find("Unread").gameObject.SetActive(!email.read);
    }

    public static void RenderToEmailMainView(Email email, GameObject uiObject)
    {
        uiObject.transform.Find("Sender").GetComponent<Text>().text = email.sender;
        uiObject.transform.Find("Subject").GetComponent<Text>().text = email.subject;
        uiObject.transform.Find("Body").GetComponent<Text>().text = email.body;

        GameObject linkButton = uiObject.transform.Find("LinkButton").gameObject;
        if(email.linkText != null)
        {
            linkButton.GetComponentInChildren<Text>().text = email.linkText;
            linkButton.SetActive(true);
        }
        else
        {
            linkButton.SetActive(false);
        }

        GameObject image = uiObject.transform.Find("Image").gameObject;
        if(email.image != null)
        {
            // image.GetComponent<Image>().sprite = SpriteManager.Get(email.image);
            image.SetActive(true);
        }
        else
        {
            image.SetActive(false);
        }
    }
}
