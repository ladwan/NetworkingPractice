using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace ForverFight.HelperScripts
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
        private Countdown instance;
        [NonSerialized]
        private bool doOnce = false;


        public int Time { get => time; set => time = value; }

        public UnityEvent AwakeEvent { get => awakeEvent; set => awakeEvent = value; }

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
            StartCoroutine(Count());
        }

        public void ResetTimer(int value)
        {
            StopAllCoroutines();
            time = value;
            StartCoroutine(Count());
        }
    }
}
