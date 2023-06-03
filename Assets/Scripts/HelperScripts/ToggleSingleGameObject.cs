using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleSingleGameObject : MonoBehaviour
{
    [SerializeField]
    private GameObject gameObjectToToggle = null;


    private bool toggle = true;


    protected void OnDisable()
    {
        if (!toggle)
        {
            toggle = true;
        }
        gameObjectToToggle.SetActive(false);
    }

    public void ToggleGameObject()
    {
        gameObjectToToggle.SetActive(toggle);
        if (toggle)
        {
            toggle = false;
        }
        else
        {
            toggle = true;
        }
    }

}
