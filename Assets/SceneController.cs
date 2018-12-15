using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneData : ScriptableObject
{ }

public class SceneController : Singleton<SceneController>
{
    public SceneData sceneData = null;
    
    #region Scene changing

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeSceneRoutine(sceneName));
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
