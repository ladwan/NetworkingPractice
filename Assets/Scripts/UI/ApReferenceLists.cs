using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Ui;
using ForverFight.Networking;
using ForverFight.Interactable.Characters;

public class ApReferenceLists : MonoBehaviour
{

    [SerializeField]
    public apDisplayTypes currentApDisplayType = apDisplayTypes.unselected;
    [SerializeField]
    private List<GameObject> apLights = new List<GameObject>();
    [SerializeField]
    private int maxAP = 0;

    public enum apDisplayTypes
    {
        unselected,
        main,
        speedster,
    };
    private bool blinkCoroutineIsRunning = false;
    private int referenceListsApValueToUpdate = 0;
    private List<GameObject> apLightsToBeBlinked = new List<GameObject>();
    private Speedster speedsterREF = null;


    public List<GameObject> ApLights => apLights;

    public List<GameObject> ApLightsToBeBlinked => apLightsToBeBlinked;

    public int ReferenceListsApValueToUpdate { get => referenceListsApValueToUpdate; set => referenceListsApValueToUpdate = value; }

    public bool BlinkCoroutineIsRunning { get => blinkCoroutineIsRunning; set => blinkCoroutineIsRunning = value; }

    public int MaxAP { get => maxAP; set => maxAP = value; }

    public apDisplayTypes CurrentApDisplayType => currentApDisplayType;

    public Speedster SpeedsterREF
    {
        get
        {
            if (speedsterREF)
            {
                return speedsterREF;
            }
            else
            {
                var tempCharacter = (Speedster)LocalStoredNetworkData.GetLocalCharacter();
                if (tempCharacter)
                {
                    speedsterREF = tempCharacter;
                    maxAP = SpeedsterREF.FasterPassive.MaxPassiveAp;
                    return speedsterREF;
                }
                else
                {
                    Debug.Log("Speedster ref came back as NULL!");
                    return null;
                }
            }
        }
    }


    protected void Awake()
    {
        switch (currentApDisplayType)
        {
            case apDisplayTypes.unselected:
                break;
            case apDisplayTypes.main:
                maxAP = 9;
                break;
        }
    }


    public void ShowAp(int currentApValue)
    {
        for (int i = 0; i < currentApValue; i++)
        {
            apLights[i].gameObject.SetActive(true);
        }
    }

    public void HideAllApLights()
    {
        for (int i = 0; i < apLights.Count; i++)
        {
            apLights[i].gameObject.SetActive(false);
        }
    }

    public IEnumerator Blink()
    {
        blinkCoroutineIsRunning = true;
        yield return new WaitForSecondsRealtime(0.5f);

        for (int i = 0; i < apLightsToBeBlinked.Count; i++)
        {
            apLightsToBeBlinked[i].gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(0.5f);

        for (int i = 0; i < apLightsToBeBlinked.Count; i++)
        {
            apLightsToBeBlinked[i].gameObject.SetActive(true);
        }

        StartCoroutine(Blink());
    }

    public void StopBlink()
    {
        StopAllCoroutines();
        blinkCoroutineIsRunning = false;
    }

    public int UpdateValueOfRelevantAp(int addend)
    {
        if (currentApDisplayType == apDisplayTypes.main)
        {
            LocalStoredNetworkData.localPlayerCurrentAP = Mathf.Clamp(LocalStoredNetworkData.localPlayerCurrentAP + addend, 0, maxAP);
            return LocalStoredNetworkData.localPlayerCurrentAP;
        }

        if (currentApDisplayType == apDisplayTypes.speedster)
        {
            SpeedsterREF.FasterPassive.PassiveAp = Mathf.Clamp(SpeedsterREF.FasterPassive.PassiveAp + addend, 0, maxAP);
            return SpeedsterREF.FasterPassive.PassiveAp;
        }
        Debug.Log("Value to update was abnormal");
        return 0;
    }

    public int SetValueOfRelevantAp(int value)
    {
        if (currentApDisplayType == apDisplayTypes.main)
        {
            LocalStoredNetworkData.localPlayerCurrentAP = Mathf.Clamp(value, 0, maxAP);
            return LocalStoredNetworkData.localPlayerCurrentAP;
        }

        if (currentApDisplayType == apDisplayTypes.speedster)
        {
            SpeedsterREF.FasterPassive.PassiveAp = Mathf.Clamp(value, 0, maxAP);
            return SpeedsterREF.FasterPassive.PassiveAp;
        }

        Debug.Log("Value to update was abnormal");
        return 0;
    }
}
