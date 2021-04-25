using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;
    public List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> tileToData = new Dictionary<TileBase, TileData>();

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

    public TileData GetTileData(Vector3Int pos)
    {
        TileBase tile = tilemap.GetTile(pos);
        if(tile != null)
        {
            return tileToData[tile];
        }

        return null;
    }
}
