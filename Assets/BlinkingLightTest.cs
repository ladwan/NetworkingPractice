using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BlinkingLightTest : MonoBehaviour
{
    [SerializeField] private Light blinkingLight;

    private bool isRunning = false;
   
    private void Update()
    {
        if (!isRunning)
        {
            var isBlinking = UnityEngine.Random.Range(0, 1f);

            var state = isBlinking > 0.5f ? Blink() : Stable();

            StartCoroutine(state);
            isRunning = true;
        }
    }


    private IEnumerator Blink()
    {
        var lifeTime = UnityEngine.Random.Range(0.5f, 1.5f);

        while (lifeTime > 0) 
        {
            blinkingLight.intensity = UnityEngine.Random.Range(0, 25f);
            lifeTime -= Time.deltaTime;
            yield return null;
        }

        isRunning = false;
    }

    private IEnumerator Stable()
    {
        var lifeTime = UnityEngine.Random.Range(5, 10f);

        while (lifeTime > 0)
        {
            blinkingLight.intensity = 25f;
            lifeTime -= Time.deltaTime;
            yield return null;
        }

        isRunning = false;
    }
}
