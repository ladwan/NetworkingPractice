using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.HelperScripts;

public class GridPoint : MonoBehaviour
{
    [Header("Status")]
    [SerializeField]
    private bool isOccupiedByPlayer;
    [SerializeField]
    private bool isHovered;
    [SerializeField]
    private bool isValid;

    [Header("Connections")]
    [SerializeField]
    private List<GridPoint> connections;

    [Header("Other")]
    [SerializeField]
    private GameObject highlight;
    [SerializeField]
    private GameObject connectionAura;
    [SerializeField]
    private Vector2 uniqueTag = new Vector2();
    [SerializeField]
    private DragMovement dragMovementREF = null;
    [SerializeField]
    private FloorGrid floorGridREF = null;


    public List<GridPoint> Connections { get => connections; set => connections = value; }

    public Vector2 UniqueTag { get => uniqueTag; set => uniqueTag = value; }

    public DragMovement DragMovementREF { get => dragMovementREF; set => dragMovementREF = value; }

    public GameObject Highlight { get => highlight; set => highlight = value; }


    // Start is called before the first frame update
    void Awake()
    {
        uniqueTag = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z);
        gameObject.name = "GridPoint" + uniqueTag;
    }

    public void ShowHighlight(bool toggle)
    {
        highlight.SetActive(toggle);
    }

    public void FindConnections(Dictionary<Vector2, GridPoint> gridDictionaryREF)
    {
        if (gridDictionaryREF.TryGetValue(uniqueTag + new Vector2(1, 0), out GridPoint validConnection01))
        {
            connections.Add(validConnection01);
        }

        if (gridDictionaryREF.TryGetValue(uniqueTag + new Vector2(-1, 0), out GridPoint validConnection02))
        {
            connections.Add(validConnection02);
        }

        if (gridDictionaryREF.TryGetValue(uniqueTag + new Vector2(0, 1), out GridPoint validConnection03))
        {
            connections.Add(validConnection03);
        }

        if (gridDictionaryREF.TryGetValue(uniqueTag + new Vector2(0, -1), out GridPoint validConnection04))
        {
            connections.Add(validConnection04);
        }
    }

    public void DisplayConnections(bool toggle)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            connections[i].connectionAura.SetActive(toggle);
        }
    }

    private void OnMouseEnter()
    {
        if (dragMovementREF.ValidDrag)
        {
            dragMovementREF.CurrentlyClickedGridPoint = this;
            if (dragMovementREF.IsThisGridPointConnected())
            {
                //If this is a vaild connected grid point, then were going to move there, but first turn off the connection aura from the grid point we are currently on.
                var currentLocationOfDragMover = Vector3ToVector2.ConvertToVector2(dragMovementREF.transform.position);
                if (floorGridREF.GridDictionary.TryGetValue(currentLocationOfDragMover, out GridPoint previousGp))
                {
                    previousGp.DisplayConnections(false);
                }
                floorGridREF.TryHighlighting(dragMovementREF.CurrentlyClickedGridPoint);
                dragMovementREF.UpdateDragMover();
                dragMovementREF.OnDragMoverPosUpdated?.Invoke();
            }
        }
    }
}
