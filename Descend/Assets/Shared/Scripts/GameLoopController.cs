using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoopController : MonoBehaviour
{
    private static bool isFirstTimeComputing = true;

    public Canvas computerView;
    public Canvas digOverlay;
    public Canvas blackoutOverlay;
    public DiggingPlayer player;

    private static GameLoopController singleton;
    public bool isComputerScene = true;

    private float mEndGameDelay = 3f;
    private float mEndGameTimer = -1f;
    private bool mWonGame = false;
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

    private void Update()
    {
        if(mEndGameTimer > 0f)
        {
            mEndGameTimer -= Time.deltaTime;
            blackoutOverlay.GetComponentInChildren<Image>().color = new Color(0, 0, 0, (mEndGameDelay - mEndGameTimer) / mEndGameDelay);
            if(mEndGameTimer <= 0f)
            {
                SaveData.Get().wonGame = mWonGame;
                SaveData.Get().gameOver = mGameOver;
                SceneManager.LoadScene("GameOver");
            }
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
            SaveData.Get().currentDay++;

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
        NewsProgression.UpdateNews();

        SaveData.Get().madePaymentToday = false; // set this AFTER email checks... just in case we send an email based on missing payments

        // Load the Computer scene
        singleton.isComputerScene = true;
        singleton.computerView.gameObject.SetActive(true);
        // Go to the "gem sale" tab
        if (isFirstTimeComputing)
        {
            // email
            singleton.computerView.GetComponentInChildren<WebBrowser>().SetTab(1);
            isFirstTimeComputing = false;
        } else
        {
            // gem selling!
            singleton.computerView.GetComponentInChildren<WebBrowser>().SetTab(0);
        }
    }

    public static void GameOver()
    {
        MusicPlayer.FadeOut();
        singleton.mEndGameTimer = singleton.mEndGameDelay;
        singleton.mGameOver = true;
    }
    
    public static void WinGame()
    {
        MusicPlayer.FadeOut();
        singleton.mEndGameTimer = singleton.mEndGameDelay;
        singleton.mWonGame = true;
    }

}
