using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiggingUIOverlay : MonoBehaviour
{
    private static DiggingUIOverlay sSingleton;

    private bool mPopupVisible = false;

    private void Start()
    {
        sSingleton = this;
    }

    public void StopDigging()
    {
        GameLoopController.StopDigging();
    }

    public static void ShowPopup(string text, string image)
    {
        sSingleton._ShowPopup(text, image);
    }

    public void ClosePopup()
    {
        Transform popup = transform.Find("Popup");
        popup.gameObject.SetActive(false);
        mPopupVisible = false;
    }

    private void _ShowPopup(string text, string image)
    {
        mPopupVisible = true;

        Transform popup = transform.Find("Popup");
        popup.gameObject.SetActive(true);

        popup.Find("Text").GetComponent<Text>().text = text;
        Transform img = popup.Find("Image");
        if(image != null && image.Length != 0)
        {
            img.gameObject.SetActive(true);
            img.GetComponent<Image>().sprite = UnityBaseCode.Rendering.SpriteDictionary.GetSprite(image);
        }
        else
        {
            img.gameObject.SetActive(false);
        }
    }

    public static bool IsPopupVisible()
    {
        return sSingleton.mPopupVisible;
    }

}
