using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpecificUi : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> charactersUi = new List<GameObject>();


    public List<GameObject> CharactersUi => charactersUi;
}
