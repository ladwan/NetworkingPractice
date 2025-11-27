using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MathTest : MonoBehaviour
{
    [SerializeField] private Transform origin;
    [SerializeField] private Transform aVector;
    [SerializeField] private Transform bVector;
    [SerializeField] private Transform pVector;


    private void OnDrawGizmos()
    {
        if (aVector == null || bVector == null || pVector == null)
        {
            Debug.LogWarning("A vector is null!");
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin.position, aVector.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin.position, bVector.position);

        var aLen = Mathf.Sqrt((float)((Math.Pow(aVector.position.x,2)) + (Math.Pow(aVector.position.z, 2))));
        var bLen = Mathf.Sqrt((float)((Math.Pow(bVector.position.x, 2)) + (Math.Pow(bVector.position.z, 2))));

        var isInside = (aLen - bLen) > 0;


        Debug.Log($"B is inside: {isInside} //  ALen: {aLen}  //  BLen {bLen}");
    }
}
