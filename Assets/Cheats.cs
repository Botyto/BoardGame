using UnityEngine;

public class Cheats : MonoBehaviour
{
#if UNITY_EDITOR
    private void Update()
    {
        TimeScale_Update();
    }

    #region Time Scale

    [SerializeField, GetSet("timeScale")]
    private float m_TimeScale = 1.0f;
    public float timeScale
    {
        get { return m_TimeScale; }
        set
        {
            if (Application.isPlaying) { Time.timeScale = value; } 
            m_TimeScale = value;
        }
    }

    private void TimeScale_Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            timeScale -= 1.0f;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            timeScale += 1.0f;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            if (Mathf.Abs(timeScale - 1.0f) > float.Epsilon)
            {
                timeScale = 1.0f;
            }
            else
            {
                timeScale = 5.0f;
            }
        }
    }

    #endregion

#endif
}
