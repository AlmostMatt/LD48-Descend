using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    IRON,
    COPPER,
    ZINC,
    TUNGSTEN,
    EMERALD,
    RUBY,
    DIAMOND,
    NUM_TYPES // leave at end
}

public static class TypeExtensions
{
    public static string GetImage(this ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.RUBY:
                return "ruby";
            default:
                return "iron";
        }
    }

    public static string GetName(this ItemType itemType)
    {
        switch (itemType)
        {
            default: return Capitalized(itemType.ToString());
        }
    }

    public static int GetValue(this ItemType itemType)
    {
        switch (itemType)
        {
            default: return 100 * (1 + (int)itemType);
        }
    }

    public static Color GetColor(this ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.IRON:
                return Color.white;
            case ItemType.COPPER:
                return new Color(209/255f,117/255f,31/255f);
            case ItemType.ZINC:
                return Color.cyan;
            case ItemType.TUNGSTEN:
                return Color.gray;
            case ItemType.EMERALD:
                return Color.green;
            case ItemType.RUBY:
                return Color.red;
            case ItemType.DIAMOND:
                return Color.white;
            default: return Color.white;
        }
    }

    private static string Capitalized(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }
}
