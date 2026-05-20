using ForeverFight.FlowControl;
using ForeverFight.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverdriveTextManager : MonoBehaviour
{
    [SerializeField] private GameObject turnsUntilOverdriveParent = null;
    [SerializeField] private GameObject overdriveParent = null;
    [SerializeField] private TMP_Text turnsUntilOverdriveText = null;
    [SerializeField] private TMP_Text overdriveText = null;


    private void OnEnable()
    {
        PlayerTurnManager.Instance.OnTurnStart += UpdateOverdriveText;
    }

    private void OnDisable()
    {
        PlayerTurnManager.Instance.OnTurnStart -= UpdateOverdriveText;
    }

    private void UpdateOverdriveText()
    {
        var turnsTilOverdrive = PlayerTurnManager.Instance.TurnsUntilOverdrive;

        if (turnsTilOverdrive > 0)
        {
            turnsUntilOverdriveText.text = turnsTilOverdrive.ToString();
            return;
        }

        turnsUntilOverdriveParent.SetActive(false);
        overdriveParent.SetActive(true);

        overdriveText.text = PlayerTurnManager.Instance.OverdriveAp.ToString();
    }
}
