using System.Collections;
using UnityEngine;

public static partial class CellEffects
{
    public static IEnumerator Announce(CellDefinition definition, GameObject cell, Player player)
    {
        Debug.LogFormat(player, "{0} at {1}", player.name, definition.name);
        yield break;
    }

    public static IEnumerator MessageBox(CellDefinition definition, GameObject cell, Player player)
    {
        var text = definition.GetParameter("text", defaultValue: "");
        var title = definition.GetParameter("title", defaultValue: "Message Box");
        var msgBox = Dialog.Spawn<MessageBox>();
        msgBox.titleControl.text = title;
        msgBox.textControl.text = text;

        yield return new WaitForObjectDestroyed(msgBox.gameObject);
    }

    public static IEnumerator DrawCard(CellDefinition definition, GameObject cell, Player player)
    {
        var deckName = definition.GetParameter("deck", defaultValue: "default");
        Deck deck = null;
        if (!GameController.instance.decks.TryGetValue(deckName, out deck))
        {
            Debug.LogFormat("<color=red>[Failed]</color> Deck with name {0} doesn't exists!", deckName);
            yield break;
        }

        yield return deck.DrawCard(player);
    }
}
