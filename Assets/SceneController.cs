using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[Serializable]
public class SceneData
{
    [Serializable]
    private struct ParamData
    {
        public string name;
        public object value;
    }

    public string sceneName = "";
    private ParamData[] parameters = new ParamData[0];

    public void SetParameter<T>(string paramName, T value)
    {
        for (int i = 0; i < parameters.Length; ++i)
        {
            if (parameters[i].name == paramName)
            {
                parameters[i].value = value;
            }
        }

        var n = parameters.Length;
        var newParameters = new ParamData[n + 1];
        Array.Copy(parameters, newParameters, parameters.Length);
        parameters = newParameters;
        parameters[n] = new ParamData { name = paramName, value = value };
    }

    public T GetParameter<T>(string paramName)
    {
        return GetParameter(paramName, default(T));
    }

    public T GetParameter<T>(string paramName, T defaultValue)
    {
        foreach (var param in parameters)
        {
            if (param.name == paramName)
            {
                return (T)param.value;
            }
        }

        return defaultValue;
    }
}

public class SceneController : Singleton<SceneController>
{
    public SceneData sceneData = null;

    public float progress { get; private set; }
    public bool isLoading { get; private set; }

    public string loadingSceneName = "";

    private HashSet<string> m_AllScenes = null;

    public static float loadingProgress { get { return (instance != null) ? instance.progress : 0.0f; } }
    public static bool isCurrentlyLoading { get { return (instance != null) ? instance.isLoading : false; } }

    #region Static interface
    
    public static T GetParameter<T>(string paramName, T defaultValue)
    {
        if (instance != null && instance.sceneData != null)
        {
            return instance.sceneData.GetParameter(paramName, defaultValue);
        }

        return defaultValue;
    }

    public static bool LoadScene(SceneData sceneData)
    {
        if (instance == null) { return false; }

        instance.ChangeScene(sceneData);

        return true;
    }

    public static bool SceneExists(SceneData sceneData)
    {
        return SceneExists(sceneData.sceneName);
    }

    public static bool SceneExists(string sceneName)
    {
        if (instance == null) { return false; }

        return instance.m_AllScenes.Contains(sceneName);
    }

    #endregion

    #region Scene changing

    public void ChangeScene(SceneData sceneData)
    {
        this.sceneData = sceneData;
        StartCoroutine(ChangeSceneRoutine(sceneData.sceneName));
    }

    private IEnumerator ChangeSceneRoutine(string sceneName)
    {
        isLoading = true;

        if (loadingSceneName != "")
        {
            if (m_AllScenes.Contains(loadingSceneName))
            {
                var loadingSceneOp = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
                loadingSceneOp.allowSceneActivation = true;
                while (!loadingSceneOp.isDone)
                {
                    Debug.LogFormat("Loading the loading screen: {1}%", sceneName, (int)((loadingSceneOp.progress / 0.9f) * 100));
                    yield return null;
                }
            }
            else
            {
                Debug.LogWarningFormat("Loading scene with name {0} doesn't exist!", loadingSceneName);
            }
        }
        
        if (m_AllScenes.Contains(sceneName))
        {
            var sceneOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            sceneOp.allowSceneActivation = true;

            while (!sceneOp.isDone)
            {
                progress = sceneOp.progress / 0.9f;
                Debug.LogFormat("Loading scene {0}: {1}%", sceneName, (int)(progress * 100));
                yield return null;
            }
        }
        else
        {
            Debug.LogErrorFormat("Scene {0} doesn't exist!", sceneName);
        }

        isLoading = false;
    }

    #endregion
    
    #region Unity internals

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        m_AllScenes = new HashSet<string>();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            scenePath = scenePath.Replace("Assets/", "");
            scenePath = scenePath.Replace(".unity", "");
            var sceneName = scenePath.Substring(scenePath.LastIndexOf("/") + 1);

            m_AllScenes.Add(scenePath);
            m_AllScenes.Add(sceneName);
        }
    }

    protected void OnEnable()
    {
        SceneManager.sceneLoaded += _OnSceneLoaded;
        SceneManager.sceneUnloaded += _OnSceneUnloaded;
    }

    protected void OnDisable()
    {
        SceneManager.sceneLoaded -= _OnSceneLoaded;
        SceneManager.sceneUnloaded -= _OnSceneUnloaded;
    }

    private void _OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SendMessage("OnSceneLoaded", scene, SendMessageOptions.DontRequireReceiver);
    }

    private void _OnSceneUnloaded(Scene scene)
    {
        SendMessage("OnSceneUnloaded", scene, SendMessageOptions.DontRequireReceiver);
    }

    #endregion
}
