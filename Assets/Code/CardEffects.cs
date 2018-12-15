using System.Collections;
using UnityEngine;

public class CardEffects
{
    public static IEnumerator ModifyTurns(CardDefinition definition, Card card, Player player)
    {
        var playerIdx = GetIntParam(definition, card, "playerIdx", player.playerNumber);
        var count = GetIntParam(definition, card, "turns", 1);

        if (playerIdx >= 0 && playerIdx < GameController.instance.playersCount)
        {
            GameController.instance.players[playerIdx].numberOfTurns += count;
        }

        return null;
    }

    public static IEnumerator SwitchTurnsDirection(CardDefinition definition, Card card, Player player)
    {
        GameController.instance.turnDirection *= -1;
        return null;
    }

    public static IEnumerator ChoosePlayer(CardDefinition definition, Card card, Player player)
    {
        var text = GetStringParam(definition, card, "text", "Choose player");
        var title = GetStringParam(definition, card, "title", "Choose player");

        var msgBox = Dialog.Spawn<MessageBox>();
        msgBox.titleControl.text = title;
        msgBox.textControl.text = text;
        msgBox.hasCloseButton = false;

        var playersCount = GameController.instance.playersCount;
        var buttons = new MessageBox.ButtonEntry[playersCount];
        msgBox.additionalButtons = buttons;
        for (int i = 0; i < playersCount; ++i)
        {
            buttons[i].text = string.Format("Player {0}", i + 1);
            buttons[i].onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            buttons[i].onClick.AddListener(() => { card.WriteData("playerIdx", i); msgBox.Close(); });
        }
        
        yield return new WaitForObjectDestroyed(msgBox.gameObject);
    }

    public static IEnumerator MoveBy(CardDefinition definition, Card card, Player player)
    {
        var count = GetIntParam(definition, card, "count", 0);
        if (count == 0) { yield break; }
        
        var allPlayers = GetIntParam(definition, card, "allPlayers", 0);
        if (allPlayers != 0)
        {
            FollowCamera.Push(Board.instance.ground);
            yield return new WaitForCamera();

            var game = GameController.instance;
            var routines = new UnityCoroutine[game.playersCount];

            for (int i = 0; i < game.playersCount; ++i)
            {
                var currentPlayer = game.players[i];
                routines[i] = new UnityCoroutine(currentPlayer.MoveBy(count));
                currentPlayer.StartCoroutineEx(routines[i].Start());
            }

            foreach (var routine in routines)
            {
                yield return new WaitForRoutine(routine);
            }

            FollowCamera.Pop();
        }
        else
        {
            var playerIdx = GetIntParam(definition, card, "playerIdx", player.playerNumber);
            if (playerIdx < 0 || playerIdx >= GameController.instance.playersCount) { yield break; }

            var targetPlayer = GameController.instance.players[playerIdx];
            FollowCamera.Push(targetPlayer);
            yield return new WaitForCamera();
            yield return targetPlayer.MoveBy(count);
            FollowCamera.Pop();
        }
    }

    public static IEnumerator ShowBoard(CardDefinition definition, Card card, Player player)
    {
        var time = GetIntParam(definition, card, "time", 5);

        FollowCamera.Push(Board.instance.ground);
        yield return new WaitForCamera();
        yield return new WaitForSeconds(time);
        FollowCamera.Pop();
    }

    #region Parameter getters

    private static string GetStringParam(CardDefinition definition, Card card, string key, string defaultValue = "")
    {
        var data = card.ReadData(key) as string;
        if (!string.IsNullOrEmpty(data))
        {
            return data;
        }

        return definition.GetParameter(key, defaultValue);
    }

    private static int GetIntParam(CardDefinition definition, Card card, string key, int defaultValue = 0)
    {
        var data = card.ReadData(key);
        if (data is int)
        {
            return (int)data;
        }

        var param = definition.GetParameter(key, defaultValue.ToString());
        int result;
        if (int.TryParse(param, out result))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }

    #endregion
}
