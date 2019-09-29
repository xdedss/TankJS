using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class NoBot : IBot
{
    public string Name { get => tankName + "-NoBot"; set { tankName = value; } }
    string tankName;

    public int RequestAction(GameInformation info)
    {
        return 0;
    }
}
