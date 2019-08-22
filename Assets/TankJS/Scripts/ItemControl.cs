using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour
{

    public ItemInformation itemInformation;
    public GameObject[] models;

    public void Init(int type)
    {
        itemInformation = new ItemInformation();
        itemInformation.x = Mathf.RoundToInt(transform.position.x);
        itemInformation.z = Mathf.RoundToInt(transform.position.z);
        itemInformation.type = type;
        for (int i = 0; i < models.Length; i++)
        {
            if (models[i]) {
                models[i].SetActive(i == type);
            }
        }
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
