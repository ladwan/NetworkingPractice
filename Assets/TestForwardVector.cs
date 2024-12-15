using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForwardVector : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float scalar = 0.0f;
    [SerializeField] Vector3 height = new();
    [SerializeField] Transform Point;

    [Space]
    [Header("Cool")]
    [SerializeField] Vector3 direction = new();
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 forwardVector = transform.forward;
        Debug.DrawLine(transform.position, transform.position + (height + (forwardVector * scalar)), Color.blue);

        direction = Point.position - transform.position;
        Debug.DrawLine(transform.position, direction + (height + (forwardVector * scalar)), Color.red);


        // Quaternion toRotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);
    }
}
