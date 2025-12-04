using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Racer : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float baseAcceleration = 1500;
    [SerializeField] private float baseMaxSpeed = 16;

    // Hit
    protected float hitTimer = 0;

    // Coins
    protected int amountOfCoins;

    // Boost
    private float boost;
    private const float MAX_BOOST = 10f;

    // Jump
    protected bool canTrick;
    protected bool isJumping;
    private float hVelocity;
    protected float height;
    private const float MAX_HEIGHT = 0.6f;
    private const float GRAVITY = 6.75f;

    // Drift
    protected bool isDrifting;
    protected float driftAngle;
    protected float driftCharge;
    private const float DRIFT_STRENGTH = 62;

    // Item
    [SerializeField] protected ItemType[] item;
    [SerializeField]private float itemCooldownMax = 0.2f;
    private float itemCooldown;

    // Effects
    protected List<Effects> effects;

    // Placement
    public int placement;
    protected Node currentNode;
    protected int lapsDone;
    protected float distanceInTrack;

    // Other
    protected bool isComputer; 
    protected bool isOffRoad;
    protected Rigidbody2D rb;
    protected Transform spriteTransform;
    protected Collider2D col;
    protected SpriteRenderer spriteRenderer;

    void Start()
    {
        itemCooldown = itemCooldownMax;
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteTransform = transform.GetChild(0);
        item = new ItemType[2];
    }

    protected void UpdateRacer()
    {
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer <= 0)
            {
                col.enabled = true;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            }
        }
        itemCooldown += Time.deltaTime;
        
        if (isJumping) UpdateHeight();
        spriteTransform.localScale = (height + 1) * Vector2.one;
    }

    public void Move(Vector2 move)
    {
        if (hitTimer > 0) return; 

        rb.angularVelocity = 0;
        
        float maxSpeed = baseMaxSpeed + Mathf.Clamp(boost, 0, MAX_BOOST) + (Mathf.Clamp(amountOfCoins, 0, 10) / 10);
        float acceleration = baseAcceleration + (Mathf.Clamp(amountOfCoins, 0, 10) * 10);

        if (isOffRoad && boost < 3)maxSpeed /= 2;

        if (isDrifting)
        {
            acceleration *= 1.4f;

            Vector2 driftMove = Quaternion.AngleAxis(DRIFT_STRENGTH * driftAngle, Vector3.forward) * transform.right;
            move = ((move / 1.3f) + driftMove).normalized;

            driftCharge += Time.deltaTime / 1.7f;
            if (move != Vector2.zero && Vector2.Angle(move, driftMove) < 35) driftCharge += Time.deltaTime * 1.35f;
        }

        if (boost > 0)
        {
            boost -= Time.deltaTime * (boost / 1.35f);
            if (boost < 0) boost = 0;
        }

        rb.AddForce(move * acceleration * Time.deltaTime);
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        LookFoward();
    }
    
    public void Hit(float hitTime,float momentumLoss)
    {
        SoundManager.Instance.PlaySound("Hit",0.3f);
        hitTimer = hitTime;
        rb.linearVelocity = rb.linearVelocity * momentumLoss;
        rb.angularVelocity = 150;

        ChangeCoins(-2);

        col.enabled = false;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.3f);

        boost = 0;
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
            spriteTransform.localScale = Vector2.one;
            hVelocity = 0;
            isJumping = false;
        }
    }

    private void LookFoward()
    {
        Vector2 v = rb.linearVelocity;
        if (v.magnitude < 0.1) return;

        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public virtual void ChangeCoins(int n)
    {
        if (n > 0)SoundManager.Instance.PlaySound("Coin",0.35f);
        amountOfCoins += n;
        if (amountOfCoins > 15) amountOfCoins = 15;
        else if (amountOfCoins < 0) amountOfCoins = 0;
    }

    public void Boost(float b)
    {
        SoundManager.Instance.PlaySound("Boost",0.45f);
        boost += b;
        if (boost > 13) boost = 13; 

        rb.linearVelocity = rb.linearVelocity.normalized * (baseMaxSpeed + boost);
    }

    public void Jump(float h, bool trickable)
    {
        
        if (isJumping) return;
        if (h > 4)SoundManager.Instance.PlaySound("Big Jump",0.7f);
        else
        {
            SoundManager.Instance.PlaySound("Jump",0.7f);
        }
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
            transform.position = currentNode.pos;
        }
        else if (height <= -0.5)
        {
            rb.linearVelocity = Vector2.zero;
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

        if (!tile || isJumping) isOffRoad = false;

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

        if (nearby || isJumping) return;

        if (TileManager.Instance.IsOffRoadTile(tile.name)) isOffRoad = true;
        else if (TileManager.Instance.IsOffMapTile(tile.name)) FallOff();
        else if (TileManager.Instance.IsJumpTile(tile.name)) Jump(2.25f, true);
        else if (TileManager.Instance.IsBigJumpTile(tile.name))
        {
            Jump(5f, true);
            Boost(6);
        } 
        else if (TileManager.Instance.IsSpeedBoostTile(tile.name)) Boost(6);
        else if (TileManager.Instance.IsItemBoxTile(tile.name))
        {
            ItemManager.Instance.GiveItem(this);
            StartCoroutine(TileManager.Instance.RespawnItemBox(pos, tile));
        }
    }

    public void UseItem(ItemDirection itemDirection)
    {
        if (itemCooldown >= itemCooldownMax)
        {
            itemCooldown = 0;
            ItemManager.Instance.UseItem(this, item[0],itemDirection, rb.linearVelocity.magnitude);
        }
    }

    public void SetItem(ItemType i, int index)
    {
        item[index] = i;
    }

    public ItemType[] GetItemSlots()
    {
        return item;
    }

    public virtual bool IsPlayer()
    {
        return false;
    }

    public void SetDistanceInTrack(float distance)
    {
        distanceInTrack = distance;

        if (distance > PlacementManager.instance.trackLength)
        {
            lapsDone++;
            distanceInTrack = 0;
            if (IsPlayer()){
                if (lapsDone+1 < PlacementManager.instance.maxLaps)SoundManager.Instance.PlaySound("Lap",0.4f);
                else
                {
                    SoundManager.Instance.PlaySound("Lap",0.75f,1.5f);
                    SoundManager.Instance.PlayMusic("Mario Circuit Final",0.5f);
                }    
            }
        }
    }

    public float GetDistanceInTrack()
    {
        return distanceInTrack;
    }

    public float GetTotalDistanceOfTrack()
    {
        return distanceInTrack + (PlacementManager.instance.trackLength * lapsDone);
    }

    public void SetNode(Node node)
    {
        currentNode = node;
    }

    public Node GetNode()
    {
        return currentNode;
    }
}
