using UnityEngine;
using UnityEngine.Tilemaps;

public class Obstacle : Object
{
    // Hit Info
    [SerializeField]private float hitTime = 1;
    [SerializeField] private float momentumLoss = 0.25f;
    [SerializeField] private float maxHits = 1;
    private GameObject owner;
    private float ownerProtectTime = 0.6f;
    private float timesHit;

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.tag == "Racer" && c.gameObject != owner)
        {
            timesHit++;
            HitRacer(c.gameObject);
        }
        if (c.tag == "Map")
        {
            OnTile(c.GetComponent<Tilemap>());
        }
    }

    void Update()
    {
        ownerProtectTime -= Time.deltaTime;
        if (ownerProtectTime <= 0) owner = null;

        if (isJumping) UpdateHeight();
    }

    private void HitRacer(GameObject racer)
    {
        racer.GetComponent<Racer>().Hit(hitTime, momentumLoss);
        if (timesHit >= maxHits) DestroyObstacle();
    }

    public void SetOwner(GameObject o)
    {
        owner = o;
    }

    private void HitItem()
    {
        if (timesHit >= maxHits) DestroyObstacle();
    }

    private void DestroyObstacle()
    {
        Destroy(gameObject);
    }
}
