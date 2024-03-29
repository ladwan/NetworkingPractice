﻿using System;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Ui;
using ForverFight.Networking;
using ForverFight.FlowControl;
using ForverFight.HelperScripts;

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


    public Dictionary<Vector2, GridPoint> GridDictionary => gridDictionary;

    public static FloorGrid Instance { get => instance; set => instance = value; }

    public DragMovement DragMoverREF { get => dragMoverREF; set => dragMoverREF = value; }

    public Action<int> OnMoveConfirmed { get => onMoveConfirmed; set => onMoveConfirmed = value; }

    public GameObject Player1Spawn { get => player1Spawn; set => player1Spawn = value; }

    public GameObject Player2Spawn { get => player2Spawn; set => player2Spawn = value; }

    public ProceduralGridManipulation ProceduralGridManipulationREF { get => proceduralGridManipulationREF; set => proceduralGridManipulationREF = value; }


    private static FloorGrid instance = null;
    private Vector2 currentLocation = new Vector2(0, 0);
    private GridPoint dragMoverGridPointREF = null; //this should return the gridPoint that the drag mover is on
    private Transform opponentSpawn = null;


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


    void Start()
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
                break;
            case 2:
                PreparePlayers(player2Spawn);
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

    public void TryHighlighting(GridPoint nextDestinationsGridPoint)
    {
        if (nextDestinationsGridPoint != GridPointOccupiedByOpponent())
        {
            if (IsGridPointInList(nextDestinationsGridPoint))
            {
                dragMoverGridPointREF = nextDestinationsGridPoint;
                currentLocation = nextDestinationsGridPoint.UniqueTag;
                AddGridPointToList(nextDestinationsGridPoint);
                nextDestinationsGridPoint.DragMovementREF.UpdateDragMoverPosition(nextDestinationsGridPoint.UniqueTag);
            }
            else
            {
                if (ActionPointsManager.Instance.CurrentApReferenceListsREF.UpdateValueOfRelevantAp(0) > 0)
                {
                    dragMoverGridPointREF = nextDestinationsGridPoint;
                    currentLocation = nextDestinationsGridPoint.UniqueTag;
                    AddGridPointToList(nextDestinationsGridPoint);
                    ActionPointsManager.Instance.BlinkCurrentListReference();
                    nextDestinationsGridPoint.DragMovementREF.UpdateDragMoverPosition(nextDestinationsGridPoint.UniqueTag);
                }
            }
        }
        else
        {
            Debug.Log("That position is already taken! Please try to move elsewhere.");
        }
    }

    //Use the network to call this on the Player who is being pulled
    public void OverridePlayerPosition(GridPoint updatedPosGp)
    {
        currentLocation = updatedPosGp.UniqueTag;
        updatedPosGp.DragMovementREF.UpdateDragMoverPosition(updatedPosGp.UniqueTag);
        ClientSend.UpdatePlayerCurrentPostition((int)currentLocation.x, (int)currentLocation.y);
        var currentLocationVector3 = new Vector3(currentLocation.x, 0, currentLocation.y);
        var moveLocalPlayer = ClientInfo.playerNumber == 1 ? player1Spawn.transform.position = currentLocationVector3 : player2Spawn.transform.position = currentLocationVector3;
    }

    public void ConfirmMove()
    {
        ClientSend.UpdatePlayerCurrentPostition((int)currentLocation.x, (int)currentLocation.y);
        var currentLocationVector3 = new Vector3(currentLocation.x, 0, currentLocation.y);

        var moveLocalPlayer = ClientInfo.playerNumber == 1 ? player1Spawn.transform.position = currentLocationVector3 : player2Spawn.transform.position = currentLocationVector3;

        BroadcastHoveredOverGridPointsCount();
        EmptyGridPointList();

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
        if (hoveredOverGridPoints.Count != 0)
        {
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
        }
        else
        {
            add = true;
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
}