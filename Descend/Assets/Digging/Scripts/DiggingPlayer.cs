using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DiggingPlayer : MonoBehaviour
{
    public TileManager tileManager;
    public Tilemap dirtTilemap;
    public Tilemap fogTilemap;

    public ParticleSystem digEffect;

    public float horizontalSpeed = 4f;
    public float verticalSpeed = 4f;
    private static float JUMP_SPEED = 10f;

    private Rigidbody2D mRigidbody;
    private BoxCollider2D mBoxCollider;

    private float GROUND_HEIGHT = 0.04f;
    private int mDigSkill;

    private float mDigTimer;
    private Dictionary<Vector3Int, float> mDigProgress = new Dictionary<Vector3Int, float>();

    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody2D>();
        mBoxCollider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isDigging = false;

        // Ignore input and stuff while not in the digging scene
        if (!GameLoopController.isDiggingScene())
        {
            return;
        }

        HandleMovement();

        // mouse digging
        bool mouse0 = Input.GetMouseButton(0);
        if(mouse0)
        {
            Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 closestPlayerPoint = mBoxCollider.bounds.ClosestPoint(mouseInWorld);

            Vector3 digTrajectory = mouseInWorld - closestPlayerPoint;
            digTrajectory.Normalize();
            Vector3Int targetTile = dirtTilemap.WorldToCell(closestPlayerPoint + digTrajectory);
            isDigging = tileManager.GetTileData(targetTile) != null;
            DigTile(targetTile);
            digEffect.transform.position = closestPlayerPoint + digTrajectory;
        }

        // reveal nearby tiles
        Vector3Int bottomLeft = fogTilemap.WorldToCell(transform.position);
        bottomLeft.x -= 2;
        bottomLeft.y -= 2;
        BoundsInt revealArea = new BoundsInt(bottomLeft, new Vector3Int(5, 7, 1));
        TileBase[] emptyTiles = new TileBase[35];
        fogTilemap.SetTilesBlock(revealArea, emptyTiles);

        var emission = digEffect.emission;
        emission.enabled = isDigging;
    }

    void HandleMovement()
    {
        Vector2 momentum = mRigidbody.velocity;

        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        float horzSpeed = horz != 0 ? horizontalSpeed * Mathf.Sign(horz) : 0f;
        float vertSpeed = momentum.y;

        // JUMP
        Bounds bounds = mBoxCollider.bounds;
        Vector2 playerFeet = new Vector2(bounds.center.x, bounds.min.y);
        float groundDetectionDepth = 0.1f; 
        Collider2D collision = Physics2D.OverlapBox(
            new Vector2(playerFeet.x, playerFeet.y - groundDetectionDepth/2f),
            new Vector2(bounds.size.x * 0.9f, groundDetectionDepth),
            0f,
            LayerMask.GetMask("Ground"));
        bool onGround = collision != null;
        if (Input.GetButton("Jump") && onGround)
        {
            vertSpeed = JUMP_SPEED;
        }

        mRigidbody.velocity = new Vector2(horzSpeed, vertSpeed);
    }

    void DigTile(Vector3Int position)
    {
        TileData tileData = tileManager.GetTileData(position);
        if(tileData != null)
        {
            float digSkill = SaveData.Get().digSkill;
            if(tileData.requiredDigSkill >= 0)
            {
                if(tileData.requiredDigSkill <= digSkill)
                {
                    float digSpeed = (digSkill - tileData.requiredDigSkill) * 0.1f + 300f;
                    float progress;
                    if(mDigProgress.TryGetValue(position, out progress))
                    {
                        progress += Time.deltaTime * digSpeed;
                        if(progress < 1f)
                        {
                            mDigProgress[position] = progress;
                        }
                        else
                        {
                            dirtTilemap.SetTile(position, null);
                            mDigProgress.Remove(position);
                        }
                    }
                    else
                    {
                        mDigProgress[position] = 0f;
                    }
                }
                else
                {
                    // mark that we should receive an email advertising the item that digs through this
                    SaveData.Get().dirtTypeAttempted = tileData.requiredDigSkill;
                }
            }
        }
    }

    public void CollectItem(Collectible c)
    {
        SaveData.Get().inventory[(int)c.itemType] += 1;
    }
}