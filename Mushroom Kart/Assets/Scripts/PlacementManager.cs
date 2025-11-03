using System.Collections.Generic;
using UnityEngine;
public class Node
{
    public Node(Vector3 _pos)
    {
        pos = _pos;
    }
    public Vector3 pos;
    public Node next;
    public Node prev;
    public bool first = false;
}
    
public class PlacementManager : MonoBehaviour
{
    public static PlacementManager instance;
    private Node firstNode;
    public GameObject[] points;
    public float trackLength;
    
    private List<Racer> racers = new List<Racer>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        CreateNodes();
        GameObject[] racersObject = GameObject.FindGameObjectsWithTag("Racer");

        foreach(GameObject racer in racersObject)
        {
            racers.Add(racer.GetComponent<Racer>());
            racer.GetComponent<Racer>().SetNode(firstNode);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Racer racer in racers)
        {
            GetDistance(racer);
        }
        FindPlacements();
    }

    public void GetDistance(Racer r)
    {
        Node n = r.GetNode();


        Vector3 distanceToNextNode = n.next.pos - n.pos;
        Vector3 distanceToRacer = r.transform.position - n.pos;

        float progress = Vector2.Dot(distanceToRacer, distanceToNextNode) / distanceToNextNode.sqrMagnitude;

        if (progress >= 1)
        {
            r.SetNode(n.next);
        } 
        else if (progress < 0)
        {
            r.SetNode(n.prev);
        } 

        float distance = progress * distanceToNextNode.magnitude;

        while (!n.first)
        {
            n = n.prev;
            distance += (n.next.pos - n.pos).magnitude;
        }

        if (Mathf.Abs(r.GetDistanceInTrack()-distance) < 100)r.SetDistanceInTrack(distance);
    }

    public void FindPlacements()
    {
        Racer[] placements = racers.ToArray();

        float minDistance;
        int racerIndex = 0;

        for (int i = 0; i < placements.Length; i++)
        {
            minDistance = Mathf.NegativeInfinity;
            for (int j = 0; j < placements.Length; j++)
            {
                if (placements[j] == null) continue;
                if (placements[j].GetTotalDistanceOfTrack() > minDistance)
                {
                    minDistance = placements[j].GetTotalDistanceOfTrack();
                    racerIndex = j;
                }
            }
            placements[racerIndex].placement = i + 1;
            placements[racerIndex] = null;
        }
    }

    private void CreateNodes()
    {
        firstNode = new Node(points[0].transform.position);
        firstNode.first = true;
        Node currentNode = firstNode;
        Node prevNode = currentNode;

        trackLength = 0;
        for (int i = 1; i < points.Length; i++)
        {
            currentNode = new Node(points[i].transform.position);
            currentNode.prev = prevNode;
            prevNode.next = currentNode;
            prevNode = currentNode;

            trackLength += (currentNode.pos - currentNode.prev.pos).magnitude;
        }
        
        currentNode.next = firstNode;
        firstNode.prev = currentNode;

        trackLength += (firstNode.pos - firstNode.prev.pos).magnitude;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        CreateNodes();
        Node currentNode = firstNode;
        Node nextNode = currentNode.next;

        for (int i = 1; i < points.Length+1; i++)
        {
            Gizmos.DrawLine(currentNode.pos, nextNode.pos);

            currentNode = nextNode;
            nextNode = currentNode.next;
        }
    }
}
