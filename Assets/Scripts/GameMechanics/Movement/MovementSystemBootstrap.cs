using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using ForeverFight.HelperScripts;
using ForeverFight.Interactable.PlayerInputInteractions;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// The single scene component of the free movement system. Everything else is
    /// constructed and wired here in code: sibling managers, the guide line, per-spawn
    /// drag colliders and NavMesh obstacles, and a runtime NavMesh bake of the floor -
    /// so no editor baking, prefab wiring or committed NavMesh asset is needed.
    /// </summary>
    public class MovementSystemBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject player1Spawn = null;
        [SerializeField] private GameObject player2Spawn = null;
        [SerializeField] private GameObject playerREF = null;
        [SerializeField] private DisplaySelectedChar displaySelectedCharREF = null;
        // Center of the playable area. The old 10x10 grid spanned integer coords 0..9,
        // so its footprint is a 10x10 square centered here. The visual floor mesh is
        // larger than the playable area, which is why we bake a dedicated plane.
        [SerializeField] private Vector3 arenaCenter = new Vector3(4.5f, 0f, 4.5f);

        [SerializeField] private float dragColliderRadius = 0.5f;
        [SerializeField] private float dragColliderHeight = 2.0f;
        [SerializeField] private float obstacleCarveRadius = 0.5f;

        private NavMeshSurface navMeshSurface = null;


        private void Awake()
        {
            gameObject.AddComponent<MovementPlanner>();
            gameObject.AddComponent<MovementExecutor>();
            gameObject.AddComponent<ApDistanceBank>();
            gameObject.AddComponent<MovementNetworkBridge>();
            gameObject.AddComponent<ForcedDisplacement>();

            var spawnManager = gameObject.AddComponent<PlayerSpawnManager>();
            spawnManager.Initialize(player1Spawn, player2Spawn, playerREF, displaySelectedCharREF);

            var guideLineObject = new GameObject("Guide Line");
            guideLineObject.transform.SetParent(transform, false);
            guideLineObject.AddComponent<MovementGuideLine>();

            SetupFloor();
            SetupSpawnObject(player1Spawn);
            SetupSpawnObject(player2Spawn);
        }

        private void Start()
        {
            // ClientInfo.playerNumber is set during connection, before the combat scene loads.
            PlayerSpawnManager.Instance.SetupSpawns();
        }


        private void SetupFloor()
        {
            // A Unity Plane primitive is exactly 10x10 units at scale 1 - the same
            // footprint as the old grid. Its collider handles drag ground raycasts;
            // its (temporarily enabled) renderer feeds the runtime NavMesh bake.
            var walkableArena = GameObject.CreatePrimitive(PrimitiveType.Plane);
            walkableArena.name = "Walkable Arena";
            walkableArena.transform.SetParent(transform, false);
            walkableArena.transform.position = arenaCenter;

            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer >= 0)
            {
                walkableArena.layer = groundLayer;
            }
            else
            {
                Debug.LogError("Layer 'Ground' is not defined in TagManager - drag ground sampling will fail");
            }

            navMeshSurface = walkableArena.AddComponent<NavMeshSurface>();
            navMeshSurface.collectObjects = CollectObjects.Children;
            navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            navMeshSurface.BuildNavMesh();

            // The bake is synchronous; hide the helper plane but keep its collider
            // so ground raycasts still resolve against the exact playable area.
            walkableArena.GetComponent<MeshRenderer>().enabled = false;
        }

        private void SetupSpawnObject(GameObject spawn)
        {
            if (spawn == null)
            {
                Debug.LogError("MovementSystemBootstrap: a player spawn reference is not assigned");
                return;
            }

            int dragLayer = LayerMask.NameToLayer("Drag Movement");
            if (dragLayer >= 0)
            {
                spawn.layer = dragLayer;
            }

            if (spawn.GetComponent<CapsuleCollider>() == null)
            {
                var capsule = spawn.AddComponent<CapsuleCollider>();
                capsule.radius = dragColliderRadius;
                capsule.height = dragColliderHeight;
                capsule.center = new Vector3(0f, dragColliderHeight * 0.5f, 0f);
            }

            var handle = spawn.GetComponent<MoveHandleInteractable>();
            if (handle == null)
            {
                handle = spawn.AddComponent<MoveHandleInteractable>();
            }
            handle.OwningSpawn = spawn;

            if (spawn.GetComponent<NavMeshObstacle>() == null)
            {
                var obstacle = spawn.AddComponent<NavMeshObstacle>();
                obstacle.shape = NavMeshObstacleShape.Capsule;
                obstacle.radius = obstacleCarveRadius;
                obstacle.height = dragColliderHeight;
                obstacle.carving = true;
                obstacle.carveOnlyStationary = true;
                obstacle.enabled = false; // PlayerSpawnManager enables the opponent's.
            }
        }
    }
}
