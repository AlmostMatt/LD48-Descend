using System.Collections;
using System.Collections.Generic;
using UnityBaseCode.Rendering;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public ItemData itemData;

    private bool mInRock = true;

    public void Start()
    {
        GetComponentInChildren<SpriteRenderer>().color = itemData.itemType.GetColor();
        GetComponentInChildren<SpriteRenderer>().sprite = SpriteDictionary.GetSprite(itemData.itemType.GetImage());
    }

    public void Update()
    {
        // Check if no longer in a tile.
        if (mInRock)
        {
            TileData data = TileManager.singleton.GetTileDataWorldPosition(transform.position);
            if (data == null)
            {
                // enable gravity and collisions (with ground)
                GetComponent<Rigidbody2D>().WakeUp();
                mInRock = false;
                // For some reason: collectibles sometimes fall off the bottom of the screen (through the tiles)
            }
        }
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
