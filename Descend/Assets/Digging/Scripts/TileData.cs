using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;
    public TileDecoration[] tileDecorations;

    public int requiredDigSkill = 0;

    // for proc gen
    public int introduceDepth; // depth at which it is possible for clusters to exist
    public int backgroundDepth; // depth at which it becomes the default tile
    public int disappearDepthStart; // depth at which cluster frequency begins to fade
    public int disappearDepthEnd; // depth at which cluster frequency is 0
    public int maxClusterSize;
}
