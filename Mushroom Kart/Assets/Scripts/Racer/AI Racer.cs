using UnityEngine;

public class AIRacer : Racer
{
    [SerializeField]private int amountOfRays = 11;
    [SerializeField]private float rayCastLength = 16;
    [SerializeField] private float rayAngle = 18;

    // Update is called once per frame
    void Update()
    {
        ShootRays();
        UpdateRacer();
        if (item[0] != ItemType.Nothing && item[0] != ItemType.GettingItem) UseItem();
    }

    private void ShootRays()
    {
        Vector3 currentDir = new Vector3(0, 0, 0);
        Vector3 dir = transform.up;
        if (isOffRoad)
        {
            dir = currentNode.pos - transform.position;
            Move(dir.normalized);
        }
        else
        {
            for (int i = 0; i < amountOfRays; i++)
            {
                RaycastHit2D raycast = Physics2D.Raycast(transform.position, dir, rayCastLength, LayerMask.GetMask("AI Wall"));
                if (!raycast.collider)
                {
                    currentDir += dir;
                }

                dir = Quaternion.AngleAxis(-rayAngle, Vector3.forward) * dir;
            }
        }

        dir = currentDir.normalized;
        Move(dir);
    }
    
    private void UseItem(){
        if (item[0] == ItemType.Banana || item[0] == ItemType.GreenShell){
            RaycastHit2D raycastForward = Physics2D.Raycast(transform.position + (2*transform.right*transform.localScale.x), transform.right,7,LayerMask.GetMask("Racer","Item"));
            if (raycastForward.collider) UseItem(ItemDirection.Foward);
            RaycastHit2D raycastBackward= Physics2D.Raycast(transform.position - (2f*transform.right*transform.localScale.x), -transform.right,7,LayerMask.GetMask("Racer","Item"));
            if (raycastBackward.collider)UseItem(ItemDirection.Backward);
            return;
        }
        else if (Random.Range(0,2) == 0)UseItem(ItemDirection.Foward);
        else{
            UseItem(ItemDirection.Backward);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 dir = transform.up;
        for (int i = 0; i < amountOfRays; i++)
        {
            RaycastHit2D raycast = Physics2D.Raycast(transform.position, dir, rayCastLength, LayerMask.GetMask("AI Wall"));
            if (raycast.collider) Gizmos.color = Color.red;
            else
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawLine(transform.position, transform.position + dir * rayCastLength);
            dir = Quaternion.AngleAxis(-rayAngle, Vector3.forward) * dir;
        }
    }
}
