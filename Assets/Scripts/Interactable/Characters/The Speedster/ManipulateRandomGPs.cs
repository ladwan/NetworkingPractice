using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.HelperScripts;

public class ManipulateRandomGPs : MonoBehaviour
{
    [SerializeField]
    private GetRandomGridPoint getRandomGridPointREF = null;
    [SerializeField]
    private GameObject objToMove = null;


    private List<GridPoint> GPs = new List<GridPoint>();
    private Coroutine sub = null;
    private int index = 0;


    // Update is called once per frame
    void Update()
    {

    }

    protected void BeginCoroutine()
    {
        if (sub == null)
        {
            GPs = getRandomGridPointREF.GeneranteListOfRandomGPs(4);
            //sub = StartCoroutine(Move());
        }
    }




    private void Caller()
    {
        BeginCoroutine();
    }

    /*
    private IEnumerator Move()
    {
        objToMove.transform.position =
        yield return new WaitForSecondsRealtime(1);
    }
    */
}
