using ForeverFight.GameMechanics.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmButtonEventListener : MonoBehaviour
{
    [SerializeField]
    private Button confirmButton = null;

    private void Start()
    {
        FloorGrid.Instance.HoveredOverGridPointsUpdated += Listen;
    }

    private void OnDestroy()
    {
        FloorGrid.Instance.HoveredOverGridPointsUpdated -= Listen;
    }


    private void Listen(int value)
    {
        confirmButton.interactable = value > 1;
    }
}
