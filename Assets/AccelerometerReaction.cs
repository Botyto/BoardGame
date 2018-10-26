using UnityEngine;
using UnityEngine.UI;

public class AccelerometerReaction : MonoBehaviour
{
    public Rigidbody body;
    public Text UI;
    public float multiplier = 1.0f;

    public void Update()
    {
        var accel = Vector3.zero;

        //if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            accel = Input.mousePosition;
        }
        // if (Application.platform == RuntimePlatform.Android)
        {
            accel = Input.acceleration;
        }

        body.velocity += accel;
        if (UI != null)
        {
            UI.text = string.Format("acceleration: {0}\nvelocity: {1}", accel, body.velocity);
        }
    }
}
