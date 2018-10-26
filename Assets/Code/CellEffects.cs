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
        //TODO - this effect should use a parameter to allaw multiple decks
        //var deck = definition.GetParameter("deck", defaultValue: "default");

        //TODO - this should be a coroutine that will wait for the card to do it's effect and be destroyed (or the other way around) (see MessageBox effect above)
        //Also where do we push the deck in the camera follow objects? (and should we?)
        GameController.instance.deck.DrawCard();
        yield break;
    }
}
