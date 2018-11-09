using System.Collections;

public class CardEffects
{
    public static IEnumerable RepeatTurn(CardDefinition card, Player player)
    {
        GameController.instance.nextPlayerIndex = player.playerNumber;
        return null;
    }
}
