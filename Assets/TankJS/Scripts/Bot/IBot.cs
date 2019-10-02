using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IBot
{
    string Name { get; set; }
    int RequestAction(GameInformation info);

    bool IsRunning { get; }
    int ActionResult { get; }
    IEnumerator RequestActionAsync(GameInformation info);
}
