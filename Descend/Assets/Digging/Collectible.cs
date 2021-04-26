using System.Collections;
using System.Collections.Generic;
using UnityBaseCode.Rendering;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public ItemType itemType;
    public void Start()
    {
        GetComponentInChildren<SpriteRenderer>().color = itemType.GetColor();
        GetComponentInChildren<SpriteRenderer>().sprite = SpriteDictionary.GetSprite(itemType.GetImage());
    }
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
