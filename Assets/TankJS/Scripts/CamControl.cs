using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{

    Vector3 anchorPoint;

    float speed = 3f;
    float rotationSpeed = 180f;

    void Start()
    {
        
    }
    
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            var euler = transform.eulerAngles;
            euler.x += Time.deltaTime * Input.GetAxis("Mouse Y") * rotationSpeed;
            euler.y += -Time.deltaTime * Input.GetAxis("Mouse X") * rotationSpeed;
            euler.x = Mathf.Clamp(CenterAngle(euler.x, 0), 0, 90);
            transform.eulerAngles = euler;
        }
        var forward = transform.forward;
        forward.y = 0;
        forward = forward.normalized;
        var up = Vector3.up;
        var right = Vector3.Cross(up, forward);

        if (Input.GetMouseButton(2))
        {
            transform.Translate(-forward * Time.deltaTime * speed * transform.position.y * Input.GetAxis("Mouse Y"), Space.World);
            transform.Translate(-right * Time.deltaTime * speed * transform.position.y * Input.GetAxis("Mouse X"), Space.World);
        }

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        var pos = transform.position;
        pos.y *= Mathf.Exp(scroll);
        transform.position = pos;
    }

    void FindAnchor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            anchorPoint = hit.point;
        }
        else
        {
            anchorPoint = Camera.main.transform.position + 10 * Camera.main.transform.forward;
        }
    }

    private float CenterAngle(float degrees, float center)
    {
        while(degrees > center + 180)
        {
            degrees -= 360;
        }
        while(degrees <= center - 180)
        {
            degrees += 360;
        }
        return degrees;
    }
}
