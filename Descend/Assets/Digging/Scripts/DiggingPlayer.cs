using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DiggingPlayer : MonoBehaviour
{
    public TileManager tileManager;
    public Tilemap dirtTilemap;
    public Tilemap fogTilemap;

    public float horizontalSpeed = 4f;
    public float verticalSpeed = 4f;
    private static float JUMP_SPEED = 5f;

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
        // Ignore input and stuff while not in the digging scene
        if (!GameLoopController.isDiggingScene())
        {
            return;
        }

        HandleMovement();

        // reveal nearby tiles
        Vector3Int bottomLeft = fogTilemap.WorldToCell(transform.position);
        bottomLeft.x -= 2;
        bottomLeft.y -= 2;
        BoundsInt revealArea = new BoundsInt(bottomLeft, new Vector3Int(5, 7, 1));
        TileBase[] emptyTiles = new TileBase[35];
        fogTilemap.SetTilesBlock(revealArea, emptyTiles);
    }

    void HandleMovement()
    {
        Vector2 momentum = mRigidbody.velocity;

        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        float horzSpeed = horz != 0 ? horizontalSpeed * Mathf.Sign(horz) : 0f;
        float vertSpeed = momentum.y;

        // JUMP
        if (Input.GetButton("Jump"))
        {
            vertSpeed = JUMP_SPEED;
        }

        mRigidbody.velocity = new Vector2(horzSpeed, vertSpeed);

        // TODO: check in the direction of motion, not just down
        Vector2 pos = transform.position;

        Bounds bounds = mBoxCollider.bounds;
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