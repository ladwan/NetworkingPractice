using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Stateless math helpers for free movement planning: ground sampling,
    /// path length, distance-based truncation and endpoint adjustments.
    /// </summary>
    public static class NavPathUtility
    {
        public const float NavSampleRadius = 2.0f;

        /// <summary>
        /// Resolves a screen ray to a point on the walkable NavMesh. Falls back to an
        /// infinite ground plane at y=0 when the ray misses the floor collider so drags
        /// past the arena edge still resolve to a clamped point.
        /// </summary>
        public static bool SampleGround(Ray ray, LayerMask groundMask, out Vector3 navMeshPoint)
        {
            Vector3 rawPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
            {
                rawPoint = hit.point;
            }
            else
            {
                var groundPlane = new Plane(Vector3.up, Vector3.zero);
                if (!groundPlane.Raycast(ray, out float enter))
                {
                    navMeshPoint = Vector3.zero;
                    return false;
                }
                rawPoint = ray.GetPoint(enter);
            }

            if (NavMesh.SamplePosition(rawPoint, out NavMeshHit navHit, NavSampleRadius, NavMesh.AllAreas))
            {
                navMeshPoint = navHit.position;
                return true;
            }

            navMeshPoint = Vector3.zero;
            return false;
        }

        public static float PathLength(IList<Vector3> points)
        {
            float length = 0f;
            for (int i = 1; i < points.Count; i++)
            {
                length += Vector3.Distance(points[i - 1], points[i]);
            }
            return length;
        }

        /// <summary>
        /// Copies corners into result, cutting the polyline at exactly maxDistance along it.
        /// Returns the resulting planned length (equal to maxDistance when the cut happened,
        /// otherwise the full path length).
        /// </summary>
        public static float TruncateAtDistance(Vector3[] corners, float maxDistance, List<Vector3> result)
        {
            result.Clear();
            if (corners.Length == 0)
            {
                return 0f;
            }

            result.Add(corners[0]);
            float accumulated = 0f;

            for (int i = 1; i < corners.Length; i++)
            {
                float segment = Vector3.Distance(corners[i - 1], corners[i]);
                if (segment <= Mathf.Epsilon)
                {
                    continue;
                }

                if (accumulated + segment >= maxDistance)
                {
                    float remain = maxDistance - accumulated;
                    result.Add(corners[i - 1] + (corners[i] - corners[i - 1]).normalized * remain);
                    return maxDistance;
                }

                accumulated += segment;
                result.Add(corners[i]);
            }

            return accumulated;
        }

        /// <summary>
        /// Walks the path endpoint back along the polyline until it sits at least
        /// radius away from blocker (the free-movement equivalent of the old
        /// "grid point occupied by opponent" rule). Returns the new path length.
        /// </summary>
        public static float PullBackFromPoint(List<Vector3> path, Vector3 blocker, float radius)
        {
            while (path.Count > 1)
            {
                Vector3 end = path[path.Count - 1];
                Vector3 flatOffset = end - blocker;
                flatOffset.y = 0f;
                float distanceToBlocker = flatOffset.magnitude;

                if (distanceToBlocker >= radius)
                {
                    break;
                }

                Vector3 previous = path[path.Count - 2];
                float segmentLength = Vector3.Distance(previous, end);
                float needed = radius - distanceToBlocker;

                if (segmentLength <= needed)
                {
                    path.RemoveAt(path.Count - 1);
                    continue;
                }

                path[path.Count - 1] = end + (previous - end).normalized * needed;
                // Recheck: pulling straight back along the segment may still be inside the radius
                // for curved approaches; loop handles it. Guard against no progress:
                if (Vector3.Distance(path[path.Count - 1], end) < 0.001f)
                {
                    path.RemoveAt(path.Count - 1);
                }
            }

            return PathLength(path);
        }

        public static void FlattenY(List<Vector3> path, float y = 0f)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var point = path[i];
                point.y = y;
                path[i] = point;
            }
        }
    }
}
