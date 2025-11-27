using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveShaderManager : ShaderManager
{
    [SerializeField] private GameObject vfxParent;
    [SerializeField] private Material shaderMat;
    [SerializeField] private float smashSpeed;
    [SerializeField] private float fadeSpeed;


    private Coroutine sub = null;
    private float initialAlphaValue;
    private float startingAlpha;


    public GameObject VFXParent => vfxParent;


    public override void BeginVFX()
    {
        if (sub != null) return;

        sub = StartCoroutine(RunShaderLogic());
    }


    private void OnEnable()
    {
        startingAlpha = shaderMat.GetFloat("_AlphaFactor");
        shaderMat.SetFloat("_EffectValue", 0);
    }

    private void OnDisable()
    {
        shaderMat.SetFloat("_AlphaFactor", startingAlpha);
        shaderMat.SetFloat("_EffectValue", 0);
    }

    private IEnumerator RunShaderLogic()
    {
        var elapsed = 0f;
        var value = 0f;
        vfxParent.SetActive(true);

        while (elapsed < smashSpeed)
        {
            value = Mathf.Lerp(0,1, elapsed/smashSpeed);
            elapsed += Time.deltaTime;

            shaderMat.SetFloat("_EffectValue", value);
            yield return null;
        }

        elapsed = 0f;
        value = 0f;
        initialAlphaValue = startingAlpha;

        while (elapsed < fadeSpeed)
        {
            value = Mathf.Lerp(startingAlpha, 0, elapsed / fadeSpeed);
            elapsed += Time.deltaTime;

            shaderMat.SetFloat("_AlphaFactor", value);
            yield return null;
        }

        vfxParent.SetActive(false);
        shaderMat.SetFloat("_AlphaFactor", initialAlphaValue);
        shaderMat.SetFloat("_EffectValue", 0);
        sub = null;
    }
}
