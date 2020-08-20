using UnityEngine;
using System.Collections;
using Core;
using System;

public class CameraShake : MonoBehaviour
{
    private float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    [SerializeField] private float shakeAmount = 0.7f;
    [SerializeField] private float decreaseFactor = 1.0f;

    Vector3 originalPos;


    void Start()
    {
        GameEventManager.Instance.AddListener<ShakeCameraEvent>(OnShakeTriggered);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.RemoveListener<ShakeCameraEvent>(OnShakeTriggered);
    }

    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }

    private void OnShakeTriggered(ShakeCameraEvent e)
    {
        shakeDuration = 0.3f;
    }
}