using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggingPlayer : MonoBehaviour
{
    public float horizontalSpeed = 4f;
    public float verticalSpeed = 4f;

    private Rigidbody2D mRigidbody;

    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        float horzSpeed = horz != 0 ? horizontalSpeed * Mathf.Sign(horz) : 0f;
        float vertSpeed = vert != 0 ? verticalSpeed * Mathf.Sign(vert) : 0f;

        mRigidbody.velocity = new Vector2(horzSpeed, vertSpeed);
    }
}
