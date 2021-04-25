using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public DiggingPlayer player;
    public Vector3 playerOffset;

    private float mInFadeSpeed = 4f;
    private float mOutFadeSpeed = 4f;
    private float mDisplayTime = 3f;
    private float mDisplayTimer;
    private float mPrevPct;
    private float mAlpha;
    private float mTargetAlpha;
    private float mShake;
    private float mShakeTimer;

    Transform mBar;

    // Start is called before the first frame update
    void Start()
    {
        mBar = transform.Find("Bar");

        // transform.SetParent(player.transform);
        // transform.localPosition = new Vector3(0, -0.1f, 0);
        mPrevPct = player.GetStaminaPct();
        mTargetAlpha = 0;
        SetAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        float pct = player.GetStaminaPct();
        //if(pct != mPrevPct)
        if(player.IsUsingStamina())
        {
            mBar.transform.localScale = new Vector3(pct, 1, 1);
            // mPrevPct = pct;
            mTargetAlpha = 1;
            mDisplayTimer = mDisplayTime;

            if(pct <= 0)
            {
                Image img = transform.GetChild(0).GetComponent<Image>();
                img.color = new Color(255, 0, 0);
                mShakeTimer = 0.25f;
            }
        }

        if(mAlpha != mTargetAlpha)
        {
            float speed = mAlpha < mTargetAlpha ? mInFadeSpeed : -mOutFadeSpeed;
            SetAlpha(mAlpha + Time.deltaTime * speed);
        }
        else if(mAlpha == 1)
        {
            mDisplayTimer -= Time.deltaTime;
            if(mDisplayTimer <= 0)
            {
                mTargetAlpha = 0f;
            }
        }


        Vector3 updatedPos = Camera.main.WorldToScreenPoint(player.transform.position + playerOffset);
        if(pct <= 0 && mShakeTimer > 0)
        {
            float noise = mShakeTimer * Mathf.Sin(mShake * 70) * 4;
            updatedPos.x += noise;
            mShake += Time.deltaTime;
            mShakeTimer -= Time.deltaTime;
        }
        transform.position = updatedPos;
    }

    private void SetAlpha(float a)
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            Image img = transform.GetChild(i).GetComponent<Image>();
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, a);
        }
        mAlpha = Mathf.Clamp(a, 0f, 1f);
    }
}
