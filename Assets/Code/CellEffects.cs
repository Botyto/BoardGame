using UnityEngine;

public static partial class CellEffects
{
    public static void Announce(CellDefinition definition, GameObject cell, Player player)
    {
        Debug.LogFormat(player, "{0} at {1}", player.name, definition.name);
    }

    public static void MessageBox(CellDefinition definition, GameObject cell, Player player)
    {
        var text = definition.GetParameter("text", defaultValue: "");
        var title = definition.GetParameter("title", defaultValue: "Message Box");
        var msgBox = Dialog.Spawn<MessageBox>();
        msgBox.titleControl.text = title;
        msgBox.textControl.text = text;
    }

    public static void DrawCard(CellDefinition definition, GameObject cell, Player player)
    {
        GameController.instance.deck.DrawCard();
    }
}
