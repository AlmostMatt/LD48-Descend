﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DiggingPlayer player = collision.GetComponent<DiggingPlayer>();
        if(player != null)
        {
            player.CollectItem(this);
            Destroy(gameObject);
        }
    }
}
