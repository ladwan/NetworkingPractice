using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Walks a player spawn along a waypoint polyline by evaluating an immutable
    /// MovePlaybackPlan at absolute elapsed time: position, velocity and the CharSpeed
    /// blend value are all reads of the same baked function at the same t, so
    /// translation and animation cannot desync on frame drops or slow devices - a
    /// hitch just samples the identical motion later. The plan's trapezoid velocity
    /// shape gives the ramp-up launch and the gradual, anticipatory stop.
    /// Runs no NavMesh queries at execution time, so a serialized waypoint list plus
    /// the four LocomotionParams floats replays identically on the remote client.
    /// </summary>
    public class MovementExecutor : MonoBehaviour
    {
        [SerializeField] private float turnSpeedDegrees = 540f;
        [SerializeField] private float minimumMoveDuration = 0.4f;
        [Tooltip("How far ahead along the path the character looks when turning. Larger = wider, smoother arcs around corners.")]
        [SerializeField] private float rotationLookAhead = 1.25f;

        private Coroutine playbackCoroutine = null;
        private bool isPlaying = false;

        public static MovementExecutor Instance { get; private set; }

        public bool IsPlaying => isPlaying;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("More than 1 MovementExecutor detected, destroying self...");
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Plays a move paced by the character's own ActiveLocomotionProfile
        /// (local abilities like the Speedster dash use this directly).
        /// </summary>
        public void Play(GameObject spawnToMove, Character character, IReadOnlyList<Vector3> waypoints, Action onComplete)
        {
            var profile = character != null ? character.ActiveLocomotionProfile : LocomotionProfile.CreateDefaultWalk();
            float pathLength = PathLength(waypoints);
            Play(spawnToMove, character, waypoints, profile.ParamsForDistance(pathLength), onComplete);
        }

        /// <summary>
        /// Plays a move paced by explicit LocomotionParams - the networked path. The
        /// mover's client derives the params once and sends them with the waypoints,
        /// so both clients bake and replay the exact same plan.
        /// </summary>
        public void Play(GameObject spawnToMove, Character character, IReadOnlyList<Vector3> waypoints, LocomotionParams locomotion, Action onComplete)
        {
            if (isPlaying)
            {
                Debug.LogWarning("~~ Attempted to start movement playback, but it is already running !");
                return;
            }

            if (spawnToMove == null || waypoints == null || waypoints.Count < 2)
            {
                onComplete?.Invoke();
                return;
            }

            var plan = new MovePlaybackPlan(waypoints, PathLength(waypoints), locomotion, minimumMoveDuration);
            isPlaying = true;
            playbackCoroutine = StartCoroutine(PlayPlan(spawnToMove, character, plan, onComplete));
        }


        private IEnumerator PlayPlan(GameObject spawnToMove, Character character, MovePlaybackPlan plan, Action onComplete)
        {
            Animator animator = character != null && character.CharacterAnimationReferences != null
                ? character.CharacterAnimationReferences.CharacterAnimator
                : null;

            var transformToMove = spawnToMove.transform;
            var waypoints = plan.Waypoints;
            transformToMove.position = waypoints[0];

            float elapsed = 0f;
            while (elapsed < plan.Duration)
            {
                elapsed = Mathf.Min(plan.Duration, elapsed + Time.deltaTime);
                float t01 = elapsed / plan.Duration;

                // One t, three reads: translation, steering target and blend value all
                // come from the same baked plan - they cannot drift apart.
                float traveled = plan.DistanceAt(t01);
                transformToMove.position = PointAlongPath(waypoints, traveled);

                Vector3 lookPoint = PointAlongPath(waypoints, Mathf.Min(plan.TotalLength, traveled + rotationLookAhead));
                Vector3 flatDirection = lookPoint - transformToMove.position;
                flatDirection.y = 0f;
                if (flatDirection.sqrMagnitude > 0.0001f)
                {
                    transformToMove.rotation = Quaternion.RotateTowards(
                        transformToMove.rotation,
                        Quaternion.LookRotation(flatDirection),
                        turnSpeedDegrees * Time.deltaTime);
                }

                if (animator != null)
                {
                    animator.SetFloat("CharSpeed", plan.GaitAt(t01));
                }

                yield return null;
            }

            // Snap the ending so both clients finish bit-identical.
            transformToMove.position = waypoints[waypoints.Count - 1];
            Vector3 finalDirection = waypoints[waypoints.Count - 1] - waypoints[waypoints.Count - 2];
            finalDirection.y = 0f;
            if (finalDirection.sqrMagnitude > 0.0001f)
            {
                transformToMove.rotation = Quaternion.LookRotation(finalDirection);
            }

            if (animator != null)
            {
                animator.SetFloat("CharSpeed", 0f);
            }

            isPlaying = false;
            playbackCoroutine = null;
            onComplete?.Invoke();
        }

        private static float PathLength(IReadOnlyList<Vector3> waypoints)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                return 0f;
            }

            float length = 0f;
            for (int i = 1; i < waypoints.Count; i++)
            {
                length += Vector3.Distance(waypoints[i - 1], waypoints[i]);
            }

            return length;
        }

        private static Vector3 PointAlongPath(IReadOnlyList<Vector3> waypoints, float distance)
        {
            float remaining = distance;
            for (int i = 1; i < waypoints.Count; i++)
            {
                float segmentLength = Vector3.Distance(waypoints[i - 1], waypoints[i]);
                if (remaining <= segmentLength)
                {
                    return segmentLength > Mathf.Epsilon
                        ? Vector3.Lerp(waypoints[i - 1], waypoints[i], remaining / segmentLength)
                        : waypoints[i];
                }

                remaining -= segmentLength;
            }

            return waypoints[waypoints.Count - 1];
        }
    }
}
