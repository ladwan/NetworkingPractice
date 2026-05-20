using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClear : MonoBehaviour
{
    [SerializeField] private string key;

    public void OnEnable()
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
}
