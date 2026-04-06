using System;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;
using System.Collections;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.GameMechanics.Movement
{
    public class FloorGrid : MonoBehaviour
    {
        [SerializeField]
        private GameObject gridPoint;
        [SerializeField]
        private Dictionary<Vector2, GridPoint> gridDictionary = new Dictionary<Vector2, GridPoint>();
        [SerializeField]
        private List<GridPoint> hoveredOverGridPoints = new List<GridPoint>();
        [SerializeField]
        private GameObject playerREF = null;
        [SerializeField]
        private GameObject player1Spawn;
        [SerializeField]
        private GameObject player2Spawn;
        [SerializeField]
        private DragMovement dragMoverREF = null;
        [SerializeField]
        private GameObject uiHolderREF = null;
        [SerializeField]
        private DisplaySelectedChar displaySelectedCharREF = null;
        [SerializeField]
        public int dieValue = 0;
        [SerializeField]
        private Action<int> onMoveConfirmed = null;
        [SerializeField]
        private Action<int> hoveredOverGridPointsUpdated = null;
        [SerializeField]
        private Action onMoveCompleted = null;
        [SerializeField]
        private ProceduralGridManipulation proceduralGridManipulationREF = null;
        [SerializeField]
        private List<List<Vector3>> networkedSegmententedMovementData = new List<List<Vector3>>();
        [SerializeField]
        private List<Quaternion> networkedSegmententedRotationData = new List<Quaternion>();
        [SerializeField]
        private float movementSpeedMultiplier = 1;


        // ~~~ ~~~ DEBUG TEST
        [SerializeField]
        private CurveMoveSpeed curveMoveSpeedREF = null;


        [SerializeField]
        private Transform transformToLerp = null;


        public Dictionary<Vector2, GridPoint> GridDictionary => gridDictionary;

        public static FloorGrid Instance { get => instance; set => instance = value; }

        public List<GridPoint> HoveredOverGridPoints { get => hoveredOverGridPoints; set => hoveredOverGridPoints = value; }

        public DragMovement DragMoverREF { get => dragMoverREF; set => dragMoverREF = value; }

        public Action<int> OnMoveConfirmed { get => onMoveConfirmed; set => onMoveConfirmed = value; }

        public Action<int> HoveredOverGridPointsUpdated { get => hoveredOverGridPointsUpdated; set => hoveredOverGridPointsUpdated = value; }

        public Action OnMoveCompleted { get => onMoveCompleted; set => onMoveCompleted = value; }

        public GameObject Player1Spawn { get => player1Spawn; set => player1Spawn = value; }

        public GameObject Player2Spawn { get => player2Spawn; set => player2Spawn = value; }

        public ProceduralGridManipulation ProceduralGridManipulationREF { get => proceduralGridManipulationREF; set => proceduralGridManipulationREF = value; }

        public GameObject LocalPlayerSpawn { get => localPlayerSpawn; set => localPlayerSpawn = value; }

        public List<List<Vector3>> NetworkedSegmententedMovementData { get => networkedSegmententedMovementData; set => networkedSegmententedMovementData = value; }

        public float MovementSpeedMultiplier { get => movementSpeedMultiplier; set => movementSpeedMultiplier = value; }


        private static FloorGrid instance = null;
        private Vector2 currentLocation = new Vector2(0, 0);
        private GridPoint dragMoverGridPointREF = null; //this should return the gridPoint that the drag mover is on
        private Transform opponentSpawn = null;
        private GameObject localPlayerSpawn = null;
        private Coroutine lerpMovementSub = null;
        private int networkedMovementDataSent = 0; //Use this to know if both movement and rotation data has been send over, if its 2 you have both


        private class MovementInstanceInfo
        {
            public Vector3 startPos;
            public Vector3 endPos;
            public Character currentCharacter;
            public GameObject spawnToBeMoved;
            public float time;
        }

        private MovementInstanceInfo currentMovementInstance = null;


        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log("More Than 1 Floor grid detected, Destroying self...");
                Destroy(instance);
            }
        }

        protected void Start()
        {
            for (int p = 0; p < 10; p++) //Create grid columns
            {
                for (int i = 0; i < 10; i++) //Create grid rows
                {
                    GameObject instantiatedGridPoint = Instantiate(
                        gridPoint,
                        new Vector3(
                            this.transform.position.x + i,
                            this.transform.position.y,
                            this.transform.position.z + p
                        ),
                        Quaternion.identity,
                        this.gameObject.transform
                    );

                    Vector2Int key = new Vector2Int(
                        Mathf.RoundToInt(this.transform.position.x + i),
                        Mathf.RoundToInt(this.transform.position.z + p)
                    );

                    gridDictionary.Add(key, instantiatedGridPoint.GetComponent<GridPoint>());
                }
            }

            foreach (GridPoint gp in gridDictionary.Values)
            {
                gp.FindConnections(gridDictionary);
            }

            //Assigning a spawn point to the player based on their ClientInfo.playerNumber
            switch (ClientInfo.playerNumber)
            {
                case 1:
                    PreparePlayers(player1Spawn);
                    localPlayerSpawn = player1Spawn;
                    break;
                case 2:
                    PreparePlayers(player2Spawn);
                    localPlayerSpawn = player2Spawn;
                    playerREF.transform.position = player2Spawn.transform.position;
                    playerREF.transform.rotation = player2Spawn.transform.rotation;
                    dragMoverREF.UpdateDragMoverPosition(Vector3ToVector2.ConvertToVector2(player2Spawn.transform.position));
                    break;
                default:
                    Debug.Log("Nope");
                    break;
            }
        }

        public void EmptyGridPointList() //Removes Highlighted Sq's
        {
            for (int i = 0; i < hoveredOverGridPoints.Count; i++)
            {
                hoveredOverGridPoints[i].ShowHighlight(false);
            }
            hoveredOverGridPoints.Clear();
            hoveredOverGridPointsUpdated?.Invoke(0);
            if (dragMoverGridPointREF)
            {
                dragMoverGridPointREF.DisplayConnections(false);
                dragMoverGridPointREF.DragMovementREF.UpdateDragMoverPosition(currentLocation);
            }

            var player1Vector2 = SpawnPointTransformToVector2(player1Spawn);
            var player2Vector2 = SpawnPointTransformToVector2(player2Spawn);
            var updatedCurrentLocation = ClientInfo.playerNumber == 1 ? currentLocation = player1Vector2 : currentLocation = player2Vector2;
        }

        public void AddGridPointToList(GridPoint gp)
        {
            gp.ShowHighlight(true);

            if (AddGridPointToListBool(gp))
            {
                hoveredOverGridPoints.Add(gp);
                hoveredOverGridPointsUpdated?.Invoke(hoveredOverGridPoints.Count);
            }
        }

        public void UpdateOpponentPosition(Vector2 value)
        {
            var newPos = new Vector3(value.x, 0, value.y);

            var moveOpponent = ClientInfo.playerNumber == 1 ? player2Spawn.transform.position = newPos : player1Spawn.transform.position = newPos;
        }

        public void TryHighlighting(GridPoint nextDestinationsGridPoint, bool moveWithoutCostingAp)
        {
            if (nextDestinationsGridPoint == GridPointOccupiedByOpponent())
            {
                Debug.Log("That position is already taken! Please try to move elsewhere.");
                return;
            }

            if (IsGridPointInList(nextDestinationsGridPoint))
            {
                UpdateMovementVariablesAndGpList(nextDestinationsGridPoint);
                nextDestinationsGridPoint.DragMovementREF.UpdateDragMoverPosition(nextDestinationsGridPoint.UniqueTag);
            }
            else
            {
                if (ActionPointsManager.Instance.CurrentApReferenceListsREF.UpdateValueOfRelevantAp(0) > 0 && !moveWithoutCostingAp)
                {
                    UpdateMovementVariablesAndGpList(nextDestinationsGridPoint);
                    ActionPointsManager.Instance.BlinkCurrentListReference();
                    nextDestinationsGridPoint.DragMovementREF.UpdateDragMoverPosition(nextDestinationsGridPoint.UniqueTag);
                }

                if (moveWithoutCostingAp)
                {
                    UpdateMovementVariablesAndGpList(nextDestinationsGridPoint);
                    nextDestinationsGridPoint.DragMovementREF.UpdateDragMoverPosition(nextDestinationsGridPoint.UniqueTag);
                }
            }
        }

        //Use the network to call this on the Player who is being pulled
        public void OverridePlayerPosition(GridPoint updatedPosGp)
        {
            currentLocation = updatedPosGp.UniqueTag;
            updatedPosGp.DragMovementREF.UpdateDragMoverPosition(updatedPosGp.UniqueTag);
            ClientSend.UpdatePlayerCurrentPostition((int)currentLocation.x, (int)currentLocation.y, 0); //TODO: FIX THIS
            var currentLocationVector3 = new Vector3(currentLocation.x, 0, currentLocation.y);
            var moveLocalPlayer = ClientInfo.playerNumber == 1 ? player1Spawn.transform.position = currentLocationVector3 : player2Spawn.transform.position = currentLocationVector3;
        }

        public void ConfirmMove()
        {
            MovementSetup();

            BroadcastHoveredOverGridPointsCount();

            ActionPointsManager.Instance.MoveWasConfirmed(ActionPointsManager.Instance.CurrentApReferenceListsREF);
        }


        public void AddStartingGpHighlight()
        {
            if (gridDictionary.TryGetValue(currentLocation, out GridPoint startingSq))
            {
                AddGridPointToList(startingSq);
            }
            else
            {
                Debug.Log("Starting sq not found !");
            }
        }

        public void BroadcastHoveredOverGridPointsCount()
        {
            onMoveConfirmed?.Invoke(hoveredOverGridPoints.Count);
        }


        private bool AddGridPointToListBool(GridPoint gp) // Determines if a gp is already in the hoveredOverGridPoints list, if not just add it to the list, if so update list and remove all gps that came after it.
        {
            bool add = false;

            if (hoveredOverGridPoints.Count == 0)
            {
                add = true;
                return add;
            }

            for (int i = 0; i < hoveredOverGridPoints.Count; i++)
            {
                if (gp != hoveredOverGridPoints[i])
                {
                    add = true;
                }
                else
                {
                    if (i + 1 < hoveredOverGridPoints.Count)
                    {
                        for (int removeMe = i + 1; removeMe != hoveredOverGridPoints.Count;)
                        {
                            hoveredOverGridPoints[removeMe].ShowHighlight(false);
                            hoveredOverGridPoints.RemoveAt(removeMe);
                            ActionPointsManager.Instance.UpdateAP(ActionPointsManager.Instance.CurrentApReferenceListsREF, 1);
                            ActionPointsManager.Instance.UpdateBlinkingAP(ActionPointsManager.Instance.CurrentApReferenceListsREF);
                        }
                    }
                    add = false;
                    hoveredOverGridPointsUpdated?.Invoke(hoveredOverGridPoints.Count);
                    break;
                }
            }

            return add;
        }

        private bool IsGridPointInList(GridPoint gp)
        {
            if (AddGridPointToListBool(gp) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Vector2 SpawnPointTransformToVector2(GameObject spawnPoint)
        {
            Vector2 newSpawnPoint = Vector3ToVector2.ConvertToVector2(spawnPoint.transform.position);
            return newSpawnPoint;
        }

        private void PreparePlayers(GameObject spawnPoint)
        {
            currentLocation = SpawnPointTransformToVector2(spawnPoint);
            displaySelectedCharREF.SpawnPlayer(spawnPoint.transform);
            opponentSpawn = ClientInfo.playerNumber == 1 ? player2Spawn.transform : player1Spawn.transform;
        }

        private GridPoint GridPointOccupiedByOpponent()
        {
            if (ClientInfo.playerNumber == 1)
            {
                if (gridDictionary.TryGetValue(Vector3ToVector2.ConvertToVector2(opponentSpawn.position), out GridPoint gridPointOccupiedByOpponent))
                {
                    return gridPointOccupiedByOpponent;
                }

            }
            else if (ClientInfo.playerNumber == 2)
            {
                if (gridDictionary.TryGetValue(Vector3ToVector2.ConvertToVector2(opponentSpawn.position), out GridPoint gridPointOccupiedByOpponent))
                {
                    return gridPointOccupiedByOpponent;
                }
            }

            Debug.Log("ClientInfo.PlayerNumber returned an abnormal value, please check code");
            return null;
        }

        private void UpdateMovementVariablesAndGpList(GridPoint nextDestinationsGridPoint)
        {
            dragMoverGridPointREF = nextDestinationsGridPoint;
            currentLocation = nextDestinationsGridPoint.UniqueTag;
            AddGridPointToList(nextDestinationsGridPoint);
        }

        private void MovementSetup()
        {
            ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating();

            var calculatedMovementData = MovementPreWork();

            if (calculatedMovementData.Item2.Count > 0)
            {
                PrepareMovementDataToBeSentOverNetwork(calculatedMovementData.Item1, true);
                PrepareRotationDataToBeSentOverNetwork(calculatedMovementData.Item2);
            }
            else
            {
                PrepareMovementDataToBeSentOverNetwork(calculatedMovementData.Item1, false);
            }

            if (lerpMovementSub == null)
            {
                lerpMovementSub = StartCoroutine(LerpMovement(calculatedMovementData, localPlayerSpawn));
            }
            else
            {
                Debug.LogWarning("~~ Attempted to start LerpMove, but it is already running !");
            }
        }

        private IEnumerator LerpMovement(Tuple<List<List<Vector3>>, List<Quaternion>> calculatedMovementData, GameObject playerSpawnToBeMoved)
        {
            var character = localPlayerSpawn == playerSpawnToBeMoved ? LocalStoredNetworkData.GetLocalCharacter() : LocalStoredNetworkData.GetOpponentCharacter();

            foreach (var movementSegment in calculatedMovementData.Item1)
            {
                if (curveMoveSpeedREF.CurrentlyEvaluating == true)
                {
                    yield return new WaitUntil(() => curveMoveSpeedREF.CurrentlyEvaluating == false);
                }
                if (movementSegment.Count > 1)
                {
                    curveMoveSpeedREF.DetermineCurveFromMovementSegment(character, movementSegment);
                }

                for (int i = 0; i < movementSegment.Count; i++)
                {
                    if (i + 1 >= movementSegment.Count)
                    {
                        continue;
                    }

                    Vector3 pos1 = playerSpawnToBeMoved.transform.position;

                    var movementInstance = new MovementInstanceInfo()
                    {
                        startPos = playerSpawnToBeMoved.transform.position,
                        endPos = movementSegment[i + 1],
                        currentCharacter = character,
                        spawnToBeMoved = playerSpawnToBeMoved,
                        time = 0.0f
                    };

                    currentMovementInstance = movementInstance;


                    yield return new WaitUntil(() => currentMovementInstance == null);
                }

                //movementSpeedMultiplier = 1;


                //Were there rotations in the over-arching instance of movement?
                //If not, move on to the next iteration of the foreach loop
                if (calculatedMovementData.Item2.Count == 0)
                {
                    continue;
                }

                //If you made it here, you have rotation data saved from the movement pre-work
                //Slerp this rotation value then remove it from the list
                var time = 0.0f;
                var rotations = calculatedMovementData.Item2;
                while (playerSpawnToBeMoved.transform.rotation.eulerAngles != rotations[0].eulerAngles)
                {
                    //Debug.Log($"Euler Angle Player: {localPlayerSpawn.transform.rotation.eulerAngles}");
                    //Debug.Log($"Euler Angle Target: {targetRotation.eulerAngles}");
                    time += Time.deltaTime;
                    playerSpawnToBeMoved.transform.rotation = Quaternion.Slerp(playerSpawnToBeMoved.transform.rotation, rotations[0], time * 10f);
                    yield return new WaitForSecondsRealtime(0.01f);
                }

                rotations.RemoveAt(0);
            }

            EmptyGridPointList();
            lerpMovementSub = null;
            networkedSegmententedMovementData.Clear();
            networkedSegmententedRotationData.Clear();
            calculatedMovementData = null;
            onMoveCompleted?.Invoke();

            if (localPlayerSpawn != playerSpawnToBeMoved)
            {
                yield break;
            }

            ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating();
        }

        //This method is used to generate the objects that will be needed for the SegmentMovementInstance recursive method
        private Tuple<List<List<Vector3>>, List<Quaternion>> MovementPreWork()
        {
            List<List<Vector3>> tempParentPosList = new List<List<Vector3>>();
            List<Quaternion> tempParentRotList = new List<Quaternion>();
            List<Vector3> tempPosList = new List<Vector3>();

            //We need an object to represent how our player will move across these gridpoints.
            //Its important to remember this logic is calculating its way through a full movement
            //It will basically do everything that needs to happen for movement to take place, then return the results
            //The below method will need to move this object around to make accurate calculations, this is why we cant just use the player itself
            //Because then it would look like the player had moved twice
            GameObject tempObj = new GameObject();
            tempObj.transform.position = localPlayerSpawn.transform.position;
            tempObj.transform.rotation = localPlayerSpawn.transform.rotation;


            //Return the segmented, calculated data that will be used to move the player
            return SegmentMovementInstance(tempParentPosList, tempParentRotList, tempPosList, tempObj, 0);
        }


        //The issue this method aims to solve is that we do not translate while we rotate
        //This seemingly small statement means that we cannot use our "bespoke movement curve" system to drive the movement
        //The curves do not expect you to stop moving in the middle of their evaluation
        //The curves lack any context of the movement instance and this will lead to odd behavior,
        //To remedy this, the below method and its dependencies were created.

        //The idea is to separate the larger movement instance into smaller, segmented, movement instances. Split by rotations
        //Once the method detects an instance of rotation, it will stop adding GP's to the current list, and make a new list
        //While this is happening it will also save out the quaternion value of that instance of rotation.

        //When all is said and done, we will end with a list of List<Vector3> and a list of quaternions
        //The list of lists of vector 3s stores our segmented movements, all the grid points that were right in front of one another, without a rotation
        //The quaternion list stores all the instances of rotation that took place

        private Tuple<List<List<Vector3>>, List<Quaternion>> SegmentMovementInstance(List<List<Vector3>> parentPosList, List<Quaternion> parentRotList, List<Vector3> tempList, GameObject ghost, int currentIndex)
        {
            tempList.Add(new Vector3(hoveredOverGridPoints[currentIndex].UniqueTag.x, 0.0f, hoveredOverGridPoints[currentIndex].UniqueTag.y));

            for (int i = currentIndex; i < hoveredOverGridPoints.Count; i++)
            {
                if (i + 1 >= hoveredOverGridPoints.Count)
                {
                    continue;
                }

                Vector3 pos1 = new Vector3(hoveredOverGridPoints[i].UniqueTag.x, 0.0f, hoveredOverGridPoints[i].UniqueTag.y);
                Vector3 pos2 = new Vector3(hoveredOverGridPoints[i + 1].UniqueTag.x, 0.0f, hoveredOverGridPoints[i + 1].UniqueTag.y);

                Vector3 directionToTarget = pos2 - ghost.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                if (ghost.transform.eulerAngles == targetRotation.eulerAngles)
                {
                    ghost.transform.position = pos2;
                    currentIndex++;
                    return SegmentMovementInstance(parentPosList, parentRotList, tempList, ghost, currentIndex);
                }

                ghost.transform.position = pos2;
                ghost.transform.rotation = targetRotation;

                currentIndex++;

                parentPosList.Add(tempList);
                parentRotList.Add(targetRotation);
                List<Vector3> newInstanceOfTempList = new List<Vector3>();
                newInstanceOfTempList.Add(pos1);
                return SegmentMovementInstance(parentPosList, parentRotList, newInstanceOfTempList, ghost, currentIndex);
            }

            parentPosList.Add(tempList);
            var myTuple = new Tuple<List<List<Vector3>>, List<Quaternion>>(parentPosList, parentRotList);
            return myTuple;
        }


        public void RemoveAllButFirstIndexOfHoveredOverGPs()
        {
            for (int i = 1; hoveredOverGridPoints.Count >= 2;)
            {
                hoveredOverGridPoints[i].ShowHighlight(false);
                hoveredOverGridPoints.RemoveAt(i);
            }

            hoveredOverGridPointsUpdated?.Invoke(hoveredOverGridPoints.Count);
        }


        public void ConstructVector3ListFromNetworkData(Vector3 vector3ToBeAdded, int count, bool hasRotations, bool completed)
        {
            if (networkedSegmententedMovementData.Count == 0)
            {
                MakeNewListForNetworkedSegmentedMovementData(vector3ToBeAdded);
                var tempLatestList = networkedSegmententedMovementData[networkedSegmententedMovementData.Count - 1];

                if (tempLatestList.Count == count && !completed)
                {
                    networkedSegmententedMovementData.Add(new List<Vector3>());
                }
                return;
            }

            var latestList = networkedSegmententedMovementData[networkedSegmententedMovementData.Count - 1];

            if (latestList.Count == count)
            {
                MakeNewListForNetworkedSegmentedMovementData(vector3ToBeAdded);
                return;
            }

            latestList.Add(vector3ToBeAdded);

            if (latestList.Count == count && !completed)
            {
                networkedSegmententedMovementData.Add(new List<Vector3>());
            }

            if (completed)
            {
                if (hasRotations)
                {
                    ConstructTupleFromNetworkData(true);
                    return;
                    //Do stuff
                }

                ConstructTupleFromNetworkData(false);
            }
        }

        public void ConstructQuaternionListFromNetworkData(Quaternion quaternionToBeAdded, int count)
        {
            networkedSegmententedRotationData.Add(quaternionToBeAdded);

            if (networkedSegmententedRotationData.Count == count)
            {
                ConstructTupleFromNetworkData(true);
                //Trigger bool to say list is done
                //IncrementValue
            }
        }

        private void MakeNewListForNetworkedSegmentedMovementData(Vector3 temp)
        {
            networkedSegmententedMovementData.Add(new List<Vector3>());
            networkedSegmententedMovementData[networkedSegmententedMovementData.Count - 1].Add(temp);
        }

        private void PrepareMovementDataToBeSentOverNetwork(List<List<Vector3>> segmentedMovements, bool waitForRotationData)
        {
            var lastListIndex = segmentedMovements[segmentedMovements.Count - 1];
            foreach (var list in segmentedMovements)
            {
                var lastGpIndex = list[list.Count - 1];
                foreach (var GP in list)
                {
                    //start deconstructing the vector 3s to be written and passed across the network
                    if (list == lastListIndex && GP == lastGpIndex)
                    {
                        ClientSend.SendSegmentedMovementData((int)GP.x, (int)GP.z, list.Count, waitForRotationData, true);
                        break;
                    }
                    ClientSend.SendSegmentedMovementData((int)GP.x, (int)GP.z, list.Count, waitForRotationData, false);
                }
            }
        }

        private void PrepareRotationDataToBeSentOverNetwork(List<Quaternion> segmentedRotations)
        {
            foreach (var rot in segmentedRotations)
            {
                ClientSend.SendSegmentedRotationData(rot.x, rot.y, rot.z, rot.w, segmentedRotations.Count);
            }
        }

        private void ConstructTupleFromNetworkData(bool waitOnRotations)
        {
            if (!waitOnRotations)
            {
                var myTuple = new Tuple<List<List<Vector3>>, List<Quaternion>>(networkedSegmententedMovementData, new List<Quaternion>());

                var remotePlayersSpawn = ClientInfo.playerNumber == 1 ? player2Spawn : player1Spawn;
                StartCoroutine(LerpMovement(myTuple, remotePlayersSpawn));
                return;
            }

            networkedMovementDataSent++;
            if (networkedMovementDataSent == 2)
            {
                var myTuple = new Tuple<List<List<Vector3>>, List<Quaternion>>(networkedSegmententedMovementData, networkedSegmententedRotationData);
                networkedMovementDataSent = 0;

                var remotePlayersSpawn = ClientInfo.playerNumber == 1 ? player2Spawn : player1Spawn;
                StartCoroutine(LerpMovement(myTuple, remotePlayersSpawn));
            }

        }

        private void Update()
        {
            if (currentMovementInstance != null)
            {
                movementSpeedMultiplier = .0066f;
                var moveSpeed = currentMovementInstance.currentCharacter.MoveSpeed;
                currentMovementInstance.time += (moveSpeed * movementSpeedMultiplier);
                currentMovementInstance.time = Mathf.Clamp01(currentMovementInstance.time);
                currentMovementInstance.time *= currentMovementInstance.currentCharacter.MoveSpeedHelper;
                currentMovementInstance.spawnToBeMoved.transform.position = Vector3.Lerp(currentMovementInstance.startPos, currentMovementInstance.endPos, currentMovementInstance.time);


                if (currentMovementInstance.time == 1)
                {
                    currentMovementInstance = null;
                }
            }

        }
    }
}