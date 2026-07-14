using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.Networking;
using ForeverFight.GameMechanics.Movement;
using UnityEngine.UI;

public class ApReferenceLists : MonoBehaviour
{

    [SerializeField]
    public apDisplayTypes currentApDisplayType = apDisplayTypes.unselected;
    [SerializeField]
    private List<GameObject> apLights = new List<GameObject>();
    [SerializeField]
    private int mainApBarMaxAP = 9;

    public enum apDisplayTypes
    {
        unselected,
        main,
        movementPassive, // Serialized as index 2 (was "speedster") - any character's movement-only passive pool.
    };

    private bool blinkCoroutineIsRunning = false;
    private int referenceListsApValueToUpdate = 0;
    private List<GameObject> apLightsToBeBlinked = new List<GameObject>();


    public List<GameObject> ApLights => apLights;

    public List<GameObject> ApLightsToBeBlinked => apLightsToBeBlinked;

    public int ReferenceListsApValueToUpdate { get => referenceListsApValueToUpdate; set => referenceListsApValueToUpdate = value; }

    public bool BlinkCoroutineIsRunning { get => blinkCoroutineIsRunning; set => blinkCoroutineIsRunning = value; }

    public int MaxAP { get => mainApBarMaxAP; set => mainApBarMaxAP = value; }

    public apDisplayTypes CurrentApDisplayType => currentApDisplayType;

    private static IMovementPassiveAp PassiveApProvider => ActionPointsManager.Instance != null
        ? ActionPointsManager.Instance.MovementPassiveApProvider
        : null;


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
            LocalStoredNetworkData.localPlayerCurrentAP = Mathf.Clamp(LocalStoredNetworkData.localPlayerCurrentAP + addend, 0, mainApBarMaxAP);
            return LocalStoredNetworkData.localPlayerCurrentAP;
        }

        if (currentApDisplayType == apDisplayTypes.movementPassive)
        {
            var provider = PassiveApProvider;
            if (provider == null)
            {
                Debug.LogError("Movement passive AP list updated but no IMovementPassiveAp provider is registered");
                return 0;
            }

            provider.PassiveAp = Mathf.Clamp(provider.PassiveAp + addend, 0, provider.MaxPassiveAp);
            return provider.PassiveAp;
        }

        Debug.LogError("Value to update was abnormal");
        return 0;
    }

    public int SetValueOfRelevantAp(int value)
    {
        if (currentApDisplayType == apDisplayTypes.main)
        {
            LocalStoredNetworkData.localPlayerCurrentAP = Mathf.Clamp(value, 0, mainApBarMaxAP);
            return LocalStoredNetworkData.localPlayerCurrentAP;
        }

        if (currentApDisplayType == apDisplayTypes.movementPassive)
        {
            var provider = PassiveApProvider;
            if (provider == null)
            {
                Debug.LogError("Movement passive AP list updated but no IMovementPassiveAp provider is registered");
                return 0;
            }

            provider.PassiveAp = Mathf.Clamp(value, 0, provider.MaxPassiveAp);
            return provider.PassiveAp;
        }

        Debug.LogError("Value to update was abnormal");
        return 0;
    }


    public void ColorAp(Color color, int amount)
    {
        for (int i = 0; i < amount && amount < apLights.Count; i++)
        {
            var image = apLights[i].gameObject.GetComponent<Image>();
            if (image != null)
            {
                image.color = color;
            }
        }
    }
}
