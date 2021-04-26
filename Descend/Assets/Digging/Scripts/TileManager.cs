using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;
    public List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> tileToData = new Dictionary<TileBase, TileData>();
    private Dictionary<Vector3Int, List<TileDecoration>> posToDecorations = new Dictionary<Vector3Int, List<TileDecoration>>();

    private void Awake()
    {
        foreach(TileData tileData in tileDatas)
        {
            foreach(TileBase tileBase in tileData.tiles)
            {
                if (!tileToData.ContainsKey(tileBase))
                {
                    tileToData.Add(tileBase, tileData);
                }
            }
        }
    }

    // Spawns tile decorationss
    public void DecorateTilemap()
    {
        for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
        {
            for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                DecorateTile(pos);
            }
        }
    }

    public void DecorateTile(Vector3Int pos)
    {
        if (!posToDecorations.ContainsKey(pos))
        {
            posToDecorations[pos] = new List<TileDecoration>();
        }
        TileData tileData = GetTileData(pos);
        if (tileData != null)
        {
            TileData above = GetTileData(new Vector3Int(pos.x, pos.y + 1, 0));
            TileData below = GetTileData(new Vector3Int(pos.x, pos.y - 1, 0));
            TileData left = GetTileData(new Vector3Int(pos.x - 1, pos.y, 0));
            TileData right = GetTileData(new Vector3Int(pos.x + 1, pos.y, 0));
            List<TileDecoration> possibleTops = new List<TileDecoration>();
            List<TileDecoration> possibleBots = new List<TileDecoration>();
            List<TileDecoration> possibleLefts = new List<TileDecoration>();
            List<TileDecoration> possibleRights = new List<TileDecoration>();
            foreach (TileDecoration tileDec in tileData.tileDecorations)
            {
                if (tileDec.decorationType == DecorationType.TOP && above == null)
                {
                    possibleTops.Add(tileDec);
                }
                if (tileDec.decorationType == DecorationType.BOTTOM && below == null)
                {
                    possibleBots.Add(tileDec);
                }
                if (tileDec.decorationType == DecorationType.SIDE && left == null)
                {
                    possibleLefts.Add(tileDec);
                }
                if (tileDec.decorationType == DecorationType.SIDE && right == null)
                {
                    possibleRights.Add(tileDec);
                }
            }
            // Spawn a random tile dec for each possible type
            if (possibleTops.Count > 0)
            {
                TileDecoration top = GameObject.Instantiate(possibleTops[Random.Range(0, possibleTops.Count)]);
                top.transform.position = pos + new Vector3(0.5f, 0.5f, 0.5f);
                posToDecorations[pos].Add(top);
            }
        }
    }

    public TileData GetTileData(Vector3Int pos)
    {
        TileBase tile = tilemap.GetTile(pos);
        if(tile != null)
        {
            return tileToData[tile];
        }

        return null;
    }

    // Destroy all attached decorations
    public void DestroyTile(Vector3Int position)
    {
        tilemap.SetTile(position, null);
        if (posToDecorations.ContainsKey(position))
        {
            foreach (TileDecoration dec in posToDecorations[position])
            {
                GameObject.Destroy(dec.gameObject);
            }
            posToDecorations[position].Clear();
        }
    }
}
