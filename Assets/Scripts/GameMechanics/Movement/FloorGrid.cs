using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGrid : MonoBehaviour
{
    [SerializeField]
    private GameObject gridPoint;
    public Grid floorSpace;
    [SerializeField]
    private Dictionary<Vector2, GridPoint> gridDictionary = new Dictionary<Vector2, GridPoint>();
    [SerializeField]
    private List<GridPoint> hoveredOverGridPoints = new List<GridPoint>();
    private Vector2 currentLocation = new Vector2(0, 0);
    // Start is called before the first frame update

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


        /*
        foreach (var myKeyValue in gridDictionary)
        {
            Debug.Log(gridDictionary.Values);
        }
        */
    }

    public void TryHighlighting(GridPoint gp, Vector2 destination)
    {
        gp.ShowHighlight(true);

        if (AddGridPointToList(gp))
        {
            hoveredOverGridPoints.Add(gp);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Vector2 moveUp = new Vector2(1, 0);
            var destination = currentLocation + moveUp;

            if (gridDictionary.TryGetValue(destination, out GridPoint value))
            {
                //print(destination);
                value = gridDictionary[destination];
                currentLocation = destination;
                TryHighlighting(value, destination);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Vector2 moveDown = new Vector2(-1, 0);
            var destination = currentLocation + moveDown;

            if (gridDictionary.TryGetValue(destination, out GridPoint value))
            {
                value = gridDictionary[destination];
                currentLocation = destination;
                TryHighlighting(value, destination);
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Vector2 moveLeft = new Vector2(0, 1);
            var destination = currentLocation + moveLeft;

            if (gridDictionary.TryGetValue(destination, out GridPoint value))
            {
                value = gridDictionary[destination];
                currentLocation = destination;
                TryHighlighting(value, destination);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Vector2 moveRight = new Vector2(0, -1);
            var destination = currentLocation + moveRight;

            if (gridDictionary.TryGetValue(destination, out GridPoint value))
            {
                value = gridDictionary[destination];
                currentLocation = destination;
                TryHighlighting(value, destination);
            }
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
                        Debug.Log(gp + " is already in list");
                        var stuff = i + 1;
                        Debug.Log("Now Removing everything after index: " + i);

                        for (int removeMe = i + 1; removeMe != hoveredOverGridPoints.Count;)
                        {
                            hoveredOverGridPoints[removeMe].ShowHighlight(false);
                            hoveredOverGridPoints.RemoveAt(removeMe);
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
}
