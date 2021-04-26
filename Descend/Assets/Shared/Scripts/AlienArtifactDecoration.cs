using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will also be a tile decoration so it will be destroyed when tile is destroyed
public class AlienArtifactDecoration : MonoBehaviour
{
    public static bool hasShownDialog = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (!hasShownDialog)
            {
                NewsProgression.AddNewsForNextDay(NewsProgression.ParallelResearchNews());
                DiggingUIOverlay.ShowPopup("What is this? It looks so strange, like it's from another world...", "artifact");
                hasShownDialog = true;
            }
        }
    }
}
