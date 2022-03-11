using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;

[CreateAssetMenu(menuName = "ChosenCharacter")]
public class CharacterScriptableObject : ScriptableObject
{
    [SerializeField]
    public Character.Identity selectedIdentity;
    [SerializeField]
    public List<Character> allCharacters = null;
}
