using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.HelperScripts;
using ForverFight.FlowControl;

public class FloorGrid : MonoBehaviour
{
    [SerializeField]
    private GameObject gridPoint;
    [SerializeField]
    private Dictionary<Vector2, GridPoint> gridDictionary = new Dictionary<Vector2, GridPoint>();
    [SerializeField]
    private List<GridPoint> hoveredOverGridPoints = new List<GridPoint>();
    [SerializeField]
    private GameObject player1Spawn;
    [SerializeField]
    private GameObject player1Camera;
    [SerializeField]
    private GameObject player2Spawn;
    [SerializeField]
    private GameObject player2Camera;
    [SerializeField]
    private DisplaySelectedChar displaySelectedCharREF = null;
    [SerializeField]
    public int dieValue = 0;

    public static FloorGrid instance = null;

    private Vector2 currentLocation = new Vector2(0, 0);
    private bool isPlayer1 = false;


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


        //Assigning a spawn point to the player based on their ClientInfo.playerNumber
        if (ClientInfo.playerNumber == 1)
        {
            currentLocation = new Vector2(player1Spawn.transform.position.x, player1Spawn.transform.position.z);
            isPlayer1 = true;
        }
        else if (ClientInfo.playerNumber == 2)
        {
            currentLocation = new Vector2(player2Spawn.transform.position.x, player2Spawn.transform.position.z);
        }

        if (ClientInfo.playerNumber == 0 || ClientInfo.playerNumber > 2)
        {
            currentLocation = new Vector2(player2Spawn.transform.position.x, player2Spawn.transform.position.z);
            Debug.Log("Client player number was abnormal, please look into code. Defaulting to spawn point 2");
            isPlayer1 = false;
        }

        if (isPlayer1)
        {
            displaySelectedCharREF.SpawnPlayer(player1Spawn.transform);
            player1Camera.SetActive(true);
        }
        else
        {
            displaySelectedCharREF.SpawnPlayer(player2Spawn.transform);
            player2Camera.SetActive(true);
        }
    }

    public void EmptyGridPointList() //Removes Highlighted Sq's
    {
        for (int i = 0; i < hoveredOverGridPoints.Count; i++)
        {
            hoveredOverGridPoints[i].ShowHighlight(false);
        }
        hoveredOverGridPoints.Clear();
        DistributedDieValue.SetDieRollValue(DistributedDieValue.unchangingDieRollValue);
        dieValue = DistributedDieValue.unchangingDieRollValue;

        var player1Vector2 = new Vector2(player1Spawn.transform.position.x, player1Spawn.transform.position.z);
        var player2Vector2 = new Vector2(player2Spawn.transform.position.x, player2Spawn.transform.position.z);
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

    #region TryHighlighting
    public void TryHighlighting(int movementValue)
    {
        dieValue = DistributedDieValue.distributedDieRollValue;

        switch (movementValue)
        {
            case 1:

                Vector2 moveUp = new Vector2(0, 0);
                if (ClientInfo.playerNumber == 1)
                {
                    moveUp = new Vector2(1, 0);
                }
                else if (ClientInfo.playerNumber == 2)
                {
                    moveUp = new Vector2(-1, 0);
                }

                var upDestination = currentLocation + moveUp;
                Debug.Log(upDestination);
                if (gridDictionary.TryGetValue(upDestination, out GridPoint upGridPoint))
                {
                    if (upGridPoint != GridPointOccupiedByOpponent())
                    {
                        if (IsGridPointInList(upGridPoint))
                        {
                            upGridPoint = gridDictionary[upDestination];
                            currentLocation = upDestination;
                            AddGridPointToList(upGridPoint);
                        }
                        else
                        {
                            if (dieValue > 0)
                            {
                                upGridPoint = gridDictionary[upDestination];
                                currentLocation = upDestination;
                                AddGridPointToList(upGridPoint);
                                dieValue--;
                                DistributedDieValue.SetDieRollValue(dieValue);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("That position is already taken! Please try to move elsewhere.");
                    }
                }

                break;

            case 2:

                Vector2 moveDown = new Vector2(0, 0);
                if (ClientInfo.playerNumber == 1)
                {
                    moveDown = new Vector2(-1, 0);
                }
                else if (ClientInfo.playerNumber == 2)
                {
                    moveDown = new Vector2(1, 0);
                }

                var downDestination = currentLocation + moveDown;

                if (gridDictionary.TryGetValue(downDestination, out GridPoint downGridPoint))
                {
                    if (downGridPoint != GridPointOccupiedByOpponent())
                    {
                        if (IsGridPointInList(downGridPoint))
                        {
                            downGridPoint = gridDictionary[downDestination];
                            currentLocation = downDestination;
                            AddGridPointToList(downGridPoint);
                        }
                        else
                        {
                            if (dieValue > 0)
                            {
                                downGridPoint = gridDictionary[downDestination];
                                currentLocation = downDestination;
                                AddGridPointToList(downGridPoint);
                                dieValue--;
                                DistributedDieValue.SetDieRollValue(dieValue);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("That position is already taken! Please try to move elsewhere. ");
                    }
                }

                break;


            case 3:

                Vector2 moveLeft = new Vector2(0, 0);
                if (ClientInfo.playerNumber == 1)
                {
                    moveLeft = new Vector2(0, 1);
                }
                else if (ClientInfo.playerNumber == 2)
                {
                    moveLeft = new Vector2(0, -1);
                }

                var leftDestination = currentLocation + moveLeft;

                if (gridDictionary.TryGetValue(leftDestination, out GridPoint leftGridPoint))
                {
                    if (leftGridPoint != GridPointOccupiedByOpponent())
                    {
                        if (IsGridPointInList(leftGridPoint))
                        {
                            leftGridPoint = gridDictionary[leftDestination];
                            currentLocation = leftDestination;
                            AddGridPointToList(leftGridPoint);
                        }
                        else
                        {
                            if (dieValue > 0)
                            {
                                leftGridPoint = gridDictionary[leftDestination];
                                currentLocation = leftDestination;
                                AddGridPointToList(leftGridPoint);
                                dieValue--;
                                DistributedDieValue.SetDieRollValue(dieValue);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("That position is already taken! Please try to move elsewhere. ");
                    }
                }

                break;

            case 4:

                Vector2 moveRight = new Vector2(0, 0);
                if (ClientInfo.playerNumber == 1)
                {
                    moveRight = new Vector2(0, -1);
                }
                else if (ClientInfo.playerNumber == 2)
                {
                    moveRight = new Vector2(0, 1);
                }

                var rightDestination = currentLocation + moveRight;

                if (gridDictionary.TryGetValue(rightDestination, out GridPoint rightGridPoint))
                {
                    if (rightGridPoint != GridPointOccupiedByOpponent())
                    {
                        if (IsGridPointInList(rightGridPoint))
                        {
                            rightGridPoint = gridDictionary[rightDestination];
                            currentLocation = rightDestination;
                            AddGridPointToList(rightGridPoint);
                        }
                        else
                        {
                            if (dieValue > 0)
                            {
                                rightGridPoint = gridDictionary[rightDestination];
                                currentLocation = rightDestination;
                                AddGridPointToList(rightGridPoint);
                                dieValue--;
                                DistributedDieValue.SetDieRollValue(dieValue);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("That position is already taken! Please try to move elsewhere. ");
                    }
                }

                break;
        }
    }
    #endregion
    public void ConfirmMove()
    {
        ClientSend.UpdatePlayerCurrentPostition((int)currentLocation.x, (int)currentLocation.y);
        var currentLocationVector3 = new Vector3(currentLocation.x, 0, currentLocation.y);

        var moveLocalPlayer = ClientInfo.playerNumber == 1 ? player1Spawn.transform.position = currentLocationVector3 : player2Spawn.transform.position = currentLocationVector3;
        EmptyGridPointList();
        PlayerTurnManager.Instance.EndTurn();
    }

    public void UpdateOpponentPosition(Vector2 value)
    {
        var newPos = new Vector3(value.x, 0, value.y);

        var moveOpponent = ClientInfo.playerNumber == 1 ? player2Spawn.transform.position = newPos : player1Spawn.transform.position = newPos;
    }

    private bool AddGridPointToListBool(GridPoint gp)
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
                            if (dieValue < 6)
                            {
                                dieValue++;
                            }
                            DistributedDieValue.SetDieRollValue(dieValue);
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

    private GridPoint GridPointOccupiedByOpponent()
    {
        if (ClientInfo.playerNumber == 1)
        {
            if (gridDictionary.TryGetValue(Vector3ToVector2.ConvertToVector2(player2Spawn.transform.position), out GridPoint gridPointOccupiedByOpponent))
            {
                return gridPointOccupiedByOpponent;
            }

        }
        else if (ClientInfo.playerNumber == 2)
        {
            if (gridDictionary.TryGetValue(Vector3ToVector2.ConvertToVector2(player1Spawn.transform.position), out GridPoint gridPointOccupiedByOpponent))
            {
                return gridPointOccupiedByOpponent;
            }
        }

        Debug.Log("ClientInfo.PlayerNumber returned an abnormal value, please check code");
        return null;
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
}
