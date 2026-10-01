using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Walks a player spawn along a waypoint polyline by evaluating an immutable
    /// MovePlaybackPlan at absolute elapsed time: position and the CharSpeed blend value
    /// both come from the same movement curve at the same t, so translation and
    /// animation cannot desync on frame drops or slow devices - a hitch just samples
    /// the identical motion later.
    /// Runs no NavMesh queries at execution time, so a serialized waypoint list plus
    /// the mover's movement state index replays identically on the remote client.
    /// </summary>
    public class MovementExecutor : MonoBehaviour
    {
        [SerializeField] private float turnSpeedDegrees = 540f;
        [Tooltip("How far ahead along the path the character looks when turning. Larger = wider, smoother arcs around corners.")]
        [SerializeField] private float rotationLookAhead = 1.25f;
        // The old grid moved v * 0.0066 of a cell per frame at ~60 fps, where v is the curve value.
        // That works out to about 0.4 cells a second per point of curve value (1 cell ~= 1 world unit),
        // so a walk value of 5 moves at 2 units a second, just like before.
        [Tooltip("World units per second for each point of movement curve value.")]
        [SerializeField] private float unitsPerSecondPerCurveValue = 0.4f;

        // Used only when a character has no movement curves set up, so a move still plays
        // instead of failing. Same shape as the old 1 cell walk curve.
        private static readonly AnimationCurve fallbackCurve = new AnimationCurve(
            new Keyframe(0f, 0f), new Keyframe(0.5f, 5f), new Keyframe(1f, 0f));

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
        /// Plays a move using the character's current movement state
        /// (local abilities like the Speedster dash use this directly).
        /// </summary>
        public void Play(GameObject spawnToMove, Character character, IReadOnlyList<Vector3> waypoints, Action onComplete)
        {
            int movementIndex = character != null ? character.MovementIndex : 0;
            Play(spawnToMove, character, waypoints, movementIndex, onComplete);
        }

        /// <summary>
        /// Plays a move using an explicit movement state - the networked path. The mover
        /// sends its movement state with the waypoints, so the remote client picks the
        /// same curve even though it doesn't know whether Ire is active.
        /// </summary>
        public void Play(GameObject spawnToMove, Character character, IReadOnlyList<Vector3> waypoints, int movementIndex, Action onComplete)
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

            float pathLength = PathLength(waypoints);
            AnimationCurve movementCurve = character != null ? character.GetMovementCurve(movementIndex, pathLength) : null;
            if (movementCurve == null || movementCurve.keys.Length == 0)
            {
                Debug.LogWarning("~~ No movement curve found, using the fallback walk curve");
                movementCurve = fallbackCurve;
            }

            var specialMovement = character != null ? character.GetSpecialMovement(movementIndex, pathLength) : null;
            var plan = specialMovement != null && specialMovement.travelType == Character.SpecialMovement.TravelType.FixedDuration
                ? new MovePlaybackPlan(waypoints, pathLength, specialMovement.travelSeconds, specialMovement.travelProgress)
                : new MovePlaybackPlan(waypoints, pathLength, movementCurve, unitsPerSecondPerCurveValue);
            isPlaying = true;
            playbackCoroutine = StartCoroutine(PlayPlan(spawnToMove, character, plan, specialMovement, onComplete));
        }


        private IEnumerator PlayPlan(GameObject spawnToMove, Character character, MovePlaybackPlan plan, Character.SpecialMovement specialMovement, Action onComplete)
        {
            Animator animator = character != null && character.CharacterAnimationReferences != null
                ? character.CharacterAnimationReferences.CharacterAnimator
                : null;

            var transformToMove = spawnToMove.transform;
            var waypoints = plan.Waypoints;
            transformToMove.position = waypoints[0];

            if (specialMovement != null)
            {
                FireSpecialAnimation(animator, specialMovement);

                if (specialMovement.windUpSeconds > 0f)
                {
                    yield return PlayWindUp(transformToMove, waypoints, specialMovement.windUpSeconds);
                }
            }

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
                    animator.SetFloat("CharSpeed", plan.BlendValueAt(t01));
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

        private static void FireSpecialAnimation(Animator animator, Character.SpecialMovement specialMovement)
        {
            if (animator == null)
            {
                return;
            }

            animator.SetFloat("CharSpeed", 0f);
            if (!string.IsNullOrEmpty(specialMovement.animatorTrigger))
            {
                animator.SetTrigger(specialMovement.animatorTrigger);
            }
        }

        // Some special moves wind up in place before they travel, so the character stands
        // still, turning to face the move, for windUpSeconds. Only then does the plan start
        // translating. Kept separate from how the move travels so any special move can use it.
        private IEnumerator PlayWindUp(Transform transformToMove, IReadOnlyList<Vector3> waypoints, float windUpSeconds)
        {
            Vector3 faceDirection = waypoints[1] - waypoints[0];
            faceDirection.y = 0f;

            float elapsed = 0f;
            while (elapsed < windUpSeconds)
            {
                elapsed += Time.deltaTime;

                if (faceDirection.sqrMagnitude > 0.0001f)
                {
                    transformToMove.rotation = Quaternion.RotateTowards(
                        transformToMove.rotation,
                        Quaternion.LookRotation(faceDirection),
                        turnSpeedDegrees * Time.deltaTime);
                }

                yield return null;
            }
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
