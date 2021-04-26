using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDiggingButton : MonoBehaviour
{
    public void StartDigging()
    {
        GameLoopController.StartDigging();
        gameObject.SetActive(false);
    }
}
