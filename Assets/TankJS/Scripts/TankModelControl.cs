using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankModelControl : MonoBehaviour
{
    public TankControl tankEntity;
    float lerpT = 0.15f;

    public GameObject shellPrefab;
    public Transform cannonPosition;
    public GameObject explosion;
    public GameObject model;
    public InfoPanelControl infoPanel;

    private bool work = false;

    public void Init()
    {
        transform.position = tankEntity.transform.position;
        transform.rotation = tankEntity.transform.rotation;
        infoPanel.UpdateName(tankEntity.tankName);
        work = true;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        if (work)
        {
            if (!tankEntity)
            {
                Explode();
                return;
            }
            infoPanel.UpdateInfo(tankEntity.tankInformation.health, tankEntity.tankInformation.attack);
            lerpT = Mathf.Min(Time.deltaTime / GameControl.instance.totalInterval * 1.5f, 1);
            transform.position = Vector3.Lerp(transform.position, tankEntity.transform.position, lerpT * 0.8f);
            transform.rotation = Quaternion.Lerp(transform.rotation, tankEntity.transform.rotation, lerpT);
        }
    }

    public void Fire(float distance)
    {
        var shell = Instantiate(shellPrefab);
        shell.transform.position = cannonPosition.position;
        shell.transform.rotation = cannonPosition.rotation;
        var euler = shell.transform.eulerAngles;
        euler.y = Mathf.RoundToInt(euler.y / 90) * 90;
        shell.transform.eulerAngles = euler;
        shell.GetComponent<ShellControl>().Init(distance);
    }

    public void Explode()
    {
        explosion.SetActive(true);
        model.SetActive(false);
        work = false;
        Destroy(gameObject, 2);
    }
}
