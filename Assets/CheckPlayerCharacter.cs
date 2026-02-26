using ForeverFight.FlowControl;
using ForeverFight.Ui.CharacterSelection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerCharacter : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private SceneLoader sceneLoaderREF = null;

    public void EnsurePlayersHaveSelectedCharacters()
    {
        var myChar = CharacterSelect.Instance.CurrentlySelectedPanel;
        var otherChar = CharacterSelect.Instance.OtherPlayerCurrentPanel;

        if ( myChar == null || otherChar == null)
        {
            HandlePlayerDisconnection.ReturnToLobby();
            return;
        }

        sceneLoaderREF.LoadScene(sceneToLoad);
    }
}
