using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Racer : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 1500;
    [SerializeField] private float baseMaxSpeed = 15;

    private float boost;

    // Drift
    private bool isDrifting;
    private float driftAngle;
    private float driftCharge;

    private Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");

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
        float newAcceleration = acceleration;

        if (isDrifting)
        {
            newAcceleration *= 1.6f;

            Vector2 driftMove = Quaternion.AngleAxis(45 * driftAngle, Vector3.forward) * transform.right;
            move += driftMove;

            driftCharge += Time.deltaTime / 4;
            if (move != Vector2.zero && Vector2.Angle(move, driftMove) < 25) driftCharge += Time.deltaTime / 2;
        }

        if (boost > 0)
        {
            boost -= Time.deltaTime * 2;
            if (boost < 0) boost = 0;
        }

        rb.AddForce(move * newAcceleration * Time.deltaTime);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        LookFoward();
    }

    private void LookFoward()
    {
        Vector2 v = rb.velocity;

        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Boost(float b)
    {
        boost += b;

        rb.velocity = rb.velocity.normalized * (baseMaxSpeed + boost);
    }
}
