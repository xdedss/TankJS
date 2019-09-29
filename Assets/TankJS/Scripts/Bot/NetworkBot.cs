using System;
using System.Collections.Generic;
using System.Net.Http;

class NetworkBot : IBot
{

    public string url;

    public string Name { get => tankName + "-NetworkBot@" + url; set { tankName = value; } }
    string tankName;

    public int RequestAction(GameInformation info)
    {
        
        throw new NotImplementedException();
    }
}
