using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    void Start()
    {
        
    }


    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * 60, Space.World);
    }
}
