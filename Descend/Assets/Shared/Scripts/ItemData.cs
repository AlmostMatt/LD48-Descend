using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public GameObject prefabObject;

    public int stage;
    public int storeValue;

    public string image;
    public string description;

    public int minDepth;
    public int peakDepth;
    public int maxDepth;
}
