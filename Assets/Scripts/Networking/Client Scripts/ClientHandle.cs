using ForeverFight.FlowControl;
using ForeverFight.GameMechanics;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.HelperScripts;
using ForeverFight.Interactable.Abilities;
using ForeverFight.Networking;
using ForeverFight.Ui;
using ForeverFight.Ui.CharacterSelection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
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

        Client.localClientInstance.localClientId = _myId;
        ClientInfo.totalPlayersConnected++;
        ClientSend.WelcomeReceived();
    }

    public static void ReceiveInitialMatchDetails(Packet _packet)
    {
        ClientInfo.matchIndex = _packet.ReadInt();
        ClientInfo.playerNumber = _packet.ReadInt();
        ClientInfo.otherUsername = _packet.ReadString();
        ClientInfo.totalPlayersConnected++;
    }

    public static void ReceiveSelectionPacket(Packet _packet)
    {
        int _panelIndex = _packet.ReadInt();
        int _playerIndex = _packet.ReadInt();
        string _otherPlayersCharName = _packet.ReadString();
        CharacterSelect.Instance.UpdateOtherPlayerSelection(_panelIndex, _playerIndex);
        LocalStoredNetworkData.locallyStoredOpponentsName = _otherPlayersCharName;
        Debug.Log("~~[MATCH] Made it");
    }

    public static void ReceiveReadyUpSignal(Packet _packet)
    {
        CharacterSelect.Instance.OtherPlayerCheckmark.enabled = true;

        if (SendReadyUp.Instance.LocalPlayerCheckmark.enabled == true)
        {
            ClientSend.EnterSyncTimerQueue();
        }
    }

    public static void ReceiveSyncedTimerTime(Packet _packet)
    {
        int _timeLeft = _packet.ReadInt();
        if (_timeLeft <= 4)
        {
            return;
        }

        CharacterSelect.Instance.CountdownTimer.Time = 4;
    }

    public static void RecieveSegmentedMovementData(Packet _packet)
    {
        int _x = _packet.ReadInt();
        int _y = _packet.ReadInt();
        int _count = _packet.ReadInt();
        bool _hasRotations = _packet.ReadBool();
        bool _completed = _packet.ReadBool();

        FloorGrid.Instance.ConstructVector3ListFromNetworkData((new Vector3(_x, 0, _y)), _count, _hasRotations, _completed);

        //make the list of lists in a place it will be created once
        //use method call and conditional logic to write data
    }

    public static void RecieveSegmentedRotationData(Packet _packet)
    {
        float _x = _packet.ReadFloat();
        float _y = _packet.ReadFloat();
        float _z = _packet.ReadFloat();
        float _w = _packet.ReadFloat();
        int _count = _packet.ReadInt();

        FloorGrid.Instance.ConstructQuaternionListFromNetworkData((new Quaternion(_x, _y, _z, _w)), _count);

        //make the list of lists in a place it will be created once
        //use method call and conditional logic to write data
    }

    // This is going to be recived by BOTH players anytime it runs
    public static void ReceiveToggleTimerSignal(Packet _packet)
    {
        int _signalInt = _packet.ReadInt();
        LocalStoredNetworkData.GetCountdownTimerScript().ToggleCountdownTimer();
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
                AnimatorStateInfo stateInfo = opponentAnimatior.GetCurrentAnimatorStateInfo(0);
                var shakeTime = opponentAnimatior.runtimeAnimatorController.animationClips[i].events[0].time; //events at the zero-th index is unsafe, works for now though! No guarantee the event youre looking for will be 0
                ExecuteMethodAfterDelay.Instance.BeginWaitUntilTrue(BeginLocalCameraShakeRecievedFromOpponent);
                break;
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

    public static void RecieveUpdatedPlayerPosition(Packet _packet)
    {
        int x = _packet.ReadInt();
        int y = _packet.ReadInt();
        int hoveredOverGPsCount = _packet.ReadInt();

        FloorGrid.Instance.UpdateOpponentPosition(new Vector2(x, y));
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

    public static void RecieveNetworkedMethodIndex(Packet _packet)
    {
        int _abilityIndex = _packet.ReadInt();
        int _methodIndex = _packet.ReadInt();

        LocalStoredNetworkData.GetOpponentCharacter().Moveset[_abilityIndex].NetworkedMethodCall(_methodIndex);
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

    private static async Task DelayedExecutionAsync()
    {
        // Polling interval (e.g., 100 milliseconds)
        const int pollingInterval = 100;

        while (PlayerTurnManager.Instance == null && Application.isPlaying)
        {
            await Task.Delay(pollingInterval);
        }
    }

    private static void BeginLocalCameraShakeRecievedFromOpponent()
    {
        CameraScreenShakeManager.Instance.StartShake(currentCameraShakeParameters);
    }
}

