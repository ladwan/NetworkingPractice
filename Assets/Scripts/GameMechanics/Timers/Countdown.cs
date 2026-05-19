using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ForeverFight.Networking;
using TMPro;

namespace ForeverFight.GameMechanics.Timers
{
    public class Countdown : MonoBehaviour
    {
        [SerializeField]
        private int time = 0;
        [SerializeField]
        private UnityEvent awakeEvent = new UnityEvent();
        [SerializeField]
        private UnityEvent countdownBegin = new UnityEvent();
        [SerializeField]
        private UnityEvent countdownFinished = new UnityEvent();
        [SerializeField]
        private TextMeshProUGUI timeText = null;


        [NonSerialized]
        private int maxTime = 25;
        [NonSerialized]
        private int initialTime = 0;
        [NonSerialized]
        private Countdown instance;
        [NonSerialized]
        private bool doOnce = false;
        [NonSerialized]
        private bool isPaused = false;
        [NonSerialized]
        private bool hasAwoken = false;


        public int Time { get => time; set => time = value; }

        public int MaxTime { get => maxTime; set => maxTime = value; }

        public UnityEvent AwakeEvent { get => awakeEvent; set => awakeEvent = value; }

        public UnityEvent CountdownBegin { get => countdownBegin; set => countdownBegin = value; }

        public UnityEvent CountdownFinished { get => countdownFinished; set => countdownFinished = value; }


        private void Awake()
        {
            initialTime = time;
            hasAwoken = true;
            Debug.Log($"[Countdown] {name} Awake — time={time}, initialTime captured={initialTime}");

            if (instance == null)
            {
                instance = this;
                LocalStoredNetworkData.countdownTimerScript = instance;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exsists, destroying object!");
                Destroy(this);
            }

            AwakeEvent?.Invoke();
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
                if (timeText)
                {
                    timeText.text = time.ToString();
                }
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
            Debug.Log($"[Countdown] {name} StartTimer — time={time}, initialTime={initialTime}, doOnce={doOnce}");
            StartCoroutine(Count());
        }

        public void StopTimer()
        {
            Debug.Log($"[Countdown] {name} StopTimer — time={time} (will NOT be reset here)");
            StopAllCoroutines();
            isPaused = false;
            doOnce = false;
        }

        public void TellNetworkToToggleTimer() //This is needed so we can call from a button
        {
            //Debug.Log("TellNetworkToToggleTimer was called !");
            ClientSend.ToggleCountdownTimer();
        }

        public void ToggleCountdownTimer() // DO NOT directly call this
        {
            if (!isPaused)
            {
                StopAllCoroutines();
                isPaused = true;
                return;
            }

            StartCoroutine(Count());
            isPaused = false;
        }

        public void ResetTimer(int value)
        {
            StopAllCoroutines();
            time = value;
            StartCoroutine(Count());
        }

        public void ResetToInitialTime()
        {
            if (!hasAwoken)
            {
                return;
            }
            StopTimer();
            time = initialTime;
        }

        public void SetDebugTime()
        {
            maxTime = 999999;
        }
    }
}

