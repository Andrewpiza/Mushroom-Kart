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
            for (int i = 0; i < amountOfRays * 2; i++)
            {
                RaycastHit2D raycast = Physics2D.Raycast(transform.position, dir, rayCastLength * 5, LayerMask.GetMask("AI Wall"));
                if (raycast.collider)
                {
                    currentDir += dir;
                }

                dir = Quaternion.AngleAxis(-rayAngle, Vector3.forward) * dir;
            }
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
            RaycastHit2D raycastForward = Physics2D.Raycast(transform.position + (2*transform.right*transform.localScale.x), transform.right,rayCastLength,LayerMask.GetMask("Racer","Item"));
            if (raycastForward.collider)ItemManager.Instance.UseItem(this, item[0],ItemDirection.Foward);
            RaycastHit2D raycastBackward= Physics2D.Raycast(transform.position - (2f*transform.right*transform.localScale.x), -transform.right,rayCastLength,LayerMask.GetMask("Racer","Item"));
            if (raycastBackward.collider)ItemManager.Instance.UseItem(this, item[0],ItemDirection.Backward);
            return;
        }
        else if (Random.Range(0,2) == 0)ItemManager.Instance.UseItem(this, item[0],ItemDirection.Foward);
        else{
            ItemManager.Instance.UseItem(this, item[0],ItemDirection.Backward);
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
