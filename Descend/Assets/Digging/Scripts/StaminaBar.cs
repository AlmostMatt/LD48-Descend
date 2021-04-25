using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBar : MonoBehaviour
{
    public DiggingPlayer player;

    Transform mBar;

    // Start is called before the first frame update
    void Start()
    {
        mBar = transform.Find("Bar");
    }

    // Update is called once per frame
    void Update()
    {
        float pct = player.GetStaminaPct();
        mBar.transform.localScale = new Vector3(pct, 1, 1);
    }
}
