using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    private Action onTesterCalled = null;


    public Action OnTesterCalled { get => onTesterCalled; set => onTesterCalled = value; }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onTesterCalled?.Invoke();
        }
    }
}
