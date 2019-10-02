using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class NoBot : IBot
{
    public string Name { get => tankName + "-NoBot"; set { tankName = value; } }

    string tankName;

    public int ActionResult => 0;
    public bool IsRunning => false;

    public int RequestAction(GameInformation info)
    {
        return 0;
    }

    public IEnumerator RequestActionAsync(GameInformation info)
    {
        yield return 0;
    }
}
