using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager singleton;

    public Tilemap tilemap;
    public List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> tileToData = new Dictionary<TileBase, TileData>();
    private Dictionary<Vector3Int, List<TileDecoration>> posToDecorations = new Dictionary<Vector3Int, List<TileDecoration>>();

    private void Awake()
    {
        singleton = this;
        foreach (TileData tileData in tileDatas)
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

    public TileData GetTileDataWorldPosition(Vector3 worldPos)
    {
        return GetTileData(tilemap.WorldToCell(worldPos));
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
    public void DestroyTile(Vector3Int pos)
    {
        tilemap.SetTile(pos, null);
        if (posToDecorations.ContainsKey(pos))
        {
            foreach (TileDecoration dec in posToDecorations[pos])
            {
                GameObject.Destroy(dec.gameObject);
            }
            posToDecorations[pos].Clear();
        }
        // Possibly spawn new decorations
        TileData above = GetTileData(new Vector3Int(pos.x, pos.y + 1, 0));
        if (above != null)
        {
            DecorateTileSide(new Vector3Int(pos.x, pos.y + 1, 0), pos, RelativePosition.BOTTOM, isGameStart: false);
        }
        TileData below = GetTileData(new Vector3Int(pos.x, pos.y - 1, 0));
        if (below != null)
        {
            DecorateTileSide(new Vector3Int(pos.x, pos.y - 1, 0), pos, RelativePosition.TOP, isGameStart: false);
        }
        TileData left = GetTileData(new Vector3Int(pos.x - 1, pos.y, 0));
        if (left != null)
        {
            // the right side of the tile on the left of this pos
            DecorateTileSide(new Vector3Int(pos.x - 1, pos.y, 0), pos, RelativePosition.SIDE, isLeft: false, isGameStart: false);
        }
        TileData right = GetTileData(new Vector3Int(pos.x + 1, pos.y, 0));
        if (right != null)
        {
            // the right side of the tile on the left of this pos
            DecorateTileSide(new Vector3Int(pos.x + 1, pos.y, 0), pos, RelativePosition.SIDE, isLeft: true, isGameStart: false);
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
            if (above == null)
            {
                DecorateTileSide(pos, new Vector3Int(pos.x, pos.y + 1, 0), RelativePosition.TOP);
            }
            if (below == null)
            {
                DecorateTileSide(pos, new Vector3Int(pos.x, pos.y - 1, 0), RelativePosition.BOTTOM);
            }
            if (left == null)
            {
                DecorateTileSide(pos, new Vector3Int(pos.x - 1, pos.y, 0), RelativePosition.SIDE, isLeft: true);
            }
            if (right == null)
            {
                DecorateTileSide(pos, new Vector3Int(pos.x + 1, pos.y, 0), RelativePosition.SIDE, isLeft: false);
            }
        }
    }

    // Precondition: tile[pos] != null and tile[pos+offset] == null
    public void DecorateTileSide(Vector3Int pos, Vector3Int offsetPos, RelativePosition relPos, bool isLeft = false, bool isGameStart=true)
    {
        TileData tileData = GetTileData(pos);
        List<TileDecoration> possibleDecs = new List<TileDecoration>();
        List<TileDecoration> possibleTrims = new List<TileDecoration>();
        foreach (TileDecoration tileDec in tileData.tileDecorations)
        {
            bool isPossible = true;
            if (tileDec.relativePos != relPos)
            {
                isPossible = false;
            }
            if (isGameStart && tileDec.createTiming == CreateTiming.AFTER_DESTROY_ONLY)
            {
                isPossible = false;
            }
            if (!isGameStart && tileDec.createTiming == CreateTiming.START_ONLY)
            {
                isPossible = false;
            }
            if (tileDec.isTall && GetTileData(offsetPos + new Vector3Int(0, 1, 0)) != null)
            {
                isPossible = false;
            }
            if (tileDec.isHanging && GetTileData(offsetPos + new Vector3Int(0, -1, 0)) != null)
            {
                isPossible = false;
            }
            if (tileDec.hasLight && pos.y < -170)
            {
                isPossible = false;
            }
            if (isPossible && !tileDec.isTrim)
            {
                possibleDecs.Add(tileDec);
            }
            if (isPossible && tileDec.isTrim)
            {
                possibleTrims.Add(tileDec);
            }
        }
        // Spawn a random tile dec for each possible type
        if (possibleDecs.Count > 0)
        {
            float randomRoll = Random.Range(0f, 1f);
            float cumulativeProb = 0f;
            TileDecoration chosenDec = null;
            foreach (TileDecoration tileDec in possibleDecs)
            {
                cumulativeProb += tileDec.probability;
                if (cumulativeProb >= randomRoll)
                {
                    chosenDec = tileDec;
                    break;
                }
            }
            if (chosenDec != null)
            {
                // TODO: guarantee to spawn a trim
                // and then also chance to spawn other things
                TileDecoration dec = GameObject.Instantiate(chosenDec);
                dec.transform.position = pos + new Vector3(0.5f, 0.5f, 0.5f);
                posToDecorations[pos].Add(dec);

                // if top or bottom, randomly reflect
                if (relPos == RelativePosition.TOP || relPos == RelativePosition.BOTTOM)
                {
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        dec.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }
                }
                if (relPos == RelativePosition.SIDE && isLeft)
                {
                    dec.transform.localScale = new Vector3(-1f, 1f, 1f);
                }
            }
        }
        // Spawn a random trim (guaranteed, equal prob)
        if (possibleTrims.Count > 0)
        {
            TileDecoration chosenTrim = possibleTrims[Random.Range(0, possibleTrims.Count)];
            TileDecoration dec = GameObject.Instantiate(chosenTrim);
            dec.transform.position = pos + new Vector3(0.5f, 0.5f, 0.5f);
            posToDecorations[pos].Add(dec);

            // if top or bottom, randomly reflect
            if (relPos == RelativePosition.TOP || relPos == RelativePosition.BOTTOM)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    dec.transform.localScale = new Vector3(-1f, 1f, 1f);
                }
            }
            if (relPos == RelativePosition.SIDE && isLeft)
            {
                dec.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }
}
