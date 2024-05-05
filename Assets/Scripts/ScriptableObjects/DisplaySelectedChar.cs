using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.GameMechanics;
using ForeverFight.Interactable;
using ForeverFight.Networking;
using ForeverFight.Ui;

public class DisplaySelectedChar : MonoBehaviour
{
    [SerializeField]
    public Dictionary<Character.Identity, Character> myCharacterDictionary = new Dictionary<Character.Identity, Character>();
    [SerializeField]
    private Character.Identity playersIdentity;
    [SerializeField]
    private CharacterScriptableObject characterScriptableObjRef;
    [SerializeField]
    private AbilitySelectionUiManager abilitySelectionUiManagerREF = null;

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
            var localPlayerCharInstance = Instantiate(value, spawnPoint);
            var opponentCharInstance = GameMechanicsManager.Instance.HandleSpawningOpponent();
            LocalStoredNetworkData.localPlayerCharacter = localPlayerCharInstance;
            LocalStoredNetworkData.opponentCharacter = opponentCharInstance;
            GameMechanicsManager.Instance.UpdateHealthSliderValues(localPlayerCharInstance, LocalStoredNetworkData.GetLocalHealthSlider());
            GameMechanicsManager.Instance.UpdateHealthSliderValues(opponentCharInstance, LocalStoredNetworkData.GetOpponentHealthSlider());
            abilitySelectionUiManagerREF.PopulateAbilityData();
        }
    }
}
