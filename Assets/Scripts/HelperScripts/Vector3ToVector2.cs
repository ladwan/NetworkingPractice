using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public static class Vector3ToVector2
    {
        public static Vector2 ConvertToVector2(Vector3 value)
        {
            var newVector2 = new Vector2(value.x, value.z);
            return newVector2;
        }
    }
}
