using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameLoopController : MonoBehaviour
{
    public Canvas computerView;
    public Canvas digOverlay;
    public DiggingPlayer player;

    private static GameLoopController singleton;
    public bool isComputerScene = true;

    private bool mGameOver = false;

    public void Start()
    {
        singleton = this;
        if (isComputerScene)
        {
            isComputerScene = false; // pretend we came from digging
            StopDigging();
        } else
        {
            isComputerScene = true; // pretend we came from computer
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
        DebtProgression.UpdateProgression(); // could cause a game over

        if(!singleton.mGameOver)
        {
            // Load the Digging scene
            singleton.isComputerScene = false;
            singleton.computerView.gameObject.SetActive(false);

            singleton.player.StartNewDigDay();

            MusicPlayer.StartPlaying(SaveData.Get().musicStage);
        }
    }

    public static void StopDigging()
    {
        MusicPlayer.FadeOut();

        // check for any emails to send
        DebtProgression.UpdateProgression();
        StoryProgression.UpdateProgression();
        AdvertisementProgression.CheckEmails();

        SaveData.Get().madePaymentToday = false; // set this AFTER email checks... just in case we send an email based on missing payments

        // Load the Computer scene
        singleton.isComputerScene = true;
        singleton.computerView.gameObject.SetActive(true);
        // Go to the "gem sale" tab
        singleton.computerView.GetComponentInChildren<WebBrowser>().SetTab(0);
    }

    public static void GameOver()
    {
        // go to a gameover scene? or a title scene?
        singleton.mGameOver = true;
        Debug.Log("GAME OVER");
    }

}
