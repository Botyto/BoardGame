using System.Collections;
using UnityEngine;

public class CardEffects
{
    public static IEnumerable ModifyTurns(CardDefinition definition, Card card, Player player)
    {
        int count;
        if (!int.TryParse(definition.GetParameter("turns", defaultValue: "1"), out count))
        {
            count = 1;
        }

        player.numberOfTurns += count;
        return null;
    }

    public static IEnumerable SwitchTurnsDirection(CardDefinition definition, Card card, Player player)
    {
        GameController.instance.turnDirection *= -1;
        return null;
    }

    public static IEnumerable ChoosePlayer(CardDefinition definition, Card card, Player player)
    {
        Debug.Log("choose player");
        var text = definition.GetParameter("text", defaultValue: "Choose player");
        var title = definition.GetParameter("title", defaultValue: "Choose player");

        var msgBox = Dialog.Spawn<MessageBox>();
        msgBox.titleControl.text = title;
        msgBox.textControl.text = text;
        msgBox.hasCloseButton = false;

        var playersCount = GameController.instance.playersCount;
        msgBox.additionalButtons = new MessageBox.ButtonEntry[playersCount];
        for (int i = 0; i < playersCount; ++i)
        {
            msgBox.additionalButtons[i].text = string.Format("Player {0}", i + 1);
            msgBox.additionalButtons[i].onClick.AddListener(() => card.WriteData("playerIdx", i));
        }

        Debug.Log("choose player 2");

        yield return new WaitForObjectDestroyed(msgBox.gameObject);

        Debug.Log("choose player 3");
    }
}
