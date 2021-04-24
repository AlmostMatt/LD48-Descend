using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    private static SaveData sSingleton;
    public static SaveData Get()
    {
        if(sSingleton == null)
        {
            sSingleton = new SaveData();
        }
        return sSingleton;
    }

    public int digSkill = 0;

    public int[] inventory = new int[(int)ItemType.NUM_ITEMS];    
}
