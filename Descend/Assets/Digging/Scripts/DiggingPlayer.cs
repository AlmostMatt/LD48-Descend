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
    public float climbSpeed = 4f;
    private static float JUMP_SPEED = 10f;
    private static float WALL_JUMP_HORZ_SPEED = 5f;
    private static float WALL_JUMP_VERT_SPEED = 8f;
    public float grappleExtendSpeed = 30f;
    public float grappleRetractSpeed = 10f;
    public int maxStamina = 100;

    /** state variables (for animation and also for logic) **/
    private bool mOnGround;
    private bool mJumping;
    private bool mWallClimbingLeft;
    private bool mWallClimbingRight;
    private static float COYOTE_TIME_DURATION = 0.05f;
    private float mCanJumpGroundTimer = 0f;
    private float mCanJumpLeftWallTimer = 0f;
    private float mCanJumpRightWallTimer = 0f;

    private Rigidbody2D mRigidbody;
    private BoxCollider2D mBoxCollider;

    private float GROUND_HEIGHT = 0.04f;
    private int mStamina;
    
    private float mDigTimer;
    private Dictionary<Vector3Int, float> mDigProgress = new Dictionary<Vector3Int, float>();

    private LineRenderer mGrappleLineRenderer;
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

        mGrappleLineRenderer = GetComponent<LineRenderer>();
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
            mouseInWorld = new Vector3(mouseInWorld.x, mouseInWorld.y, 0f);
            Vector3 closestPlayerPoint = mBoxCollider.bounds.ClosestPoint(mouseInWorld);

            float MAX_DIG_RANGE = 0.7f;
            Vector3 digTrajectory = mouseInWorld - closestPlayerPoint;
            Vector3 digPos = closestPlayerPoint + (Mathf.Min(digTrajectory.magnitude, MAX_DIG_RANGE)) * digTrajectory.normalized;
            Vector3Int targetTile = dirtTilemap.WorldToCell(digPos);
            isDigging = true; // tileManager.GetTileData(targetTile) != null;
            DigTile(targetTile);
            digEffect.transform.position = digPos;
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

        mRigidbody.gravityScale = (mWallClimbingLeft || mWallClimbingRight || mGrappleState != GRAPPLE_NONE) ? 0f : 1f;

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

        float horzInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        // Instantaneous things should set immediateVx
        // Things that accel or decel over time should set desiredVx
        float immediateVx = momentum.x;
        float immediateVy = momentum.y;
        float desiredVx = horzInput != 0 ? horizontalSpeed * Mathf.Sign(horzInput) : 0f;
        Bounds bounds = mBoxCollider.bounds;

        /**
         * Check on-ground and against-wall
         **/
        Vector2 playerFeet = new Vector2(bounds.center.x, bounds.min.y);
        float groundDetectionDepth = 0.1f;
        Vector2 bottomOffset = new Vector2(0f, groundDetectionDepth / 2f); ;
        Collider2D groundCollision = Physics2D.OverlapBox(
            playerFeet - bottomOffset,
            new Vector2(bounds.size.x * 0.9f, groundDetectionDepth),
            0f,
            LayerMask.GetMask("Ground"));
        mOnGround = (!mWallClimbingLeft && !mWallClimbingRight) && groundCollision != null && momentum.y <= 0f;
        float wallDetectionDepth = 0.1f;
        Vector2 playerRight = new Vector2(bounds.max.x, bounds.center.y);
        Vector2 playerLeft = new Vector2(bounds.min.x, bounds.center.y);
        Vector2 sideOffset = new Vector2(wallDetectionDepth / 2f, 0f);
        Vector2 detectionSize = new Vector2(wallDetectionDepth, bounds.size.y * 0.9f);
        Collider2D leftCollision = Physics2D.OverlapBox(
            playerLeft - sideOffset, detectionSize, 0f, LayerMask.GetMask("Ground"));
        Collider2D rightCollision = Physics2D.OverlapBox(
            playerRight + sideOffset, detectionSize, 0f, LayerMask.GetMask("Ground"));

        // WALL CLIMB
        // Start a wall climb when falling and pressed against a wall
        if (!mWallClimbingLeft && leftCollision != null && (horzInput < 0f || momentum.x < 0f) && momentum.y <= 0f)
        {
            Debug.Log("now on left wall");
            mWallClimbingLeft = true;
        }
        if (!mWallClimbingRight && rightCollision != null && (horzInput > 0f || momentum.x > 0f) && momentum.y <= 0f)
        {
            Debug.Log("now on right wall");
            mWallClimbingRight = true;
        }
        // Check if no longer touching the wall
        if (mWallClimbingLeft && leftCollision == null)
        {
            Debug.Log("no longer touching wall");
            mWallClimbingLeft = false;
        }
        if (mWallClimbingRight && rightCollision == null)
        {
            Debug.Log("no longer touching wall");
            mWallClimbingRight = false;
        }
        // Left and right arrows can let go of the wall
        if (mWallClimbingLeft && horzInput > 0f)
        {
            Debug.Log("let go of wall");
            mWallClimbingLeft = false;
        }
        if (mWallClimbingRight && horzInput < 0f)
        {
            Debug.Log("let go of wall");
            mWallClimbingRight = false;
        }
        // Up/down input to climb on the wall
        if (mWallClimbingLeft || mWallClimbingRight)
        {
            immediateVy = vertInput != 0 ? climbSpeed * Mathf.Sign(vertInput) : 0f;
        }

        mCanJumpLeftWallTimer -= Time.fixedDeltaTime;
        mCanJumpRightWallTimer -= Time.fixedDeltaTime;
        mCanJumpGroundTimer -= Time.fixedDeltaTime;
        if (mWallClimbingLeft)
        {
            mCanJumpLeftWallTimer = COYOTE_TIME_DURATION;
        }
        if (mWallClimbingRight)
        {
            mCanJumpRightWallTimer = COYOTE_TIME_DURATION;
        }
        if (mOnGround)
        {
            mCanJumpGroundTimer = COYOTE_TIME_DURATION;
        }

        if (Input.GetButton("Jump"))
        {
            // JUMP FROM WALL
            if (mCanJumpLeftWallTimer > 0f || mCanJumpRightWallTimer > 0f)
            {
                Debug.Log("jumped off of wall");
                if (mCanJumpLeftWallTimer > 0f)
                {
                    mWallClimbingLeft = false;
                    mCanJumpLeftWallTimer = 0f;
                    immediateVx = WALL_JUMP_HORZ_SPEED;
                    desiredVx = WALL_JUMP_HORZ_SPEED;
                }
                else
                {
                    mWallClimbingRight = false;
                    mCanJumpRightWallTimer = 0f;
                    immediateVx = -WALL_JUMP_HORZ_SPEED;
                    desiredVx = -WALL_JUMP_HORZ_SPEED;
                }
                immediateVy = WALL_JUMP_VERT_SPEED;
                mJumping = true;
            }
            // JUMP FROM GROUND
            else if (mCanJumpGroundTimer > 0f)
            {
                Debug.Log("jumped from ground");
                immediateVy = JUMP_SPEED;
                mJumping = true;
            }
        }

        // Check if still in "jumping" state.
        if (mJumping)
        {
            mJumping = (momentum.y >= 0f);
        }

        Vector2 desiredV = new Vector2(desiredVx, immediateVy);
        float hAccel = 1000000f;
        if (!mOnGround)
        {
            hAccel = 20f;
        }
        // Horizontal acceleration. Accel*time = deltav, so accel =deltav/time
        float deltaV = (desiredV.x - immediateVx);
        //if (Mathf.Abs(deltaV) > 3f) {
        //    Debug.Log(deltaV);
        //}
        float desiredAccel = deltaV/Time.fixedDeltaTime;
        float actualAccel = Mathf.Clamp(desiredAccel, -hAccel, hAccel);
        mRigidbody.velocity = new Vector2(immediateVx + (actualAccel * Time.fixedDeltaTime), desiredV.y);
    }

    private void StartGrapple(Vector2 targetPoint, Vector2 toTarget)
    {
        mGrappleState = GRAPPLE_EXTEND;
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
            mGrappleLineRenderer.positionCount = 0;
            mGrappleState = GRAPPLE_NONE;
        }

        if(mGrappleState == GRAPPLE_EXTEND)
        {
            Vector3 grappleHeadPos = mGrappleTarget;
            Vector3[] positions = new Vector3[2];
            positions[0] = mBoxCollider.bounds.center;
            positions[1] = grappleHeadPos;
            mGrappleLineRenderer.positionCount = 2;
            mGrappleLineRenderer.SetPositions(positions);

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
            Vector3 grappleHeadPos = mGrappleTarget;
            Vector3[] positions = new Vector3[2];
            positions[0] = mBoxCollider.bounds.center;
            positions[1] = grappleHeadPos;
            mGrappleLineRenderer.positionCount = 2;
            mGrappleLineRenderer.SetPositions(positions);

            if(Vector2.Distance(mBoxCollider.bounds.center, mGrappleTarget) <= 0.3f)
            {
                Debug.Log("grapple hang");
                mGrappleState = GRAPPLE_HANG;
            }
        }
    }

    void DigTile(Vector3Int position)
    {
        if(mStamina <= 0) return; // TODO: communicate somehow

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
                            --mStamina;
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

    public void StartNewDigDay()
    {
        mStamina = maxStamina;
    }

    private void OnDrawGizmos()
    {
        if(mBoxCollider != null)
        {
            Gizmos.DrawLine(mBoxCollider.bounds.center, mGrappleTarget);
        }
    }
}