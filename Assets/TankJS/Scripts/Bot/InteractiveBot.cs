using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class InteractiveBot : IBot
{
    public InteractiveBot()
    {
        keys = keysWASD;
    }
    public InteractiveBot(KeySet keySet)
    {
        switch (keySet)
        {
            case KeySet.WASD:
                keys = keysWASD;
                break;
            case KeySet.IJKL:
                keys = keysIJKL;
                break;
        }
    }
    
    public enum KeySet
    {
        WASD,
        IJKL,
    }

    static KeyCode[] keysWASD = new KeyCode[] { KeyCode.Space, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Q, KeyCode.Alpha2 };
    static KeyCode[] keysIJKL = new KeyCode[] { KeyCode.Space, KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L, KeyCode.U, KeyCode.Alpha8 };
    KeyCode[] keys;

    public int RequestAction(GameInformation info)
    {
        for(int i = 1; i < keys.Length; i++)
        {
            if (Input.GetKey(keys[i]))
            {
                return i;
            }
        }
        return 0;
    }
}