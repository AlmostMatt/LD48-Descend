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
    void Update()
    {
        // Ignore input and stuff while not in the digging scene
        if (!GameLoopController.isDiggingScene())
        {
            return;
        }

        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        float horzSpeed = horz != 0 ? horizontalSpeed * Mathf.Sign(horz) : 0f;
        
        float vertSpeed = vert != 0 ? verticalSpeed * Mathf.Sign(vert) : 0f;

        if(transform.position.y >= GROUND_HEIGHT)
        {
            // can't move up when above ground
            vertSpeed = Mathf.Min(0, vertSpeed);

            if(transform.position.y > GROUND_HEIGHT)
                transform.position = new Vector3(transform.position.x, GROUND_HEIGHT, 0);
        }

        mRigidbody.velocity = new Vector2(horzSpeed, vertSpeed);

        // TODO: check in the direction of motion, not just down
        Vector2 pos = transform.position;

        Bounds bounds = mBoxCollider.bounds;
        if(vertSpeed != 0)
        {
            Vector2 checkPos = transform.position;
            checkPos.y = vertSpeed > 0 ? bounds.max.y + 0.1f : bounds.min.y - 0.1f;

            checkPos.x = bounds.center.x;
            Vector3Int centerBlockingTile = dirtTilemap.WorldToCell(checkPos);
            DigTile(centerBlockingTile);

            checkPos.x = bounds.min.x;
            Vector3Int minBlockingTile = dirtTilemap.WorldToCell(checkPos);
            if(minBlockingTile != centerBlockingTile)
            {
                DigTile(minBlockingTile);
            }

            checkPos.x = bounds.max.x;
            Vector3Int maxBlockingTile = dirtTilemap.WorldToCell(checkPos);
            if(maxBlockingTile != centerBlockingTile && maxBlockingTile != minBlockingTile)
            {
                DigTile(maxBlockingTile);
            }
        }

        if(horzSpeed != 0)
        {
            Vector2 checkPos = transform.position;
            checkPos.x = horzSpeed > 0 ? bounds.max.x + 0.1f : bounds.min.x - 0.1f;

            checkPos.y = bounds.min.y;
            Vector3Int minBlockingTile = dirtTilemap.WorldToCell(checkPos);
            DigTile(minBlockingTile);

            checkPos.y = bounds.center.y;
            Vector3Int centerBlockingTile = dirtTilemap.WorldToCell(checkPos);
            if(centerBlockingTile != minBlockingTile)
            {
                DigTile(centerBlockingTile);
            }

            checkPos.y = bounds.max.y;
            Vector3Int maxBlockingTile = dirtTilemap.WorldToCell(checkPos);
            if(maxBlockingTile != minBlockingTile && maxBlockingTile != centerBlockingTile)
            {
                DigTile(maxBlockingTile);
            }
        }


        // reveal nearby tiles
        Vector3Int bottomLeft = fogTilemap.WorldToCell(transform.position);
        bottomLeft.x -= 2;
        bottomLeft.y -= 2;
        BoundsInt revealArea = new BoundsInt(bottomLeft, new Vector3Int(5, 7, 1));
        TileBase[] emptyTiles = new TileBase[35];
        fogTilemap.SetTilesBlock(revealArea, emptyTiles);
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