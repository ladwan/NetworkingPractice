using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class GetRandomGridPoint : MonoBehaviour
    {
        private FloorGrid floorGridREF = null;
        private List<GridPoint> randomGridPoints = new List<GridPoint>();


        protected void OnEnable()
        {
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                floorGridREF = FloorGrid.Instance;
            }
        }


        public List<GridPoint> GeneranteListOfRandomGPs(int n)
        {
            ReturnRandomGridPoint(n);
            return randomGridPoints;
        }


        private void ReturnRandomGridPoint(int numberOfCalls)
        {
            int randomX = Random.Range(0, 10);
            int randomY = Random.Range(0, 10);
            Vector2 randomVector2 = new Vector2(randomX, randomY);
            GridPoint randomGridPoint = floorGridREF.GridDictionary[randomVector2];
            randomGridPoints.Add(randomGridPoint);
            numberOfCalls--;

            if (numberOfCalls > 0)
            {
                ReturnRandomGridPoint(numberOfCalls);
            }
        }
    }
}
