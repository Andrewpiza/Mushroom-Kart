using UnityEngine;
using UnityEngine.Tilemaps;

public class Obstacle : Object
{
    // Hit Info
    [SerializeField]private float hitTime = 1;
    [SerializeField] private float momentumLoss = 0.25f;
    [SerializeField] private float maxHits = 1;
    private GameObject owner;
    private float ownerProtectTime;
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

    private void HitRacer(GameObject racer)
    {
        Debug.Log("HIT RACER");
        racer.GetComponent<Racer>().Hit(hitTime, momentumLoss);
        if (timesHit >= maxHits) DestroyObstacle();
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
