using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.HelperScripts;

public class DragMovement : MonoBehaviour
{
    [SerializeField]
    private Vector2 currentLocationOfDragMover = new Vector2(0, 0);
    [SerializeField]
    private FloorGrid floorGridREF = null;
    [SerializeField]
    private Transform playerPositionREF = null;


    private Action onDragMoverPosUpdated = null;
    private GridPoint currentlyClickedGridPoint = null;
    private GridPoint gridPointCurrentlyDisplayingConnections = null;
    private Vector3 YAxisOffset = new Vector3(0, 0.5f, 0);
    private bool validDrag = false;


    public GridPoint CurrentlyClickedGridPoint { get => currentlyClickedGridPoint; set => currentlyClickedGridPoint = value; }

    public bool ValidDrag { get => validDrag; set => validDrag = value; }

    public GridPoint GridPointCurrentlyDisplayingConnections { get => gridPointCurrentlyDisplayingConnections; set => gridPointCurrentlyDisplayingConnections = value; }

    public Vector2 CurrentLocationOfDragMover { get => currentLocationOfDragMover; set => currentLocationOfDragMover = value; }

    public Action OnDragMoverPosUpdated { get => onDragMoverPosUpdated; set => onDragMoverPosUpdated = value; }


    protected void OnMouseDown()
    {
        UpdateDragMover();
    }

    protected void OnMouseUp()
    {
        validDrag = false;
        currentlyClickedGridPoint = null;
    }


    public void UpdateDragMover()
    {
        currentLocationOfDragMover = Vector3ToVector2.ConvertToVector2(gameObject.transform.position);
        if (floorGridREF.GridDictionary.TryGetValue(currentLocationOfDragMover, out GridPoint currentGp))
        {
            currentGp.DisplayConnections(true);
            gridPointCurrentlyDisplayingConnections = currentGp;
            validDrag = true;
        }
    }

    public void UpdateDragMoverPosition()
    {
        gameObject.transform.position = playerPositionREF.position + YAxisOffset;
    }

    public void UpdateDragMoverPosition(Vector2 updatedPos)
    {
        gameObject.transform.position = new Vector3(updatedPos.x, 0, updatedPos.y) + YAxisOffset;
    }

    public bool IsThisGridPointConnected()
    {
        if (currentlyClickedGridPoint)
        {
            for (int i = 0; i < gridPointCurrentlyDisplayingConnections.Connections.Count; i++)
            {
                if (gridPointCurrentlyDisplayingConnections.Connections[i] == currentlyClickedGridPoint)
                {
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    public void ResetDragMover()
    {
        if (validDrag)
        {
            validDrag = false;
        }
        if (gridPointCurrentlyDisplayingConnections)
        {
            gridPointCurrentlyDisplayingConnections.DisplayConnections(false);
            gridPointCurrentlyDisplayingConnections = null;
        }
        UpdateDragMoverPosition();
        gameObject.SetActive(false);
    }
}
