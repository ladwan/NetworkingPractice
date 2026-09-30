using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// One move, driven by a single curve - the same curves the old CurveMoveSpeed used.
    ///
    /// The curve's value is used for two things at the same moment:
    ///   - it's the CharSpeed value fed to the blend tree (idle -> walk -> run)
    ///   - it sets how fast the character is translating (value x UnitsPerSecondPerCurveValue)
    /// Because both come from the same number, the animation can't go back to idle while the
    /// character is still sliding: when the value is 0 the character is standing still.
    ///
    /// The old curves were hand made per grid cell count, so they don't cover a NavMesh path's
    /// exact length. The plan stretches the curve in time (never its values) until it covers the
    /// path exactly. Old curves were built so this stretch is close to 1, so moves feel the same.
    ///
    /// Everything is worked out once up front with fixed step counts, so both clients
    /// get the exact same motion from the same packet.
    /// </summary>
    public class MovePlaybackPlan
    {
        private const int BakeSteps = 256;

        // How far along the path the character is at each evenly spaced point in time
        private readonly float[] distanceByTime = new float[BakeSteps + 1];
        private readonly AnimationCurve curve;
        private readonly float curveLength;

        public IReadOnlyList<Vector3> Waypoints { get; }
        public float TotalLength { get; }
        public float Duration { get; }


        public MovePlaybackPlan(IReadOnlyList<Vector3> waypoints, float totalLength, AnimationCurve movementCurve, float unitsPerSecondPerCurveValue)
        {
            Waypoints = waypoints;
            TotalLength = Mathf.Max(0.001f, totalLength);
            curve = movementCurve;
            curveLength = Mathf.Max(0.01f, curve.keys[curve.keys.Length - 1].time);

            // Add up the area under the curve. Speed is proportional to the curve value,
            // so the running area is how far the character has gone at each point in time.
            float step = curveLength / BakeSteps;
            float area = 0f;
            float previousValue = Mathf.Max(0f, curve.Evaluate(0f));
            var areaByTime = new float[BakeSteps + 1];

            for (int i = 1; i <= BakeSteps; i++)
            {
                float value = Mathf.Max(0f, curve.Evaluate(i * step));
                area += (previousValue + value) * 0.5f * step;
                areaByTime[i] = area;
                previousValue = value;
            }

            // Stretch time so the curve covers this path at the old translation speed
            float naturalDistance = area * unitsPerSecondPerCurveValue;
            float timeStretch = naturalDistance > 0.0001f ? TotalLength / naturalDistance : 1f;
            Duration = curveLength * timeStretch;

            for (int i = 0; i <= BakeSteps; i++)
            {
                // A curve with no area can't drive movement, so fall back to moving at a steady pace
                distanceByTime[i] = area > 0.0001f
                    ? areaByTime[i] / area * TotalLength
                    : (float)i / BakeSteps * TotalLength;
            }

            distanceByTime[BakeSteps] = TotalLength;
        }


        /// <summary>Distance traveled along the path at normalized time (0..1).</summary>
        public float DistanceAt(float t01)
        {
            float scaled = Mathf.Clamp01(t01) * BakeSteps;
            int index = Mathf.Min(BakeSteps - 1, (int)scaled);
            return Mathf.Lerp(distanceByTime[index], distanceByTime[index + 1], scaled - index);
        }

        /// <summary>The CharSpeed blend value at normalized time (0..1), read straight off the curve.</summary>
        public float BlendValueAt(float t01)
        {
            return curve.Evaluate(Mathf.Clamp01(t01) * curveLength);
        }
    }
}
