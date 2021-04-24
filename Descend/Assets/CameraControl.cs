using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject player;

    public float horizontalBoundsPercent = 0.2f;
    public float verticalBoundsPercent = 0.1f;

    private Camera mCamera;

    private float mFrustumHeight;
    private float mFrustumWidth;

    private void Awake()
    {
        mCamera = GetComponent<Camera>();
        float distance = -transform.position.z;
        mFrustumHeight = 2.0f * distance * Mathf.Tan(mCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        mFrustumWidth = mFrustumHeight * mCamera.aspect;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerRelativeToCamera = player.transform.position - transform.position;
        float adjustX = 0, adjustY = 0;
        float offLeftSide = playerRelativeToCamera.x + mFrustumWidth * horizontalBoundsPercent;
        if(offLeftSide < 0)
        {
            adjustX = offLeftSide;
        }
        else
        {
            float offRightSide = playerRelativeToCamera.x - mFrustumWidth * horizontalBoundsPercent;
            if(offRightSide > 0)
            {
                adjustX = offRightSide;
            }
        }

        float offTop = playerRelativeToCamera.y - mFrustumHeight * verticalBoundsPercent;
        if(offTop > 0)
        {
            adjustY = offTop;
        }
        else
        {
            float offBottom = playerRelativeToCamera.y + mFrustumHeight * verticalBoundsPercent;
            if(offBottom < 0)
            {
                adjustY = offBottom;
            }
        }

        if(adjustX != 0 || adjustY != 0)
        {
            transform.position = new Vector3(transform.position.x + adjustX, transform.position.y + adjustY, transform.position.z);
        }
    }
}
