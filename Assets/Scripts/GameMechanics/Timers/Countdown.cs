using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Countdown : MonoBehaviour
{
    [SerializeField]
    private int time = 0;
    [SerializeField]
    private UnityEvent countdownBegin = new UnityEvent();
    [SerializeField]
    private UnityEvent countdownFinished = new UnityEvent();


    [NonSerialized]
    private Countdown instance;
    [NonSerialized]
    private bool doOnce = false;


    public int Time { get => time; set => time = value; }

    public UnityEvent CountdownBegin { get => countdownBegin; set => countdownBegin = value; }

    public UnityEvent CountdownFinished { get => countdownFinished; set => countdownFinished = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exsists, destroying object!");
            Destroy(this);
        }
    }

    private IEnumerator Count()
    {
        if (doOnce == false)
        {
            countdownBegin?.Invoke();
            doOnce = true;
        }

        yield return new WaitForSecondsRealtime(1);
        if (time > 0)
        {
            time--;
            StartCoroutine(Count());
        }
        else
        {
            countdownFinished?.Invoke();
            StopCoroutine(Count());
        }
    }


    public void StartTimer()
    {
        StartCoroutine(Count());
    }
}
