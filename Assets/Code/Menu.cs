
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
        StartCoroutine(loadScene(1));
        NumberOfPlayers = 5;
        //  SceneManager.LoadScene("Assets/Scenes/Board.unity");
        //  GameObject.Find("GameController").GetComponent<GameController>().playersCount = 6;

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


    IEnumerator loadScene(int index)
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        scene.allowSceneActivation = false;
        sceneAsync = scene;

        //Wait until we are done loading the scene
        while (scene.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + scene.progress);
            yield return null;
        }

        while (!scene.isDone)
        {
            // wait until it is really finished
            yield return null;
        }

        OnFinishedLoadingAllScene();
    }

    void enableScene(int index)
    {
        //Activate the Scene
        sceneAsync.allowSceneActivation = true;


        Scene sceneToLoad = SceneManager.GetSceneByBuildIndex(index);
        if (sceneToLoad.IsValid())
        {
            Debug.Log("Scene is Valid");
            canvasMenu.gameObject.SetActive(false);
            canvasPlayerSelection.gameObject.SetActive(false);
            SceneManager.SetActiveScene(sceneToLoad);

        }
    }

    void OnFinishedLoadingAllScene()
    {

        Debug.Log("Done Loading Scene");
        enableScene(1);
     //   GameObject.Find("GameController").GetComponent<GameController>().playersCount = 6;

        Debug.Log("Scene Activated!");
    }


}
