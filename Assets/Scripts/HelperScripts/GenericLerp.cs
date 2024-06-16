using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class GenericLerp : MonoBehaviour
    {
        private float elapsedTime = 0;
        private float percent = 0;
        private Coroutine sub = null;


        public void BeginLerpCoroutine(GameObject objToMove, Vector3 startingPos, Vector3 endingPos, float duration)
        {
            if (sub == null)
            {
                sub = StartCoroutine(LerpCoroutine(objToMove, startingPos, endingPos, duration));
            }
        }


        private IEnumerator LerpCoroutine(GameObject objToMove, Vector3 startingPos, Vector3 endingPos, float duration)
        {
            if (percent < 1)
            {
                elapsedTime += Time.deltaTime;
                percent = elapsedTime / duration;
                objToMove.transform.position = Vector3.Lerp(startingPos, endingPos, percent);
                yield return new WaitForEndOfFrame();
                StartCoroutine(LerpCoroutine(objToMove, startingPos, endingPos, duration));
            }
            else
            {
                elapsedTime = 0;
                percent = 0;
                sub = null;
            }
        }
    }
}
