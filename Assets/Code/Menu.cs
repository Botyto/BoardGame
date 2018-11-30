
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Canvas canvasMenu;
    public Canvas canvasOptions;
    public Canvas canvasPlayerSelection;

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
    
    public void StartGame()
    {
        SceneManager.LoadScene("Assets/Scenes/Board.unity");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
