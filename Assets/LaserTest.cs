using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UnityEngine.UI.Image;

public class LaserTest : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        RaycastHit hit;
        var origin = this.transform.position;

        if (Physics.Raycast(origin, this.transform.right, out hit)) 
        {  
            Gizmos.DrawLine(origin, hit.point);
            Gizmos.DrawSphere(hit.point, 0.5f);

            var dir = this.transform.right;
            var dot = hit.normal.x * dir.x + hit.normal.y * dir.y + hit.normal.z * dir.z;

            var signedDistance = hit.normal * dot;
            var projectedPoint = hit.point + signedDistance;



            //var reflectedVector = ReflectVector(hit, this.transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(signedDistance, 0.5f);
        }
    }

    //private Vector3 ReflectVector(RaycastHit hit, Vector3 pos)
    //{
    //    var dir = pos.x * pos.x + pos.z * pos.z;

    //    var projectedDistance = hit.normal * dir;
    //}
}
