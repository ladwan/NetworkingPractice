using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGrid : MonoBehaviour
{
    [SerializeField]
    private GameObject gridPoint;
    [SerializeField]
    private Dictionary<Vector2, GridPoint> gridDictionary = new Dictionary<Vector2, GridPoint>();
    [SerializeField]
    private List<GridPoint> hoveredOverGridPoints = new List<GridPoint>();
    [SerializeField]
    private Transform player1Spawn;
    [SerializeField]
    private Transform player2Spawn;
    [SerializeField]
    private DisplaySelectedChar displaySelectedCharREF = null;
    [SerializeField]
    private Transform tempTransform = null;


    private Vector3 tempVector3 = new Vector3(0, 0);
    private Vector2 currentLocation = new Vector2(0, 0);

    [SerializeField]
    public int dieValue = 0;

    void Start()
    {
        for (int p = 0; p < 10; p++)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject instantiatedGridPoint = Instantiate(gridPoint, new Vector3(this.transform.position.x + i, this.transform.position.y, this.transform.position.z + p), new Quaternion(), this.gameObject.transform);
                gridDictionary.Add(new Vector2(this.transform.position.x + i, this.transform.position.z + p), instantiatedGridPoint.GetComponent<GridPoint>());
            }
        }


        //Assigning a spawn point to the player based on their ClientInfo.playerNumber
        if (ClientInfo.playerNumber == 1)
        {
            currentLocation = new Vector2(player1Spawn.position.x, player1Spawn.position.z);
        }
        else if (ClientInfo.playerNumber == 2)
        {
            currentLocation = new Vector2(player2Spawn.position.x, player2Spawn.position.z);
        }

        if (ClientInfo.playerNumber == 0)
        {
            currentLocation = new Vector2(player1Spawn.position.x, player1Spawn.position.z);
            Debug.Log("Client player number was 0, defaulting to spawn point 1");
        }


        // Organizing the position data into a transform so it can be used to spawn the player
        tempVector3.x = currentLocation.x;
        tempVector3.z = currentLocation.y;
        tempTransform.position = tempVector3;
        displaySelectedCharREF.SpawnPlayer(tempTransform);
    }
    public void EmptyGridPointList()
    {
        for (int i = 0; i < hoveredOverGridPoints.Count; i++)
        {
            hoveredOverGridPoints[i].ShowHighlight(false);
        }
        hoveredOverGridPoints.Clear();
        DistributedDieValue.SetDieRollValue(DistributedDieValue.unchangingDieRollValue);
        dieValue = DistributedDieValue.unchangingDieRollValue;
        currentLocation = new Vector2(0, 0);
    }

    public void AddGridPointToList(GridPoint gp, Vector2 destination)
    {
        gp.ShowHighlight(true);

        if (AddGridPointToList(gp))
        {
            hoveredOverGridPoints.Add(gp);
        }
    }

    public void TryHighlighting(int movementValue)
    {
        dieValue = DistributedDieValue.distributedDieRollValue;

        switch (movementValue)
        {
            case 1:

                Vector2 moveUp = new Vector2(1, 0);
                var upDestination = currentLocation + moveUp;
                if (gridDictionary.TryGetValue(upDestination, out GridPoint upGridPoint))
                {
                    if (isGridPointInList(upGridPoint))
                    {
                        upGridPoint = gridDictionary[upDestination];
                        currentLocation = upDestination;
                        AddGridPointToList(upGridPoint, upDestination);
                    }
                    else
                    {
                        if (dieValue > 0)
                        {
                            upGridPoint = gridDictionary[upDestination];
                            currentLocation = upDestination;
                            AddGridPointToList(upGridPoint, upDestination);
                            dieValue--;
                            DistributedDieValue.SetDieRollValue(dieValue);
                        }
                    }
                }

                break;

            case 2:

                Vector2 moveDown = new Vector2(-1, 0);
                var downDestination = currentLocation + moveDown;

                if (gridDictionary.TryGetValue(downDestination, out GridPoint downGridPoint))
                {
                    if (isGridPointInList(downGridPoint))
                    {
                        downGridPoint = gridDictionary[downDestination];
                        currentLocation = downDestination;
                        AddGridPointToList(downGridPoint, downDestination);
                    }
                    else
                    {
                        if (dieValue > 0)
                        {
                            downGridPoint = gridDictionary[downDestination];
                            currentLocation = downDestination;
                            AddGridPointToList(downGridPoint, downDestination);
                            dieValue--;
                            DistributedDieValue.SetDieRollValue(dieValue);
                        }
                    }
                }

                break;


            case 3:

                Vector2 moveLeft = new Vector2(0, 1);
                var leftDestination = currentLocation + moveLeft;

                if (gridDictionary.TryGetValue(leftDestination, out GridPoint leftGridPoint))
                {
                    if (isGridPointInList(leftGridPoint))
                    {
                        leftGridPoint = gridDictionary[leftDestination];
                        currentLocation = leftDestination;
                        AddGridPointToList(leftGridPoint, leftDestination);
                    }
                    else
                    {
                        if (dieValue > 0)
                        {
                            leftGridPoint = gridDictionary[leftDestination];
                            currentLocation = leftDestination;
                            AddGridPointToList(leftGridPoint, leftDestination);
                            dieValue--;
                            DistributedDieValue.SetDieRollValue(dieValue);
                        }
                    }
                }

                break;

            case 4:

                Vector2 moveRight = new Vector2(0, -1);
                var rightDestination = currentLocation + moveRight;

                if (gridDictionary.TryGetValue(rightDestination, out GridPoint rightGridPoint))
                {
                    if (isGridPointInList(rightGridPoint))
                    {
                        rightGridPoint = gridDictionary[rightDestination];
                        currentLocation = rightDestination;
                        AddGridPointToList(rightGridPoint, rightDestination);
                    }
                    else
                    {
                        if (dieValue > 0)
                        {
                            rightGridPoint = gridDictionary[rightDestination];
                            currentLocation = rightDestination;
                            AddGridPointToList(rightGridPoint, rightDestination);
                            dieValue--;
                            DistributedDieValue.SetDieRollValue(dieValue);
                        }
                    }
                }
                break;
        }
    }


    private bool AddGridPointToList(GridPoint gp)
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

    private bool isGridPointInList(GridPoint gp)
    {
        if (AddGridPointToList(gp) == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
