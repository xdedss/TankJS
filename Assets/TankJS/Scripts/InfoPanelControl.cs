using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class InfoPanelControl : MonoBehaviour
{
    public float scaleFactor;
    public Transform target;

    public TextMesh nameText;
    public TextMesh infoText;

    public GameObject runningMark;

    public void UpdateInfo(int hp, int attack)
    {
        infoText.text = string.Format("<color=#0f0>{0}</color> <color=#f80>{1}</color>", hp, attack);
    }

    public void UpdateName(string name)
    {
        nameText.text = name;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        transform.rotation = target.rotation;
        var distance = (transform.position - target.position).magnitude;
        transform.localScale = Vector3.one * scaleFactor * distance;
    }
}
