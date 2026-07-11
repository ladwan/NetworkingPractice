using System.Collections;
using System.Collections.Generic;
using ForeverFight.Interactable.Abilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Stance")]
public class StanceScriptableObject : ScriptableObject
{
    [SerializeField]
    private List<CharAbility> stanceMoveset = new List<CharAbility>();
}
