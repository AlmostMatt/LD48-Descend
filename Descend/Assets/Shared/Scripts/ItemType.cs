using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    IRON,
    // COPPER,
    SILVER,
    GOLD, // TODO: correct the ItemType assets specifying the relative depths of these
    // ZINC,
    //TUNGSTEN, // commented out just to keep the number of enum values <=6 to fit in UI
    CRYSTAL,
    RUBY,
    EMERALD,
    // DIAMOND,
    NUM_TYPES // leave at end
}

public static class TypeExtensions
{
    public static string GetImage(this ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.IRON:
                return "iron";
            case ItemType.SILVER:
                return "silver";
            case ItemType.GOLD:
                return "gold";
            case ItemType.RUBY:
                return "ruby";
            case ItemType.CRYSTAL:
                return "crystal";
            case ItemType.EMERALD:
                return "emerald";
            default:
                return "stone";
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
            //case ItemType.COPPER:
            //    return new Color(209/255f,117/255f,31/255f);
            //case ItemType.ZINC:
            //    return Color.cyan;
            //case ItemType.TUNGSTEN:
            //    return Color.gray;
            //case ItemType.EMERALD:
            //    return Color.green;
            //case ItemType.RUBY:
            //    return Color.red;
            //case ItemType.DIAMOND:
            //    return Color.white;
            // These ItemTypes have colored images, so they do not require a tint
            default:
                return Color.white;
        }
    }

    private static string Capitalized(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }
}
