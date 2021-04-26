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
    public int maxStamina = 5;

    private bool mInputDisabled = false;

    private float BASICALLY_ZERO = 0.0001f;

    /** state variables (for animation and also for logic) **/
    private int mFacing = 1;
    private bool mIsTryingToDig;
    private bool mIsDigging;
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
    private bool mUsingStamina;

    public List<AudioClip> digAudioClips;
    public List<AudioClip> clankAudioClips;
    private float mDigSoundTimer;
    private Dictionary<Vector3Int, float> mDigProgress = new Dictionary<Vector3Int, float>();

    public List<AudioClip> collectAudioClips;

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

        mStamina = maxStamina;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Input and animation can happen here
    private void Update()
    {
        mIsTryingToDig = false;
        mIsDigging = false;
        mUsingStamina = false;
        var emission = digEffect.emission;

        // Ignore input and stuff while not in the digging scene, or if there's a popup
        if (!GameLoopController.isDiggingScene() || DiggingUIOverlay.IsPopupVisible() || mInputDisabled)
        {
            AnimatePausedPlayer();
            emission.enabled = false;
            return;
        }
        HandleMouseInput();
        UpdateAnimationState();

        // reveal nearby tiles
        RevealFogTiles();
        
        emission.enabled = mIsDigging;
    }

    // Movement and physics should happen here
    void FixedUpdate()
    {
        // Ignore input and stuff while not in the digging scene
        if (!GameLoopController.isDiggingScene() || DiggingUIOverlay.IsPopupVisible())
        {
            mRigidbody.velocity = Vector3.zero;
            mRigidbody.gravityScale = 0f;
            return;
        }

        if(!mInputDisabled)
        {
            HandleMovement();

            mRigidbody.gravityScale = (mWallClimbingLeft || mWallClimbingRight || mGrappleState != GRAPPLE_NONE) ? 0f : 1f;
        }
    }

    void RevealFogTiles()
    {
        // Reveals all tiles in a circle
        float radius = 5.7f; // a little bigger than the players light source
        int rSteps = 24;
        int tSteps = 96;
        for (int r = 1; r <= rSteps; r++)
        {
            for (int t = 0; t < tSteps; t++)
            {
                float rad = (radius * r) / rSteps;
                float angle = (2f * Mathf.PI * t) / tSteps;
                Vector3 point = mBoxCollider.bounds.center + rad * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector3Int tilePos = fogTilemap.WorldToCell(point);
                fogTilemap.SetTile(tilePos, null);
            }
        }
    }

    void HandleMouseInput()
    {
        // mouse digging
        bool mouse0 = Input.GetMouseButton(0);
        if (mouse0)
        {
            Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseInWorld = new Vector3(mouseInWorld.x, mouseInWorld.y, 0f);
            Vector3 closestPlayerPoint = mBoxCollider.bounds.ClosestPoint(mouseInWorld);

            float MAX_DIG_RANGE = 0.7f;
            Vector3 digTrajectory = mouseInWorld - closestPlayerPoint;
            Vector3 digPos = closestPlayerPoint + (Mathf.Min(digTrajectory.magnitude, MAX_DIG_RANGE)) * digTrajectory.normalized;
            Vector3Int targetTile = dirtTilemap.WorldToCell(digPos);
            // tileManager.GetTileData(targetTile) != null;
            digEffect.transform.position = digPos;
            DigTile(targetTile);
        }

        // grappling
        bool mouse1 = Input.GetMouseButtonDown(1);
        if (mouse1)
        {
            Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 playerCenter = mBoxCollider.bounds.center;

            Vector3 grappleTrajectory = mouseInWorld - playerCenter;
            RaycastHit2D hit = Physics2D.Raycast(playerCenter, grappleTrajectory, 20f, GROUND_LAYER_MASK);
            if (hit.collider != null)
            {
                StartGrapple(hit.point, grappleTrajectory);
            }
        }

        if (mGrappleState > 0)
        {
            UpdateGrapple();
        }
    }

    void HandleMovement()
    {
        float horzInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        bool jumpInput = Input.GetButton("Jump");

        if(mGrappleState > 0)
        {
            // if(mGrappleState == GRAPPLE_HANG)
            {
                if(horzInput != 0 || vertInput != 0)
                {
                    mGrappleLineRenderer.positionCount = 0;
                    mGrappleState = GRAPPLE_NONE;
                    return;
                }
            }

            if(mGrappleState == GRAPPLE_RETRACT)
            {
                mRigidbody.velocity = (mGrappleTarget - mBoxCollider.bounds.center).normalized * grappleRetractSpeed;
            } else if (mGrappleState == GRAPPLE_HANG)
            {
                mRigidbody.velocity = Vector2.zero;
            }
            return;
        }

        Vector2 momentum = mRigidbody.velocity;

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
        Vector2 bottomOffset = Vector2.zero; // new Vector2(0f, groundDetectionDepth / 2f); ;
        Collider2D groundCollision = Physics2D.OverlapBox(
            playerFeet - bottomOffset,
            new Vector2(bounds.size.x * 0.9f, groundDetectionDepth),
            0f,
            LayerMask.GetMask("Ground"));
        mOnGround = groundCollision != null && momentum.y <= BASICALLY_ZERO;
        float wallDetectionDepth = 0.05f;
        Vector2 playerRight = new Vector2(bounds.max.x, bounds.center.y);
        Vector2 playerLeft = new Vector2(bounds.min.x, bounds.center.y);
        Vector2 sideOffset = Vector2.zero; // new Vector2(wallDetectionDepth / 2f, 0f);
        Vector2 detectionSize = new Vector2(wallDetectionDepth, bounds.size.y * 0.9f);
        Collider2D leftCollision = Physics2D.OverlapBox(
            playerLeft - sideOffset, detectionSize, 0f, LayerMask.GetMask("Ground"));
        Collider2D rightCollision = Physics2D.OverlapBox(
            playerRight + sideOffset, detectionSize, 0f, LayerMask.GetMask("Ground"));

        // WALL CLIMB
        // Start a wall climb when falling and pressed against a wall
        if (!mWallClimbingLeft && leftCollision != null && (horzInput < 0f || momentum.x < -BASICALLY_ZERO) && momentum.y <= BASICALLY_ZERO)
        {
            Debug.Log("now on left wall");
            mWallClimbingLeft = true;
        }
        if (!mWallClimbingRight && rightCollision != null && (horzInput > 0f || momentum.x > BASICALLY_ZERO) && momentum.y <= BASICALLY_ZERO)
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

        if (jumpInput && !mJumping)
        {
            // JUMP FROM GROUND
            if (mCanJumpGroundTimer > 0f)
            {
                Debug.Log("jumped from ground");
                immediateVy = JUMP_SPEED;
                mJumping = true;
            }
            // JUMP FROM WALL
            else if (mCanJumpLeftWallTimer > 0f || mCanJumpRightWallTimer > 0f)
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
            if (mJumping)
            {
                // Don't allow another jump or climb command next frame
                mCanJumpLeftWallTimer = 0f;
                mCanJumpRightWallTimer = 0f;
                mCanJumpGroundTimer = 0f;
                mWallClimbingLeft = false;
                mWallClimbingRight = false;
            }
        }

        // Check if still in "jumping" state.
        if (mJumping)
        {
            mJumping = (momentum.y >= BASICALLY_ZERO);
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

    private void SetFacingDirection(int facingDirection)
    {
        mFacing = facingDirection;
        transform.localScale = new Vector3(mFacing * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void AnimatePausedPlayer()
    {
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetFloat("Speed", 0f);
        anim.SetBool("GrappleExtend", false);
        anim.SetBool("GrappleRetract", false);
        anim.SetBool("GrappleHang", false);
        anim.SetBool("GrappleNone", true); // GrappleNone means it is ok to go to non-grapple states
        anim.SetBool("Digging", false);
        anim.SetBool("Climbing", false);
        anim.SetBool("Jumping", false);
        anim.SetBool("Falling", false);
        anim.SetFloat("ClimbSpeed", 0f);
    }

    void UpdateAnimationState()
    {
        // Animation state should depend on state variables:
        // mOnGround
        // mWallClimbingLeft
        // mWallClimbingRight
        // mRigidbody.velocity
        // mIsDigging
        // mJumping
        Animator anim = GetComponentInChildren<Animator>();
        float stopThreshold = 0.1f;
        if (mRigidbody.velocity.x > stopThreshold)
        {
            SetFacingDirection(1);
        }
        else if (mRigidbody.velocity.x < -stopThreshold)
        {
            SetFacingDirection(-1);
        }

        // TODO: adjust walking/running threshold in the animation state machine
        anim.SetFloat("Speed", Mathf.Abs(mRigidbody.velocity.x));

        // Grapple state
        anim.SetBool("GrappleExtend", mGrappleState == GRAPPLE_EXTEND);
        anim.SetBool("GrappleRetract", mGrappleState == GRAPPLE_RETRACT);
        anim.SetBool("GrappleHang", mGrappleState == GRAPPLE_HANG);
        anim.SetBool("GrappleNone", mGrappleState == GRAPPLE_NONE);

        // Digging state
        anim.SetBool("Digging", mIsTryingToDig);

        // TODO: if climbing, jumping, falling, etc can have multiple true, set the one that is most relevant to animation
        anim.SetBool("Climbing", (mWallClimbingLeft || mWallClimbingRight) && !mOnGround);
        anim.SetBool("Jumping", mJumping);
        anim.SetBool("Falling", !mOnGround && !mJumping && !mWallClimbingLeft && !mWallClimbingRight && mGrappleState == GRAPPLE_NONE);

        // This is relevant to whether climbing animation should play or be paused.
        bool movingVertically = Mathf.Abs(mRigidbody.velocity.y) > 0.1f;
        anim.SetFloat("ClimbSpeed", movingVertically ? 1f : 0f);
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
        if(mGrappleState == GRAPPLE_EXTEND)
        {
            Vector3 grappleHeadPos = mGrappleTarget;
            Vector3[] positions = new Vector3[2];
            positions[0] = transform.Find("GrappleHandPos").position;// mBoxCollider.bounds.center;
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
            positions[0] = transform.Find("GrappleHandPos").position; // mBoxCollider.bounds.center;
            positions[1] = grappleHeadPos;
            mGrappleLineRenderer.positionCount = 2;
            mGrappleLineRenderer.SetPositions(positions);

            // Optional: check from middle of side/top/bot of player instead of entire player box
            // That would make player slide a bit closer to the point
            Vector3 closestPlayerPoint = mBoxCollider.bounds.ClosestPoint(mGrappleTarget);
            if (Vector2.Distance(closestPlayerPoint, mGrappleTarget) <= 0.05f)
            {
                Debug.Log("grapple hang");
                mGrappleState = GRAPPLE_HANG;
                positions = new Vector3[2];
                positions[0] = mBoxCollider.bounds.center;
                positions[1] = grappleHeadPos;
                mGrappleLineRenderer.positionCount = 2;
                mGrappleLineRenderer.SetPositions(positions);
            }
        }
    }

    void DigTile(Vector3Int position)
    {
        mUsingStamina = true;
        
        if(mStamina <= 0)
        {
            return; // TODO: communicate somehow
        }

        mIsTryingToDig = true;
        TileData tileData = tileManager.GetTileData(position);
        if(tileData != null)
        {
            float digSkill = SaveData.Get().digSkill;
            if (tileData.requiredDigSkill >= 0 && tileData.requiredDigSkill <= digSkill)
            {
                mIsDigging = true;

                float digSpeed = (digSkill - tileData.requiredDigSkill) * 0.2f + 200f;
                float progress;
                if(mDigProgress.TryGetValue(position, out progress))
                {
                    mDigSoundTimer -= Time.deltaTime;
                    if(mDigSoundTimer <= 0f)
                    {
                        PlayDigSound();
                        mDigSoundTimer = 0.4f;
                    }

                    progress += Time.deltaTime * digSpeed;
                    if(progress < 1f)
                    {
                        mDigProgress[position] = progress;
                    }
                    else
                    {
                        tileManager.DestroyTile(position);
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
                if(tileData.requiredDigSkill >= 0)
                {
                    // mark that we should receive an email advertising the item that digs through this
                    SaveData.Get().dirtTypeAttempted = tileData.requiredDigSkill;
                }

                mDigSoundTimer -= Time.deltaTime;
                if(mDigSoundTimer <= 0f)
                {
                    PlayClankSound();
                    mDigSoundTimer = 0.4f;
                }
            }
        }
    }

    public void CollectItem(Collectible c)
    {
        SaveData saveData = SaveData.Get();
        ItemData itemData = c.itemData;
        saveData.inventory[(int)itemData.itemType] += 1;

        PlayCollectSound();

        if(!saveData.FoundItemType(itemData.itemType))
        {
            string text = itemData.description;
            if (text.Length == 0)
            {
                text = "You found " + itemData.itemType.GetName() + "!";
            }
            DiggingUIOverlay.ShowPopup(text, itemData.itemType.GetImage());
            saveData.SetFoundItemType(itemData.itemType);
        }
    }

    public void StartNewDigDay()
    {
        mStamina = maxStamina;
    }

    public float GetStaminaPct()
    {
        return mStamina / (float)maxStamina;
    }

    public bool IsUsingStamina()
    {
        return mUsingStamina;
    }

    public void DisableInput()
    {
        mInputDisabled = true;
    }

    private void PlayDigSound()
    {
        AudioClip clip = digAudioClips[Random.Range(0, digAudioClips.Count)];
        GetComponent<AudioSource>().PlayOneShot(clip);
    }

    private void PlayClankSound()
    {
        AudioClip clip = clankAudioClips[Random.Range(0, clankAudioClips.Count)];
        GetComponent<AudioSource>().PlayOneShot(clip);
    }

    private void PlayCollectSound()
    {
        AudioClip clip = collectAudioClips[Random.Range(0, collectAudioClips.Count)];
        GetComponent<AudioSource>().PlayOneShot(clip);
    }

    private void OnDrawGizmos()
    {
        if(mBoxCollider != null)
        {
            Gizmos.DrawLine(mBoxCollider.bounds.center, mGrappleTarget);
        }
    }
}