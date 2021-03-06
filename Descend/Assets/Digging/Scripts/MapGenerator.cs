using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int minX;
    public int maxX;
    public TileManager manager;

    public Tilemap bkgTilemap;
    public Tilemap mainTilemap;
    public Tilemap fogTilemap;
    public TileBase[] bkgTiles;
    public TileData dirtTile;
    public TileBase[] fogTiles;

    public List<ItemData> gems;

    public List<PremadeTilemapData> premadeTilemaps;

    private List<TileData> mTileDatas;

    private int mMaxDepth;

    public void Awake()
    {
        mTileDatas = manager.tileDatas;
        GenerateMap();
        SpawnGems();
    }

    void GenerateMap()
    {
        // First, totally fill out the bkg, main, and fog tilemaps
        int minY = -249; // Assuming 30 tiles per diggabiltiy and 6 diggability
        int maxY = 0;
        FillTilemap(bkgTilemap, bkgTiles, minX/2-3, maxX/2+3, minY/2, maxY/2, false, true); // bkgTilemap tilemap has a 2x scale and the same origin
        FillTilemap(mainTilemap, dirtTile.tiles, minX, maxX, minY, maxY, false, true);
        FillTilemap(fogTilemap, fogTiles, minX*2, maxX * 2, minY * 2 + 20, maxY * 2); // fog tilemap has scale 0.5

        int clusterStartDepth = 1000000;
        int clusterEndDepth = 0;
        foreach(TileData tileData in mTileDatas)
        {
            // fill out "layers" of tile types
            int backgroundSize = (tileData.disappearDepthStart - tileData.backgroundDepth);
            if(backgroundSize > 0)
            {
                FillTilemap(mainTilemap, tileData.tiles, minX, maxX, -tileData.disappearDepthStart, -tileData.backgroundDepth, false, true);
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
                mMaxDepth = clusterEndDepth;
            }
        }

        FillTilemap(mainTilemap, mTileDatas[0].tiles, minX-3, minX, -mMaxDepth, 0);
        FillTilemap(mainTilemap, mTileDatas[0].tiles, maxX, maxX+3, -mMaxDepth, 0);

        // Spawn clusters
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
                
                if(clusterChance > 0 && clusterChance <= 1 && Random.Range(0f, 1f) <= clusterChance)
                {
                    int x = Random.Range(minX, maxX);
                    MakeCluster(x, depth, tileData);
                    break;
                }
            }
        }
        // Spawn caverns (little empty spaces)
        for (int depth = minY; depth <= maxY; ++depth)
        {
            // Cavern probability as a function of depth
            // Cavern size as a function of depth
            float cavProb = 0.5f;
            int cavSize = Random.Range(3, 20);
            int x = Random.Range(minX, maxX);
            if (Random.Range(0f, 1f) <= cavProb)
            {
                MakeCavern(x, depth, cavSize);
            }
        }

        // place premade stuff
        foreach (PremadeTilemapData data in premadeTilemaps)
        {
            Tilemap premadeTilemap = data.tilemapObject.GetComponentInChildren<Tilemap>();
            premadeTilemap.CompressBounds();
            BoundsInt bounds = premadeTilemap.cellBounds;
            TileBase[] allTiles = premadeTilemap.GetTilesBlock(bounds);

            int x = data.x;
            int y = data.y;
            Vector3Int newMin = new Vector3Int(x, y, 0);
            BoundsInt targetBounds = new BoundsInt(newMin, bounds.size);
            mainTilemap.SetTilesBlock(targetBounds, allTiles);

            Transform spawnParent = data.tilemapObject.transform.Find("SpawnObjects");
            if(spawnParent != null)
            {
                for(int i = 0; i < spawnParent.childCount; ++i)
                {
                    GameObject child = spawnParent.GetChild(i).gameObject;
                    Instantiate(child, newMin + (child.transform.position - bounds.min), child.transform.rotation);
                }
            }
        }

        // Place tile decorations
        manager.DecorateTilemap();
    }

    private int GetDebtForStage(int stage)
    {
        // TODO: base these off of the prices of the digging upgrades
        switch(stage)
        {
            case 0:
                return 10000;
            case 1:
                return ShopView2.PICKAXE_COST_1;
            case 2:
                return ShopView2.PICKAXE_COST_2;
            case 3:
                return ShopView2.PICKAXE_COST_3;
            case 4:
                return ShopView2.PICKAXE_COST_4;
            case 5:
                return ShopView2.PICKAXE_COST_5;
        }

        return 0;
    }

    void SpawnGems()
    {
        int[] itemCounts = new int[(int)ItemType.NUM_TYPES];
        foreach(ItemData gemData in gems)
        {
            int maxValue = Mathf.CeilToInt(GetDebtForStage(gemData.stage) * 0.9f);
            int value = gemData.storeValue;

            int maxToSpawn = Mathf.CeilToInt(maxValue / (float)value);
            itemCounts[(int)gemData.itemType] = maxToSpawn;
        }

        for(int depth = 1; depth <= mMaxDepth; ++depth)
        {
            foreach(ItemData gemData in gems)
            {
                if(itemCounts[(int)gemData.itemType] == 0) continue;

                if(depth >= gemData.minDepth && depth < gemData.maxDepth)
                {
                    float peakChance = 2 / (float)(gemData.maxDepth - gemData.minDepth);
                    float spawnChance = 0f;
                    if(depth < gemData.peakDepth)
                    {
                        float t = depth - gemData.minDepth;
                        spawnChance = t * peakChance / (gemData.peakDepth - gemData.minDepth);
                    }
                    else
                    {
                        float t = depth - gemData.maxDepth;
                        spawnChance = t * peakChance / (gemData.peakDepth - gemData.maxDepth);
                    }

                    if(spawnChance > 0f)
                    {
                        for(int x = minX; x < maxX; ++x)
                        {
                            Vector3Int cellPosition = new Vector3Int(x, -depth, 0);
                            TileBase tile = mainTilemap.GetTile(cellPosition);
                            if(tile != null && tile != mTileDatas[0].tiles[0])
                            {
                                if(Random.Range(0f, 1f) <= spawnChance)
                                {
                                    Vector3 worldPosition = mainTilemap.CellToWorld(cellPosition);
                                    worldPosition.x += 0.5f;
                                    worldPosition.y += 0.5f;
                                    worldPosition.z = -1f;
                                    Instantiate(gemData.prefabObject, worldPosition, Quaternion.identity); // broken now? unneeded?

                                    if(--itemCounts[(int)gemData.itemType] <= 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void MakeCavern(int x, int depth, int cavernSize)
    {
        int clusterSize = cavernSize;

        foreach (Vector3Int pos in GetPositionsForCluster(x, depth, cavernSize))
        {
            SetTile(mainTilemap, null, pos.x, pos.y, false, false);
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

        // Debug.Log("cluster size at " + depth + ": " + clusterSize);

        for(int y = depth - (clusterSize / 2); y <= depth + (clusterSize / 2); ++y)
        {
            int baseSize = clusterSize - Mathf.Abs(y - depth);
            int leftSize = baseSize + Random.Range(0,2);
            int rightSize = baseSize + Random.Range(0,2);
            for(int j = x - leftSize; j < x + rightSize; ++j)
            {
                SetTile(mainTilemap, data, j, -y, false, false);
            }
        }
    }

    // Sets individual tiles (possibly rotating or reflecting tiles)
    void SetTile(Tilemap tilemap, TileData tiledata, int x, int y, bool allowRotation = true, bool allowReflection = true)
    {
        if(x < minX || x >= maxX) return;

        Vector3Int tilePos = new Vector3Int(x, y, 0);
        TileBase tile = null;
        if (tiledata != null)
        {
            tile = tiledata.tiles[Random.Range(0, tiledata.tiles.Length)];
        }
        tilemap.SetTile(tilePos, tile);
        tilemap.SetTransformMatrix(tilePos, CreateMatrix(allowRotation, allowReflection));
    }

    // Fills a rectangular region (possibly rotating or reflecting tiles)
    void FillTilemap(Tilemap tilemap, TileBase[] possibleTiles, int minX, int maxX, int minY, int maxY, bool allowRotation = true, bool allowReflection = true)
    {
        int width = (maxX - minX);
        int height = (maxY - minY);
        TileBase[] tiles = new TileBase[width * height];
        for (int i = 0; i < tiles.Length; ++i)
        {
            tiles[i] = possibleTiles[Random.Range(0, possibleTiles.Length)];
        }
        BoundsInt area = new BoundsInt(minX, minY, 0, width, height, 1);
        tilemap.SetTilesBlock(area, tiles);
        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                int z = 0;
                tilemap.SetTransformMatrix(new Vector3Int(x, y, z), CreateMatrix(allowRotation, allowReflection));
            }
        }
    }

    private Matrix4x4 CreateMatrix(bool allowRotation, bool allowReflection)
    {

        Vector3 scale = Vector3.one;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        if (allowRotation)
        {
            rotation = Quaternion.Euler(0f, 0f, 90f * Random.Range(0, 4)); // max exclusive
        }
        if (allowReflection)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                scale = new Vector3(-1f, 1f, 1f);
            }
        }
        return Matrix4x4.TRS(Vector3.zero, rotation, scale);
    }

    private List<Vector3Int> GetPositionsForCluster(int clusterX, int clusterY, int numTiles)
    {
        List<Vector3Int> result = new List<Vector3Int>();
        HashSet<Vector3Int> posSeen = new HashSet<Vector3Int>();
        List<Vector3Int> possiblePos = new List<Vector3Int>();
        Vector3Int tile1 = new Vector3Int(clusterX, clusterY, 0);
        posSeen.Add(tile1);
        possiblePos.Add(tile1);
        while (result.Count < numTiles && possiblePos.Count > 0)
        {
            // select a random possible tile
            int i = Random.Range(0, possiblePos.Count);
            Vector3Int pos = possiblePos[i];
            // NOTE: this is not the most efficient. slightly better would be to swap to end then pop
            possiblePos.RemoveAt(i);
            result.Add(pos);
            // Add nearby tiles to possible
            foreach (Vector3Int neighbor in new Vector3Int[]{
                new Vector3Int(pos.x+1, pos.y, 0),
                new Vector3Int(pos.x-1, pos.y, 0),
                new Vector3Int(pos.x, pos.y+1, 0),
                new Vector3Int(pos.x, pos.y-1, 0)
            })
            {
                if (!posSeen.Contains(neighbor))
                {
                    posSeen.Add(neighbor);
                    possiblePos.Add(neighbor);
                }
            }
        }
        return result;
    }
}
