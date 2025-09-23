using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Racer : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseAcceleration = 1500;
    [SerializeField] private float baseMaxSpeed = 15;

    // Boost
    private float boost;

    // Jump
    private bool isJumping;
    private float hVelocity;
    private float height;
    private const float MAX_HEIGHT = 1;
    private const float GRAVITY = 6f;
    // Drift
    private bool isDrifting;
    private float driftAngle;
    private float driftCharge;
    private const float DRIFT_STRENGTH = 48;

    // Other
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space)) Jump(2f);

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
        float maxSpeed = baseMaxSpeed + boost;
        float acceleration = baseAcceleration;

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
            boost -= Time.deltaTime * 2;
            if (boost < 0) boost = 0;
        }

        rb.AddForce(move * acceleration * Time.deltaTime);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        LookFoward();
        if (isJumping)UpdateHeight();
    }


    public void UpdateHeight()
    {
        height += hVelocity * Time.deltaTime;
        if (height > MAX_HEIGHT) height = MAX_HEIGHT;

        hVelocity -= GRAVITY * Time.deltaTime;

        transform.localScale = (height+1) * Vector2.one;

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

    public void Boost(float b)
    {
        boost += b;

        rb.velocity = rb.velocity.normalized * (baseMaxSpeed + boost);
    }

    public void Jump(float h)
    {
        if (isJumping) return;
        isJumping = true;
        hVelocity = h;
    }
}
