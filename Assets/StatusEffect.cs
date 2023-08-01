using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Ui;
using ForverFight.Interactable.Abilities;
using TMPro;

public class StatusEffect : CharAbility
{
    [Serializable]
    public struct StatusEffectStruct
    {
        public Transform parentTransform;
        public GameObject characterSpecificUi;
        public TMP_Text statusEffectDurationTmp;
        public StatusEffectDisplayFormatter formatter;
    }

    public void FormatStatusEffectDisplayData(StatusEffectStruct statusEffectData)
    {

    }

    public void SendFormattedDataToManager(StatusEffectStruct myGroup)
    {
        //Send This data to the status effect manager
        //Debug.Log($"My int : {myGroup.testInt} , My GameObject : {myGroup.testGameObject.name} , My string : {myGroup.testString}");
    }
}
