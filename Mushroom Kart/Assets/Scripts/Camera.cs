using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Transform follow; 
    [SerializeField]private float speed = 0.25f;
    [SerializeField]private Vector3 offset;

    void Awake()
    {
        follow = GameObject.Find("Racer").transform;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,follow.position + offset,speed);
    }
}
