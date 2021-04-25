using UnityEngine;
using System.Collections;

public class PlayerState
{
    private static PlayerState singleton;
    public static PlayerState Get()
    {
        if (singleton == null)
        {
            singleton = new PlayerState();
        }
        return singleton;
    }

    private int mBalance = 1000;

    public int GetBalance()
    {
        return mBalance;
    }

    public void DecreaseBalance(int amount)
    {
        // TODO: money spending sound effect
        mBalance -= amount;
    }

    public void IncreaseBalance(int amount)
    {
        // TODO: money earned sound effect
        mBalance += amount;
    }
}
