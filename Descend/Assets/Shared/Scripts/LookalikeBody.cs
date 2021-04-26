using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookalikeBody : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            SaveData saveData = SaveData.Get();
            if(!saveData.foundBody)
            {
                saveData.foundBody = true;
                DiggingUIOverlay.ShowPopup("A dead body!", "deadbody");

                // todo: email/document
            }
        }
    }
}
