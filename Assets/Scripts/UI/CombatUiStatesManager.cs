using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ForeverFight.Ui
{
    public class CombatUiStatesManager : MonoBehaviour
    {
        public enum CombatUiState
        {
            unselected = 0,
            main = 1,
            movement = 2,
            combat = 3,
        };


        [SerializeField]
        private CombatUiState currentCombatUiState = CombatUiState.unselected;
        [SerializeField]
        private Action onCombatUiStateChange = null;


        private static CombatUiStatesManager instance;


        public CombatUiState CurrentCombatUiState => currentCombatUiState;

        public Action OnCombatUiStateChange { get => onCombatUiStateChange; set => onCombatUiStateChange = value; }

        public static CombatUiStatesManager Instance { get => instance; set => instance = value; }


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
            currentCombatUiState = (CombatUiState)value;
            onCombatUiStateChange?.Invoke();
        }
    }
}