using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatUiStatesManager : MonoBehaviour
{
    [SerializeField]
    private combatUiState currentCombatUiState = combatUiState.unselected;


    public delegate void CombatUiStatesDelegate();
    public event CombatUiStatesDelegate onCombatUiStateChange;
    public enum combatUiState
    {
        unselected = 0,
        main = 1,
        movement = 2,
        attack = 3,
    };


    private static CombatUiStatesManager instance;

    public static CombatUiStatesManager Instance { get => instance; set => instance = value; }

    public combatUiState CurrentCombatUiState => currentCombatUiState;


    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exsists, destroying object!");
            Destroy(this);
        }
    }


    public void SetCombatUiState(int value)
    {
        currentCombatUiState = (combatUiState)value;
        onCombatUiStateChange?.Invoke();
    }
}
