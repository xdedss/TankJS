using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TankInformation
{
    public int id;
    public int faction;
    public int x;
    public int z;
    /// <summary>
    /// 0=z+,1=x+,2=z-,3=x-
    /// </summary>
    public int rotation;
    public int health;
    public int attack;
    //public bool buffRadar;
    //public bool buffAttack;
}
