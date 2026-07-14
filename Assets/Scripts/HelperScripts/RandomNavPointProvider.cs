using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ForeverFight.GameMechanics.Movement;

namespace ForeverFight.HelperScripts
{
    /// <summary>
    /// NavMesh-validated random points inside the arena, replacing GetRandomGridPoint.
    /// Bounds default to the old 10x10 grid footprint (0..9 on X and Z).
    /// </summary>
    public class RandomNavPointProvider : MonoBehaviour
    {
        [SerializeField] private Rect arenaBounds = new Rect(0f, 0f, 9f, 9f);

        public List<Vector3> GetRandomPoints(int count)
        {
            var points = new List<Vector3>(count);
            const int maxAttemptsPerPoint = 10;

            for (int i = 0; i < count; i++)
            {
                for (int attempt = 0; attempt < maxAttemptsPerPoint; attempt++)
                {
                    var candidate = new Vector3(
                        Random.Range(arenaBounds.xMin, arenaBounds.xMax),
                        0f,
                        Random.Range(arenaBounds.yMin, arenaBounds.yMax));

                    if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, NavPathUtility.NavSampleRadius, NavMesh.AllAreas))
                    {
                        var point = hit.position;
                        point.y = 0f;
                        points.Add(point);
                        break;
                    }
                }
            }

            return points;
        }
    }
}
