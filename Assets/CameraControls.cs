using System;
using System.Collections;
using UnityEngine;
using ForeverFight.Interactable.Abilities;
using Random = UnityEngine.Random;

public class CameraControls : MonoBehaviour
{
    [SerializeField] Transform cameraTransform = null;


    private static CameraControls instance = null;
    private CharAbility.CameraShakeParameters testParams = new CharAbility.CameraShakeParameters();


    public static CameraControls Instance { get => instance; set => instance = value; }


    protected void Awake()
    {
        if (!instance)
        {
            instance = this;
            testParams.duration = 1.5f;
            testParams.magnitude = 0.3f;
            return;
        }

        Destroy(this);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartShake(testParams);
        }
    }


    #region Shake Functionality
    public void StartShake(CharAbility.CameraShakeParameters parameters)
    {
        var originalPos = cameraTransform.localPosition;
        StartCoroutine(Shake(originalPos, parameters.duration, parameters.magnitude));
    }

    private IEnumerator Shake(Vector3 originalPos, float duration, float magnitude)
    {
        float elapsed = 0.0f;
        var curve = new AnimationCurve();
        curve = AnimationCurve.EaseInOut(0, magnitude, duration, 0);

        while (elapsed < duration)
        {
            float x = Random.Range(-1, 1.1f) * magnitude;
            float y = Random.Range(-1, 1.1f) * magnitude;

            cameraTransform.localPosition = new Vector3(x, y, cameraTransform.localPosition.z);
            //originalPos.x = cameraTransform.localPosition.x;

            magnitude = curve.Evaluate(elapsed);
            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.localPosition = originalPos;
    }
    #endregion
}
