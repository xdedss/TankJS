using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TankControl : MonoBehaviour
{
    public IBot bot;
    public string tankName;

    public TankInformation tankInformation;
    public GameInformation visibleInfo;

    public GameObject modelPrefab;
    public TankModelControl modelControl;

    
    int radarTime = 0;

    void Start()
    {

    }
    
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        tankInformation.health -= amount;

        if(tankInformation.health <= 0)
        {
            modelControl.Explode();
            GameControl.instance.tanks.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public void BuffHealth()
    {
        tankInformation.health += Configurations.BuffHealth;
    }

    public void BuffAttack()
    {
        tankInformation.attack = Configurations.BuffAttack;
    }

    public void BuffRadar()
    {
        radarTime = Configurations.RadarDuration;
    }

    public void Init()
    {
        tankInformation = new TankInformation();
        tankInformation.attack = Configurations.DefaultAttack;
        tankInformation.health = Configurations.DefaultHealth;
        UpdatePositionInfo();
        visibleInfo = new GameInformation();
        visibleInfo.mapData = GameControl.instance.mapData;
        visibleInfo.me = tankInformation;

        var model = Instantiate(modelPrefab);
        modelControl = model.GetComponent<TankModelControl>();
        modelControl.tankEntity = this;
        modelControl.Init();
    }

    public IEnumerator Round()
    {
        if (tankInformation.health > 0)
        {
            visibleInfo.tanks.Clear();
            visibleInfo.items.Clear();
            foreach (var t in GameControl.instance.tanks)
            {
                if (t.tankInformation.id != tankInformation.id)
                {
                    if (radarTime > 0 || t.tankInformation.faction == tankInformation.faction || CanSee(t))
                    {
                        visibleInfo.tanks.Add(t.tankInformation);
                    }
                }
            }
            foreach (var item in GameControl.instance.items)
            {
                if (CanSee(item.itemInformation.x, item.itemInformation.z))
                {
                    visibleInfo.items.Add(item.itemInformation);
                }
            }
            if (radarTime > 0) radarTime--;
            //var action = bot.RequestAction(visibleInfo);
            yield return bot.RequestActionAsync(visibleInfo);
            ExecuteAction(bot.ActionResult);
        }
        else
        {
            yield return 0;
        }
    }

    public bool CanSee(TankControl t)
    {
        return GameControl.instance.CanSee(this, t);
    }
    public bool CanSee(int x, int z)
    {
        return GameControl.instance.CanSee(tankInformation.x, tankInformation.z, tankInformation.rotation, x, z);
    }
    //public bool CanSee(int id)
    //{
    //    return GameControl.instance.CanSee(tankInformation.id, id);
    //}

    private void Snap()
    {
        var pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        pos.z = Mathf.Round(pos.z);
        var euler = transform.eulerAngles;
        euler.y = Mathf.Round(euler.y / 90) * 90;
        transform.position = pos;
        transform.eulerAngles = euler;
    }

    private void UpdatePositionInfo()
    {
        tankInformation.x = Mathf.RoundToInt(transform.position.x);
        tankInformation.z = Mathf.RoundToInt(transform.position.z);
        tankInformation.rotation = Mathf.RoundToInt(transform.eulerAngles.y / 90) % 4;
    }

    private void ExecuteAction(int id)
    {
        switch (id)
        {
            case 0:
                break;
            case 1:
                MoveForward();
                break;
            case 2:
                MoveBackward();
                break;
            case 3:
                TurnLeft();
                break;
            case 4:
                TurnRight();
                break;
            case 5:
                Attack();
                break;
            case 6:
                MoveForward2();
                break;
        }
    }

    private void CheckItem()
    {
        var item = GameControl.instance.SampleItem(tankInformation.x, tankInformation.z);
        if (item)
        {
            switch (item.itemInformation.type)
            {
                case 1:
                    BuffHealth();
                    break;
                case 2:
                    BuffAttack();
                    break;
                case 3:
                    BuffRadar();
                    break;
            }
            GameControl.instance.items.Remove(item);
            Destroy(item.gameObject);
        }
    }

    private void MoveForward()
    {
        var forward = transform.position + transform.forward;
        int fx = Mathf.RoundToInt(forward.x);
        int fz = Mathf.RoundToInt(forward.z);
        var forwardMap = GameControl.instance.Sample(fx, fz);
        var forwardTank = GameControl.instance.SampleTank(fx, fz);
        if (forwardMap == 0 && !forwardTank)
        {
            transform.Translate(new Vector3(0, 0, 1), Space.Self);
            UpdatePositionInfo();
            CheckItem();
        }
    }
    private void MoveBackward()
    {
        var backward = transform.position - transform.forward;
        int bx = Mathf.RoundToInt(backward.x);
        int bz = Mathf.RoundToInt(backward.z);
        var backwardMap = GameControl.instance.Sample(bx, bz);
        var backwardTank = GameControl.instance.SampleTank(bx, bz);
        if (backwardMap == 0 && !backwardTank)
        {
            transform.Translate(new Vector3(0, 0, -1), Space.Self);
            UpdatePositionInfo();
            CheckItem();
        }
    }
    private void TurnRight()
    {
        var euler = transform.eulerAngles;
        euler.y += 90;
        transform.eulerAngles = euler;
        UpdatePositionInfo();
    }
    private void TurnLeft()
    {
        var euler = transform.eulerAngles;
        euler.y -= 90;
        transform.eulerAngles = euler;
        UpdatePositionInfo();
    }
    private void Attack()
    {
        var start = transform.position + transform.forward * 0.48f + transform.up * 0.5f;
        var dir = transform.forward;
        Ray ray = new Ray(start, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 40, 3 << 9))
        {
            if (hit.transform.gameObject.layer == 10)
            {
                var anotherTank = hit.transform.parent.GetComponent<TankControl>();
                int damage = tankInformation.attack - Mathf.FloorToInt(Mathf.FloorToInt(hit.distance) * Configurations.DistanceFac);
                if (damage > 0)
                {
                    anotherTank.TakeDamage(damage);
                }
            }
            modelControl.Fire(hit.distance);
        }
        else
        {
            modelControl.Fire(40);
        }

        tankInformation.attack = Configurations.DefaultAttack;
    }
    private void MoveForward2()
    {
        MoveForward();
        MoveForward();
        //var forward = transform.position + transform.forward;
        //var forward2 = forward + transform.forward;
        //int fx = Mathf.RoundToInt(forward.x);
        //int fz = Mathf.RoundToInt(forward.z);
        //int fx2 = Mathf.RoundToInt(forward2.x);
        //int fz2 = Mathf.RoundToInt(forward2.z);
        //var forwardMap = GameControl.instance.Sample(fx, fz);
        //var forward2Map = GameControl.instance.Sample(fx2, fz2);
        //var forwardTank = GameControl.instance.SampleTank(fx, fz);
        //var forward2Tank = GameControl.instance.SampleTank(fx2, fz2);
        //if (forwardMap == 0 && forward2Map == 0 && !forwardTank && !forward2Tank)
        //{
        //    transform.Translate(new Vector3(0, 0, 2), Space.Self);
        //    UpdatePositionInfo();
        //    CheckItem();
        //}
    }
}
