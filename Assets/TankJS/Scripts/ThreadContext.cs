using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NiL.JS.Core;
using NiL.JS.Extensions;
using System.Threading;
using System;

public class ThreadContext
{
    public string scriptPath;
    public Context ctx;

    JSValue evalResult;
    Exception evalException;
    bool evalSuccessful;

    public delegate void JSCallback(JSValue result);
    public delegate void JSErrorCallback(Exception exception);

    public ThreadContext(string scriptPath)
    {
        this.scriptPath = scriptPath;

        var globalCtx = new GlobalContext();
        ctx = new Context(globalCtx);
    }

    /// <summary>
    /// 限定运行时间，如果超时则抛出异常
    /// </summary>
    /// <param name="code"></param>
    /// <param name="milliTimeout">毫秒</param>
    /// <returns></returns>
    public JSValue TimedEval(string code, int milliTimeout)
    {
        //Debug.Log("executing: "+code);
        var thread = new Thread(() => {
            try
            {
                JSValue res;
                evalSuccessful = false;
                res = ctx.Eval(code);
                //Debug.Log(code + " : " + res);
                evalResult = res;
                evalSuccessful = true;
            }
            catch(ThreadAbortException)
            {
                evalException = new Exception("max time exceeded");
            }
            catch(Exception ex)
            {
                evalException = ex;
            }
        });
        thread.Start();
        var time = 0;
        for (int i = 0; i < 20; i++)
        {
            if (!thread.IsAlive)
            {
                break;
            }
            time += 1;
            Thread.Sleep(1);
        }
        for (int i = 0; i < milliTimeout / 10 - 2; i++)
        {
            if (!thread.IsAlive)
            {
                break;
            }
            time += 10;
            Thread.Sleep(10);
        }
        if (thread.IsAlive)
        {
            thread.Abort();
            thread.Join();
        }
        if (evalSuccessful)
        {
            Debug.Log("JS eval successful, time = " + time);
            return evalResult;
        }
        else
        {
            throw evalException;
        }
    }

    public void AsyncEval(string code, JSCallback callback, JSErrorCallback errorCallback)
    {
        var thread = new Thread(() => {
            try
            {
                var res = ctx.Eval(code);
                callback.Invoke(res);
            }
            catch (Exception ex)
            {
                errorCallback.Invoke(ex);
            }
        });
        thread.Start();
    }

    public void AsyncTimedEval(string code, int milliTimeout, JSCallback callback, JSErrorCallback errorCallback)
    {
        var thread = new Thread(() => {
            try
            {
                var res = ctx.Eval(code);
                callback.Invoke(res);
            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    errorCallback.Invoke(ex);
                }
            }
        });
        var timeThread = new Thread(() =>
        {
            try
            {
                Thread.Sleep(Configurations.ScriptTimeout);
                if (thread.IsAlive)
                {
                    thread.Abort();
                    errorCallback(new Exception("max time exceeded"));
                }
            }
            catch(Exception ex)
            {
                Debug.LogWarning("This is impossible!");
                Debug.LogError(ex);
            }
        });
        thread.Start();
        timeThread.Start();
    }
}
