using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameLoop
{
    public static void StartDigging()
    {
        // Load the Digging scene
        SceneManager.LoadScene("Digging", LoadSceneMode.Single);
    }

    public static void StopDigging()
    {
        // Load the Computer scene
        SceneManager.LoadScene("ComputerScene", LoadSceneMode.Single);
    }
}
