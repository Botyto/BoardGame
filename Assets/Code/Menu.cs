using UnityEngine;

public class Menu : MonoBehaviour {

    public Canvas canvasMenu;
    public Canvas canvasOptions;
    public Canvas canvasPlayerSelection;
    
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
        var sceneData = new SceneData();
        sceneData.sceneName = "Board";
        sceneData.SetParameter("playersCount", numOfPlayers);
        SceneController.LoadScene(sceneData);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
