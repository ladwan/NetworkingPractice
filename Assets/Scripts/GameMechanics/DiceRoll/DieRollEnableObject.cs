using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieRollEnableObject : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objectsToEnable = new List<GameObject>();
    [SerializeField]
    private List<GameObject> objectsToDisable = new List<GameObject>();

    public void ToggleUi()
    {
        for (int i = 0; i < objectsToEnable.Count; i++)
        {
            objectsToEnable[i].SetActive(true);
        }

        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            objectsToDisable[i].SetActive(false);
        }
    }
}
