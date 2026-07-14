using System;
using UnityEngine;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// The four scalars that fully describe one move's pacing. The mover's client
    /// derives them from its LocomotionProfile at confirm time and sends them in the
    /// move packet, so the remote client bakes an identical MovePlaybackPlan without
    /// knowing anything about buff state (Ire/Haste can expire mid-flight harmlessly).
    /// </summary>
    public struct LocomotionParams
    {
        public float CruiseSpeed;    // m/s at full stride for this move
        public float AccelDistance;  // meters spent ramping up from standstill
        public float DecelDistance;  // meters of braking before the endpoint
        public float PeakGait;       // CharSpeed blend value at cruise (in the character's blend tree scale)
    }

    /// <summary>
    /// Per-character movement feel, keyed on how far the move travels: short moves
    /// walk, long moves run, per the character's curves. Lives as a serialized field
    /// on Character (code defaults, inspector-tunable on the prefab - no assets).
    /// Gait values are in the character's existing CharSpeed blend tree scale
    /// (Brawn walk=5 / run=10, Speedster walk=5 / sprint=22).
    /// </summary>
    [Serializable]
    public class LocomotionProfile
    {
        [Tooltip("Path length (world units) -> average cruise speed in m/s.")]
        [SerializeField] private AnimationCurve cruiseSpeedByDistance = AnimationCurve.Constant(0f, 15f, 2f);
        [Tooltip("Path length (world units) -> CharSpeed blend value at cruise, in this character's blend tree scale.")]
        [SerializeField] private AnimationCurve gaitByDistance = AnimationCurve.Constant(0f, 15f, 5f);
        [Tooltip("Meters spent accelerating from standstill.")]
        [SerializeField] private float accelDistance = 0.75f;
        [Tooltip("Meters of braking before the endpoint - the gradual, anticipatory stop.")]
        [SerializeField] private float decelDistance = 1.5f;
        [Tooltip("If > 0, moves longer than this skip the acceleration ramp and launch at full speed (buff profiles like Ire/Haste). The braking tail is unaffected; shorter moves keep the normal ramp.")]
        [SerializeField] private float instantStartBeyond = 0f;


        public LocomotionParams ParamsForDistance(float pathLength)
        {
            bool instantStart = instantStartBeyond > 0f && pathLength > instantStartBeyond;

            return new LocomotionParams
            {
                CruiseSpeed = Mathf.Max(0.1f, cruiseSpeedByDistance.Evaluate(pathLength)),
                AccelDistance = instantStart ? 0.01f : Mathf.Max(0.01f, accelDistance),
                DecelDistance = Mathf.Max(0.01f, decelDistance),
                PeakGait = Mathf.Max(0f, gaitByDistance.Evaluate(pathLength)),
            };
        }


        /// <summary>Generic walk-everywhere default (Character base / Elemental).</summary>
        public static LocomotionProfile CreateDefaultWalk()
        {
            return new LocomotionProfile
            {
                cruiseSpeedByDistance = AnimationCurve.Constant(0f, 15f, 2f),
                gaitByDistance = AnimationCurve.Constant(0f, 15f, 5f),
                accelDistance = 0.75f,
                decelDistance = 1.5f,
            };
        }

        /// <summary>
        /// Brawn under Ire: past 3 units he launches straight into his run at full speed
        /// (no ramp-up, braking tail kept); 3 units or less he still just walks.
        /// </summary>
        public static LocomotionProfile CreateBrawnIre()
        {
            return new LocomotionProfile
            {
                cruiseSpeedByDistance = new AnimationCurve(
                    new Keyframe(0f, 2f), new Keyframe(3f, 2f), new Keyframe(3.01f, 3.2f), new Keyframe(15f, 3.2f)),
                gaitByDistance = new AnimationCurve(
                    new Keyframe(0f, 5f), new Keyframe(3f, 5f), new Keyframe(3.01f, 10f), new Keyframe(15f, 10f)),
                accelDistance = 0.75f,
                decelDistance = 2f,
                instantStartBeyond = 3f,
            };
        }

        /// <summary>
        /// Speedster: short moves walk, long moves ramp steeply toward sprint - both
        /// speed and gait scale hard with distance.
        /// </summary>
        public static LocomotionProfile CreateSpeedster()
        {
            return new LocomotionProfile
            {
                cruiseSpeedByDistance = new AnimationCurve(
                    new Keyframe(0f, 2f), new Keyframe(3f, 2.5f), new Keyframe(7f, 4.5f), new Keyframe(12f, 6f)),
                gaitByDistance = new AnimationCurve(
                    new Keyframe(0f, 5f), new Keyframe(3f, 5f), new Keyframe(7f, 20f), new Keyframe(12f, 22f)),
                accelDistance = 1f,
                decelDistance = 2.5f,
            };
        }

        /// <summary>
        /// Speedster under Haste: past 3 units he launches straight into his sprint at
        /// full speed (no ramp-up, braking tail kept); 3 units or less he still walks
        /// like his normal short moves.
        /// </summary>
        public static LocomotionProfile CreateSpeedsterHasted()
        {
            return new LocomotionProfile
            {
                cruiseSpeedByDistance = new AnimationCurve(
                    new Keyframe(0f, 2f), new Keyframe(3f, 2.5f), new Keyframe(3.01f, 6f), new Keyframe(15f, 6f)),
                gaitByDistance = new AnimationCurve(
                    new Keyframe(0f, 5f), new Keyframe(3f, 5f), new Keyframe(3.01f, 22f), new Keyframe(15f, 22f)),
                accelDistance = 1f,
                decelDistance = 2.5f,
                instantStartBeyond = 3f,
            };
        }
    }
}
