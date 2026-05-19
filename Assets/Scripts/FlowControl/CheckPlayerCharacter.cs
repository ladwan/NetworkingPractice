using ForeverFight.Ui.CharacterSelection;
using UnityEngine;

namespace ForeverFight.FlowControl
{
    public class CheckPlayerCharacter : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad = "";

        public void EnsurePlayersHaveSelectedCharacters()
        {
            var myChar = CharacterSelect.Instance.CurrentlySelectedPanel;
            var otherChar = CharacterSelect.Instance.OtherPlayerCurrentPanel;

            if ( myChar == null || otherChar == null)
            {
                HandlePlayerDisconnection.ReturnToLobby();
                return;
            }

            SceneLoader.Instance.LoadScene(sceneToLoad);
        }
    }
}
