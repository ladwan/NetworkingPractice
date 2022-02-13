using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticDieValue
{
    public static int staticDieRollValue = 0;
    public static int SetDieRollValue(int value)
    {
        staticDieRollValue = value;
        return value;
    }
}
