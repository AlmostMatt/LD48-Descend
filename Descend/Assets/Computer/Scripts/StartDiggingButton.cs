using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDiggingButton : MonoBehaviour
{
    public Transform areYouSureBox;

    public void StartDigging()
    {
        // areyousurebox button has this script too. but it will be active
        if (SaveData.Get().madePaymentToday || areYouSureBox.gameObject.activeInHierarchy)
        {
            areYouSureBox.transform.gameObject.SetActive(false);
            GameLoopController.StartDigging();
            gameObject.SetActive(false);
        } else
        {
            areYouSureBox.transform.gameObject.SetActive(true);
        }
    }

    public void Nevermind()
    {
        areYouSureBox.transform.gameObject.SetActive(false);
    }
}
