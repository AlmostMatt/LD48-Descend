using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RelativePosition
{
    TOP,
    SIDE,
    BOTTOM
}

public enum CreateTiming
{
    START_ONLY,
    AFTER_DESTROY_ONLY,
    EITHER,
}

public class TileDecoration : MonoBehaviour
{
    // TODO: figure out if decorations should have a chance or be guaranteed etc
    // ALSO, check if top layer (grass) should be special cased
    // TODO: indicate if toppers are initial-only or if they can be left behind when other tiles are destroyed
    // check if "one of x" (for example grass + rock is ok)

    // I want to guarantee that grass spawns at start and does not spawn later

    public bool isTrim = false;
    public RelativePosition relativePos = RelativePosition.TOP;
    public CreateTiming createTiming = CreateTiming.EITHER;
    public float probability = 0.1f;

    public bool isTall = false; // needs an empty tile above
    public bool isHanging = false; // needs an empty tile below
}
