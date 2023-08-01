using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Interactable.Abilities
{
    public class Haste : MonoBehaviour
    {
        [SerializeField]
        private FasterPassive fasterPassiveREF = null;
        [SerializeField]
        private QuickPunch quickPunchREF = null;


    }

    /*
    The ult cost X amount of Ap
    After this turn for the next 3 turn you turn up!
    Passive action point pool doubles to 6
    attack radius doubles to 2sq's

    after those 3 turn, undo this effect
    */
}