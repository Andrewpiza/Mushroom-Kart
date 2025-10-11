using UnityEngine;
using UnityEngine.Tilemaps;

public class Object : MonoBehaviour
{
    // Jumping
    private bool isJumping;
    private float hVelocity;
    private float height;
    private const float MAX_HEIGHT = 1.2f;
    private const float GRAVITY = 6.75f;

    // Other
    private Rigidbody2D rb;
    private Transform spriteTransform;

    void Start()
    {
        spriteTransform = transform.GetChild(0);
    }

    void Update()
    {
        if (isJumping) UpdateHeight();
    }

    public void UpdateHeight()
    {
        height += hVelocity * Time.deltaTime;
        if (height > MAX_HEIGHT) height = MAX_HEIGHT;

        hVelocity -= GRAVITY * Time.deltaTime;

        if (height <= 0)
        {
            height = 0;
            spriteTransform.localScale = Vector2.one;
            hVelocity = 0;
            isJumping = false;
        }
    }

    public void Jump(float h)
    {
        if (isJumping) return;
        isJumping = true;
        hVelocity = h;
    }

    public void FallOff()
    {
        height -= GRAVITY * Time.deltaTime;

        if (height <= -1)
        {
            height = 0;
        }
        else if (height <= -0.5)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.tag == "Map")
        {
            OnTile(c.GetComponent<Tilemap>());
        }
    }

    public void OnTile(Tilemap tilemap)
    {
        Vector3Int pos = tilemap.WorldToCell(transform.position);
        TileBase tile = tilemap.GetTile(pos);

        if (!tile) return;

        if (TileManager.Instance.IsOffMapTile(tile.name) && !isJumping) FallOff();
        else if (TileManager.Instance.IsJumpTile(tile.name)) Jump(2.25f);
    }
}
