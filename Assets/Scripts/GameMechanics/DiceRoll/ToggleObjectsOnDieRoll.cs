using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.GameMechanics.DiceRoll
{
    public class ToggleObjectsOnDieRoll : MonoBehaviour
    {
        [SerializeField]
        private RollDice rollDiceREF = null;
        [SerializeField]
        private List<GameObject> objectsToEnable = new List<GameObject>();
        [SerializeField]
        private List<GameObject> objectsToDisable = new List<GameObject>();


        //This will be called by anim event on Spawn anim
        public void TriggerDieRollAfterSpawnAnimation()
        {
            rollDiceREF.RollSixSidedDie();
            rollDiceREF.CallUiDelay();
        }


        public void ToggleObjects()
        {
            for (int i = 0; i < objectsToEnable.Count; i++)
            {
                objectsToEnable[i].SetActive(true);
            }

            for (int i = 0; i < objectsToDisable.Count; i++)
            {
                objectsToDisable[i].SetActive(false);
            }
        }
    }
}