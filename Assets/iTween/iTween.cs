using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>Version: 2.0.7</para>
/// <para>Author: Bob Berkebile (http://pixelplacement.com)</para>
/// <para>Support: http://itween.pixelplacement.com</para>
/// </summary>
public partial class iTween : MonoBehaviour
{
    #region Variables

    //repository of all living iTweens:
    public static List<Hashtable> tweens = new List<Hashtable>();

    //status members (made public for visual troubleshooting in the inspector):
    [Header("Tween")]
    public string id;
    public string tweenName;
    public string type;
    public string method;
    public EaseType easeType;
    [Header("Timing")]
    public float time;
    public float delay;
    [Header("Play Control")]
    public LoopType loopType;
    public bool isRunning;
    public bool isPaused;
    
    //private members:
    private delegate float EasingFunction(float start, float end, float Value);
    private delegate void ApplyTween();

    private float m_RunningTime;
    private float m_Percentage;
    private float m_DelayStarted; //probably not neccesary that this be protected but it shuts Unity's compiler up about this being "never used"
    private bool m_Kinematic;
    private bool m_IsLocal;
    private bool m_Loop;
    private bool m_Reverse;
    private bool m_WasPaused;
    private bool m_Physics;
    private Hashtable m_TweenArguments;
    private Space m_Space;
    private EasingFunction m_EaseFunc;
    private ApplyTween m_ApplyFunc;
    private AudioSource m_AudioSource;
    private Vector3[] m_Vector3s;
    private Vector2[] m_Vector2s;
    private Color[,] m_Colors;
    private float[] m_Floats;
    private Rect[] m_Rects;
    private CRSpline m_Path;
    private Vector3 m_PreUpdate;
    private Vector3 m_PostUpdate;
    private NamedValueColor m_Namedcolorvalue;

    private float m_LastRealTime;
    private bool m_UseRealTime;

    private Transform m_ThisTransform;
    
    #endregion

    #region Defaults

    /// <summary>
    /// A collection of baseline presets that iTween needs and utilizes if certain parameters are not provided. 
    /// </summary>
    public static class Defaults
    {
        //general defaults:
        public static float time = 1f;
        public static float delay = 0f;
        public static NamedValueColor namedColorValue = NamedValueColor._Color;
        public static LoopType loopType = LoopType.None;
        public static EaseType easeType = EaseType.EaseOutExpo;
        public static float lookSpeed = 3f;
        public static bool isLocal = false;
        public static Space space = Space.Self;
        public static bool orientToPath = false;
        public static Color color = Color.white;
        //update defaults:
        public static float updateTimePercentage = .05f;
        public static float updateTime = 1f * updateTimePercentage;
        //path look ahead amount:
        public static float lookAhead = .05f;
        public static bool useRealTime = false;
        //look direction:
        public static Vector3 up = Vector3.up;
    }

    #endregion
    
    #region #1 Generate Method Targets

    //call correct set target method and set tween application delegate:
    void GenerateTargets()
    {
        switch (type)
        {
            case "value":
                switch (method)
                {
                    case "float":
                        GenerateFloatTargets();
                        m_ApplyFunc = ApplyFloatTargets;
                        break;
                    case "vector2":
                        GenerateVector2Targets();
                        m_ApplyFunc = ApplyVector2Targets;
                        break;
                    case "vector3":
                        GenerateVector3Targets();
                        m_ApplyFunc = ApplyVector3Targets;
                        break;
                    case "color":
                        GenerateColorTargets();
                        m_ApplyFunc = ApplyColorTargets;
                        break;
                    case "rect":
                        GenerateRectTargets();
                        m_ApplyFunc = ApplyRectTargets;
                        break;
                }
                break;
            case "color":
                switch (method)
                {
                    case "to":
                        GenerateColorToTargets();
                        m_ApplyFunc = ApplyColorToTargets;
                        break;
                }
                break;
            case "audio":
                switch (method)
                {
                    case "to":
                        GenerateAudioToTargets();
                        m_ApplyFunc = ApplyAudioToTargets;
                        break;
                }
                break;
            case "move":
                switch (method)
                {
                    case "to":
                        //using a path?
                        if (m_TweenArguments.Contains("path"))
                        {
                            GenerateMoveToPathTargets();
                            m_ApplyFunc = ApplyMoveToPathTargets;
                        }
                        else
                        {
                            //not using a path?
                            GenerateMoveToTargets();
                            m_ApplyFunc = ApplyMoveToTargets;
                        }
                        break;
                    case "by":
                    case "add":
                        GenerateMoveByTargets();
                        m_ApplyFunc = ApplyMoveByTargets;
                        break;
                }
                break;
            case "scale":
                switch (method)
                {
                    case "to":
                        GenerateScaleToTargets();
                        m_ApplyFunc = ApplyScaleToTargets;
                        break;
                    case "by":
                        GenerateScaleByTargets();
                        m_ApplyFunc = ApplyScaleToTargets;
                        break;
                    case "add":
                        GenerateScaleAddTargets();
                        m_ApplyFunc = ApplyScaleToTargets;
                        break;
                }
                break;
            case "rotate":
                switch (method)
                {
                    case "to":
                        GenerateRotateToTargets();
                        m_ApplyFunc = ApplyRotateToTargets;
                        break;
                    case "add":
                        GenerateRotateAddTargets();
                        m_ApplyFunc = ApplyRotateAddTargets;
                        break;
                    case "by":
                        GenerateRotateByTargets();
                        m_ApplyFunc = ApplyRotateAddTargets;
                        break;
                }
                break;
            case "shake":
                switch (method)
                {
                    case "position":
                        GenerateShakePositionTargets();
                        m_ApplyFunc = ApplyShakePositionTargets;
                        break;
                    case "scale":
                        GenerateShakeScaleTargets();
                        m_ApplyFunc = ApplyShakeScaleTargets;
                        break;
                    case "rotation":
                        GenerateShakeRotationTargets();
                        m_ApplyFunc = ApplyShakeRotationTargets;
                        break;
                }
                break;
            case "punch":
                switch (method)
                {
                    case "position":
                        GeneratePunchPositionTargets();
                        m_ApplyFunc = ApplyPunchPositionTargets;
                        break;
                    case "rotation":
                        GeneratePunchRotationTargets();
                        m_ApplyFunc = ApplyPunchRotationTargets;
                        break;
                    case "scale":
                        GeneratePunchScaleTargets();
                        m_ApplyFunc = ApplyPunchScaleTargets;
                        break;
                }
                break;
            case "look":
                switch (method)
                {
                    case "to":
                        GenerateLookToTargets();
                        m_ApplyFunc = ApplyLookToTargets;
                        break;
                }
                break;
            case "stab":
                GenerateStabTargets();
                m_ApplyFunc = ApplyStabTargets;
                break;
        }
    }

    #endregion

    #region #2 Generate Specific Targets

    void GenerateRectTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Rects = new Rect[3];

        //from and to values:
        m_Rects[0] = (Rect)m_TweenArguments["from"];
        m_Rects[1] = (Rect)m_TweenArguments["to"];
    }

    void GenerateColorTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Colors = new Color[1, 3];

        //from and to values:
        m_Colors[0, 0] = (Color)m_TweenArguments["from"];
        m_Colors[0, 1] = (Color)m_TweenArguments["to"];
    }

    void GenerateVector3Targets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector3s = new Vector3[3];

        //from and to values:
        m_Vector3s[0] = (Vector3)m_TweenArguments["from"];
        m_Vector3s[1] = (Vector3)m_TweenArguments["to"];

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateVector2Targets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector2s = new Vector2[3];

        //from and to values:
        m_Vector2s[0] = (Vector2)m_TweenArguments["from"];
        m_Vector2s[1] = (Vector2)m_TweenArguments["to"];

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            Vector3 fromV3 = new Vector3(m_Vector2s[0].x, m_Vector2s[0].y, 0);
            Vector3 toV3 = new Vector3(m_Vector2s[1].x, m_Vector2s[1].y, 0);
            float distance = Math.Abs(Vector3.Distance(fromV3, toV3));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateFloatTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Floats = new float[3];

        //from and to values:
        m_Floats[0] = (float)m_TweenArguments["from"];
        m_Floats[1] = (float)m_TweenArguments["to"];

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(m_Floats[0] - m_Floats[1]);
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateColorToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        //colors = new Color[3];

        //from and init to values:
        if (GetComponent<Renderer>())
        {
            m_Colors = new Color[GetComponent<Renderer>().materials.Length, 3];
            for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
            {
                m_Colors[i, 0] = GetComponent<Renderer>().materials[i].GetColor(m_Namedcolorvalue.ToString());
                m_Colors[i, 1] = GetComponent<Renderer>().materials[i].GetColor(m_Namedcolorvalue.ToString());
            }
            //colors[0] = colors[1] = renderer.material.color;	
        }
        else if (GetComponent<Light>())
        {
            m_Colors = new Color[1, 3];
            m_Colors[0, 0] = m_Colors[0, 1] = GetComponent<Light>().color;
        }
        else
        {
            m_Colors = new Color[1, 3]; //empty placeholder incase the GO is perhaps an empty holder or something similar
        }

        //to values:
        if (m_TweenArguments.Contains("color"))
        {
            //colors[1]=(Color)tweenArguments["color"];
            for (int i = 0; i < m_Colors.GetLength(0); i++)
            {
                m_Colors[i, 1] = (Color)m_TweenArguments["color"];
            }
        }
        else
        {
            if (m_TweenArguments.Contains("r"))
            {
                //colors[1].r=(float)tweenArguments["r"];
                for (int i = 0; i < m_Colors.GetLength(0); i++)
                {
                    m_Colors[i, 1].r = (float)m_TweenArguments["r"];
                }
            }
            if (m_TweenArguments.Contains("g"))
            {
                //colors[1].g=(float)tweenArguments["g"];
                for (int i = 0; i < m_Colors.GetLength(0); i++)
                {
                    m_Colors[i, 1].g = (float)m_TweenArguments["g"];
                }
            }
            if (m_TweenArguments.Contains("b"))
            {
                //colors[1].b=(float)tweenArguments["b"];
                for (int i = 0; i < m_Colors.GetLength(0); i++)
                {
                    m_Colors[i, 1].b = (float)m_TweenArguments["b"];
                }
            }
            if (m_TweenArguments.Contains("a"))
            {
                //colors[1].a=(float)tweenArguments["a"];
                for (int i = 0; i < m_Colors.GetLength(0); i++)
                {
                    m_Colors[i, 1].a = (float)m_TweenArguments["a"];
                }
            }
        }

        //alpha or amount?
        if (m_TweenArguments.Contains("amount"))
        {
            //colors[1].a=(float)tweenArguments["amount"];
            for (int i = 0; i < m_Colors.GetLength(0); i++)
            {
                m_Colors[i, 1].a = (float)m_TweenArguments["amount"];
            }
        }
        else if (m_TweenArguments.Contains("alpha"))
        {
            //colors[1].a=(float)tweenArguments["alpha"];
            for (int i = 0; i < m_Colors.GetLength(0); i++)
            {
                m_Colors[i, 1].a = (float)m_TweenArguments["alpha"];
            }
        }
    }

    void GenerateAudioToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector2s = new Vector2[3];

        //set audioSource:
        if (m_TweenArguments.Contains("audiosource"))
        {
            m_AudioSource = (AudioSource)m_TweenArguments["audiosource"];
        }
        else
        {
            if (GetComponent<AudioSource>())
            {
                m_AudioSource = GetComponent<AudioSource>();
            }
            else
            {
                //throw error if no AudioSource is available:
                Debug.LogError("iTween Error: AudioTo requires an AudioSource.");
                Dispose();
            }
        }

        //from values and default to values:
        m_Vector2s[0] = m_Vector2s[1] = new Vector2(m_AudioSource.volume, m_AudioSource.pitch);

        //to values:
        if (m_TweenArguments.Contains("volume"))
        {
            m_Vector2s[1].x = (float)m_TweenArguments["volume"];
        }
        if (m_TweenArguments.Contains("pitch"))
        {
            m_Vector2s[1].y = (float)m_TweenArguments["pitch"];
        }
    }

    void GenerateStabTargets()
    {
        //set audioSource:
        if (m_TweenArguments.Contains("audiosource"))
        {
            m_AudioSource = (AudioSource)m_TweenArguments["audiosource"];
        }
        else
        {
            if (GetComponent<AudioSource>())
            {
                m_AudioSource = GetComponent<AudioSource>();
            }
            else
            {
                //add and populate AudioSource if one doesn't exist:
                gameObject.AddComponent<AudioSource>();
                m_AudioSource = GetComponent<AudioSource>();
                m_AudioSource.playOnAwake = false;

            }
        }

        //populate audioSource's clip:
        m_AudioSource.clip = (AudioClip)m_TweenArguments["audioclip"];

        //set audio's pitch and volume if requested:
        if (m_TweenArguments.Contains("pitch"))
        {
            m_AudioSource.pitch = (float)m_TweenArguments["pitch"];
        }
        if (m_TweenArguments.Contains("volume"))
        {
            m_AudioSource.volume = (float)m_TweenArguments["volume"];
        }

        //set run time based on length of clip after pitch is augmented
        time = m_AudioSource.clip.length / m_AudioSource.pitch;
    }

    void GenerateLookToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector3s = new Vector3[3];

        //from values:
        m_Vector3s[0] = m_ThisTransform.eulerAngles;

        //set look:
        if (m_TweenArguments.Contains("looktarget"))
        {
            if (m_TweenArguments["looktarget"].GetType() == typeof(Transform))
            {
                //transform.LookAt((Transform)tweenArguments["looktarget"]);
                m_ThisTransform.LookAt((Transform)m_TweenArguments["looktarget"], (Vector3?)m_TweenArguments["up"] ?? Defaults.up);
            }
            else if (m_TweenArguments["looktarget"].GetType() == typeof(Vector3))
            {
                //transform.LookAt((Vector3)tweenArguments["looktarget"]);
                m_ThisTransform.LookAt((Vector3)m_TweenArguments["looktarget"], (Vector3?)m_TweenArguments["up"] ?? Defaults.up);
            }
        }
        else
        {
            Debug.LogError("iTween Error: LookTo needs a 'looktarget' property!");
            Dispose();
        }

        //to values:
        m_Vector3s[1] = m_ThisTransform.eulerAngles;
        m_ThisTransform.eulerAngles = m_Vector3s[0];

        //axis restriction:
        if (m_TweenArguments.Contains("axis"))
        {
            switch ((string)m_TweenArguments["axis"])
            {
                case "x":
                    m_Vector3s[1].y = m_Vector3s[0].y;
                    m_Vector3s[1].z = m_Vector3s[0].z;
                    break;
                case "y":
                    m_Vector3s[1].x = m_Vector3s[0].x;
                    m_Vector3s[1].z = m_Vector3s[0].z;
                    break;
                case "z":
                    m_Vector3s[1].x = m_Vector3s[0].x;
                    m_Vector3s[1].y = m_Vector3s[0].y;
                    break;
            }
        }

        //shortest distance:
        m_Vector3s[1] = new Vector3(clerp(m_Vector3s[0].x, m_Vector3s[1].x, 1), clerp(m_Vector3s[0].y, m_Vector3s[1].y, 1), clerp(m_Vector3s[0].z, m_Vector3s[1].z, 1));

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateMoveToPathTargets()
    {
        Vector3[] suppliedPath;

        //create and store path points:
        if (m_TweenArguments["path"].GetType() == typeof(Vector3[]))
        {
            Vector3[] temp = (Vector3[])m_TweenArguments["path"];
            //if only one point is supplied fall back to MoveTo's traditional use since we can't have a curve with one value:
            if (temp.Length == 1)
            {
                Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
                Dispose();
            }
            suppliedPath = new Vector3[temp.Length];
            Array.Copy(temp, suppliedPath, temp.Length);
        }
        else
        {
            Transform[] temp = (Transform[])m_TweenArguments["path"];
            //if only one point is supplied fall back to MoveTo's traditional use since we can't have a curve with one value:
            if (temp.Length == 1)
            {
                Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
                Dispose();
            }
            suppliedPath = new Vector3[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                suppliedPath[i] = temp[i].position;
            }
        }

        //do we need to plot a path to get to the beginning of the supplied path?		
        bool plotStart;
        int offset;
        if (m_ThisTransform.position != suppliedPath[0])
        {
            if (!m_TweenArguments.Contains("movetopath") || (bool)m_TweenArguments["movetopath"] == true)
            {
                plotStart = true;
                offset = 3;
            }
            else
            {
                plotStart = false;
                offset = 2;
            }
        }
        else
        {
            plotStart = false;
            offset = 2;
        }

        //build calculated path:
        m_Vector3s = new Vector3[suppliedPath.Length + offset];
        if (plotStart)
        {
            m_Vector3s[1] = m_ThisTransform.position;
            offset = 2;
        }
        else
        {
            offset = 1;
        }

        //populate calculate path;
        Array.Copy(suppliedPath, 0, m_Vector3s, offset, suppliedPath.Length);

        //populate start and end control points:
        //vector3s[0] = vector3s[1] - vector3s[2];
        m_Vector3s[0] = m_Vector3s[1] + (m_Vector3s[1] - m_Vector3s[2]);
        m_Vector3s[m_Vector3s.Length - 1] = m_Vector3s[m_Vector3s.Length - 2] + (m_Vector3s[m_Vector3s.Length - 2] - m_Vector3s[m_Vector3s.Length - 3]);

        //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
        if (m_Vector3s[1] == m_Vector3s[m_Vector3s.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[m_Vector3s.Length];
            Array.Copy(m_Vector3s, tmpLoopSpline, m_Vector3s.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            m_Vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, m_Vector3s, tmpLoopSpline.Length);
        }

        //create Catmull-Rom path:
        m_Path = new CRSpline(m_Vector3s);

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = PathLength(m_Vector3s);
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateMoveToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector3s = new Vector3[3];

        //from values:
        if (m_IsLocal)
        {
            m_Vector3s[0] = m_Vector3s[1] = m_ThisTransform.localPosition;
        }
        else
        {
            m_Vector3s[0] = m_Vector3s[1] = m_ThisTransform.position;
        }

        //to values:
        if (m_TweenArguments.Contains("position"))
        {
            if (m_TweenArguments["position"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)m_TweenArguments["position"];
                m_Vector3s[1] = trans.position;
            }
            else if (m_TweenArguments["position"].GetType() == typeof(Vector3))
            {
                m_Vector3s[1] = (Vector3)m_TweenArguments["position"];
            }
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = (float)m_TweenArguments["z"];
            }
        }

        //handle orient to path request:
        if (m_TweenArguments.Contains("orienttopath") && (bool)m_TweenArguments["orienttopath"])
        {
            m_TweenArguments["looktarget"] = m_Vector3s[1];
        }

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateMoveByTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Translate usage to allow Space utilization, [4] original rotation to make sure look requests don't interfere with the direction object should move in, [5] for dial in location:
        m_Vector3s = new Vector3[6];

        //grab starting rotation:
        m_Vector3s[4] = m_ThisTransform.eulerAngles;

        //from values:
        m_Vector3s[0] = m_Vector3s[1] = m_Vector3s[3] = m_ThisTransform.position;

        //to values:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] = m_Vector3s[0] + (Vector3)m_TweenArguments["amount"];
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = m_Vector3s[0].x + (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = m_Vector3s[0].y + (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = m_Vector3s[0].z + (float)m_TweenArguments["z"];
            }
        }

        //calculation for dial in:
        m_ThisTransform.Translate(m_Vector3s[1], m_Space);
        m_Vector3s[5] = m_ThisTransform.position;
        m_ThisTransform.position = m_Vector3s[0];

        //handle orient to path request:
        if (m_TweenArguments.Contains("orienttopath") && (bool)m_TweenArguments["orienttopath"])
        {
            m_TweenArguments["looktarget"] = m_Vector3s[1];
        }

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateScaleToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector3s = new Vector3[3];

        //from values:
        m_Vector3s[0] = m_Vector3s[1] = m_ThisTransform.localScale;

        //to values:
        if (m_TweenArguments.Contains("scale"))
        {
            if (m_TweenArguments["scale"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)m_TweenArguments["scale"];
                m_Vector3s[1] = trans.localScale;
            }
            else if (m_TweenArguments["scale"].GetType() == typeof(Vector3))
            {
                m_Vector3s[1] = (Vector3)m_TweenArguments["scale"];
            }
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = (float)m_TweenArguments["z"];
            }
        }

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateScaleByTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector3s = new Vector3[3];

        //from values:
        m_Vector3s[0] = m_Vector3s[1] = m_ThisTransform.localScale;

        //to values:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] = Vector3.Scale(m_Vector3s[1], (Vector3)m_TweenArguments["amount"]);
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x *= (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y *= (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z *= (float)m_TweenArguments["z"];
            }
        }

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateScaleAddTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector3s = new Vector3[3];

        //from values:
        m_Vector3s[0] = m_Vector3s[1] = m_ThisTransform.localScale;

        //to values:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] += (Vector3)m_TweenArguments["amount"];
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x += (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y += (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z += (float)m_TweenArguments["z"];
            }
        }

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateRotateToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector3s = new Vector3[3];

        //from values:
        if (m_IsLocal)
        {
            m_Vector3s[0] = m_Vector3s[1] = m_ThisTransform.localEulerAngles;
        }
        else
        {
            m_Vector3s[0] = m_Vector3s[1] = m_ThisTransform.eulerAngles;
        }

        //to values:
        if (m_TweenArguments.Contains("rotation"))
        {
            if (m_TweenArguments["rotation"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)m_TweenArguments["rotation"];
                m_Vector3s[1] = trans.eulerAngles;
            }
            else if (m_TweenArguments["rotation"].GetType() == typeof(Vector3))
            {
                m_Vector3s[1] = (Vector3)m_TweenArguments["rotation"];
            }
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = (float)m_TweenArguments["z"];
            }
        }

        //shortest distance:
        m_Vector3s[1] = new Vector3(clerp(m_Vector3s[0].x, m_Vector3s[1].x, 1), clerp(m_Vector3s[0].y, m_Vector3s[1].y, 1), clerp(m_Vector3s[0].z, m_Vector3s[1].z, 1));

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateRotateAddTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Rotate usage to allow Space utilization:
        m_Vector3s = new Vector3[5];

        //from values:
        m_Vector3s[0] = m_Vector3s[1] = m_Vector3s[3] = m_ThisTransform.eulerAngles;

        //to values:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] += (Vector3)m_TweenArguments["amount"];
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x += (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y += (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z += (float)m_TweenArguments["z"];
            }
        }

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateRotateByTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Rotate usage to allow Space utilization:
        m_Vector3s = new Vector3[4];

        //from values:
        m_Vector3s[0] = m_Vector3s[1] = m_Vector3s[3] = m_ThisTransform.eulerAngles;

        //to values:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] += Vector3.Scale((Vector3)m_TweenArguments["amount"], new Vector3(360, 360, 360));
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x += 360 * (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y += 360 * (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z += 360 * (float)m_TweenArguments["z"];
            }
        }

        //need for speed?
        if (m_TweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(m_Vector3s[0], m_Vector3s[1]));
            time = distance / (float)m_TweenArguments["speed"];
        }
    }

    void GenerateShakePositionTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] original rotation to make sure look requests don't interfere with the direction object should move in:
        m_Vector3s = new Vector3[4];

        //grab starting rotation:
        m_Vector3s[3] = m_ThisTransform.eulerAngles;

        //root:
        m_Vector3s[0] = m_ThisTransform.position;

        //amount:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] = (Vector3)m_TweenArguments["amount"];
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = (float)m_TweenArguments["z"];
            }
        }
    }

    void GenerateShakeScaleTargets()
    {
        //values holder [0] root value, [1] amount, [2] generated amount:
        m_Vector3s = new Vector3[3];

        //root:
        m_Vector3s[0] = m_ThisTransform.localScale;

        //amount:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] = (Vector3)m_TweenArguments["amount"];
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = (float)m_TweenArguments["z"];
            }
        }
    }

    void GenerateShakeRotationTargets()
    {
        //values holder [0] root value, [1] amount, [2] generated amount:
        m_Vector3s = new Vector3[3];

        //root:
        m_Vector3s[0] = m_ThisTransform.eulerAngles;

        //amount:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] = (Vector3)m_TweenArguments["amount"];
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = (float)m_TweenArguments["z"];
            }
        }
    }

    void GeneratePunchPositionTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Translate usage to allow Space utilization, [4] original rotation to make sure look requests don't interfere with the direction object should move in:
        m_Vector3s = new Vector3[5];

        //grab starting rotation:
        m_Vector3s[4] = m_ThisTransform.eulerAngles;

        //from values:
        m_Vector3s[0] = m_ThisTransform.position;
        m_Vector3s[1] = m_Vector3s[3] = Vector3.zero;

        //to values:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] = (Vector3)m_TweenArguments["amount"];
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = (float)m_TweenArguments["z"];
            }
        }
    }

    void GeneratePunchRotationTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Translate usage to allow Space utilization:
        m_Vector3s = new Vector3[4];

        //from values:
        m_Vector3s[0] = m_ThisTransform.eulerAngles;
        m_Vector3s[1] = m_Vector3s[3] = Vector3.zero;

        //to values:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] = (Vector3)m_TweenArguments["amount"];
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = (float)m_TweenArguments["z"];
            }
        }
    }

    void GeneratePunchScaleTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        m_Vector3s = new Vector3[3];

        //from values:
        m_Vector3s[0] = m_ThisTransform.localScale;
        m_Vector3s[1] = Vector3.zero;

        //to values:
        if (m_TweenArguments.Contains("amount"))
        {
            m_Vector3s[1] = (Vector3)m_TweenArguments["amount"];
        }
        else
        {
            if (m_TweenArguments.Contains("x"))
            {
                m_Vector3s[1].x = (float)m_TweenArguments["x"];
            }
            if (m_TweenArguments.Contains("y"))
            {
                m_Vector3s[1].y = (float)m_TweenArguments["y"];
            }
            if (m_TweenArguments.Contains("z"))
            {
                m_Vector3s[1].z = (float)m_TweenArguments["z"];
            }
        }
    }

    #endregion

    #region #3 Apply Targets

    void ApplyRectTargets()
    {
        //calculate:
        m_Rects[2].x = m_EaseFunc(m_Rects[0].x, m_Rects[1].x, m_Percentage);
        m_Rects[2].y = m_EaseFunc(m_Rects[0].y, m_Rects[1].y, m_Percentage);
        m_Rects[2].width = m_EaseFunc(m_Rects[0].width, m_Rects[1].width, m_Percentage);
        m_Rects[2].height = m_EaseFunc(m_Rects[0].height, m_Rects[1].height, m_Percentage);

        //apply:
        m_TweenArguments["onupdateparams"] = m_Rects[2];

        //dial in:
        if (m_Percentage == 1)
        {
            m_TweenArguments["onupdateparams"] = m_Rects[1];
        }
    }

    void ApplyColorTargets()
    {
        //calculate:
        m_Colors[0, 2].r = m_EaseFunc(m_Colors[0, 0].r, m_Colors[0, 1].r, m_Percentage);
        m_Colors[0, 2].g = m_EaseFunc(m_Colors[0, 0].g, m_Colors[0, 1].g, m_Percentage);
        m_Colors[0, 2].b = m_EaseFunc(m_Colors[0, 0].b, m_Colors[0, 1].b, m_Percentage);
        m_Colors[0, 2].a = m_EaseFunc(m_Colors[0, 0].a, m_Colors[0, 1].a, m_Percentage);

        //apply:
        m_TweenArguments["onupdateparams"] = m_Colors[0, 2];

        //dial in:
        if (m_Percentage == 1)
        {
            m_TweenArguments["onupdateparams"] = m_Colors[0, 1];
        }
    }

    void ApplyVector3Targets()
    {
        //calculate:
        m_Vector3s[2].x = m_EaseFunc(m_Vector3s[0].x, m_Vector3s[1].x, m_Percentage);
        m_Vector3s[2].y = m_EaseFunc(m_Vector3s[0].y, m_Vector3s[1].y, m_Percentage);
        m_Vector3s[2].z = m_EaseFunc(m_Vector3s[0].z, m_Vector3s[1].z, m_Percentage);

        //apply:
        m_TweenArguments["onupdateparams"] = m_Vector3s[2];

        //dial in:
        if (m_Percentage == 1)
        {
            m_TweenArguments["onupdateparams"] = m_Vector3s[1];
        }
    }

    void ApplyVector2Targets()
    {
        //calculate:
        m_Vector2s[2].x = m_EaseFunc(m_Vector2s[0].x, m_Vector2s[1].x, m_Percentage);
        m_Vector2s[2].y = m_EaseFunc(m_Vector2s[0].y, m_Vector2s[1].y, m_Percentage);

        //apply:
        m_TweenArguments["onupdateparams"] = m_Vector2s[2];

        //dial in:
        if (m_Percentage == 1)
        {
            m_TweenArguments["onupdateparams"] = m_Vector2s[1];
        }
    }

    void ApplyFloatTargets()
    {
        //calculate:
        m_Floats[2] = m_EaseFunc(m_Floats[0], m_Floats[1], m_Percentage);

        //apply:
        m_TweenArguments["onupdateparams"] = m_Floats[2];

        //dial in:
        if (m_Percentage == 1)
        {
            m_TweenArguments["onupdateparams"] = m_Floats[1];
        }
    }

    void ApplyColorToTargets()
    {
        //calculate:
        for (int i = 0; i < m_Colors.GetLength(0); i++)
        {
            m_Colors[i, 2].r = m_EaseFunc(m_Colors[i, 0].r, m_Colors[i, 1].r, m_Percentage);
            m_Colors[i, 2].g = m_EaseFunc(m_Colors[i, 0].g, m_Colors[i, 1].g, m_Percentage);
            m_Colors[i, 2].b = m_EaseFunc(m_Colors[i, 0].b, m_Colors[i, 1].b, m_Percentage);
            m_Colors[i, 2].a = m_EaseFunc(m_Colors[i, 0].a, m_Colors[i, 1].a, m_Percentage);
        }
        /*
		colors[2].r = ease(colors[0].r,colors[1].r,percentage);
		colors[2].g = ease(colors[0].g,colors[1].g,percentage);
		colors[2].b = ease(colors[0].b,colors[1].b,percentage);
		colors[2].a = ease(colors[0].a,colors[1].a,percentage);
		*/

        //apply:
        if (GetComponent<Renderer>())
        {
            //renderer.material.color=colors[2];
            for (int i = 0; i < m_Colors.GetLength(0); i++)
            {
                GetComponent<Renderer>().materials[i].SetColor(m_Namedcolorvalue.ToString(), m_Colors[i, 2]);
            }
        }
        else if (GetComponent<Light>())
        {
            //light.color=colors[2];	
            GetComponent<Light>().color = m_Colors[0, 2];
        }

        //dial in:
        if (m_Percentage == 1)
        {
            if (GetComponent<Renderer>())
            {
                //renderer.material.color=colors[1];	
                for (int i = 0; i < m_Colors.GetLength(0); i++)
                {
                    GetComponent<Renderer>().materials[i].SetColor(m_Namedcolorvalue.ToString(), m_Colors[i, 1]);
                }
            }
            else if (GetComponent<Light>())
            {
                //light.color=colors[1];	
                GetComponent<Light>().color = m_Colors[0, 1];
            }
        }
    }

    void ApplyAudioToTargets()
    {
        //calculate:
        m_Vector2s[2].x = m_EaseFunc(m_Vector2s[0].x, m_Vector2s[1].x, m_Percentage);
        m_Vector2s[2].y = m_EaseFunc(m_Vector2s[0].y, m_Vector2s[1].y, m_Percentage);

        //apply:
        m_AudioSource.volume = m_Vector2s[2].x;
        m_AudioSource.pitch = m_Vector2s[2].y;

        //dial in:
        if (m_Percentage == 1)
        {
            m_AudioSource.volume = m_Vector2s[1].x;
            m_AudioSource.pitch = m_Vector2s[1].y;
        }
    }

    void ApplyStabTargets()
    {
        //unnecessary but here just in case
    }

    void ApplyMoveToPathTargets()
    {
        m_PreUpdate = m_ThisTransform.position;
        float t = m_EaseFunc(0, 1, m_Percentage);
        float lookAheadAmount;

        //clamp easing equation results as "back" will fail since overshoots aren't handled in the Catmull-Rom interpolation:
        if (m_IsLocal)
        {
            m_ThisTransform.localPosition = m_Path.Interp(Mathf.Clamp(t, 0, 1));
        }
        else
        {
            m_ThisTransform.position = m_Path.Interp(Mathf.Clamp(t, 0, 1));
        }

        //handle orient to path request:
        if (m_TweenArguments.Contains("orienttopath") && (bool)m_TweenArguments["orienttopath"])
        {

            //plot a point slightly ahead in the interpolation by pushing the percentage forward using the default lookahead value:
            float tLook;
            if (m_TweenArguments.Contains("lookahead"))
            {
                lookAheadAmount = (float)m_TweenArguments["lookahead"];
            }
            else
            {
                lookAheadAmount = Defaults.lookAhead;
            }
            //tLook = ease(0,1,percentage+lookAheadAmount);			
            tLook = m_EaseFunc(0, 1, Mathf.Min(1f, m_Percentage + lookAheadAmount));

            //locate new leading point with a clamp as stated above:
            //Vector3 lookDistance = path.Interp(Mathf.Clamp(tLook,0,1)) - transform.position;
            m_TweenArguments["looktarget"] = m_Path.Interp(Mathf.Clamp(tLook, 0, 1));
        }

        //need physics?
        m_PostUpdate = m_ThisTransform.position;
        if (m_Physics)
        {
            m_ThisTransform.position = m_PreUpdate;
            GetComponent<Rigidbody>().MovePosition(m_PostUpdate);
        }
    }

    void ApplyMoveToTargets()
    {
        //record current:
        m_PreUpdate = m_ThisTransform.position;


        //calculate:
        m_Vector3s[2].x = m_EaseFunc(m_Vector3s[0].x, m_Vector3s[1].x, m_Percentage);
        m_Vector3s[2].y = m_EaseFunc(m_Vector3s[0].y, m_Vector3s[1].y, m_Percentage);
        m_Vector3s[2].z = m_EaseFunc(m_Vector3s[0].z, m_Vector3s[1].z, m_Percentage);

        //apply:	
        if (m_IsLocal)
        {
            m_ThisTransform.localPosition = m_Vector3s[2];
        }
        else
        {
            m_ThisTransform.position = m_Vector3s[2];
        }

        //dial in:
        if (m_Percentage == 1)
        {
            if (m_IsLocal)
            {
                m_ThisTransform.localPosition = m_Vector3s[1];
            }
            else
            {
                m_ThisTransform.position = m_Vector3s[1];
            }
        }

        //need physics?
        m_PostUpdate = m_ThisTransform.position;
        if (m_Physics)
        {
            m_ThisTransform.position = m_PreUpdate;
            GetComponent<Rigidbody>().MovePosition(m_PostUpdate);
        }
    }

    void ApplyMoveByTargets()
    {
        m_PreUpdate = m_ThisTransform.position;

        //reset rotation to prevent look interferences as object rotates and attempts to move with translate and record current rotation
        Vector3 currentRotation = new Vector3();

        if (m_TweenArguments.Contains("looktarget"))
        {
            currentRotation = m_ThisTransform.eulerAngles;
            m_ThisTransform.eulerAngles = m_Vector3s[4];
        }

        //calculate:
        m_Vector3s[2].x = m_EaseFunc(m_Vector3s[0].x, m_Vector3s[1].x, m_Percentage);
        m_Vector3s[2].y = m_EaseFunc(m_Vector3s[0].y, m_Vector3s[1].y, m_Percentage);
        m_Vector3s[2].z = m_EaseFunc(m_Vector3s[0].z, m_Vector3s[1].z, m_Percentage);

        //apply:
        m_ThisTransform.Translate(m_Vector3s[2] - m_Vector3s[3], m_Space);

        //record:
        m_Vector3s[3] = m_Vector3s[2];

        //reset rotation:
        if (m_TweenArguments.Contains("looktarget"))
        {
            m_ThisTransform.eulerAngles = currentRotation;
        }

        /*
		//dial in:
		if(percentage==1){	
			transform.position=vector3s[5];
		}
		*/

        //need physics?
        m_PostUpdate = m_ThisTransform.position;
        if (m_Physics)
        {
            m_ThisTransform.position = m_PreUpdate;
            GetComponent<Rigidbody>().MovePosition(m_PostUpdate);
        }
    }

    void ApplyScaleToTargets()
    {
        //calculate:
        m_Vector3s[2].x = m_EaseFunc(m_Vector3s[0].x, m_Vector3s[1].x, m_Percentage);
        m_Vector3s[2].y = m_EaseFunc(m_Vector3s[0].y, m_Vector3s[1].y, m_Percentage);
        m_Vector3s[2].z = m_EaseFunc(m_Vector3s[0].z, m_Vector3s[1].z, m_Percentage);

        //apply:
        m_ThisTransform.localScale = m_Vector3s[2];

        //dial in:
        if (m_Percentage == 1)
        {
            m_ThisTransform.localScale = m_Vector3s[1];
        }
    }

    void ApplyLookToTargets()
    {
        //calculate:
        m_Vector3s[2].x = m_EaseFunc(m_Vector3s[0].x, m_Vector3s[1].x, m_Percentage);
        m_Vector3s[2].y = m_EaseFunc(m_Vector3s[0].y, m_Vector3s[1].y, m_Percentage);
        m_Vector3s[2].z = m_EaseFunc(m_Vector3s[0].z, m_Vector3s[1].z, m_Percentage);

        //apply:
        if (m_IsLocal)
        {
            m_ThisTransform.localRotation = Quaternion.Euler(m_Vector3s[2]);
        }
        else
        {
            m_ThisTransform.rotation = Quaternion.Euler(m_Vector3s[2]);
        };
    }

    void ApplyRotateToTargets()
    {
        m_PreUpdate = m_ThisTransform.eulerAngles;

        //calculate:
        m_Vector3s[2].x = m_EaseFunc(m_Vector3s[0].x, m_Vector3s[1].x, m_Percentage);
        m_Vector3s[2].y = m_EaseFunc(m_Vector3s[0].y, m_Vector3s[1].y, m_Percentage);
        m_Vector3s[2].z = m_EaseFunc(m_Vector3s[0].z, m_Vector3s[1].z, m_Percentage);

        //apply:
        if (m_IsLocal)
        {
            m_ThisTransform.localRotation = Quaternion.Euler(m_Vector3s[2]);
        }
        else
        {
            m_ThisTransform.rotation = Quaternion.Euler(m_Vector3s[2]);
        };

        //dial in:
        if (m_Percentage == 1)
        {
            if (m_IsLocal)
            {
                m_ThisTransform.localRotation = Quaternion.Euler(m_Vector3s[1]);
            }
            else
            {
                m_ThisTransform.rotation = Quaternion.Euler(m_Vector3s[1]);
            };
        }

        //need physics?
        m_PostUpdate = m_ThisTransform.eulerAngles;
        if (m_Physics)
        {
            m_ThisTransform.eulerAngles = m_PreUpdate;
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(m_PostUpdate));
        }
    }

    void ApplyRotateAddTargets()
    {
        m_PreUpdate = m_ThisTransform.eulerAngles;

        //calculate:
        m_Vector3s[2].x = m_EaseFunc(m_Vector3s[0].x, m_Vector3s[1].x, m_Percentage);
        m_Vector3s[2].y = m_EaseFunc(m_Vector3s[0].y, m_Vector3s[1].y, m_Percentage);
        m_Vector3s[2].z = m_EaseFunc(m_Vector3s[0].z, m_Vector3s[1].z, m_Percentage);

        //apply:
        m_ThisTransform.Rotate(m_Vector3s[2] - m_Vector3s[3], m_Space);

        //record:
        m_Vector3s[3] = m_Vector3s[2];

        //need physics?
        m_PostUpdate = m_ThisTransform.eulerAngles;
        if (m_Physics)
        {
            m_ThisTransform.eulerAngles = m_PreUpdate;
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(m_PostUpdate));
        }
    }

    void ApplyShakePositionTargets()
    {
        //preUpdate = transform.position;
        if (m_IsLocal)
        {
            m_PreUpdate = m_ThisTransform.localPosition;
        }
        else
        {
            m_PreUpdate = m_ThisTransform.position;
        }

        //reset rotation to prevent look interferences as object rotates and attempts to move with translate and record current rotation
        Vector3 currentRotation = new Vector3();

        if (m_TweenArguments.Contains("looktarget"))
        {
            currentRotation = m_ThisTransform.eulerAngles;
            m_ThisTransform.eulerAngles = m_Vector3s[3];
        }

        //impact:
        if (m_Percentage == 0)
        {
            m_ThisTransform.Translate(m_Vector3s[1], m_Space);
        }

        //transform.position=vector3s[0];
        //reset:
        if (m_IsLocal)
        {
            m_ThisTransform.localPosition = m_Vector3s[0];
        }
        else
        {
            m_ThisTransform.position = m_Vector3s[0];
        }

        //generate:
        float diminishingControl = 1 - m_Percentage;
        m_Vector3s[2].x = UnityEngine.Random.Range(-m_Vector3s[1].x * diminishingControl, m_Vector3s[1].x * diminishingControl);
        m_Vector3s[2].y = UnityEngine.Random.Range(-m_Vector3s[1].y * diminishingControl, m_Vector3s[1].y * diminishingControl);
        m_Vector3s[2].z = UnityEngine.Random.Range(-m_Vector3s[1].z * diminishingControl, m_Vector3s[1].z * diminishingControl);

        //apply:	
        //transform.Translate(vector3s[2],space);	
        if (m_IsLocal)
        {
            m_ThisTransform.localPosition += m_Vector3s[2];
        }
        else
        {
            m_ThisTransform.position += m_Vector3s[2];
        }

        //reset rotation:
        if (m_TweenArguments.Contains("looktarget"))
        {
            m_ThisTransform.eulerAngles = currentRotation;
        }

        //need physics?
        m_PostUpdate = m_ThisTransform.position;
        if (m_Physics)
        {
            m_ThisTransform.position = m_PreUpdate;
            GetComponent<Rigidbody>().MovePosition(m_PostUpdate);
        }
    }

    void ApplyShakeScaleTargets()
    {
        //impact:
        if (m_Percentage == 0)
        {
            m_ThisTransform.localScale = m_Vector3s[1];
        }

        //reset:
        m_ThisTransform.localScale = m_Vector3s[0];

        //generate:
        float diminishingControl = 1 - m_Percentage;
        m_Vector3s[2].x = UnityEngine.Random.Range(-m_Vector3s[1].x * diminishingControl, m_Vector3s[1].x * diminishingControl);
        m_Vector3s[2].y = UnityEngine.Random.Range(-m_Vector3s[1].y * diminishingControl, m_Vector3s[1].y * diminishingControl);
        m_Vector3s[2].z = UnityEngine.Random.Range(-m_Vector3s[1].z * diminishingControl, m_Vector3s[1].z * diminishingControl);

        //apply:
        m_ThisTransform.localScale += m_Vector3s[2];
    }

    void ApplyShakeRotationTargets()
    {
        m_PreUpdate = m_ThisTransform.eulerAngles;

        //impact:
        if (m_Percentage == 0)
        {
            m_ThisTransform.Rotate(m_Vector3s[1], m_Space);
        }

        //reset:
        m_ThisTransform.eulerAngles = m_Vector3s[0];

        //generate:
        float diminishingControl = 1 - m_Percentage;
        m_Vector3s[2].x = UnityEngine.Random.Range(-m_Vector3s[1].x * diminishingControl, m_Vector3s[1].x * diminishingControl);
        m_Vector3s[2].y = UnityEngine.Random.Range(-m_Vector3s[1].y * diminishingControl, m_Vector3s[1].y * diminishingControl);
        m_Vector3s[2].z = UnityEngine.Random.Range(-m_Vector3s[1].z * diminishingControl, m_Vector3s[1].z * diminishingControl);

        //apply:
        m_ThisTransform.Rotate(m_Vector3s[2], m_Space);

        //need physics?
        m_PostUpdate = m_ThisTransform.eulerAngles;
        if (m_Physics)
        {
            m_ThisTransform.eulerAngles = m_PreUpdate;
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(m_PostUpdate));
        }
    }

    void ApplyPunchPositionTargets()
    {
        m_PreUpdate = m_ThisTransform.position;

        //reset rotation to prevent look interferences as object rotates and attempts to move with translate and record current rotation
        Vector3 currentRotation = new Vector3();

        if (m_TweenArguments.Contains("looktarget"))
        {
            currentRotation = m_ThisTransform.eulerAngles;
            m_ThisTransform.eulerAngles = m_Vector3s[4];
        }

        //calculate:
        if (m_Vector3s[1].x > 0)
        {
            m_Vector3s[2].x = punch(m_Vector3s[1].x, m_Percentage);
        }
        else if (m_Vector3s[1].x < 0)
        {
            m_Vector3s[2].x = -punch(Mathf.Abs(m_Vector3s[1].x), m_Percentage);
        }
        if (m_Vector3s[1].y > 0)
        {
            m_Vector3s[2].y = punch(m_Vector3s[1].y, m_Percentage);
        }
        else if (m_Vector3s[1].y < 0)
        {
            m_Vector3s[2].y = -punch(Mathf.Abs(m_Vector3s[1].y), m_Percentage);
        }
        if (m_Vector3s[1].z > 0)
        {
            m_Vector3s[2].z = punch(m_Vector3s[1].z, m_Percentage);
        }
        else if (m_Vector3s[1].z < 0)
        {
            m_Vector3s[2].z = -punch(Mathf.Abs(m_Vector3s[1].z), m_Percentage);
        }

        //apply:
        m_ThisTransform.Translate(m_Vector3s[2] - m_Vector3s[3], m_Space);

        //record:
        m_Vector3s[3] = m_Vector3s[2];

        //reset rotation:
        if (m_TweenArguments.Contains("looktarget"))
        {
            m_ThisTransform.eulerAngles = currentRotation;
        }

        //dial in:
        /*
		if(percentage==1){	
			transform.position=vector3s[0];
		}
		*/

        //need physics?
        m_PostUpdate = m_ThisTransform.position;
        if (m_Physics)
        {
            m_ThisTransform.position = m_PreUpdate;
            GetComponent<Rigidbody>().MovePosition(m_PostUpdate);
        }
    }

    void ApplyPunchRotationTargets()
    {
        m_PreUpdate = m_ThisTransform.eulerAngles;

        //calculate:
        if (m_Vector3s[1].x > 0)
        {
            m_Vector3s[2].x = punch(m_Vector3s[1].x, m_Percentage);
        }
        else if (m_Vector3s[1].x < 0)
        {
            m_Vector3s[2].x = -punch(Mathf.Abs(m_Vector3s[1].x), m_Percentage);
        }
        if (m_Vector3s[1].y > 0)
        {
            m_Vector3s[2].y = punch(m_Vector3s[1].y, m_Percentage);
        }
        else if (m_Vector3s[1].y < 0)
        {
            m_Vector3s[2].y = -punch(Mathf.Abs(m_Vector3s[1].y), m_Percentage);
        }
        if (m_Vector3s[1].z > 0)
        {
            m_Vector3s[2].z = punch(m_Vector3s[1].z, m_Percentage);
        }
        else if (m_Vector3s[1].z < 0)
        {
            m_Vector3s[2].z = -punch(Mathf.Abs(m_Vector3s[1].z), m_Percentage);
        }

        //apply:
        m_ThisTransform.Rotate(m_Vector3s[2] - m_Vector3s[3], m_Space);

        //record:
        m_Vector3s[3] = m_Vector3s[2];

        //dial in:
        /*
		if(percentage==1){	
			transform.eulerAngles=vector3s[0];
		}
		*/

        //need physics?
        m_PostUpdate = m_ThisTransform.eulerAngles;
        if (m_Physics)
        {
            m_ThisTransform.eulerAngles = m_PreUpdate;
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(m_PostUpdate));
        }
    }

    void ApplyPunchScaleTargets()
    {
        //calculate:
        if (m_Vector3s[1].x > 0)
        {
            m_Vector3s[2].x = punch(m_Vector3s[1].x, m_Percentage);
        }
        else if (m_Vector3s[1].x < 0)
        {
            m_Vector3s[2].x = -punch(Mathf.Abs(m_Vector3s[1].x), m_Percentage);
        }
        if (m_Vector3s[1].y > 0)
        {
            m_Vector3s[2].y = punch(m_Vector3s[1].y, m_Percentage);
        }
        else if (m_Vector3s[1].y < 0)
        {
            m_Vector3s[2].y = -punch(Mathf.Abs(m_Vector3s[1].y), m_Percentage);
        }
        if (m_Vector3s[1].z > 0)
        {
            m_Vector3s[2].z = punch(m_Vector3s[1].z, m_Percentage);
        }
        else if (m_Vector3s[1].z < 0)
        {
            m_Vector3s[2].z = -punch(Mathf.Abs(m_Vector3s[1].z), m_Percentage);
        }

        //apply:
        m_ThisTransform.localScale = m_Vector3s[0] + m_Vector3s[2];

        //dial in:
        /*
		if(percentage==1){	
			transform.localScale=vector3s[0];
		}
		*/
    }

    #endregion

    #region #4 Tween Steps

    IEnumerator TweenDelay()
    {
        m_DelayStarted = Time.time;
        yield return new WaitForSeconds(delay);
        if (m_WasPaused)
        {
            m_WasPaused = false;
            TweenStart();
        }
    }

    void TweenStart()
    {
        CallBack("onstart");
        
        if (!m_Loop)
        {
            ConflictCheck();
        }

        //run stab:
        if (type == "stab")
        {
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
        }

        //toggle isKinematic for iTweens that may interfere with physics:
        if (type == "move" || type == "scale" || type == "rotate" || type == "punch" || type == "shake" || type == "curve" || type == "look")
        {
            EnableKinematic();
        }

        isRunning = true;
    }

    IEnumerator TweenRestart()
    {
        if (delay > 0)
        {
            m_DelayStarted = Time.time;
            yield return new WaitForSeconds(delay);
        }
        m_Loop = true;
        TweenStart();
    }

    void TweenUpdate()
    {
        m_ApplyFunc();
        CallBack("onupdate");
        UpdatePercentage();
    }

    void TweenComplete()
    {
        isRunning = false;

        //dial in percentage to 1 or 0 for final run:
        if (m_Percentage > .5f)
        {
            m_Percentage = 1f;
        }
        else
        {
            m_Percentage = 0;
        }

        //apply dial in and final run:
        m_ApplyFunc();
        if (type == "value")
        {
            CallBack("onupdate"); //CallBack run for ValueTo since it only calculates and applies in the update callback
        }

        //loop or dispose?
        if (loopType == LoopType.None)
        {
            Dispose();
        }
        else
        {
            TweenLoop();
        }

        CallBack("oncomplete");
    }

    void TweenLoop()
    {
        DisableKinematic(); //give physics control again
        switch (loopType)
        {
            case LoopType.Loop:
                //rewind:
                m_Percentage = 0;
                m_RunningTime = 0;
                m_ApplyFunc();

                //replay:
                StartCoroutine("TweenRestart");
                break;
            case LoopType.PingPong:
                m_Reverse = !m_Reverse;
                m_RunningTime = 0;

                //replay:
                StartCoroutine("TweenRestart");
                break;
        }
    }

    #endregion

    #region #5 Update Callable

    /// <summary>
    /// Returns a Rect that is eased between a current and target value by the supplied speed.
    /// </summary>
    /// <returns>
    /// A <see cref="Rect"/
    /// </returns>
    /// <param name='currentValue'>
    /// A <see cref="Rect"/> the starting or initial value
    /// </param>
    /// <param name='targetValue'>
    /// A <see cref="Rect"/> the target value that the current value will be eased to.
    /// </param>
    /// <param name='speed'>
    /// A <see cref="System.Single"/> to be used as rate of speed (larger number equals faster animation)
    /// </param>
    public static Rect RectUpdate(Rect currentValue, Rect targetValue, float speed)
    {
        Rect diff = new Rect(FloatUpdate(currentValue.x, targetValue.x, speed), FloatUpdate(currentValue.y, targetValue.y, speed), FloatUpdate(currentValue.width, targetValue.width, speed), FloatUpdate(currentValue.height, targetValue.height, speed));
        return (diff);
    }

    /// <summary>
    /// Returns a Vector3 that is eased between a current and target value by the supplied speed.
    /// </summary>
    /// <returns>
    /// A <see cref="Vector3"/>
    /// </returns>
    /// <param name='currentValue'>
    /// A <see cref="Vector3"/> the starting or initial value
    /// </param>
    /// <param name='targetValue'>
    /// A <see cref="Vector3"/> the target value that the current value will be eased to.
    /// </param>
    /// <param name='speed'>
    /// A <see cref="System.Single"/> to be used as rate of speed (larger number equals faster animation)
    /// </param>
    public static Vector3 Vector3Update(Vector3 currentValue, Vector3 targetValue, float speed)
    {
        Vector3 diff = targetValue - currentValue;
        currentValue += (diff * speed) * Time.deltaTime;
        return (currentValue);
    }

    /// <summary>
    /// Returns a Vector2 that is eased between a current and target value by the supplied speed.
    /// </summary>
    /// <returns>
    /// A <see cref="Vector2"/>
    /// </returns>
    /// <param name='currentValue'>
    /// A <see cref="Vector2"/> the starting or initial value
    /// </param>
    /// <param name='targetValue'>
    /// A <see cref="Vector2"/> the target value that the current value will be eased to.
    /// </param>
    /// <param name='speed'>
    /// A <see cref="System.Single"/> to be used as rate of speed (larger number equals faster animation)
    /// </param>
    public static Vector2 Vector2Update(Vector2 currentValue, Vector2 targetValue, float speed)
    {
        Vector2 diff = targetValue - currentValue;
        currentValue += (diff * speed) * Time.deltaTime;
        return (currentValue);
    }

    /// <summary>
    /// Returns a float that is eased between a current and target value by the supplied speed.
    /// </summary>
    /// <returns>
    /// A <see cref="System.Single"/>
    /// </returns>
    /// <param name='currentValue'>
    /// A <see cref="System.Single"/> the starting or initial value
    /// </param>
    /// <param name='targetValue'>
    /// A <see cref="System.Single"/> the target value that the current value will be eased to.
    /// </param>
    /// <param name='speed'>
    /// A <see cref="System.Single"/> to be used as rate of speed (larger number equals faster animation)
    /// </param>
    public static float FloatUpdate(float currentValue, float targetValue, float speed)
    {
        float diff = targetValue - currentValue;
        currentValue += (diff * speed) * Time.deltaTime;
        return (currentValue);
    }

    /// <summary>
    /// Similar to FadeTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void FadeUpdate(GameObject target, Hashtable args)
    {
        args["a"] = args["alpha"];
        ColorUpdate(target, args);
    }

    /// <summary>
    /// Similar to FadeTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void FadeUpdate(GameObject target, float alpha, float time)
    {
        FadeUpdate(target, Hash("alpha", alpha, "time", time));
    }

    /// <summary>
    /// Similar to ColorTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="r">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color red.
    /// </param>
    /// <param name="g">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="b">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="a">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="namedcolorvalue">
    /// A <see cref="NamedColorValue"/> or <see cref="System.String"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ColorUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        float time;
        Color[] colors = new Color[4];

        //handle children:
        if (!args.Contains("includechildren") || (bool)args["includechildren"])
        {
            foreach (Transform child in target.transform)
            {
                ColorUpdate(child.gameObject, args);
            }
        }

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.updateTimePercentage;
        }
        else
        {
            time = Defaults.updateTime;
        }

        //init values:
        if (target.GetComponent<Renderer>())
        {
            colors[0] = colors[1] = target.GetComponent<Renderer>().material.color;
        }
        else if (target.GetComponent<Light>())
        {
            colors[0] = colors[1] = target.GetComponent<Light>().color;
        }

        //to values:
        if (args.Contains("color"))
        {
            colors[1] = (Color)args["color"];
        }
        else
        {
            if (args.Contains("r"))
            {
                colors[1].r = (float)args["r"];
            }
            if (args.Contains("g"))
            {
                colors[1].g = (float)args["g"];
            }
            if (args.Contains("b"))
            {
                colors[1].b = (float)args["b"];
            }
            if (args.Contains("a"))
            {
                colors[1].a = (float)args["a"];
            }
        }

        //calculate:
        colors[3].r = Mathf.SmoothDamp(colors[0].r, colors[1].r, ref colors[2].r, time);
        colors[3].g = Mathf.SmoothDamp(colors[0].g, colors[1].g, ref colors[2].g, time);
        colors[3].b = Mathf.SmoothDamp(colors[0].b, colors[1].b, ref colors[2].b, time);
        colors[3].a = Mathf.SmoothDamp(colors[0].a, colors[1].a, ref colors[2].a, time);

        //apply:
        if (target.GetComponent<Renderer>())
        {
            target.GetComponent<Renderer>().material.color = colors[3];
        }
        else if (target.GetComponent<Light>())
        {
            target.GetComponent<Light>().color = colors[3];
        }
    }

    /// <summary>
    /// Similar to ColorTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ColorUpdate(GameObject target, Color color, float time)
    {
        ColorUpdate(target, Hash("color", color, "time", time));
    }

    /// <summary>
    /// Similar to AudioTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="audiosource">
    /// A <see cref="AudioSource"/> for which AudioSource to use.
    /// </param> 
    /// <param name="volume">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target pitch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void AudioUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        AudioSource audioSource;
        float time;
        Vector2[] vector2s = new Vector2[4];

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.updateTimePercentage;
        }
        else
        {
            time = Defaults.updateTime;
        }

        //set audioSource:
        if (args.Contains("audiosource"))
        {
            audioSource = (AudioSource)args["audiosource"];
        }
        else
        {
            if (target.GetComponent<AudioSource>())
            {
                audioSource = target.GetComponent<AudioSource>();
            }
            else
            {
                //throw error if no AudioSource is available:
                Debug.LogError("iTween Error: AudioUpdate requires an AudioSource.");
                return;
            }
        }

        //from values:
        vector2s[0] = vector2s[1] = new Vector2(audioSource.volume, audioSource.pitch);

        //set to:
        if (args.Contains("volume"))
        {
            vector2s[1].x = (float)args["volume"];
        }
        if (args.Contains("pitch"))
        {
            vector2s[1].y = (float)args["pitch"];
        }

        //calculate:
        vector2s[3].x = Mathf.SmoothDampAngle(vector2s[0].x, vector2s[1].x, ref vector2s[2].x, time);
        vector2s[3].y = Mathf.SmoothDampAngle(vector2s[0].y, vector2s[1].y, ref vector2s[2].y, time);

        //apply:
        audioSource.volume = vector2s[3].x;
        audioSource.pitch = vector2s[3].y;
    }

    /// <summary>
    /// Similar to AudioTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="volume">
    /// A <see cref="System.Single"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> for the target pitch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void AudioUpdate(GameObject target, float volume, float pitch, float time)
    {
        AudioUpdate(target, Hash("volume", volume, "pitch", pitch, "time", time));
    }

    /// <summary>
    /// Similar to RotateTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="rotation">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param> 
    public static void RotateUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        bool isLocal;
        float time;
        Vector3[] vector3s = new Vector3[4];
        Vector3 preUpdate = target.transform.eulerAngles;

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.updateTimePercentage;
        }
        else
        {
            time = Defaults.updateTime;
        }

        //set isLocal:
        if (args.Contains("islocal"))
        {
            isLocal = (bool)args["islocal"];
        }
        else
        {
            isLocal = Defaults.isLocal;
        }

        //from values:
        if (isLocal)
        {
            vector3s[0] = target.transform.localEulerAngles;
        }
        else
        {
            vector3s[0] = target.transform.eulerAngles;
        }

        //set to:
        if (args.Contains("rotation"))
        {
            if (args["rotation"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["rotation"];
                vector3s[1] = trans.eulerAngles;
            }
            else if (args["rotation"].GetType() == typeof(Vector3))
            {
                vector3s[1] = (Vector3)args["rotation"];
            }
        }

        //calculate:
        vector3s[3].x = Mathf.SmoothDampAngle(vector3s[0].x, vector3s[1].x, ref vector3s[2].x, time);
        vector3s[3].y = Mathf.SmoothDampAngle(vector3s[0].y, vector3s[1].y, ref vector3s[2].y, time);
        vector3s[3].z = Mathf.SmoothDampAngle(vector3s[0].z, vector3s[1].z, ref vector3s[2].z, time);

        //apply:
        if (isLocal)
        {
            target.transform.localEulerAngles = vector3s[3];
        }
        else
        {
            target.transform.eulerAngles = vector3s[3];
        }

        //need physics?
        if (target.GetComponent<Rigidbody>() != null)
        {
            Vector3 postUpdate = target.transform.eulerAngles;
            target.transform.eulerAngles = preUpdate;
            target.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
        }
    }

    /// <summary>
    /// Similar to RotateTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="rotation">
    /// A <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void RotateUpdate(GameObject target, Vector3 rotation, float time)
    {
        RotateUpdate(target, Hash("rotation", rotation, "time", time));
    }

    /// <summary>
    /// Similar to ScaleTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options.  Does not utilize an EaseType. 
    /// </summary>
    /// <param name="scale">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param> 
    public static void ScaleUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        float time;
        Vector3[] vector3s = new Vector3[4];

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.updateTimePercentage;
        }
        else
        {
            time = Defaults.updateTime;
        }

        //init values:
        vector3s[0] = vector3s[1] = target.transform.localScale;

        //to values:
        if (args.Contains("scale"))
        {
            if (args["scale"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["scale"];
                vector3s[1] = trans.localScale;
            }
            else if (args["scale"].GetType() == typeof(Vector3))
            {
                vector3s[1] = (Vector3)args["scale"];
            }
        }
        else
        {
            if (args.Contains("x"))
            {
                vector3s[1].x = (float)args["x"];
            }
            if (args.Contains("y"))
            {
                vector3s[1].y = (float)args["y"];
            }
            if (args.Contains("z"))
            {
                vector3s[1].z = (float)args["z"];
            }
        }

        //calculate:
        vector3s[3].x = Mathf.SmoothDamp(vector3s[0].x, vector3s[1].x, ref vector3s[2].x, time);
        vector3s[3].y = Mathf.SmoothDamp(vector3s[0].y, vector3s[1].y, ref vector3s[2].y, time);
        vector3s[3].z = Mathf.SmoothDamp(vector3s[0].z, vector3s[1].z, ref vector3s[2].z, time);

        //apply:
        target.transform.localScale = vector3s[3];
    }

    /// <summary>
    /// Similar to ScaleTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options.  Does not utilize an EaseType.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="scale">
    /// A <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ScaleUpdate(GameObject target, Vector3 scale, float time)
    {
        ScaleUpdate(target, Hash("scale", scale, "time", time));
    }

    /// <summary>
    /// Similar to MoveTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="position">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a point in space the GameObject will animate to.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param> 
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    public static void MoveUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        float time;
        Vector3[] vector3s = new Vector3[4];
        bool isLocal;
        Vector3 preUpdate = target.transform.position;

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.updateTimePercentage;
        }
        else
        {
            time = Defaults.updateTime;
        }

        //set isLocal:
        if (args.Contains("islocal"))
        {
            isLocal = (bool)args["islocal"];
        }
        else
        {
            isLocal = Defaults.isLocal;
        }

        //init values:
        if (isLocal)
        {
            vector3s[0] = vector3s[1] = target.transform.localPosition;
        }
        else
        {
            vector3s[0] = vector3s[1] = target.transform.position;
        }

        //to values:
        if (args.Contains("position"))
        {
            if (args["position"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["position"];
                vector3s[1] = trans.position;
            }
            else if (args["position"].GetType() == typeof(Vector3))
            {
                vector3s[1] = (Vector3)args["position"];
            }
        }
        else
        {
            if (args.Contains("x"))
            {
                vector3s[1].x = (float)args["x"];
            }
            if (args.Contains("y"))
            {
                vector3s[1].y = (float)args["y"];
            }
            if (args.Contains("z"))
            {
                vector3s[1].z = (float)args["z"];
            }
        }

        //calculate:
        vector3s[3].x = Mathf.SmoothDamp(vector3s[0].x, vector3s[1].x, ref vector3s[2].x, time);
        vector3s[3].y = Mathf.SmoothDamp(vector3s[0].y, vector3s[1].y, ref vector3s[2].y, time);
        vector3s[3].z = Mathf.SmoothDamp(vector3s[0].z, vector3s[1].z, ref vector3s[2].z, time);

        //handle orient to path:
        if (args.Contains("orienttopath") && (bool)args["orienttopath"])
        {
            args["looktarget"] = vector3s[3];
        }

        //look applications:
        if (args.Contains("looktarget"))
        {
            LookUpdate(target, args);
        }

        //apply:
        if (isLocal)
        {
            target.transform.localPosition = vector3s[3];
        }
        else
        {
            target.transform.position = vector3s[3];
        }

        //need physics?
        if (target.GetComponent<Rigidbody>() != null)
        {
            Vector3 postUpdate = target.transform.position;
            target.transform.position = preUpdate;
            target.GetComponent<Rigidbody>().MovePosition(postUpdate);
        }
    }

    /// <summary>
    /// Similar to MoveTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="position">
    /// A <see cref="Vector3"/> for a point in space the GameObject will animate to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void MoveUpdate(GameObject target, Vector3 position, float time)
    {
        MoveUpdate(target, Hash("position", position, "time", time));
    }

    /// <summary>
    /// Similar to LookTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="looktarget">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param> 
    public static void LookUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        float time;
        Vector3[] vector3s = new Vector3[5];

        //set smooth time:
        if (args.Contains("looktime"))
        {
            time = (float)args["looktime"];
            time *= Defaults.updateTimePercentage;
        }
        else if (args.Contains("time"))
        {
            time = (float)args["time"] * .15f;
            time *= Defaults.updateTimePercentage;
        }
        else
        {
            time = Defaults.updateTime;
        }

        //from values:
        vector3s[0] = target.transform.eulerAngles;

        //set look:
        if (args.Contains("looktarget"))
        {
            if (args["looktarget"].GetType() == typeof(Transform))
            {
                //target.transform.LookAt((Transform)args["looktarget"]);
                target.transform.LookAt((Transform)args["looktarget"], (Vector3?)args["up"] ?? Defaults.up);
            }
            else if (args["looktarget"].GetType() == typeof(Vector3))
            {
                //target.transform.LookAt((Vector3)args["looktarget"]);
                target.transform.LookAt((Vector3)args["looktarget"], (Vector3?)args["up"] ?? Defaults.up);
            }
        }
        else
        {
            Debug.LogError("iTween Error: LookUpdate needs a 'looktarget' property!");
            return;
        }

        //to values and reset look:
        vector3s[1] = target.transform.eulerAngles;
        target.transform.eulerAngles = vector3s[0];

        //calculate:
        vector3s[3].x = Mathf.SmoothDampAngle(vector3s[0].x, vector3s[1].x, ref vector3s[2].x, time);
        vector3s[3].y = Mathf.SmoothDampAngle(vector3s[0].y, vector3s[1].y, ref vector3s[2].y, time);
        vector3s[3].z = Mathf.SmoothDampAngle(vector3s[0].z, vector3s[1].z, ref vector3s[2].z, time);

        //apply:
        target.transform.eulerAngles = vector3s[3];

        //axis restriction:
        if (args.Contains("axis"))
        {
            vector3s[4] = target.transform.eulerAngles;
            switch ((string)args["axis"])
            {
                case "x":
                    vector3s[4].y = vector3s[0].y;
                    vector3s[4].z = vector3s[0].z;
                    break;
                case "y":
                    vector3s[4].x = vector3s[0].x;
                    vector3s[4].z = vector3s[0].z;
                    break;
                case "z":
                    vector3s[4].x = vector3s[0].x;
                    vector3s[4].y = vector3s[0].y;
                    break;
            }

            //apply axis restriction:
            target.transform.eulerAngles = vector3s[4];
        }
    }

    /// <summary>
    /// Similar to LookTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void LookUpdate(GameObject target, Vector3 looktarget, float time)
    {
        LookUpdate(target, Hash("looktarget", looktarget, "time", time));
    }

    #endregion
    
    #region Unity internals
    
    void Awake()
    {
        m_ThisTransform = transform;
        m_LastRealTime = Time.realtimeSinceStartup;
    }

    IEnumerator Start()
    {
        if (delay > 0)
        {
            yield return StartCoroutine("TweenDelay");
        }
        TweenStart();
    }

    //non-physics
    void Update()
    {
        if (isRunning && !m_Physics)
        {
            if (!m_Reverse)
            {
                if (m_Percentage < 1f)
                {
                    TweenUpdate();
                }
                else
                {
                    TweenComplete();
                }
            }
            else
            {
                if (m_Percentage > 0)
                {
                    TweenUpdate();
                }
                else
                {
                    TweenComplete();
                }
            }
        }
    }

    //physics
    void FixedUpdate()
    {
        if (isRunning && m_Physics)
        {
            if (!m_Reverse)
            {
                if (m_Percentage < 1f)
                {
                    TweenUpdate();
                }
                else
                {
                    TweenComplete();
                }
            }
            else
            {
                if (m_Percentage > 0)
                {
                    TweenUpdate();
                }
                else
                {
                    TweenComplete();
                }
            }
        }
    }

    void LateUpdate()
    {
        //look applications:
        if (m_TweenArguments.Contains("looktarget") && isRunning)
        {
            if (type == "move" || type == "shake" || type == "punch")
            {
                LookUpdate(gameObject, m_TweenArguments);
            }
        }
    }

    void OnEnable()
    {
        if (isRunning)
        {
            EnableKinematic();
        }

        //resume delay:
        if (isPaused)
        {
            isPaused = false;
            if (delay > 0)
            {
                m_WasPaused = true;
                ResumeDelay();
            }
        }
    }

    void OnDisable()
    {
        DisableKinematic();
    }
    
    void OnDrawGizmosSelected()
    {
        if (type == "move")
        {
            iTweenDrawUtilities.DrawPath(m_Vector3s);
        }
    }

    #endregion

    #region Helpers

    public static Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        //create and store path points:
        suppliedPath = path;

        //populate calculate path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

        //populate start and end control points:
        //vector3s[0] = vector3s[1] - vector3s[2];
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

        //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
        if (vector3s[1] == vector3s[vector3s.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
            Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
        }

        return (vector3s);
    }

    //andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
    public static Vector3 Interp(Vector3[] pts, float t)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = pts[currPt];
        Vector3 b = pts[currPt + 1];
        Vector3 c = pts[currPt + 2];
        Vector3 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }

    //andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
    private class CRSpline
    {
        public Vector3[] points;

        public CRSpline(params Vector3[] pts)
        {
            this.points = new Vector3[pts.Length];
            Array.Copy(pts, this.points, pts.Length);
        }

        public Vector3 Interp(float t)
        {
            int numSections = points.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt;
            Vector3 a = points[currPt];
            Vector3 b = points[currPt + 1];
            Vector3 c = points[currPt + 2];
            Vector3 d = points[currPt + 3];
            return .5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
        }
    }

    //catalog new tween and add component phase of iTween:
    static iTween Launch(GameObject target, Hashtable args)
    {
        if (!args.Contains("id"))
        {
            args["id"] = GenerateID();
        }
        if (!args.Contains("target"))
        {
            args["target"] = target;
        }

        tweens.Insert(0, args);
        var tweenComponent = target.AddComponent<iTween>();
        tweenComponent.SetTweenArguments(args);
        return tweenComponent;
    }

    void SetTweenArguments(Hashtable args)
    {
        m_TweenArguments = args;
        RetrieveArgs();
        GenerateTargets();
    }

    //cast any accidentally supplied doubles and ints as floats as iTween only uses floats internally and unify parameter case:
    static Hashtable CleanArgs(Hashtable args)
    {
        Hashtable argsCopy = new Hashtable(args.Count);
        Hashtable argsCaseUnified = new Hashtable(args.Count);

        foreach (DictionaryEntry item in args)
        {
            argsCopy.Add(item.Key, item.Value);
        }

        foreach (DictionaryEntry item in argsCopy)
        {
            if (item.Value.GetType() == typeof(System.Int32))
            {
                int original = (int)item.Value;
                float casted = (float)original;
                args[item.Key] = casted;
            }
            if (item.Value.GetType() == typeof(System.Double))
            {
                double original = (double)item.Value;
                float casted = (float)original;
                args[item.Key] = casted;
            }
        }

        //unify parameter case:
        foreach (DictionaryEntry item in args)
        {
            argsCaseUnified.Add(item.Key.ToString().ToLower(), item.Value);
        }

        //swap back case unification:
        args = argsCaseUnified;

        return args;
    }

    //random ID generator:
    static string GenerateID()
    {
        //		int strlen = 15;
        //		char[] chars = {'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','0','1','2','3','4','5','6','7','8'};
        //		int num_chars = chars.Length - 1;
        //		string randomChar = "";
        //		for (int i = 0; i < strlen; i++) {
        //			randomChar += chars[(int)Mathf.Floor(UnityEngine.Random.Range(0,num_chars))];
        //		}
        return System.Guid.NewGuid().ToString();
    }

    //grab and set generic, neccesary iTween arguments:
    void RetrieveArgs()
    {
        if (m_TweenArguments == null)
        {
            foreach (Hashtable item in tweens)
            {
                if ((GameObject)item["target"] == gameObject)
                {
                    m_TweenArguments = item;
                    break;
                }
            }
        }

        id = (string)m_TweenArguments["id"];
        type = (string)m_TweenArguments["type"];
        tweenName = (string)m_TweenArguments["name"];
        method = (string)m_TweenArguments["method"];

        if (m_TweenArguments.Contains("time"))
        {
            time = (float)m_TweenArguments["time"];
        }
        else
        {
            time = Defaults.time;
        }

        //do we need to use physics, is there a rigidbody?
        if (GetComponent<Rigidbody>() != null)
        {
            m_Physics = true;
        }

        if (m_TweenArguments.Contains("delay"))
        {
            delay = (float)m_TweenArguments["delay"];
        }
        else
        {
            delay = Defaults.delay;
        }

        if (m_TweenArguments.Contains("namedcolorvalue"))
        {
            //allows namedcolorvalue to be set as either an enum(C# friendly) or a string(JS friendly), string case usage doesn't matter to further increase usability:
            if (m_TweenArguments["namedcolorvalue"].GetType() == typeof(NamedValueColor))
            {
                m_Namedcolorvalue = (NamedValueColor)m_TweenArguments["namedcolorvalue"];
            }
            else
            {
                try
                {
                    m_Namedcolorvalue = (NamedValueColor)Enum.Parse(typeof(NamedValueColor), (string)m_TweenArguments["namedcolorvalue"], true);
                }
                catch
                {
                    Debug.LogWarning("iTween: Unsupported namedcolorvalue supplied! Default will be used.");
                    m_Namedcolorvalue = NamedValueColor._Color;
                }
            }
        }
        else
        {
            m_Namedcolorvalue = Defaults.namedColorValue;
        }

        if (m_TweenArguments.Contains("looptype"))
        {
            //allows loopType to be set as either an enum(C# friendly) or a string(JS friendly), string case usage doesn't matter to further increase usability:
            if (m_TweenArguments["looptype"].GetType() == typeof(LoopType))
            {
                loopType = (LoopType)m_TweenArguments["looptype"];
            }
            else
            {
                try
                {
                    loopType = (LoopType)Enum.Parse(typeof(LoopType), (string)m_TweenArguments["looptype"], true);
                }
                catch
                {
                    Debug.LogWarning("iTween: Unsupported loopType supplied! Default will be used.");
                    loopType = LoopType.None;
                }
            }
        }
        else
        {
            loopType = LoopType.None;
        }

        if (m_TweenArguments.Contains("easetype"))
        {
            //allows easeType to be set as either an enum(C# friendly) or a string(JS friendly), string case usage doesn't matter to further increase usability:
            if (m_TweenArguments["easetype"].GetType() == typeof(EaseType))
            {
                easeType = (EaseType)m_TweenArguments["easetype"];
            }
            else
            {
                try
                {
                    easeType = (EaseType)Enum.Parse(typeof(EaseType), (string)m_TweenArguments["easetype"], true);
                }
                catch
                {
                    Debug.LogWarning("iTween: Unsupported easeType supplied! Default will be used.");
                    easeType = Defaults.easeType;
                }
            }
        }
        else
        {
            easeType = Defaults.easeType;
        }

        if (m_TweenArguments.Contains("space"))
        {
            //allows space to be set as either an enum(C# friendly) or a string(JS friendly), string case usage doesn't matter to further increase usability:
            if (m_TweenArguments["space"].GetType() == typeof(Space))
            {
                m_Space = (Space)m_TweenArguments["space"];
            }
            else
            {
                try
                {
                    m_Space = (Space)Enum.Parse(typeof(Space), (string)m_TweenArguments["space"], true);
                }
                catch
                {
                    Debug.LogWarning("iTween: Unsupported space supplied! Default will be used.");
                    m_Space = Defaults.space;
                }
            }
        }
        else
        {
            m_Space = Defaults.space;
        }

        if (m_TweenArguments.Contains("islocal"))
        {
            m_IsLocal = (bool)m_TweenArguments["islocal"];
        }
        else
        {
            m_IsLocal = Defaults.isLocal;
        }

        if (m_TweenArguments.Contains("ignoretimescale"))
        {
            m_UseRealTime = (bool)m_TweenArguments["ignoretimescale"];
        }
        else
        {
            m_UseRealTime = Defaults.useRealTime;
        }

        //instantiates a cached ease equation reference:
        GetEasingFunction();
    }

    //instantiates a cached ease equation refrence:
    void GetEasingFunction()
    {
        switch (easeType)
        {
            case EaseType.EaseInQuad: m_EaseFunc = iTweenEaseFunctions.EaseInQuad; break;
            case EaseType.EaseOutQuad: m_EaseFunc = iTweenEaseFunctions.EaseOutQuad; break;
            case EaseType.EaseInOutQuad: m_EaseFunc = iTweenEaseFunctions.EaseInOutQuad; break;
            case EaseType.EaseInCubic: m_EaseFunc = iTweenEaseFunctions.EaseInCubic; break;
            case EaseType.EaseOutCubic: m_EaseFunc = iTweenEaseFunctions.EaseOutCubic; break;
            case EaseType.EaseInOutCubic: m_EaseFunc = iTweenEaseFunctions.EaseInOutCubic; break;
            case EaseType.EaseInQuart: m_EaseFunc = iTweenEaseFunctions.EaseInQuart; break;
            case EaseType.EaseOutQuart: m_EaseFunc = iTweenEaseFunctions.EaseOutQuart; break;
            case EaseType.EaseInOutQuart: m_EaseFunc = iTweenEaseFunctions.EaseInOutQuart; break;
            case EaseType.EaseInQuint: m_EaseFunc = iTweenEaseFunctions.EaseInQuint; break;
            case EaseType.EaseOutQuint: m_EaseFunc = iTweenEaseFunctions.EaseOutQuint; break;
            case EaseType.EaseInOutQuint: m_EaseFunc = iTweenEaseFunctions.EaseInOutQuint; break;
            case EaseType.EaseInSine: m_EaseFunc = iTweenEaseFunctions.EaseInSine; break;
            case EaseType.EaseOutSine: m_EaseFunc = iTweenEaseFunctions.EaseOutSine; break;
            case EaseType.EaseInOutSine: m_EaseFunc = iTweenEaseFunctions.EaseInOutSine; break;
            case EaseType.EaseInExpo: m_EaseFunc = iTweenEaseFunctions.EaseInExpo; break;
            case EaseType.EaseOutExpo: m_EaseFunc = iTweenEaseFunctions.EaseOutExpo; break;
            case EaseType.EaseInOutExpo: m_EaseFunc = iTweenEaseFunctions.EaseInOutExpo; break;
            case EaseType.EaseInCirc: m_EaseFunc = iTweenEaseFunctions.EaseInCirc; break;
            case EaseType.EaseOutCirc: m_EaseFunc = iTweenEaseFunctions.EaseOutCirc; break;
            case EaseType.EaseInOutCirc: m_EaseFunc = iTweenEaseFunctions.EaseInOutCirc; break;
            case EaseType.Linear: m_EaseFunc = iTweenEaseFunctions.Linear; break;
            case EaseType.Spring: m_EaseFunc = iTweenEaseFunctions.Spring; break;
            //case EaseType.bounce: ease = bounce; break;
            case EaseType.EaseInBounce: m_EaseFunc = iTweenEaseFunctions.EaseInBounce; break;
            case EaseType.EaseOutBounce: m_EaseFunc = iTweenEaseFunctions.EaseOutBounce; break;
            case EaseType.EaseInOutBounce: m_EaseFunc = iTweenEaseFunctions.EaseInOutBounce; break;
            case EaseType.EaseInBack: m_EaseFunc = iTweenEaseFunctions.EaseInBack; break;
            case EaseType.EaseOutBack: m_EaseFunc = iTweenEaseFunctions.EaseOutBack; break;
            case EaseType.EaseInOutBack: m_EaseFunc = iTweenEaseFunctions.EaseInOutBack; break;
            //*case EaseType.elastic: ease = elastic); break;
            case EaseType.EaseInElastic: m_EaseFunc = iTweenEaseFunctions.EaseInElastic; break;
            case EaseType.EaseOutElastic: m_EaseFunc = iTweenEaseFunctions.EaseOutElastic; break;
            case EaseType.EaseInOutElastic: m_EaseFunc = iTweenEaseFunctions.EaseInOutElastic; break;
            default: m_EaseFunc = iTweenEaseFunctions.Linear; break;
        }
    }

    //calculate percentage of tween based on time:
    void UpdatePercentage()
    {
        if (m_UseRealTime)
        {
            m_RunningTime += (Time.realtimeSinceStartup - m_LastRealTime);
        }
        else
        {
            m_RunningTime += Time.deltaTime;
        }

        if (m_Reverse)
        {
            m_Percentage = 1 - m_RunningTime / time;
        }
        else
        {
            m_Percentage = m_RunningTime / time;
        }

        m_LastRealTime = Time.realtimeSinceStartup;
    }

    void CallBack(string callbackType)
    {
        if (m_TweenArguments.Contains(callbackType) && !m_TweenArguments.Contains("ischild"))
        {
            //establish target:
            GameObject target;
            if (m_TweenArguments.Contains(callbackType + "target"))
            {
                target = (GameObject)m_TweenArguments[callbackType + "target"];
            }
            else
            {
                target = gameObject;
            }

            //throw an error if a string wasn't passed for callback:
            if (m_TweenArguments[callbackType].GetType() == typeof(System.String))
            {
                target.SendMessage((string)m_TweenArguments[callbackType], (object)m_TweenArguments[callbackType + "params"], SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.LogError("iTween Error: Callback method references must be passed as a String!");
                Destroy(this);
            }
        }
    }

    void Dispose()
    {
        for (int i = 0; i < tweens.Count; i++)
        {
            Hashtable tweenEntry = tweens[i];
            if ((string)tweenEntry["id"] == id)
            {
                tweens.RemoveAt(i);
                break;
            }
        }
        Destroy(this);
    }

    void ConflictCheck()
    {
        //if a new iTween is about to run and is of the same type as an in progress iTween this will destroy the previous if the new one is NOT identical in every way or it will destroy the new iTween if they are:
        Component[] tweens = GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            if (item.type == "value")
            {
                return;
            }
            else if (item.isRunning && item.type == type)
            {
                //cancel out if this is a shake or punch variant:
                if (item.method != method)
                {
                    return;
                }

                //step 1: check for length first since it's the fastest:
                if (item.m_TweenArguments.Count != m_TweenArguments.Count)
                {
                    item.Dispose();
                    return;
                }

                //step 2: side-by-side check to figure out if this is an identical tween scenario to handle Update usages of iTween:
                foreach (DictionaryEntry currentProp in m_TweenArguments)
                {
                    if (!item.m_TweenArguments.Contains(currentProp.Key))
                    {
                        item.Dispose();
                        return;
                    }
                    else
                    {
                        if (!item.m_TweenArguments[currentProp.Key].Equals(m_TweenArguments[currentProp.Key]) && (string)currentProp.Key != "id")
                        {//if we aren't comparing ids and something isn't exactly the same replace the running iTween: 
                            item.Dispose();
                            return;
                        }
                    }
                }

                //step 3: prevent a new iTween addition if it is identical to the currently running iTween
                Dispose();
                //Destroy(this);	
            }
        }
    }

    void EnableKinematic()
    {
        if (gameObject.GetComponent(typeof(Rigidbody)))
        {
            var rigidbody = GetComponent<Rigidbody>();
            if (!rigidbody.isKinematic)
            {
                m_Kinematic = true;
                rigidbody.isKinematic = true;
            }
        }
    }

    void DisableKinematic()
    {
        var rigidbody = GetComponent<Rigidbody>();
        if (m_Kinematic && rigidbody.isKinematic)
        {
            m_Kinematic = false;
            rigidbody.isKinematic = false;
        }
    }

    void ResumeDelay()
    {
        StartCoroutine("TweenDelay");
    }

    #endregion
    
    #region Interpolation functions

    private static float clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) * 0.5f);
        float retval = 0.0f;
        float diff = 0.0f;
        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;
        return retval;
    }

    private static float punch(float amplitude, float value)
    {
        float s = 9;
        if (value == 0)
        {
            return 0;
        }
        else if (value == 1)
        {
            return 0;
        }
        float period = 1 * 0.3f;
        s = period / (2 * Mathf.PI) * Mathf.Asin(0);
        return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }

    #endregion
    
    #region Deprecated and Renamed

    //public static void audioFrom(GameObject target, Hashtable args) { Debug.LogError("iTween Error: audioFrom() has been renamed to AudioFrom()."); }
    //public static void audioTo(GameObject target, Hashtable args) { Debug.LogError("iTween Error: audioTo() has been renamed to AudioTo()."); }
    //public static void colorFrom(GameObject target, Hashtable args) { Debug.LogError("iTween Error: colorFrom() has been renamed to ColorFrom()."); }
    //public static void colorTo(GameObject target, Hashtable args) { Debug.LogError("iTween Error: colorTo() has been renamed to ColorTo()."); }
    //public static void fadeFrom(GameObject target, Hashtable args) { Debug.LogError("iTween Error: fadeFrom() has been renamed to FadeFrom()."); }
    //public static void fadeTo(GameObject target, Hashtable args) { Debug.LogError("iTween Error: fadeTo() has been renamed to FadeTo()."); }
    //public static void lookFrom(GameObject target, Hashtable args) { Debug.LogError("iTween Error: lookFrom() has been renamed to LookFrom()."); }
    //public static void lookFromWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: lookFromWorld() has been deprecated. Please investigate LookFrom()."); }
    //public static void lookTo(GameObject target, Hashtable args) { Debug.LogError("iTween Error: lookTo() has been renamed to LookTo()."); }
    //public static void lookToUpdate(GameObject target, Hashtable args) { Debug.LogError("iTween Error: lookToUpdate() has been renamed to LookUpdate()."); }
    //public static void lookToUpdateWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: lookToUpdateWorld() has been deprecated. Please investigate LookUpdate()."); }
    //public static void moveAdd(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveAdd() has been renamed to MoveAdd()."); }
    //public static void moveAddWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveAddWorld() has been deprecated. Please investigate MoveAdd()."); }
    //public static void moveBy(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveBy() has been renamed to MoveBy()."); }
    //public static void moveByWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveAddWorld() has been deprecated. Please investigate MoveAdd()."); }
    //public static void moveFrom(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveFrom() has been renamed to MoveFrom()."); }
    //public static void moveFromWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveFromWorld() has been deprecated. Please investigate MoveFrom()."); }
    //public static void moveTo(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveTo() has been renamed to MoveTo()."); }
    //public static void moveToBezier(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveToBezier() has been deprecated. Please investigate MoveTo() and the "path" property."); }
    //public static void moveToBezierWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveToBezierWorld() has been deprecated. Please investigate MoveTo() and the "path" property."); }
    //public static void moveToUpdate(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveToUpdate() has been renamed to MoveUpdate()."); }
    //public static void moveToUpdateWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveToUpdateWorld() has been deprecated. Please investigate MoveUpdate()."); }
    //public static void moveToWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: moveToWorld() has been deprecated. Please investigate MoveTo()."); }
    //public static void punchPosition(GameObject target, Hashtable args) { Debug.LogError("iTween Error: punchPosition() has been renamed to PunchPosition()."); }
    //public static void punchPositionWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: punchPositionWorld() has been deprecated. Please investigate PunchPosition()."); }
    //public static void punchRotation(GameObject target, Hashtable args) { Debug.LogError("iTween Error: punchPosition() has been renamed to PunchRotation()."); }
    //public static void punchRotationWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: punchRotationWorld() has been deprecated. Please investigate PunchRotation()."); }
    //public static void punchScale(GameObject target, Hashtable args) { Debug.LogError("iTween Error: punchScale() has been renamed to PunchScale()."); }
    //public static void rotateAdd(GameObject target, Hashtable args) { Debug.LogError("iTween Error: rotateAdd() has been renamed to RotateAdd()."); }
    //public static void rotateBy(GameObject target, Hashtable args) { Debug.LogError("iTween Error: rotateBy() has been renamed to RotateBy()."); }
    //public static void rotateByWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: rotateByWorld() has been deprecated. Please investigate RotateBy()."); }
    //public static void rotateFrom(GameObject target, Hashtable args) { Debug.LogError("iTween Error: rotateFrom() has been renamed to RotateFrom()."); }
    //public static void rotateTo(GameObject target, Hashtable args) { Debug.LogError("iTween Error: rotateTo() has been renamed to RotateTo()."); }
    //public static void scaleAdd(GameObject target, Hashtable args) { Debug.LogError("iTween Error: scaleAdd() has been renamed to ScaleAdd()."); }
    //public static void scaleBy(GameObject target, Hashtable args) { Debug.LogError("iTween Error: scaleBy() has been renamed to ScaleBy()."); }
    //public static void scaleFrom(GameObject target, Hashtable args) { Debug.LogError("iTween Error: scaleFrom() has been renamed to ScaleFrom()."); }
    //public static void scaleTo(GameObject target, Hashtable args) { Debug.LogError("iTween Error: scaleTo() has been renamed to ScaleTo()."); }
    //public static void shake(GameObject target, Hashtable args) { Debug.LogError("iTween Error: scale() has been deprecated. Please investigate ShakePosition(), ShakeRotation() and ShakeScale()."); }
    //public static void shakeWorld(GameObject target, Hashtable args) { Debug.LogError("iTween Error: shakeWorld() has been deprecated. Please investigate ShakePosition(), ShakeRotation() and ShakeScale()."); }
    //public static void stab(GameObject target, Hashtable args) { Debug.LogError("iTween Error: stab() has been renamed to Stab()."); }
    //public static void stop(GameObject target, Hashtable args) { Debug.LogError("iTween Error: stop() has been renamed to Stop()."); }
    //public static void stopType(GameObject target, Hashtable args) { Debug.LogError("iTween Error: stopType() has been deprecated. Please investigate Stop()."); }

    //public static void tweenCount(GameObject target, Hashtable args) { Debug.LogError("iTween Error: tweenCount() has been deprecated. Please investigate Count()."); }

    #endregion
}

// Copyright (c) 2011 - 2018 Bob Berkebile (pixelplacment)
// Please direct any bugs/comments/suggestions to http://pixelplacement.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
