using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IBot
{
    string Name { get; set; }
    int RequestAction(GameInformation info);
}
