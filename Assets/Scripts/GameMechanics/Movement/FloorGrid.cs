using System;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;
using System.Collections;
using static ForeverFight.Interactable.Abilities.CharAbility;

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
        private ProceduralGridManipulation proceduralGridManipulationREF = null;




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

        public GameObject Player1Spawn { get => player1Spawn; set => player1Spawn = value; }

        public GameObject Player2Spawn { get => player2Spawn; set => player2Spawn = value; }

        public ProceduralGridManipulation ProceduralGridManipulationREF { get => proceduralGridManipulationREF; set => proceduralGridManipulationREF = value; }

        public GameObject LocalPlayerSpawn { get => localPlayerSpawn; set => localPlayerSpawn = value; }


        private static FloorGrid instance = null;
        private Vector2 currentLocation = new Vector2(0, 0);
        private GridPoint dragMoverGridPointREF = null; //this should return the gridPoint that the drag mover is on
        private Transform opponentSpawn = null;
        private GameObject localPlayerSpawn = null;
        private Coroutine lerpMovementSub = null;


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
                    GameObject instantiatedGridPoint = Instantiate(gridPoint, new Vector3(this.transform.position.x + i, this.transform.position.y, this.transform.position.z + p), new Quaternion(), this.gameObject.transform);
                    gridDictionary.Add(new Vector2(this.transform.position.x + i, this.transform.position.z + p), instantiatedGridPoint.GetComponent<GridPoint>());
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

            FormatNetworkedMovementData.movementComplete += StartMoveTest;
        }

        private void OnDestroy()
        {
            FormatNetworkedMovementData.movementComplete -= StartMoveTest;
        }

        public void EmptyGridPointList() //Removes Highlighted Sq's
        {
            for (int i = 0; i < hoveredOverGridPoints.Count; i++)
            {
                hoveredOverGridPoints[i].ShowHighlight(false);
            }
            hoveredOverGridPoints.Clear();
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
                    //ActionPointsManager.Instance.BlinkCurrentListReference();
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
            if (lerpMovementSub == null)
            {
                lerpMovementSub = StartCoroutine(LerpMovement());
            }
            else
            {
                Debug.LogWarning("~~ Attempted to start LerpMove, but it is already running !");
                return;
            }

            FormatMoveData();
            var currentLocationVector3 = new Vector3(currentLocation.x, 0, currentLocation.y);

            //var moveLocalPlayer = ClientInfo.playerNumber == 1 ? player1Spawn.transform.position = currentLocationVector3 : player2Spawn.transform.position = currentLocationVector3;

            BroadcastHoveredOverGridPointsCount();
            //EmptyGridPointList();

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


        //TODO: Pop this stuff out as a new script
        private IEnumerator LerpMovement()
        {
            var animatorREF = LocalStoredNetworkData.GetLocalCharacterAnimationReferences();
            var parameters = new CameraShakeParameters();

            ToggleTimerAndUi.Instance.TestMethod(animatorREF.CharacterAnimator, "Run", parameters);

            var importantMovementData = MovementPreWork();
            //curveMoveSpeedREF.TestDynamicMoveSpeed();


            foreach (var movementSegment in importantMovementData.Item1)
            {
                for (int i = 0; i < movementSegment.Count; i++)
                {
                    if (i + 1 >= movementSegment.Count)
                    {
                        continue;
                    }

                    var t = 0.0f;
                    Vector3 pos1 = localPlayerSpawn.transform.position;
                    while (localPlayerSpawn.transform.position != movementSegment[i + 1])
                    {
                        t += Time.deltaTime * LocalStoredNetworkData.GetLocalCharacter().MoveSpeed;
                        t = Mathf.Clamp01(t);
                        localPlayerSpawn.transform.position = Vector3.Lerp(pos1, movementSegment[i + 1], t);
                        yield return new WaitForSecondsRealtime(0.01f);
                    }
                }


                //You do? Run method to pop 0th index off Queue
                //Do rotation

                if (importantMovementData.Item2.Count == 0)
                {
                    continue;
                }

                var time = 0.0f;
                var rotations = importantMovementData.Item2;
                while (localPlayerSpawn.transform.rotation.eulerAngles != rotations[0].eulerAngles)
                {
                    //Debug.Log($"Euler Angle Player: {localPlayerSpawn.transform.rotation.eulerAngles}");
                    //Debug.Log($"Euler Angle Target: {targetRotation.eulerAngles}");
                    time += Time.deltaTime;
                    localPlayerSpawn.transform.rotation = Quaternion.Slerp(localPlayerSpawn.transform.rotation, rotations[0], time * 10f);
                    yield return new WaitForSecondsRealtime(0.01f);
                }

                rotations.RemoveAt(0);
            }





            //for (int i = 0; i < hoveredOverGridPoints.Count; i++)
            //{
            //    Debug.Log($"~~~ Value of i: {i}");
            //    if (i + 1 >= hoveredOverGridPoints.Count)
            //    {
            //        continue;
            //    }

            //    Vector3 pos1 = localPlayerSpawn.transform.position;
            //    Vector3 pos2 = new Vector3(hoveredOverGridPoints[i + 1].UniqueTag.x, 0.0f, hoveredOverGridPoints[i + 1].UniqueTag.y);

            //    Vector3 directionToTarget = pos2 - localPlayerSpawn.transform.position;
            //    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            //    //Rotate
            //    var time = 0.0f;


            //    //Pop this out as its own IEnum to rotate while you translate
            //    if (localPlayerSpawn.transform.rotation.eulerAngles != targetRotation.eulerAngles)
            //    {
            //        StartCoroutine(RotateWhileMoving(targetRotation));
            //    }
            //    /*                while (localPlayerSpawn.transform.rotation.eulerAngles != targetRotation.eulerAngles)
            //                    {
            //                        //Debug.Log($"Euler Angle Player: {localPlayerSpawn.transform.rotation.eulerAngles}");
            //                        //Debug.Log($"Euler Angle Target: {targetRotation.eulerAngles}");
            //                        time += Time.deltaTime;
            //                        localPlayerSpawn.transform.rotation = Quaternion.Slerp(localPlayerSpawn.transform.rotation, targetRotation, time * 10f);
            //                        yield return new WaitForSecondsRealtime(0.01f);
            //                    }*/

            //    //Translate
            //    var t = 0.0f;
            //    while (localPlayerSpawn.transform.position != pos2)
            //    {
            //        t += Time.deltaTime * LocalStoredNetworkData.GetLocalCharacter().MoveSpeed;
            //        t = Mathf.Clamp01(t);
            //        localPlayerSpawn.transform.position = Vector3.Lerp(pos1, pos2, t);
            //        yield return new WaitForSecondsRealtime(0.01f);
            //    }
            //}

            EmptyGridPointList();
            lerpMovementSub = null;
            Debug.Log($"~~~ Successful move: {localPlayerSpawn.transform.position}");
            ToggleTimerAndUi.Instance.TestMethod(animatorREF.CharacterAnimator, "Idle", parameters);
        }



        private Tuple<List<List<Vector3>>, List<Quaternion>> MovementPreWork()
        {
            List<List<Vector3>> tempParentPosList = new List<List<Vector3>>();
            List<Quaternion> tempParentRotList = new List<Quaternion>();
            List<Vector3> tempPosList = new List<Vector3>();

            GameObject tempObj = new GameObject();
            tempObj.transform.position = localPlayerSpawn.transform.position;
            tempObj.transform.rotation = localPlayerSpawn.transform.rotation;

            return TestMove(tempParentPosList, tempParentRotList, tempPosList, tempObj, 0);
        }


        //TODO:
        //Handle recusive returns
        //Keep track of Quaternions after rotation events
        //Return Parent list

        private Tuple<List<List<Vector3>>, List<Quaternion>> TestMove(List<List<Vector3>> parentPosList, List<Quaternion> parentRotList, List<Vector3> tempList, GameObject ghost, int currentIndex)
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
                    return TestMove(parentPosList, parentRotList, tempList, ghost, currentIndex);
                }
                else
                {
                    ghost.transform.position = pos2;
                    ghost.transform.rotation = targetRotation;

                    currentIndex++;

                    parentPosList.Add(tempList);
                    parentRotList.Add(targetRotation);
                    List<Vector3> newInstanceOfTempList = new List<Vector3>();
                    newInstanceOfTempList.Add(pos1);
                    return TestMove(parentPosList, parentRotList, newInstanceOfTempList, ghost, currentIndex);
                }
            }

            parentPosList.Add(tempList);
            var myTuple = new Tuple<List<List<Vector3>>, List<Quaternion>>(parentPosList, parentRotList);
            return myTuple;
        }












        private void StartMoveTest(List<Vector3> remotePlayersHoveredOverGPs)
        {
            StartCoroutine(LerpMovement(remotePlayersHoveredOverGPs));
        }

        private IEnumerator LerpMovement(List<Vector3> remotePlayersHoveredOverGPs)
        {
            for (int i = 0; i < remotePlayersHoveredOverGPs.Count; i++)
            {
                if (i + 1 >= remotePlayersHoveredOverGPs.Count)
                {
                    continue;
                }

                var remotePlayersSpawn = ClientInfo.playerNumber == 1 ? player2Spawn.transform : player1Spawn.transform;

                Vector3 pos1 = remotePlayersSpawn.position;
                Vector3 pos2 = remotePlayersHoveredOverGPs[i + 1];

                Vector3 directionToTarget = pos2 - remotePlayersSpawn.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                //Rotate
                var time = 0.0f;
                while (remotePlayersSpawn.rotation.eulerAngles != targetRotation.eulerAngles)
                {
                    //Debug.Log($"Euler Angle Player: {localPlayerSpawn.transform.rotation.eulerAngles}");
                    //Debug.Log($"Euler Angle Target: {targetRotation.eulerAngles}");
                    time += Time.deltaTime;
                    remotePlayersSpawn.rotation = Quaternion.Slerp(remotePlayersSpawn.rotation, targetRotation, time * 10f);
                    yield return new WaitForSecondsRealtime(0.01f);
                }

                //Translate
                var t = 0.0f;
                while (remotePlayersSpawn.position != pos2)
                {
                    t += Time.deltaTime * LocalStoredNetworkData.GetOpponentCharacter().MoveSpeed;
                    t = Mathf.Clamp01(t);
                    remotePlayersSpawn.position = Vector3.Lerp(pos1, pos2, t);
                    yield return new WaitForSecondsRealtime(0.01f);
                }
            }

            EmptyGridPointList();
            FormatNetworkedMovementData.RemotePlayersHoveredOverGPs.Clear();
        }

        private void FormatMoveData()
        {
            foreach (GridPoint gp in hoveredOverGridPoints)
            {
                ClientSend.UpdatePlayerCurrentPostition((int)gp.UniqueTag.x, (int)gp.UniqueTag.y, hoveredOverGridPoints.Count);
                //Debug.Log($"~~~ GP: {new Vector2((int)gp.UniqueTag.x, (int)gp.UniqueTag.y)}");
            }
        }

        public void RemoveAllButFirstIndexOfHoveredOverGPs()
        {
            for (int i = 1; hoveredOverGridPoints.Count >= 2;)
            {
                hoveredOverGridPoints[i].ShowHighlight(false);
                hoveredOverGridPoints.RemoveAt(i);
            }
        }
    }
}