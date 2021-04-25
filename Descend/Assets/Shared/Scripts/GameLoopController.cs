using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameLoopController : MonoBehaviour
{
    public Canvas computerView;
    public Canvas digOverlay;

    private static GameLoopController singleton;
    public bool isComputerScene = true;

    public void Start()
    {
        singleton = this;
        if (isComputerScene)
        {
            StopDigging();
        } else
        {
            StartDigging();
        }
    }

    public static bool IsComputerScene()
    {
        return singleton.isComputerScene;
    }

    public static bool isDiggingScene()
    {
        return !singleton.isComputerScene;
    }

    public static void StartDigging()
    {
        // Load the Digging scene
        singleton.isComputerScene = false;
        singleton.computerView.gameObject.SetActive(false);
    }

    public static void StopDigging()
    {
        // check for any emails to send
        AdvertisementProgression.CheckEmails();

        // Load the Computer scene
        singleton.isComputerScene = true;
        singleton.computerView.gameObject.SetActive(true);
        // Go to the "gem sale" tab
        singleton.computerView.GetComponentInChildren<WebBrowser>().SetTab(0);
    }
}
