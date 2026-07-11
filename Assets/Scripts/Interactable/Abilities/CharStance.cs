using System.Collections;
using System.Collections.Generic;
using ForeverFight.Interactable.Abilities;
using UnityEngine;

public abstract class CharStance : MonoBehaviour
{
    [SerializeField]
    private List<CharAbility> stanceMoveset = new List<CharAbility>();

    public List<CharAbility> StanceMoveset => stanceMoveset;
}
