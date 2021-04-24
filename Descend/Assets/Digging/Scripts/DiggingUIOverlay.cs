using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggingUIOverlay : MonoBehaviour
{
    public void StopDigging()
    {
        GameLoopController.StopDigging();
    }
}
