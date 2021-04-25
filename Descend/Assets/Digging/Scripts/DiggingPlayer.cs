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
    public float grappleExtendSpeed = 30f;
    public float grappleRetractSpeed = 10f;

    private Rigidbody2D mRigidbody;
    private BoxCollider2D mBoxCollider;

    private float GROUND_HEIGHT = 0.04f;
    private int mDigSkill;

    private float mDigTimer;
    private Dictionary<Vector3Int, float> mDigProgress = new Dictionary<Vector3Int, float>();

    private const int GRAPPLE_NONE = 0;
    private const int GRAPPLE_EXTEND = 1;
    private const int GRAPPLE_RETRACT = 2;
    private const int GRAPPLE_HANG = 3;
    private int mGrappleState = GRAPPLE_NONE;
    private float mGrappleTimer = -1f;
    private Vector3 mGrappleDirection;
    private Vector3 mGrappleTarget;

    private int GROUND_LAYER_MASK;

    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody2D>();
        mBoxCollider = GetComponent<BoxCollider2D>();

        GROUND_LAYER_MASK = LayerMask.GetMask("Ground");
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

        // mouse digging
        bool mouse0 = Input.GetMouseButton(0);
        if(mouse0)
        {
            Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 closestPlayerPoint = mBoxCollider.bounds.ClosestPoint(mouseInWorld);

            Vector3 digTrajectory = mouseInWorld - closestPlayerPoint;
            digTrajectory.Normalize();
            Vector3Int targetTile = dirtTilemap.WorldToCell(closestPlayerPoint + digTrajectory);
            isDigging = true; // tileManager.GetTileData(targetTile) != null;
            DigTile(targetTile);
            digEffect.transform.position = closestPlayerPoint + digTrajectory;
        }

        // grappling
        bool mouse1 = Input.GetMouseButtonDown(1);
        if(mouse1)
        {
            Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 playerCenter = mBoxCollider.bounds.center;

            Vector3 grappleTrajectory = mouseInWorld - playerCenter;
            RaycastHit2D hit = Physics2D.Raycast(playerCenter, grappleTrajectory, 20f, GROUND_LAYER_MASK);
            if(hit.collider != null)
            {
                StartGrapple(hit.point, grappleTrajectory);
            }
        }

        if(mGrappleState > 0)
        {
            UpdateGrapple();
        }        

        HandleMovement();

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
        if(mGrappleState > 0)
        {
            if(mGrappleState == GRAPPLE_RETRACT)
            {
                mRigidbody.velocity = (mGrappleTarget - mBoxCollider.bounds.center).normalized * grappleRetractSpeed;
            }
            return;
        }

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

    private void StartGrapple(Vector2 targetPoint, Vector2 toTarget)
    {
        mGrappleState = GRAPPLE_EXTEND;
        mRigidbody.gravityScale = 0;
        mGrappleDirection = toTarget.normalized;
        mGrappleTarget = targetPoint;
        Debug.Log("grapple target: " + mGrappleTarget);

        float grappleDistance = toTarget.magnitude;
        float timeToExtend = grappleDistance / grappleExtendSpeed;
        mGrappleTimer = timeToExtend;
    }

    private void UpdateGrapple()
    {
        float vert = Input.GetAxis("Vertical");
        if(vert < 0)
        {
            mGrappleState = GRAPPLE_NONE;
            mRigidbody.gravityScale = 1;
        }

        if(mGrappleState == GRAPPLE_EXTEND)
        {
            if(mGrappleTimer >= 0f)
            {
                mGrappleTimer -= Time.deltaTime;
                if(mGrappleTimer < 0f)
                {
                    mGrappleState = GRAPPLE_RETRACT;
                }
            }
        }
        else if(mGrappleState == GRAPPLE_RETRACT)
        {
            if(Vector2.Distance(mBoxCollider.bounds.center, mGrappleTarget) <= 0.3f)
            {
                Debug.Log("grapple hang");
                mGrappleState = GRAPPLE_HANG;
            }
        }
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

    private void OnDrawGizmos()
    {
        if(mBoxCollider != null)
        {
            Gizmos.DrawLine(mBoxCollider.bounds.center, mGrappleTarget);
        }
    }
}