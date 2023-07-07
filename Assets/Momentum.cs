using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Momentum : MonoBehaviour
{
    [SerializeField]
    private int storedMomentum = 0;
    [SerializeField]
    private int abilityDuration = 2;
    [SerializeField]
    private float multiplier = 2f;
    [SerializeField]
    private Tester testerREF = null;


    public int StoredMomentum { get => storedMomentum; set => storedMomentum = value; }


    protected void OnEnable()
    {
        testerREF.OnTesterCalled += ApplyStatusEffect;
    }

    protected void OnDisable()
    {
        testerREF.OnTesterCalled -= ApplyStatusEffect;
    }


    public void ApplyStatusEffect()
    {
        if (abilityDuration > 0)
        {
            //Get the value of sq's the user has moved since activation, this can be found in the floor grids hovered over grid points
            //Take that value and multiply it by the multiplier float
            //store that product in a variable


            abilityDuration--;
        }
        else
        {
            //if the duration has ended reset all stored momentum and reset the product variable
        }
    }

    //We need an int that keeps track of how many sqs the player has moved since activating the ability 
    //We need to make a system ability activated for multiple turns
    //We need to decide how many turns this ability will last for and store that in an int
}
