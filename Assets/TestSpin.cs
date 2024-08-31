using ForeverFight.Interactable.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpin : MonoBehaviour
{
    [SerializeField] private Transform rotTransform = null;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float speed = 0;


    private float timePassed = 0;


    // Update is called once per frame
    private void Update()
    {
        var percent = (timePassed += Time.deltaTime * speed) % 1;
        rotTransform.eulerAngles = Vector3.Lerp(Vector3.zero, offset, percent);
    }
}
