using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using NiL.JS.Core;
using NiL.JS.Extensions;
using System.Runtime.Serialization.Json;

public class ScriptBot : IBot
{

    public string scriptPath;
    public string name;
    public ThreadContext threadContext;

    public ScriptBot(string scriptPath)
    {
        this.scriptPath = scriptPath;
        var sp = scriptPath.Split('\\');
        name = sp[sp.Length - 1];
        threadContext = new ThreadContext(scriptPath);
        threadContext.ctx.DefineConstructor(typeof(Vector2));
        //threadContext.ctx.DefineConstructor(typeof(TankInformation));
        //threadContext.ctx.DefineConstructor(typeof(ItemInformation));
        //threadContext.ctx.DefineConstructor(typeof(GameInformation));
        threadContext.ctx.DefineVariable("log").Assign(JSValue.Marshal(new Action<string>(DebugMessage)));
        threadContext.ctx.DefineVariable("info");

        var script = File.OpenText(scriptPath).ReadToEnd();

        try
        {
            threadContext.TimedEval(script, 2000);
        }
        catch(Exception ex)
        {
            HandleJSError(ex);
        }

    }

    public int RequestAction(GameInformation info)
    {
        try
        {
            //threadContext.ctx.GetVariable("info").Assign(info);
            threadContext.ctx.Eval("info=" + ObjectToJson(info));
            var res = threadContext.TimedEval("update()", 1000);
            var action = res.As<int>();
            Debug.Log(name + " -> " + action);
            return action;
        }
        catch (Exception ex)
        {
            HandleJSError(ex);
        }
        return 0;
    }

    private void HandleJSError(Exception ex)
    {
        Debug.LogError(ex);
    }

    private void DebugMessage(string msg)
    {
        Debug.Log(name + " : " + msg);
    }

    //https://www.cnblogs.com/JiYF/p/8628942.html
    private static string ObjectToJson(object obj)
    {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        MemoryStream stream = new MemoryStream();
        serializer.WriteObject(stream, obj);
        byte[] dataBytes = new byte[stream.Length];
        stream.Position = 0;
        stream.Read(dataBytes, 0, (int)stream.Length);
        return System.Text.Encoding.UTF8.GetString(dataBytes);
    }
}
