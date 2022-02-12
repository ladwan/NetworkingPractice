using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GridPoint upperConnection;
    [SerializeField]
    private GridPoint lowerConnection;
    [SerializeField]
    private GridPoint leftConnection;
    [SerializeField]
    private GridPoint rightConnection;

    [Header("Other")]
    [SerializeField]
    private GameObject highlight;
    [SerializeField]
    private Vector2 uniqueTag = new Vector2();
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
    private void Start()
    {


    }
}
