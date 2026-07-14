using UnityEngine;
using UnityEngine.AI;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Re-homes the non-movement half of the old FloorGrid: spawn assignment from
    /// ClientInfo.playerNumber, local/opponent spawn lookup, and world-space position
    /// sync entry points used by the network layer and forced moves. Also enables the
    /// opponent's carving NavMeshObstacle so planned paths bend around them.
    /// </summary>
    public class PlayerSpawnManager : MonoBehaviour
    {
        private GameObject player1Spawn = null;
        private GameObject player2Spawn = null;
        private GameObject playerREF = null;
        private DisplaySelectedChar displaySelectedCharREF = null;
        private GameObject localPlayerSpawn = null;
        private GameObject opponentSpawn = null;

        public static PlayerSpawnManager Instance { get; private set; }

        public GameObject LocalPlayerSpawn => localPlayerSpawn;

        public GameObject OpponentSpawn => opponentSpawn;

        public GameObject Player1Spawn => player1Spawn;

        public GameObject Player2Spawn => player2Spawn;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("More than 1 PlayerSpawnManager detected, destroying self...");
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Initialize(GameObject p1Spawn, GameObject p2Spawn, GameObject playerReference, DisplaySelectedChar displaySelectedChar)
        {
            player1Spawn = p1Spawn;
            player2Spawn = p2Spawn;
            playerREF = playerReference;
            displaySelectedCharREF = displaySelectedChar;
        }

        /// <summary>Runs the old FloorGrid.Start spawn switch. Call after ClientInfo.playerNumber is set.</summary>
        public void SetupSpawns()
        {
            switch (ClientInfo.playerNumber)
            {
                case 1:
                    PreparePlayers(player1Spawn);
                    localPlayerSpawn = player1Spawn;
                    opponentSpawn = player2Spawn;
                    break;
                case 2:
                    PreparePlayers(player2Spawn);
                    localPlayerSpawn = player2Spawn;
                    opponentSpawn = player1Spawn;
                    if (playerREF != null)
                    {
                        playerREF.transform.position = player2Spawn.transform.position;
                        playerREF.transform.rotation = player2Spawn.transform.rotation;
                    }
                    break;
                default:
                    Debug.Log("ClientInfo.playerNumber returned an abnormal value, spawns not assigned");
                    return;
            }

            ToggleObstacle(localPlayerSpawn, false);
            ToggleObstacle(opponentSpawn, true);
        }

        public void UpdateOpponentPosition(Vector3 position)
        {
            if (opponentSpawn != null)
            {
                opponentSpawn.transform.position = position;
            }
        }

        public void OverrideLocalPlayerPosition(Vector3 position)
        {
            if (localPlayerSpawn != null)
            {
                localPlayerSpawn.transform.position = position;
            }
        }


        private void PreparePlayers(GameObject spawnPoint)
        {
            if (displaySelectedCharREF != null)
            {
                displaySelectedCharREF.SpawnPlayer(spawnPoint.transform);
            }
        }

        private static void ToggleObstacle(GameObject spawn, bool enabled)
        {
            if (spawn != null && spawn.TryGetComponent<NavMeshObstacle>(out var obstacle))
            {
                obstacle.enabled = enabled;
            }
        }
    }
}
