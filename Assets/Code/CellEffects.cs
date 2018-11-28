using System.Collections;
using UnityEngine;

public static partial class CellEffects
{
    public static IEnumerator Announce(CellDefinition definition, Cell cell, Player player)
    {
        Debug.LogFormat(player, "{0} at {1}", player.name, definition.name);
        return null;
    }

    public static IEnumerator MessageBox(CellDefinition definition, Cell cell, Player player)
    {
        var text = definition.GetParameter("text", defaultValue: "");
        var title = definition.GetParameter("title", defaultValue: "Message Box");
        var msgBox = Dialog.Spawn<MessageBox>();
        msgBox.titleControl.text = title;
        msgBox.textControl.text = text;

        yield return new WaitForObjectDestroyed(msgBox.gameObject);
    }

    public static IEnumerator DrawCard(CellDefinition definition, Cell cell, Player player)
    {
#if UNITY_EDITOR
        Deck debugDeck = null;
        if (GameController.instance.decks.TryGetValue("debug", out debugDeck))
        {
            if (debugDeck.deckSize > 0)
            {
                return debugDeck.DrawCard(player);
            }
        }
#endif

        var deckName = definition.GetParameter("deck", defaultValue: "default");
        Deck deck = null;
        if (!GameController.instance.decks.TryGetValue(deckName, out deck))
        {
            Debug.LogFormat("<color=red>[Failed]</color> Deck with name {0} doesn't exists!", deckName);
            return null;
        }

        return deck.DrawCard(player);
    }
}
