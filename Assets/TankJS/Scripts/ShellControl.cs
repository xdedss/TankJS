using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellControl : MonoBehaviour
{
    float distance;
    float speed = 100;
    bool fired = false;

    public GameObject explosion;
    public GameObject model;

    public void Init(float distance)
    {
        this.distance = distance;
        //Debug.Log("distance=" + distance);
        fired = true;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!fired) return;
        float step = speed * Time.fixedDeltaTime;
        bool expl = step > distance;
        step = Mathf.Min(step, distance);
        transform.Translate(new Vector3(0, 0, step), Space.Self);
        distance -= step;
        if (expl) Explode();
    }

    private void Explode()
    {
        explosion.SetActive(true);
        model.SetActive(false);
        Destroy(gameObject, 2);
    }
}
