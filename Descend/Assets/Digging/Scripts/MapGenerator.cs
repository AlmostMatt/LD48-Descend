using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int minX;
    public int maxX;
    public TileManager manager;
    public Tilemap tilemap;

    private List<TileData> mTileDatas;

    public void Awake()
    {
        mTileDatas = manager.tileDatas;
        GenerateMap();
    }

    void GenerateMap()
    {
        int clusterStartDepth = 1000000;
        int clusterEndDepth = 0;
        foreach(TileData tileData in mTileDatas)
        {
            // fill out any background region tiles first...
            int backgroundSize = (tileData.disappearDepthStart - tileData.backgroundDepth);
            if(backgroundSize > 0)
            {
                int width = (maxX - minX);
                TileBase[] tiles = new TileBase[width * backgroundSize];
                for(int i = 0; i < tiles.Length; ++i)
                {
                    tiles[i] = tileData.tiles[Random.Range(0, tileData.tiles.Length)];
                }
                BoundsInt area = new BoundsInt(minX, -tileData.disappearDepthStart, 0, width, backgroundSize, 1);
                tilemap.SetTilesBlock(area, tiles);
            }

            if(tileData.introduceDepth != tileData.backgroundDepth)
            {
                if(tileData.introduceDepth < clusterStartDepth)
                {
                    clusterStartDepth = tileData.introduceDepth;
                }
            }

            if(tileData.disappearDepthEnd > clusterEndDepth)
            {
                clusterEndDepth = tileData.disappearDepthEnd;
            }
        }

        for(int depth = clusterStartDepth; depth <= clusterEndDepth; ++depth)
        {
            foreach(TileData tileData in mTileDatas)
            {
                float clusterChance = 0;
                if(depth < tileData.backgroundDepth && tileData.backgroundDepth > tileData.introduceDepth)
                {
                    clusterChance = (depth - tileData.introduceDepth) / (float)(tileData.backgroundDepth - tileData.introduceDepth);
                }
                else if(depth >= tileData.disappearDepthStart)
                {
                    clusterChance = 1 - ((depth - tileData.disappearDepthStart) / (float)(tileData.disappearDepthEnd - tileData.disappearDepthStart));
                }
                
                if(clusterChance > 0 && clusterChance <= 1 && Random.Range(0f, 1f) > clusterChance)
                {
                    int x = Random.Range(minX, maxX);
                    MakeCluster(x, depth, tileData);
                    break;
                }
            }
        }
    }

    void MakeCluster(int x, int depth, TileData data)
    {
        if(depth < data.introduceDepth) return;
        if(depth > data.disappearDepthEnd) return;
        if(data.backgroundDepth <= depth && depth < data.disappearDepthStart) return;

        int maxClusterSize = data.maxClusterSize;

        // cut out the background interval and stitch the rest together
        int backgroundSize = data.disappearDepthStart - data.backgroundDepth;
        int startDepth = data.introduceDepth;
        int endDepth = data.disappearDepthEnd - backgroundSize;

        int transformedDepth = depth >= data.disappearDepthStart ? depth - backgroundSize : depth;

        float depthLerp = (transformedDepth - startDepth) / (float)(endDepth - startDepth);
        float midDepth = (startDepth + endDepth) / 2;
        float a = 4 * (1 - maxClusterSize);
        int clusterSize = Mathf.FloorToInt(a * depthLerp * (depthLerp - 1) + 1);

        Debug.Log("cluster size at " + depth + ": " + clusterSize);

        Vector3Int tilePos = new Vector3Int();
        for(int y = depth - (clusterSize / 2); y <= depth + (clusterSize / 2); ++y)
        {
            int baseSize = clusterSize - Mathf.Abs(y - depth);
            int leftSize = baseSize + Random.Range(0,2);
            int rightSize = baseSize + Random.Range(0,2);
            for(int j = x - leftSize; j < x + rightSize; ++j)
            {
                TileBase tile = data.tiles[Random.Range(0, data.tiles.Length)];
                tilePos.x = j;
                tilePos.y = -y;
                tilemap.SetTile(tilePos, tile);
            }
        }
    }
}
