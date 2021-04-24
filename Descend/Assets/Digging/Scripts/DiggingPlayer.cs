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
            Vector3Int blockingTile = dirtTilemap.WorldToCell(checkPos);
            DigTile(blockingTile);

            checkPos.x = bounds.min.x;
            blockingTile = dirtTilemap.WorldToCell(checkPos);
            DigTile(blockingTile);

            checkPos.x = bounds.max.x;
            blockingTile = dirtTilemap.WorldToCell(checkPos);
            DigTile(blockingTile);
        }

        if(horzSpeed != 0)
        {
            Vector2 checkPos = transform.position;
            checkPos.x = horzSpeed > 0 ? bounds.max.x + 0.1f : bounds.min.x - 0.1f;

            checkPos.y = bounds.min.y;
            Vector3Int blockingTile = dirtTilemap.WorldToCell(checkPos);
            DigTile(blockingTile);

            checkPos.y = bounds.center.y;
            blockingTile = dirtTilemap.WorldToCell(checkPos);
            DigTile(blockingTile);

            checkPos.y = bounds.max.y;
            blockingTile = dirtTilemap.WorldToCell(checkPos);
            DigTile(blockingTile);
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
            if(tileData.requiredDigSkill <= SaveData.Get().digSkill)
            {
                dirtTilemap.SetTile(position, null);
            }
        }
    }

    public void CollectItem(Collectible c)
    {
        SaveData.Get().inventory[(int)c.itemType] += 1;
    }
}