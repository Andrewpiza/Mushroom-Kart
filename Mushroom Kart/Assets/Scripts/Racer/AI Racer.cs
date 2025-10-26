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
    }

    private void ShootRays(){
        Vector3 currentDir = transform.right;
        Vector3 dir = transform.up;
        for (int i = 0;i < amountOfRays;i ++){
            RaycastHit2D raycast = Physics2D.Raycast(transform.position, dir, rayCastLength, LayerMask.GetMask("AI Wall"));
            if (!raycast.collider)
            {
                currentDir += dir;
            }

            dir = Quaternion.AngleAxis(-rayAngle,Vector3.forward) * dir;
        }

        dir = currentDir.normalized;
        Move(dir);
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
