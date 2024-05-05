using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui.CharacterSelection;

namespace ForeverFight.FlowControl
{
    public class SendReadyUp : MonoBehaviour
    {
        [SerializeField]
        private static SendReadyUp instance = null;
        [SerializeField]
        private GameObject localPlayerCheckmark = null;


        public static SendReadyUp Instance { get => instance; set => instance = value; }
        public GameObject LocalPlayerCheckmark { get => localPlayerCheckmark; set => localPlayerCheckmark = value; }


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

            if (CharacterSelect.Instance.OtherPlayerCheckmark.activeInHierarchy)
            {
                ClientSend.EnterSyncTimerQueue();
            }

        }
    }
}
