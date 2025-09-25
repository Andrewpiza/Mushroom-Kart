using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Racer : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseAcceleration = 1500;
    [SerializeField] private float baseMaxSpeed = 15;

    // Coins
    private int amountOfCoins;

    // Boost
    private float boost;

    // Jump
    private bool canTrick;
    private bool isJumping;
    private float hVelocity;
    private float height;
    private const float MAX_HEIGHT = 1.2f;
    private const float GRAVITY = 6.6f;
    // Drift
    private bool isDrifting;
    private float driftAngle;
    private float driftCharge;
    private const float DRIFT_STRENGTH = 48;

    // Other
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 respawnPoint;
    private TextMeshProUGUI coinText;

    void Start()
    {
        respawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coinText = GameObject.Find("Coin Text").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && canTrick)
        {
            canTrick = false;
            Boost(1);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (driftAngle == 0)
            {
                if (Vector2.SignedAngle(transform.right, rb.velocity.normalized) > 0.25) driftAngle = 1;
                if (Vector2.SignedAngle(transform.right, rb.velocity.normalized) < -0.25) driftAngle = -1;
            }
            else
            {
                isDrifting = true;
            }
        }
        else if (isDrifting == true)
        {
            isDrifting = false;
            driftAngle = 0;
            Boost(driftCharge);
            driftCharge = 0;
        }

        Move(new Vector2(xMove, yMove));
    }

    public void Move(Vector2 move)
    {
        float maxSpeed = baseMaxSpeed + boost + (amountOfCoins/10);
        float acceleration = baseAcceleration + (amountOfCoins*10);

        if (isDrifting)
        {
            acceleration *= 1.4f;

            Vector2 driftMove = Quaternion.AngleAxis(DRIFT_STRENGTH * driftAngle, Vector3.forward) * transform.right;
            move += driftMove;

            driftCharge += Time.deltaTime / 4;
            if (move != Vector2.zero && Vector2.Angle(move, driftMove) < 25) driftCharge += Time.deltaTime / 2;
        }

        if (boost > 0)
        {
            boost -= Time.deltaTime * (boost/1.5f);
            if (boost < 0) boost = 0;
        }

        rb.AddForce(move * acceleration * Time.deltaTime);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        LookFoward();
        if (isJumping) UpdateHeight();
        transform.localScale = (height + 1) * Vector2.one;
    }


    public void UpdateHeight()
    {
        height += hVelocity * Time.deltaTime;
        if (height > MAX_HEIGHT) height = MAX_HEIGHT;

        hVelocity -= GRAVITY * Time.deltaTime;

        if (hVelocity <= MAX_HEIGHT / 2) canTrick = false;

        if (height <= 0)
        {
            height = 0;
            transform.localScale = Vector2.one;
            hVelocity = 0;
            isJumping = false;
        }
    }

    private void LookFoward()
    {
        Vector2 v = rb.velocity;
        if (v.magnitude < 0.1) return;

        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void ChangeCoins(int n)
    {
        amountOfCoins += n;
        if (amountOfCoins > 15) amountOfCoins = 15;
        else if (amountOfCoins < 0) amountOfCoins = 0;

        coinText.text = amountOfCoins + "";
    }

    public void Boost(float b)
    {
        boost += b;

        rb.velocity = rb.velocity.normalized * (baseMaxSpeed + boost);
    }

    public void Jump(float h, bool trickable)
    {
        if (isJumping) return;
        canTrick = trickable;
        isJumping = true;
        hVelocity = h;
    }

    public void FallOff()
    { 
        height -= GRAVITY * Time.deltaTime;

        if (height <= -1)
        {
            height = 0;
            transform.position = respawnPoint;
        }
        else if (height <= -0.5)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            boost = 0;
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
        bool nearby = false;

        if (!tile)
        {
            pos = TileManager.Instance.FindNearbyTile(pos);
            tile = tilemap.GetTile(pos);
            nearby = true;
            if (!tile) return;
        }

        if (TileManager.Instance.IsCoinTile(tile.name))
        {
            ChangeCoins(1);
            StartCoroutine(TileManager.Instance.RespawnCoin(pos, tile));
        }

        if (nearby) return;
        
        if (TileManager.Instance.IsOffMapTile(tile.name) && !isJumping) FallOff();
        else if (TileManager.Instance.IsJumpTile(tile.name)) Jump(2.6f, true);
        else if (TileManager.Instance.IsSpeedBoostTile(tile.name)) Boost(2);
        else if (TileManager.Instance.IsItemBoxTile(tile.name))
        {
            // Give Item
            StartCoroutine(TileManager.Instance.RespawnItemBox(pos, tile));
        }
    }
}
