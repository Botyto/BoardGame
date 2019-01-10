using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AccelerometerReaction : MonoBehaviour
{
    public Rigidbody body;
    public Text UI;
    public float multiplier = 1.0f;

    //    public void Update()
    //    {
    //        var accel = Vector3.zero;

    //        //if (Application.platform == RuntimePlatform.WindowsEditor)
    //        {
    //            accel = Input.mousePosition;
    //        }
    //        // if (Application.platform == RuntimePlatform.Android)
    //        {
    //            accel = Input.acceleration;
    //        }

    //        body.velocity += accel;
    //        if (UI != null)
    //        {
    //            UI.text = string.Format("acceleration: {0}\nvelocity: {1}", accel, body.velocity);
    //        }
    //    }

    float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
    float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation,
    // or at least according to Brady! ;)
    float shakeDetectionThreshold = 1.0f;

    float lowPassFilterFactor;
    Vector3 lowPassValue;


    void Start()
    {
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    public IEnumerator WaitForShake()
    {
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        while (deltaAcceleration.sqrMagnitude < shakeDetectionThreshold)
        {
            acceleration = Input.acceleration;
            lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
            deltaAcceleration = acceleration - lowPassValue;
            yield return null;
        }
    }
}
