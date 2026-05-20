using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    private static DoNotDestroy instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.transform.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }
}
