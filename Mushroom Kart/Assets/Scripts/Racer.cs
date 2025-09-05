using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Racer : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 1500;
    [SerializeField] private float maxSpeed = 15;

    [SerializeField] private bool isDrifting;

    private Rigidbody2D rb;
    private float driftAngle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift)) {

            isDrifting = true;
        } 
        else
        {
            isDrifting = false;
        }
        
        Move(new Vector2(xMove, yMove));
    }

    public void Move(Vector2 move)
    {
        float newAcceleration = acceleration;

        if (isDrifting)
        {
            newAcceleration *= 1.5f;
            
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
}
