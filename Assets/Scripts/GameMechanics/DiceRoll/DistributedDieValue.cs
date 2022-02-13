using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DistributedDieValue
{
    public static int distributedDieRollValue = 0;
    public static void SetDieRollValue(int value)
    {
        distributedDieRollValue = value;
        Debug.Log("Die roll value is : " + distributedDieRollValue);
    }
}
