using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    [SerializeField] private List<Transform> transforms = null;
    [SerializeField] private Transform transformToMove = null;
    [SerializeField] private float speed = 1;
    [SerializeField] private float rotationSpeed = 1;

    private Coroutine sub = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeginCoroutine();
        }
    }

    private void BeginCoroutine()
    {
        if (sub != null)
        {
            return;
        }

        sub = StartCoroutine(Move());
    }


    private IEnumerator Move()
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            if (i + 1 >= transforms.Count)
            {
                continue;
            }

            Vector3 pos1 = transformToMove.position;
            Vector3 pos2 = transforms[i + 1].position;

            Vector3 directionToTarget = pos2 - transformToMove.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            Debug.Log($"Angle: {targetRotation.eulerAngles}");

            Debug.DrawLine(transform.position, directionToTarget * 5, Color.red);

            //Rotate
            while (transformToMove.rotation != targetRotation)
            {
                transformToMove.rotation = Quaternion.Slerp(transformToMove.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                yield return new WaitForSecondsRealtime(0.01f);
            }

            //Translate
            var t = 0.0f;
            while (transformToMove.position != pos2)
            {
                t += Time.deltaTime * speed;
                t = Mathf.Clamp01(t);
                transformToMove.position = Vector3.Lerp(pos1, pos2, t);
                yield return new WaitForSecondsRealtime(0.01f);
            }

        }

        sub = null;
    }
}
