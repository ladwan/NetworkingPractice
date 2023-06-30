using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPassiveAbility
{
    public string PassiveAbilityName { get; set; }

    public string PassiveAbilityDescription { get; set; }


    public void ApplyPassive()
    {
    }
}
