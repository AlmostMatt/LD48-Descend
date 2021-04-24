using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;

    public int requiredDigSkill = 0;

    // for proc gen
    public int introduceDepth;
    public int backgroundDepth;
    public int disappearDepthStart;
    public int disappearDepthEnd;
    public int maxClusterSize;
}
