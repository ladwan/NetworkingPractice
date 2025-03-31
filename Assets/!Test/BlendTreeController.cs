using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendTreeController : MonoBehaviour
{
    [SerializeField]
    private float charSpeed = 0;
    [SerializeField]
    private Animator animREF = null;


    private void Update()
    {
        animREF.SetFloat("CharSpeed", charSpeed);
    }
}
