using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable;
using ForeverFight.Interactable.Characters;

namespace FlowControl
{
    public class SpawnOpponentScript : MonoBehaviour
    {
        [SerializeField]
        private Transform player1Spawn = null;
        [SerializeField]
        private Transform player2Spawn = null;
        [SerializeField]
        private DisplaySelectedChar displaySelectedCharREF = null;

        public Character SpawnOpponent(Character.Identity identity)
        {
            Transform spawnPoint = null;

            //Inverse, to put opponent at the spawn the local player isnt at !
            if (ClientInfo.playerNumber == 1)
            {
                spawnPoint = player2Spawn;
            }
            if (ClientInfo.playerNumber == 2)
            {
                spawnPoint = player1Spawn;
            }

            if (displaySelectedCharREF.myCharacterDictionary.TryGetValue(identity, out Character value))
            {
                return Instantiate(value, spawnPoint);
            }

            return null;
        }
    }
}
