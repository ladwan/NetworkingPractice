using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// An immutable, pre-baked description of one move: distance-along-path and
    /// velocity as pure functions of elapsed time. Built once at confirm from
    /// (waypoints, LocomotionParams); playback then evaluates it statelessly each
    /// frame, so translation and the CharSpeed animation value are two reads of the
    /// same function at the same t - they cannot drift apart on frame drops, and the
    /// bake uses fixed step counts so both clients derive bit-identical motion from
    /// the same packet.
    ///
    /// Velocity shape: trapezoid over distance - smooth ramp up across AccelDistance,
    /// cruise, smooth brake across DecelDistance to a near-stop at the endpoint
    /// (the gradual, anticipatory stop). Short moves squash to a triangle.
    /// </summary>
    public class MovePlaybackPlan
    {
        private const int BakeSteps = 256;    // distance-domain integration steps
        private const int TimeSamples = 64;   // uniform time-grid resolution
        // Velocity never quite hits zero at the edges so integration stays finite;
        // visually this is the first/last toe-push of the move.
        private const float EdgeSpeedFraction = 0.12f;

        private readonly float[] distanceByTime = new float[TimeSamples + 1];
        private readonly float[] velocityByTime = new float[TimeSamples + 1];

        public IReadOnlyList<Vector3> Waypoints { get; }
        public float TotalLength { get; }
        public float Duration { get; }
        public LocomotionParams Params { get; }


        public MovePlaybackPlan(IReadOnlyList<Vector3> waypoints, float totalLength, LocomotionParams locomotion, float minimumDuration)
        {
            Waypoints = waypoints;
            TotalLength = Mathf.Max(0.001f, totalLength);
            Params = locomotion;

            // Squash accel+decel proportionally when the move is shorter than both.
            float accel = locomotion.AccelDistance;
            float decel = locomotion.DecelDistance;
            float rampTotal = accel + decel;
            if (rampTotal > TotalLength)
            {
                float squash = TotalLength / rampTotal;
                accel *= squash;
                decel *= squash;
            }

            // Integrate time over distance at fixed steps: dt = ds / v(s).
            float ds = TotalLength / BakeSteps;
            var timeAtDistance = new float[BakeSteps + 1];
            timeAtDistance[0] = 0f;
            for (int i = 1; i <= BakeSteps; i++)
            {
                float sMid = (i - 0.5f) * ds; // midpoint sampling for stability
                timeAtDistance[i] = timeAtDistance[i - 1] + ds / VelocityAtDistance(sMid, TotalLength, accel, decel, locomotion.CruiseSpeed);
            }

            float rawDuration = timeAtDistance[BakeSteps];
            // Respect a minimum duration so tiny moves still read as motion. Stretching
            // time scales velocity (and therefore the gait value) down uniformly.
            Duration = Mathf.Max(minimumDuration, rawDuration);
            float timeScale = Duration / rawDuration;

            // Invert to uniform time grid: distance and velocity at each t sample.
            int cursor = 0;
            for (int i = 0; i <= TimeSamples; i++)
            {
                float t = (float)i / TimeSamples * rawDuration;
                while (cursor < BakeSteps && timeAtDistance[cursor + 1] < t)
                {
                    cursor++;
                }

                if (cursor >= BakeSteps)
                {
                    distanceByTime[i] = TotalLength;
                }
                else
                {
                    float segmentDt = timeAtDistance[cursor + 1] - timeAtDistance[cursor];
                    float frac = segmentDt > 1e-6f ? (t - timeAtDistance[cursor]) / segmentDt : 1f;
                    distanceByTime[i] = (cursor + Mathf.Clamp01(frac)) * ds;
                }

                velocityByTime[i] = VelocityAtDistance(
                    Mathf.Clamp(distanceByTime[i], 0f, TotalLength), TotalLength, accel, decel, locomotion.CruiseSpeed) / timeScale;
            }

            distanceByTime[TimeSamples] = TotalLength;
        }


        /// <summary>Distance traveled along the path at normalized time (0..1).</summary>
        public float DistanceAt(float t01)
        {
            return SampleArray(distanceByTime, t01);
        }

        /// <summary>Instantaneous speed in m/s at normalized time (0..1).</summary>
        public float VelocityAt(float t01)
        {
            return SampleArray(velocityByTime, t01);
        }

        /// <summary>
        /// The CharSpeed blend value at normalized time: the gait peak scaled by how
        /// close to cruise speed we currently are. Ramps up through the lower gaits on
        /// launch and back down through them while braking - the blend tree slides
        /// through walk/run continuously, and the stop is gradual by construction.
        /// </summary>
        public float GaitAt(float t01)
        {
            return Params.PeakGait * Mathf.Clamp01(VelocityAt(t01) / Params.CruiseSpeed);
        }


        private static float VelocityAtDistance(float s, float totalLength, float accel, float decel, float cruise)
        {
            float multiplier = 1f;
            if (s < accel)
            {
                multiplier = Mathf.SmoothStep(EdgeSpeedFraction, 1f, s / accel);
            }

            float remaining = totalLength - s;
            if (remaining < decel)
            {
                multiplier = Mathf.Min(multiplier, Mathf.SmoothStep(EdgeSpeedFraction, 1f, remaining / decel));
            }

            return cruise * multiplier;
        }

        private static float SampleArray(float[] samples, float t01)
        {
            float scaled = Mathf.Clamp01(t01) * (samples.Length - 1);
            int index = Mathf.Min(samples.Length - 2, (int)scaled);
            return Mathf.Lerp(samples[index], samples[index + 1], scaled - index);
        }
    }
}
