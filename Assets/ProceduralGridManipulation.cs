using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.HelperScripts
{
    public class ProceduralGridManipulation : MonoBehaviour
    {
        [SerializeField]
        private Vector2 startingPointCoordinates = Vector2.zero;
        [SerializeField]
        private Vector2 endingPointCoordinates = Vector2.zero;
        [SerializeField]
        private List<Vector2> uniqueTagKeys = new List<Vector2>();


        private Action enemyHitAWallAction = null;
        private bool doOnceUntilReset = false;


        public Vector2 StartingPointCoordinates { get => startingPointCoordinates; set => startingPointCoordinates = value; }

        public Vector2 EndingPointCoordinates { get => endingPointCoordinates; set => endingPointCoordinates = value; }

        public Action EnemyHitAWallAction { get => enemyHitAWallAction; set => enemyHitAWallAction = value; }


        public void PullEnemy(int pullAmount)
        {
            var pathFromUsToEnemy = ReturnProceduralPath();
            if (pathFromUsToEnemy != null && pathFromUsToEnemy.Count > 0)
            {
                var updatedEnemyVector2IndexAfterPull = DeterminePull(pathFromUsToEnemy, pullAmount);
                Vector2 gridPointCoordinatesPlayerWillEndOnAfterPull = pathFromUsToEnemy[updatedEnemyVector2IndexAfterPull];
                ClientSend.OverrideOppositePlayersPostition((int)gridPointCoordinatesPlayerWillEndOnAfterPull.x, (int)gridPointCoordinatesPlayerWillEndOnAfterPull.y);
            }
        }

        public void KnockbackEnemy(int knockbackAmount)
        {
            var pathFromUsToEnemy = ReturnProceduralPath();
            var direction = CalculateDifference(pathFromUsToEnemy[0], pathFromUsToEnemy[1]); //We know our local player will occupy the first gp in this list, and that the enemy will occupy the 2nd
            Knockback(direction, knockbackAmount);
        }

        public void IsVector2AValidGridPoint(Vector2 newCoordinates) // Does this Vector 2 corespond to one of out gridPoints in the gridpointDictionary? if so use it to override the movement
        {
            if (FloorGrid.Instance.GridDictionary.TryGetValue(newCoordinates, out GridPoint posAfterPull))
            {
                FloorGrid.Instance.OverridePlayerPosition(posAfterPull);
                return;
            }

            Debug.LogError("Not a Vaild GP");
        }

        public List<Vector2> ReturnProceduralPath()
        {
            UpdatePlayerPositionVector2s();

            if (FloorGrid.Instance.GridDictionary.TryGetValue(startingPointCoordinates, out GridPoint startingGP) && FloorGrid.Instance.GridDictionary.TryGetValue(endingPointCoordinates, out GridPoint endingGP))
            {
                return GenerateProceduralPath(startingGP, endingGP);
            }

            Debug.LogError("Invalid starting or ending Vector 2");
            return null;
        }


        private List<Vector2> GenerateProceduralPath(GridPoint startPoint, GridPoint endPoint)
        {
            var orderedPairList = new List<Vector2>();
            orderedPairList.Add(startPoint.UniqueTag);
            var orderedPairToUpdate = startPoint.UniqueTag;

            while (orderedPairToUpdate != endPoint.UniqueTag)
            {
                var tempX = ReturnUpdatedVectorCoordinate(orderedPairToUpdate.x, endPoint.UniqueTag.x);
                var tempY = ReturnUpdatedVectorCoordinate(orderedPairToUpdate.y, endPoint.UniqueTag.y);
                orderedPairToUpdate = new Vector2(tempX, tempY);

                orderedPairList.Add(orderedPairToUpdate);
            }

            return orderedPairList;
        }

        private float ReturnUpdatedVectorCoordinate(float startPoint, float endPoint)
        {
            if (startPoint < endPoint)
            {
                return startPoint += 1;
            }
            if (startPoint > endPoint)
            {
                return startPoint -= 1;
            }

            Debug.Log("Points were equal to eachother");
            return startPoint;
        }


        private int DeterminePull(List<Vector2> path, int pullAmount)
        {
            var currentEnemyPos = path.Count - 1;
            return currentEnemyPos -= pullAmount;
        }

        private Vector2 CalculateDifference(Vector2 startingGpCoordinate, Vector2 coordinateInDirectionOfEnemy)
        {
            return coordinateInDirectionOfEnemy - startingGpCoordinate;
        }

        private void Knockback(Vector2 direction, int knockbackValue)
        {
            Vector2 nextGpInKnockbackOrder = Vector2.zero;
            Vector2 lastValidGp = Vector2.zero;

            if (FloorGrid.Instance.GridDictionary.TryGetValue(startingPointCoordinates, out GridPoint startingGP))
            {
                nextGpInKnockbackOrder = startingGP.UniqueTag;
            }

            for (int i = knockbackValue; i > 0; i--)
            {
                if (FloorGrid.Instance.GridDictionary.TryGetValue(nextGpInKnockbackOrder += direction, out GridPoint gp))
                {
                    lastValidGp = gp.UniqueTag;
                }
                else
                {
                    enemyHitAWallAction?.Invoke();
                    Debug.Log("Enemy Hit A Wall!");
                    break;
                }
            }

            ClientSend.OverrideOppositePlayersPostition((int)lastValidGp.x, (int)lastValidGp.y);
        }

        private void UpdatePlayerPositionVector2s()
        {
            if (ClientInfo.playerNumber == 1)
            {
                startingPointCoordinates = Vector3ToVector2.ConvertToVector2(FloorGrid.Instance.Player1Spawn.transform.position);
                endingPointCoordinates = Vector3ToVector2.ConvertToVector2(FloorGrid.Instance.Player2Spawn.transform.position);
            }
            else
            {
                startingPointCoordinates = Vector3ToVector2.ConvertToVector2(FloorGrid.Instance.Player2Spawn.transform.position);
                endingPointCoordinates = Vector3ToVector2.ConvertToVector2(FloorGrid.Instance.Player1Spawn.transform.position);
            }

        }
    }
}
