using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DecorationType
{
    TOP,
    SIDE,
    BOTTOM
}

public class TileDecoration : MonoBehaviour
{
    // TODO: figure out if decorations should have a chance or be guaranteed etc
    // ALSO, check if top layer (grass) should be special cased


    public DecorationType decorationType = DecorationType.TOP;

    public void Start()
    {
        // if top or bottom, randomly reflect
        // if left vs right, reflect
    }
}
