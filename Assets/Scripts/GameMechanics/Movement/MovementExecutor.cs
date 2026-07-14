using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Walks a player spawn along a waypoint polyline. Replaces the grid system's
    /// LerpMovement / SegmentMovementInstance / CurveMoveSpeed trio with a single
    /// distance-cursor advance and smooth turn-while-walking. Runs no NavMesh queries
    /// at execution time, so a serialized waypoint list replays identically on the
    /// remote client.
    /// </summary>
    public class MovementExecutor : MonoBehaviour
    {
        [SerializeField] private float turnSpeedDegrees = 540f;
        [SerializeField] private float minimumMoveDuration = 0.4f;

        private Coroutine playbackCoroutine = null;
        private bool isPlaying = false;
        // Ability seam replacing the old Character.MovementIndex anim-curve swap (Ire).
        private float speedMultiplier = 1f;

        public static MovementExecutor Instance { get; private set; }

        public bool IsPlaying => isPlaying;

        public float SpeedMultiplier { get => speedMultiplier; set => speedMultiplier = value; }


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

        public void Play(GameObject spawnToMove, Character character, IReadOnlyList<Vector3> waypoints, Action onComplete)
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

            isPlaying = true;
            playbackCoroutine = StartCoroutine(MoveAlongPath(spawnToMove, character, waypoints, onComplete));
        }


        private IEnumerator MoveAlongPath(GameObject spawnToMove, Character character, IReadOnlyList<Vector3> waypoints, Action onComplete)
        {
            float totalLength = NavPathUtility.PathLength(new List<Vector3>(waypoints));
            float baseSpeed = character != null ? Mathf.Max(0.01f, character.BaseMoveSpeed) : 1f;

            // Respect a minimum duration so tiny moves still read as motion.
            float duration = Mathf.Max(minimumMoveDuration, totalLength / baseSpeed);
            float effectiveBaseSpeed = totalLength / duration;

            Animator animator = character != null && character.CharacterAnimationReferences != null
                ? character.CharacterAnimationReferences.CharacterAnimator
                : null;
            AnimationCurve speedCurve = character != null ? character.RunSpeedCurve : null;
            bool hasCurve = speedCurve != null && speedCurve.length > 0;

            var transformToMove = spawnToMove.transform;
            transformToMove.position = waypoints[0];

            float traveled = 0f;
            int segmentIndex = 1;
            float segmentStartDistance = 0f;
            float segmentLength = Vector3.Distance(waypoints[0], waypoints[1]);

            while (traveled < totalLength)
            {
                float t01 = totalLength > Mathf.Epsilon ? traveled / totalLength : 1f;
                float curveMultiplier = hasCurve ? Mathf.Max(0.05f, speedCurve.Evaluate(t01)) : 1f;
                float currentSpeed = effectiveBaseSpeed * curveMultiplier * speedMultiplier;

                traveled = Mathf.Min(totalLength, traveled + currentSpeed * Time.deltaTime);

                // Advance the segment cursor past any fully-consumed segments.
                while (segmentIndex < waypoints.Count - 1 && traveled > segmentStartDistance + segmentLength)
                {
                    segmentStartDistance += segmentLength;
                    segmentIndex++;
                    segmentLength = Vector3.Distance(waypoints[segmentIndex - 1], waypoints[segmentIndex]);
                }

                Vector3 from = waypoints[segmentIndex - 1];
                Vector3 to = waypoints[segmentIndex];
                float alongSegment = segmentLength > Mathf.Epsilon
                    ? Mathf.Clamp01((traveled - segmentStartDistance) / segmentLength)
                    : 1f;

                transformToMove.position = Vector3.Lerp(from, to, alongSegment);

                Vector3 flatDirection = to - from;
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
                    animator.SetFloat("CharSpeed", currentSpeed / effectiveBaseSpeed);
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
    }
}
