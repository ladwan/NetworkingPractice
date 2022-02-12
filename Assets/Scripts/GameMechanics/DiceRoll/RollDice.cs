using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class RollDice
{
    public static int RandomRoll()
    {
        var value = Random.Range(1, 7);
        return value;
    }
}

