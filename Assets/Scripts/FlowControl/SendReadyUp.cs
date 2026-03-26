using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui.CharacterSelection;
using UnityEngine.UI;

namespace ForeverFight.FlowControl
{
    public class SendReadyUp : MonoBehaviour
    {
        [SerializeField]
        private static SendReadyUp instance = null;
        [SerializeField]
        private Image localPlayerCheckmark = null;


        public static SendReadyUp Instance => instance;
        public Image LocalPlayerCheckmark => localPlayerCheckmark;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("SendReadyUp Instance already exsists, destroying object!");
                Destroy(this);
            }

        }

        public void SendReadyUpSignal()
        {
            ClientSend.SendReadyUp();

            if (CharacterSelect.Instance.OtherPlayerCheckmark.enabled)
            {
                ClientSend.EnterSyncTimerQueue();
            }
        }
    }
}
