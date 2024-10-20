using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Threading.Tasks;

public class SyncAsyncButtonManager : MonoBehaviour
{
    [SerializeField] private int delay = 0;
    [SerializeField] private int syncLoopCount = 0;
    [SerializeField] private int asyncLoopCount = 0;
    [SerializeField] private TMP_Text syncLog = null;
    [SerializeField] private TMP_Text asyncLog = null;


    private Coroutine sub = null;
    private string logMessage = "Hey, I ran !";
    List<Task<Stopwatch>> tasks = new List<Task<Stopwatch>>();


    public void DoLongRunningMethod()
    {
        var watch = Stopwatch.StartNew();
        watch.Start();

        BeginCorutine(Delay(LongRunningMethod, watch, delay));
    }

    public async void DoAsync()
    {
        if (asyncLoopCount > 0)
        {
            var watch = Stopwatch.StartNew();
            watch.Start();

            var t = await LongRunningMethodAsync(watch);
            t.Stop();

            asyncLog.text += logMessage + " // Time : " + t.ElapsedMilliseconds + "\r\n";
            asyncLoopCount--;
            DoAsync();
        }
    }

    public void Stuff()
    {
        List<Task<Stopwatch>> tasks = new List<Task<Stopwatch>>();

        var watch = Stopwatch.StartNew();
        watch.Start();
        tasks.Add(LongRunningMethodAsync(watch));
    }

    public async void DoAsyncParallel()
    {
        var watch = Stopwatch.StartNew();
        watch.Start();

        for (int i = 0; i < asyncLoopCount; i++)
        {
            tasks.Add(Task.Run(() => LongRunningMethodAsync(watch)));
        }

        UnityEngine.Debug.Log($"tasks {tasks.Count}");
        var t = await Task.WhenAll(tasks);
        watch.Stop();
        asyncLog.text += logMessage + " // Time : " + watch.ElapsedMilliseconds + "\r\n";
    }



    private void BeginCorutine(IEnumerator myIEnumerator)
    {
        if (sub is not null)
        {
            StopCoroutine(sub);
            sub = null;
            UnityEngine.Debug.Log("Process terminated early");
        }

        sub = StartCoroutine(myIEnumerator);
    }

    private IEnumerator Delay(Action<Stopwatch> callback, Stopwatch watch, int delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        callback(watch);
    }

    private void LongRunningMethod(Stopwatch watch)
    {
        watch.Stop();
        syncLog.text += logMessage + " // Time : " + watch.ElapsedMilliseconds + "\r\n";
        syncLoopCount--;
        if (syncLoopCount > 0)
        {
            DoLongRunningMethod();
        }
    }


    private async Task<Stopwatch> LongRunningMethodAsync(Stopwatch watch)
    {
        await Task.Delay(1000);
        UnityEngine.Debug.Log($"I did stuff");
        return watch;
    }
}
