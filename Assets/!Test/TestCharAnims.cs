using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharAnims : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int animIndex = 0;
    [SerializeField] private List<string> triggerNames = new();


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            animator.SetTrigger(triggerNames[animIndex]);
        }
    }
}
