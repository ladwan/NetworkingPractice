using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ForverFight.Ui
{
    public class ApLight : MonoBehaviour
    {
        [SerializeField]
        private Image apLightImage = null;


        public void StartBlink()
        {
            StartCoroutine(Blink());
        }

        public void StopBlink()
        {
            StopAllCoroutines();
        }

        private IEnumerator Blink()
        {
            yield return new WaitForSecondsRealtime(1);
            apLightImage.enabled = false;
            yield return new WaitForSecondsRealtime(1);
            apLightImage.enabled = true;
            StartCoroutine(Blink());
        }
    }
}