using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable.Characters;

public class CharacterIdentityManager : MonoBehaviour
{
    [SerializeField]
    private CharacterScriptableObject characterScriptableObjectREF;

    public void SetSelectedIdentity(Character.Identity id)
    {
        characterScriptableObjectREF.selectedIdentity = id;
    }
}
