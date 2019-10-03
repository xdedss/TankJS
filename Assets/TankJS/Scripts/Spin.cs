using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Space space = Space.World;
    public Vector3 axis = Vector3.up;
    public float velocity = 1f;

    void Start()
    {
        
    }


    void Update()
    {
        transform.Rotate(axis, Time.deltaTime * 60 * velocity, space);
    }
}
