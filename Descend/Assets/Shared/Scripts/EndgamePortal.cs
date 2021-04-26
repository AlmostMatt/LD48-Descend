using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndgamePortal : MonoBehaviour
{
    private float mShrinkLerp = 1f;
    private float mShrinkRate = 0.3f;
    GameObject mPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mPlayer != null)
        {
            Vector2 playerToHere = (transform.position - mPlayer.transform.position);
            mPlayer.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            mPlayer.GetComponent<Rigidbody2D>().AddForce(playerToHere.normalized * 0.6f);
            mPlayer.GetComponent<Rigidbody2D>().AddTorque(0.2f);
            mShrinkLerp -= Time.deltaTime * mShrinkRate;
            mPlayer.transform.localScale = new Vector3(mShrinkLerp, mShrinkLerp, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // "suck in" the player
            mPlayer = collision.gameObject;
            mPlayer.GetComponent<DiggingPlayer>().DisableInput();
            mPlayer.GetComponent<Rigidbody2D>().gravityScale = 0;
            mPlayer.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            GameLoopController.WinGame();
        }
    }
}
