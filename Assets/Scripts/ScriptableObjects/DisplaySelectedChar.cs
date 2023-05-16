using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.GameMechanics;
using Interactable;


public class DisplaySelectedChar : MonoBehaviour
{
    [SerializeField]
    public Dictionary<Character.Identity, Character> myCharacterDictionary = new Dictionary<Character.Identity, Character>();

    [SerializeField]
    private Character.Identity playersIdentity;
    [SerializeField]
    private CharacterScriptableObject characterScriptableObjRef;

    protected void Awake()
    {
        playersIdentity = characterScriptableObjRef.selectedIdentity;
        myCharacterDictionary.Add(Character.Identity.Brawn, characterScriptableObjRef.allCharacters[0]);
        myCharacterDictionary.Add(Character.Identity.Speedster, characterScriptableObjRef.allCharacters[1]);
    }


    public void SpawnPlayer(Transform spawnPoint)
    {
        if (myCharacterDictionary.TryGetValue(playersIdentity, out Character value))
        {
            Instantiate(value, spawnPoint);
            GameMechanicsManager.Instance.HandleSpawningOpponent();
        }
    }
}
