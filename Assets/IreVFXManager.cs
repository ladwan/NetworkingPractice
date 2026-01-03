using System;
using System.Collections;
using System.Collections.Generic;
using ForeverFight.Interactable.Abilities;
using UnityEditor;
using UnityEngine;

public class IreVFXManager : MonoBehaviour
{
    public Ire ireREF = null; //Get rid of this and use actual ability for the while loop
    
    private static readonly int BaseColor = Shader.PropertyToID("_Base_Color");
    [SerializeField] private Color endColor;
    [SerializeField] private Material mat;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private List<GameObject> steamParticles = null;
    
    
    private Coroutine sub = null;
    
    
    public void BeginCoroutine(Ire tempIreREF)
    {
        if (sub != null && ireREF == null)
        {
            return;
        }

        //ireREF = tempIreREF;
        sub = StartCoroutine(IreCoroutine(tempIreREF));
    }
    

    private IEnumerator IreCoroutine(Ire tempIreREF)
    {
        foreach (var particleGameObject in steamParticles)
        {
            particleGameObject.SetActive(true);
        }

        float t = 0.0f;
        //TODO: you need to find a way to ensure the abilty is activate and inactive but you cant
        // because its not active on the remote, you need to come up with a hacky way to fix this 
        while (tempIreREF.StatusActive || )
        {
            t += Time.deltaTime * pulseSpeed;

            // PingPong goes 0 → 1 → 0
            float lerpValue = Mathf.PingPong(t, 1f);

            Color pulsingColor = Color.Lerp(Color.white, endColor, lerpValue);

            mat.SetColor(BaseColor, pulsingColor);
            
            yield return new WaitForSeconds(0.1f);
        }
        
        foreach (var particleGameObject in steamParticles)
        {
            particleGameObject.SetActive(false);
        }
        
        sub = null;
        mat.SetColor(BaseColor,Color.white);
        yield return null;
    }

    private void OnDisable()
    {
        sub = null;
        mat.SetColor(BaseColor,Color.white);
    }

    private void OnDestroy()
    {
        sub = null;
        mat.SetColor(BaseColor,Color.white);
    }
}
