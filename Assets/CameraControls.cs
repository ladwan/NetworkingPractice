using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraControls : MonoBehaviour
{
    [SerializeField] Transform cameraTransform = null;


    private static CameraControls instance = null;


    public static CameraControls Instance { get => instance; set => instance = value; }


    protected void Awake()
    {
        if (!instance)
        {
            instance = this;
            return;
        }

        Destroy(this);
    }


    #region Zoom Functionality
    public void Zoom(float startDelay, float distace, float speed, float duration)
    {
        var originalPos = cameraTransform.position;
        var zoomedPos = new Vector3(cameraTransform.position.x + distace, cameraTransform.position.y, cameraTransform.position.z);
        StartCoroutine(ZoomLerp(startDelay, zoomedPos, originalPos, duration, speed));
    }

    //Ensure speed != 0
    private IEnumerator ZoomLerp(float startDelay, Vector3 zoomedPos, Vector3 originalPos, float duration, float speed)
    {
        yield return new WaitForSecondsRealtime(startDelay);

        float percent = 0;

        while (Math.Round(cameraTransform.position.x, 2) != Math.Round(zoomedPos.x, 2))
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, zoomedPos, percent);
            percent += Time.deltaTime * speed;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        percent = 0;
        yield return new WaitForSecondsRealtime(duration);

        while (Math.Round(cameraTransform.position.x, 3) != Math.Round(originalPos.x, 3))
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, originalPos, percent);
            percent += Time.deltaTime * speed;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    #endregion


    #region Shake Functionality
    public void StartShake(float startDelay, float duration, float magnitude)
    {
        var originalPos = cameraTransform.localPosition;
        StartCoroutine(Shake(startDelay, originalPos, duration, magnitude));
    }

    private IEnumerator Shake(float startDelay, Vector3 originalPos, float duration, float magnitude)
    {
        yield return new WaitForSecondsRealtime(startDelay);

        float elapsed = 0.0f;
        var curve = new AnimationCurve();
        curve = AnimationCurve.EaseInOut(0, magnitude, duration, 0);

        while (elapsed < duration)
        {
            float z = Random.Range(-1, 1.1f) * magnitude;
            float y = Random.Range(-1, 1.1f) * magnitude;

            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, y, z);
            originalPos.x = cameraTransform.localPosition.x;

            magnitude = curve.Evaluate(elapsed);
            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.localPosition = originalPos;
    }
    #endregion


    #region Rotate Functionality
    public IEnumerator Rotate(float startDelay, Vector3 offset, float speed, float duration)
    {
        yield return new WaitForSecondsRealtime(startDelay);

        float percent = 0;
        var originalEulerAngles = cameraTransform.localEulerAngles;

        while (Math.Truncate(cameraTransform.localEulerAngles.y) != Math.Truncate(offset.y))
        {
            cameraTransform.localEulerAngles = Vector3.Lerp(originalEulerAngles, offset, percent);
            percent += Time.deltaTime * speed;
            yield return null;
        }

        percent = 0;
        yield return new WaitForSecondsRealtime(duration);
        var offsetEulerAngles = cameraTransform.localEulerAngles;

        while (Math.Truncate(cameraTransform.localEulerAngles.y) != Math.Truncate(originalEulerAngles.y))
        {
            cameraTransform.localEulerAngles = Vector3.Lerp(offsetEulerAngles, originalEulerAngles, percent);
            percent += Time.deltaTime * speed;
            yield return null;
        }

        cameraTransform.localEulerAngles = originalEulerAngles;
    }

    public IEnumerator Rotate360(float startDelay, float speed)
    {
        yield return new WaitForSecondsRealtime(startDelay);

        Quaternion originalRotation = cameraTransform.rotation;
        float startRotation = cameraTransform.eulerAngles.y;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t) % 360.0f;
            cameraTransform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
        }

        cameraTransform.rotation = originalRotation;
    }
    #endregion

}
