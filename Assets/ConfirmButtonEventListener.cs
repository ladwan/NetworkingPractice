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
        MovementPlanner.Instance.OnPlanUpdated += Listen;
        MovementPlanner.Instance.OnPlanCleared += Disable;
    }

    private void OnDestroy()
    {
        if (MovementPlanner.Instance != null)
        {
            MovementPlanner.Instance.OnPlanUpdated -= Listen;
            MovementPlanner.Instance.OnPlanCleared -= Disable;
        }
    }


    private void Listen(float pathLength, int apCost)
    {
        confirmButton.interactable = apCost > 0;
    }

    private void Disable()
    {
        confirmButton.interactable = false;
    }
}
