using UnityEngine;

public class SmoothLookAt : MonoBehaviour
{
    public Transform target; // The target to look at
    public float rotationSpeed = 5f; // Speed of rotation

    void Update()
    {
        if (target != null)
        {
            // Calculate the direction to the target
            Vector3 directionToTarget = target.position - transform.position;

            // Ensure the direction vector is not zero to avoid invalid rotations
            if (directionToTarget != Vector3.zero)
            {
                // Calculate the target rotation
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                if (transform.rotation != targetRotation)
                {
                    // Smoothly interpolate to the target rotation
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }
    }
}