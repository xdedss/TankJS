using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;

class NetworkBot : IBot
{

    public string url;

    public string Name { get { return tankName + "-NetworkBot@" + url; } set { tankName = value; } }

    public int ActionResult { get { throw new NotImplementedException(); } }
    public bool IsRunning { get { throw new NotImplementedException(); } }

    string tankName;

    public int RequestAction(GameInformation info)
    {
        
        throw new NotImplementedException();
    }

    public IEnumerator RequestActionAsync(GameInformation info)
    {
        throw new NotImplementedException();
    }
}
