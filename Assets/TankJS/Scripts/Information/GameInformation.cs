using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameInformation
{
    public int[][] mapData;
    public TankInformation me;
    public List<TankInformation> tanks = new List<TankInformation>();
    public List<ItemInformation> items = new List<ItemInformation>();
}
