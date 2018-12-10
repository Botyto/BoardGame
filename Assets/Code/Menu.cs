
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour {

    public Canvas canvasMenu;
    public Canvas canvasOptions;
    public Canvas canvasPlayerSelection;

    private AsyncOperation sceneAsync;
    public int NumberOfPlayers;

    public void ShowOptions()
    {
        canvasMenu.gameObject.SetActive(false);
        canvasOptions.gameObject.SetActive(true);
    }
    public void HideOptions()
    {
        canvasOptions.gameObject.SetActive(false);
        canvasMenu.gameObject.SetActive(true);
    }

    public void ShowPlayerSelection()
    {
        canvasMenu.gameObject.SetActive(false);
        canvasPlayerSelection.gameObject.SetActive(true);
    }
    public void BackToMenuFromPlayersSelect()
    {
        canvasPlayerSelection.gameObject.SetActive(false);
        canvasMenu.gameObject.SetActive(true);
    }
    
    public void StartGame(int numOfPlayers)
    {
        canvasMenu.gameObject.SetActive(true);
        StartCoroutine(loadScene());
        NumberOfPlayers = numOfPlayers;
    }

    public void Quit()
    {
        Application.Quit();
    }


    //TODO: on start check if they are set up 
    //      otherwise game will crash if start checking them in scene "board"
    public void SkipDiceThrow(bool isCheck)
    {
        PlayerPrefs.SetInt("Skip Dice Throw", isCheck ? 1 : 0);
    }
    public void ShakeThrowDice(bool isCheck)
    {
        PlayerPrefs.SetInt("Shake Throw Dice", isCheck ? 1 : 0);
    }


    IEnumerator loadScene()
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        scene.allowSceneActivation = false;
        sceneAsync = scene;

        while (!scene.isDone)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + scene.progress);

            // Check if the load has finished
            if (scene.progress >= 0.9f)
            {
                scene.allowSceneActivation = true;
            }
            yield return null;
        }

        OnFinishedLoadingAllScene();
    }

    void OnFinishedLoadingAllScene()
    {
        enableScene();
        Debug.Log("Scene Activated!");
    }

    void enableScene()
    {
        Scene sceneToLoad = SceneManager.GetSceneByBuildIndex(1);
        if (sceneToLoad.IsValid())
        {
            canvasMenu.gameObject.SetActive(false);
            canvasPlayerSelection.gameObject.SetActive(false);
            SceneManager.SetActiveScene(sceneToLoad);
        }
    }

}
