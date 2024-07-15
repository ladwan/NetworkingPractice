using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.GameMechanics.Movement;


namespace ForeverFight.HelperScripts
{
    public class GetRandomGridPoint : MonoBehaviour
    {
        //private FloorGrid floorGridREF = null;
        private List<GridPoint> randomGridPoints = new List<GridPoint>();


        public List<GridPoint> GeneranteListOfRandomGPs(int n)
        {
            ReturnRandomGridPoint(n);
            return randomGridPoints;
        }

        public void ClearList<T>(List<T> genericList)
        {
            genericList.Clear();
            randomGridPoints.Clear();
        }

        private void ReturnRandomGridPoint(int numberOfCalls)
        {
            int randomX = Random.Range(0, 10);
            int randomY = Random.Range(0, 10);
            Vector2 randomVector2 = new Vector2(randomX, randomY);
            GridPoint randomGridPoint = FloorGrid.Instance.GridDictionary[randomVector2];
            randomGridPoints.Add(randomGridPoint);
            numberOfCalls--;

            if (numberOfCalls > 0)
            {
                ReturnRandomGridPoint(numberOfCalls);
            }
        }
    }
}
