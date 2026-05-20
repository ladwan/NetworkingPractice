using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLobbyPanelUi : MonoBehaviour
{
 [SerializeField] private Text numOfPlayerText = null;
 [SerializeField] private Text matchStartCountdown = null;

 
 private PlayerCountListener playerCountListenerREF = null;
 
 
  private void OnEnable()
  {
      playerCountListenerREF = ScenePersistentNetworkUiConnectionManager.Instance.PlayerCountListener;
      if (playerCountListenerREF == null)
          return;
      
      matchStartCountdown = playerCountListenerREF.MatchStartCountdown;
      numOfPlayerText = playerCountListenerREF.MatchStartCountdown;
  }

  private void Update()
  {
      if(playerCountListenerREF != null)
      {
          matchStartCountdown.text = playerCountListenerREF.StartMatchTimerREF.Time.ToString();
      }
  }
}
