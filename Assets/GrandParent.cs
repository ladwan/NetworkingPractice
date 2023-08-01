using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrandParent : MonoBehaviour
{
    [SerializeField]
    private int duration = 0;

    public int Duration { get => duration; set => duration = value; }
}
