using UnityEngine;
using UnityEngine.UI;

public class MainMenuDialog : Dialog
{
    public override void OnOpen()
    {
        var welcome = ResolveNode("MainMenuMode");
        var options = ResolveNode("OptionsMode");
        var newGame = ResolveNode("NewGameMode");

        //Welcome
        var newGameButton = welcome.ResolveNode<Button>("NewGame");
        newGameButton.onClick.AddListener(delegate { welcome.gameObject.SetActive(false); newGame.gameObject.SetActive(true); });

        var optionsButton = welcome.ResolveNode<Button>("Options");
        optionsButton.onClick.AddListener(delegate { welcome.gameObject.SetActive(false); options.gameObject.SetActive(true); });
        
        //Options
        var skipThrowToggle = options.ResolveNode<Toggle>("SkipThrowToggle");
        skipThrowToggle.isOn = GameSettings.Get<bool>("SkipThrow");
        skipThrowToggle.onValueChanged.AddListener(v => GameSettings.Set("SkipThrow", v));

        var shakeToThrowToggle = options.ResolveNode<Toggle>("ShakeToThrowToggle");
        shakeToThrowToggle.isOn = GameSettings.Get<bool>("Shake");
        shakeToThrowToggle.onValueChanged.AddListener(v => GameSettings.Set("Shake", v));

        options.ResolveNode<Button>("Back").onClick.AddListener(delegate { options.gameObject.SetActive(false); welcome.gameObject.SetActive(true); });
        options.gameObject.SetActive(false);

        //New Game
        var playersChoice = newGame.ResolveNode<ScrollRectSnap>("PlayerCountChoice");

        var startGame = newGame.ResolveNode<Button>("StartGame");
        startGame.onClick.AddListener(delegate {
            var item = playersChoice.snappedItem;
            Debug.Assert(item != null);
            
            var numPlayers = 2;
            var itemText = item.GetComponent<Text>();
            Debug.Assert(itemText != null);
            
            int textNumber;
            if (int.TryParse(itemText.text, out textNumber))
            {
                numPlayers = textNumber;
            }
            
            var sceneData = new SceneData();
            sceneData.sceneName = "Board";
            sceneData.SetParameter("playersCount", numPlayers);
            SceneController.LoadScene(sceneData);
        });

        newGame.ResolveNode<Button>("Back").onClick.AddListener(delegate { newGame.gameObject.SetActive(false); welcome.gameObject.SetActive(true); });
        newGame.gameObject.SetActive(false);
    }
}
