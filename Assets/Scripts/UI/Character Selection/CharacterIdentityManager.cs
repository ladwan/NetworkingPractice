using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
public class CharacterIdentityManager : MonoBehaviour
{
    [SerializeField]
    private CharacterScriptableObject characterScriptableObjectREF;

    public void SetSelectedIdentity(Character.Identity id)
    {
        characterScriptableObjectREF.selectedIdentity = id;
    }
}
