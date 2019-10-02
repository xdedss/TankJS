using System.Collections;
using System.Diagnostics;
using System.IO;
using System;
using UnityEngine;
using NiL.JS.Core;
using NiL.JS.Extensions;
using System.Runtime.Serialization.Json;

public class ScriptBot : IBot
{

    public string scriptPath;
    public ThreadContext threadContext;

    public string Name { get => tankName + "-" + jsName; set { tankName = value; } }
    string jsName;
    string tankName;
    string logFileName = "";

    public int ActionResult { get; private set; }
    public bool IsRunning { get; private set; }


    Stopwatch watch = new Stopwatch();

    public ScriptBot(string scriptPath)
    {
        this.scriptPath = scriptPath;
        var sp = scriptPath.Split('\\');
        jsName = sp[sp.Length - 1];
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
        catch (JSException ex)
        {
            HandleJSError(ex);
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }

    }

    public int RequestAction(GameInformation info)
    {
        try
        {
            //threadContext.ctx.GetVariable("info").Assign(info);
            threadContext.ctx.Eval("info=" + ObjectToJson(info));
            var res = threadContext.TimedEval("update()", Configurations.ScriptTimeout);
            var action = res.As<int>();
            UnityEngine.Debug.Log(Name + " -> " + action);
            return action;
        }
        catch (JSException ex)
        {
            HandleJSError(ex);
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        return 0;
    }

    public IEnumerator RequestActionAsync(GameInformation info)
    {
        IsRunning = true;
        ActionResult = 0;
        threadContext.ctx.Eval("info=" + ObjectToJson(info));
        watch.Restart();
        threadContext.AsyncTimedEval("update()", Configurations.ScriptTimeout, (res) => {
            IsRunning = false;
            watch.Stop();
            try
            {
                ActionResult = res.As<int>();
            }
            catch(Exception ex)
            {
                HandleError(ex);
            }
        }, (err) => {
            IsRunning = false;
            watch.Stop();
            if (err is JSException)
                HandleJSError(err as JSException);
            else
                HandleError(err);
        });
        while (IsRunning)
        {
            yield return 0;
        }
        var time = Mathf.CeilToInt(1000f * watch.ElapsedTicks / (float)Stopwatch.Frequency);
        UnityEngine.Debug.Log(string.Format("Async eval completed in {0} ms", time));
        DebugMessage(string.Format("-------- {0} ms --------", time));
    }


    private void HandleJSError(JSException ex)
    {
        UnityEngine.Debug.LogError(ex);
        LogToFile("[ERROR]" + ex.Message + ex.Error.GetProperty("stack"));
    }
    private void HandleError(Exception ex)
    {
        UnityEngine.Debug.LogError(ex);
        LogToFile("[ERROR]" + ex.Message + ex.StackTrace);
    }

    private void DebugMessage(string msg)
    {
        //Debug.Log(Name + " : " + msg);
        LogToFile(msg);
    }

    private void LogToFile(string msg)
    {
        if (logFileName == "") logFileName = string.Format("{3}{0}_{1}_{2}.log", jsName, DateTime.Now.ToString("MMdd_HHmmss"), tankName, Configurations.BotFolder);
        
        var sw = File.AppendText(logFileName);
        sw.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff]") + msg);
        sw.Flush();
        sw.Close();
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
