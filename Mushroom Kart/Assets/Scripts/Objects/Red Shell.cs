using UnityEngine;

public class RedShell : Obstacle
{
    [SerializeField] private float baseAcceleration = 1500;
    [SerializeField] private float baseMaxSpeed = 16;
    private Node currentNode;

    public override void SetOwner(GameObject o)
    {
        owner = o;
        currentNode = owner.GetComponent<Racer>().GetNode();
    }

    // Update is called once per frame
    void Update()
    {
        Move((currentNode.next.pos - transform.position).normalized);
        if (Vector2.Distance(transform.position, currentNode.next.pos) < 1) currentNode = currentNode.next;
    }
    
    public void Move(Vector2 move)
    {

        rb.angularVelocity = 0;

        rb.AddForce(move * baseAcceleration * Time.deltaTime);
        if (rb.linearVelocity.magnitude > baseMaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * baseMaxSpeed;
        }
    }
}
