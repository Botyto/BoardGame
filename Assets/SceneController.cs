using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneData
{
    [Serializable]
    private struct ParamData
    {
        public string name;
        public object value;
    }

    public string sceneName = ""; //TODO how should we know that this is the correct data for this scene?
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

    #endregion

    #region Scene changing

    public void ChangeScene(SceneData sceneData)
    {
        this.sceneData = sceneData;
        StartCoroutine(ChangeSceneRoutine(sceneData.sceneName));
    }

    private IEnumerator ChangeSceneRoutine(string sceneName)
    {
        var sceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        sceneOperation.allowSceneActivation = true;

        while (!sceneOperation.isDone)
        {
            Debug.LogFormat("Loading scene {0}: {1}%", sceneName, sceneOperation.progress);
            yield return null;
        }
    }

    #endregion

    #region Callbacks

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    private void OnSceneUnloaded(Scene scene)
    {

    }

#endregion

#region Unity internals

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    protected void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    protected void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

#endregion
}
