using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;
using ForeverFight.GameMechanics;
using ForeverFight.Ui.CharacterSelection;
using ForeverFight.Interactable.Abilities;
using ForeverFight.GameMechanics.Movement;
using System.Threading.Tasks;
using static ForeverFight.Interactable.Abilities.CharAbility;

public class ClientHandle : MonoBehaviour
{
    private MonoBehaviour clientHandleREF = null;
    private static CameraShakeParameters currentCameraShakeParameters = new CameraShakeParameters();
    public static Action winnerStatusReceived = null;


    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();
        int _totalPlayers = _packet.ReadInt();
        ClientInfo.totalPlayersConnected = _totalPlayers;
        if (ClientInfo.totalPlayersConnected == 1)
        {
            ClientInfo.playerNumber = 1;
        }
        else if (ClientInfo.totalPlayersConnected == 2)
        {
            ClientInfo.playerNumber = 2;
        }
        Debug.Log($"Message from server: {_msg}. Player id: {_myId} Total PLayers :{ClientInfo.totalPlayersConnected}");
        Client.localClientInstance.localClientId = _myId;

        ClientSend.WelcomeReceived();
    }
    public static void RecieveUpdatedPlayerPosition(Packet _packet)
    {
        int x = _packet.ReadInt();
        int y = _packet.ReadInt();
        FloorGrid.Instance.UpdateOpponentPosition(new Vector2(x, y));
    }

    public static void ReceiveTotalPlayerUpdate(Packet _packet)
    {
        int _totalPlayers = _packet.ReadInt();
        ClientInfo.totalPlayersConnected++;
    }

    public static void ReceiveSelectionPacket(Packet _packet)
    {
        int _panelIndex = _packet.ReadInt();
        int _playerIndex = _packet.ReadInt();
        string _otherPlayersCharName = _packet.ReadString();
        CharacterSelect.Instance.UpdateOtherPlayerSelection(_panelIndex, _playerIndex);
        LocalStoredNetworkData.locallyStoredOpponentsName = _otherPlayersCharName;
    }

    public static void ReceiveUsername(Packet _packet)
    {
        string _username = _packet.ReadString();


        ClientInfo.otherUsername = _username;
    }

    public async static void ReceiveStartTurnSignal(Packet _packet)
    {
        int signalInt = _packet.ReadInt();

        if (PlayerTurnManager.Instance == null)
        {
            await DelayedExecutionAsync();
        }

        PlayerTurnManager.Instance.StartTurn();
    }

    private static async Task DelayedExecutionAsync()
    {
        // Polling interval (e.g., 100 milliseconds)
        const int pollingInterval = 100;

        while (PlayerTurnManager.Instance == null && Application.isPlaying)
        {
            await Task.Delay(pollingInterval);
        }
    }

    public static void ReceiveReadyUpSignal(Packet _packet)
    {
        CharacterSelect.Instance.OtherPlayerCheckmark.SetActive(true);

        if (SendReadyUp.Instance.LocalPlayerCheckmark.activeInHierarchy)
        {
            ClientSend.EnterSyncTimerQueue();
        }
    }

    public static void ReceiveSyncedTimerTime(Packet _packet)
    {
        int _timeLeft = _packet.ReadInt();
        CharacterSelect.Instance.CountdownTimer.Time = 4;
    }

    public static void ReceiveToggleTimerSignal(Packet _packet)
    {
        int _signalInt = _packet.ReadInt();
        LocalStoredNetworkData.GetCountdownTimerScript().ToggleCountdownTimer();
    }

    public static void ReceiveAnimationTrigger(Packet _packet)
    {
        string trigger = _packet.ReadString();
        float duration = _packet.ReadFloat();
        float magnitude = _packet.ReadFloat();
        var tempParameters = new CharAbility.CameraShakeParameters();
        var opponentAnimatior = LocalStoredNetworkData.GetOpponentCharacter().CharacterAnimationReferences.CharacterAnimator;

        opponentAnimatior.SetTrigger(trigger);
        tempParameters.duration = duration;
        tempParameters.magnitude = magnitude;
        currentCameraShakeParameters = tempParameters;

        if (tempParameters.duration is 0)
        {
            return;
        }

        // Loop through until you find the state that matches the name of the trigger. Once found grab what time its animation trigger is at, use that as the delay for the camera shake!
        for (int i = 0; i < opponentAnimatior.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (opponentAnimatior.runtimeAnimatorController.animationClips[i].name == trigger)
            {
                var shakeTime = opponentAnimatior.runtimeAnimatorController.animationClips[i].events[0].time; //events at the zero-th index is unsafe, works for now though! No guarantee the event youre looking for will be 0
                ExecuteMethodAfterDelay.Instance.BeginDelay(shakeTime, BeginLocalCameraShakeRecievedFromOpponent);
            }
        }
    }

    public static void ReceiveDamage(Packet _packet)
    {
        int _damageAmount = _packet.ReadInt();
        DamageManager.Instance.ReceiveDamage(_damageAmount);
    }

    public static void ClientReceiveStatusEffectData(Packet _packet)
    {
        int _statusEffectIdentifier = _packet.ReadInt();
        int _duration = _packet.ReadInt();
        int _ownership = _packet.ReadInt();
        bool _endThisStatusEffect = _packet.ReadBool();

        StatusEffectStaticManager.Instance.UpdateNetworkedStatusEffectDisplay(_statusEffectIdentifier, _duration, _ownership, _endThisStatusEffect);
        //Debug.Log($"Status Effect Identifer : {_statusEffectIdentifier} Ownership is Player {_ownership}");
    }

    public static void ClientReceiveCurrentStatusEffectDuration(Packet _packet)
    {
        int _currentDuration = _packet.ReadInt();

        //StatusEffectStaticManager.Instance.Test(_statusEffectIdentifier, _duration, _ownership);
        //Debug.Log($"Status Effect Identifer : {_statusEffectIdentifier} Ownership is Player {_ownership}");
    }

    public static void ClientReceiveStoredMomentumValue(Packet _packet)
    {
        int _storedMomentum = _packet.ReadInt();

        var slot = StatusEffectStaticManager.Instance.RemoteStatusEffectDisplayManager.GetMatchingStatusEffectSlot(StatusEffect.StatusEffectType.Momentum);
        var displayReferences = slot.CharacterSpecificUi.GetComponent<MomentumDisplayReferences>();
        if (displayReferences)
        {
            displayReferences.StoredMomentumDisplayTmp.text = _storedMomentum.ToString();
        }
        else
        {
            Debug.Log("Could not find display references!");
        }
    }

    public static void RecieveOverrodePosition(Packet _packet)
    {
        int x = _packet.ReadInt();
        int y = _packet.ReadInt();
        Vector2 gridPointVector2 = new Vector2(x, y);

        FloorGrid.Instance.ProceduralGridManipulationREF.IsVector2AValidGridPoint(gridPointVector2);
    }

    public static void RecieveWinStatus(Packet _packet)
    {
        bool winStatus = _packet.ReadBool();

        winnerStatusReceived?.Invoke();
    }

    private static void BeginLocalCameraShakeRecievedFromOpponent()
    {
        CameraControls.Instance.StartShake(currentCameraShakeParameters);
    }
}

