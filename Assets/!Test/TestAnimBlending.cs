using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimBlending : MonoBehaviour
{

    [SerializeField] private Animator myAnimator;
    [SerializeField] private Transform myChar;
    [SerializeField] private float offset = 3.0f;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myAnimator.SetTrigger("Lunge");
            StartCoroutine(Lerp());
        }
    }

    private IEnumerator Lerp()
    {
        var t = 0.0f;
        var lerpEnd = new Vector3(myChar.transform.position.x, myChar.transform.position.y, myChar.transform.position.z + offset);

        yield return new WaitForSecondsRealtime(0.5f);
        while (myChar.transform.position != lerpEnd)
        {
            t += Time.deltaTime;
            myChar.transform.position = Vector3.Lerp(myChar.transform.position, lerpEnd, t);
            yield return new WaitForSecondsRealtime(0.0075f);
        }
    }
}
