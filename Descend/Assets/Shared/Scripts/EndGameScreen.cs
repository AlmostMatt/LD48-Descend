using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreen : MonoBehaviour
{
    void Start()
    {
        SaveData saveData = SaveData.Get();
        bool lost = saveData.gameOver;
        bool won = saveData.wonGame;

        transform.Find("Text").GetComponent<Text>().text = lost ? "GAME OVER" : "THE END";
    }

    void Update()
    {
        
    }
}
