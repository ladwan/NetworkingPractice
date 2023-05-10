using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMovement : MonoBehaviour
{
    [SerializeField]
    private Vector2 currentLocationOfDragMover = new Vector2(0, 0);
    [SerializeField]
    private FloorGrid floorGridREF = null;

    private GridPoint currentlyClickedGridPoint = null;
    private GridPoint gridPointCurrentlyDisplayingConnections = null;
    private bool validDrag = false;


    public GridPoint CurrentlyClickedGridPoint { get => currentlyClickedGridPoint; set => currentlyClickedGridPoint = value; }

    public bool ValidDrag { get => validDrag; set => validDrag = value; }

    public GridPoint GridPointCurrentlyDisplayingConnections { get => gridPointCurrentlyDisplayingConnections; set => gridPointCurrentlyDisplayingConnections = value; }


    private void OnMouseDown()
    {
        UpdateDragMover();
    }

    private void OnMouseUp()
    {

        //if (gridPointCurrentlyDisplayingConnections)
        //{
        //  gridPointCurrentlyDisplayingConnections.DisplayConnections(false);
        validDrag = false;
        currentlyClickedGridPoint = null;
        // }

    }

    private void OnMouseDrag()
    {
        if (validDrag)
        {

        }
    }

    public void UpdateDragMover()
    {
        currentLocationOfDragMover = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
        if (floorGridREF.GridDictionary.TryGetValue(currentLocationOfDragMover, out GridPoint currentGp))
        {
            currentGp.DisplayConnections(true);
            gridPointCurrentlyDisplayingConnections = currentGp;
            validDrag = true;
        }
    }

    public void UpdateDragMoverPosition(Vector2 updatedPos)
    {
        gameObject.transform.position = new Vector3(updatedPos.x, 0.5f, updatedPos.y);
    }

    public bool IsThisGridPointConnected()
    {
        if (currentlyClickedGridPoint)
        {
            for (int i = 0; i < gridPointCurrentlyDisplayingConnections.Connections.Count; i++)
            {
                if (gridPointCurrentlyDisplayingConnections.Connections[i] == currentlyClickedGridPoint)
                {
                    Debug.Log("GP found !");
                    return true;
                }
            }

            Debug.Log("This GP was not in the connections !");
            return false;
        }
        Debug.Log("No currently clicked GP found !");
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
