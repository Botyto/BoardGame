using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public enum DeckType
    {
        All,
        Default,
        Dink,
        Game,
    }

    [Header("Deck")]
    public string id = "default";
    public GameObject cardPrefab = null;
    public DeckType deckType = DeckType.All; //TODO should this be a list of filters for easier setup in inspector?
    [SerializeField] //TODO visible for debug purposes, hide later
    private List<CardDefinition> m_Cards = null;
    [SerializeField] //TODO visible for debug purposes, hide later
    private List<int> m_CardsQueue = null;

    private void OnEnable()
    {
        m_Cards = new List<CardDefinition>();
        m_CardsQueue = new List<int>();
        foreach (var definition in Resources.LoadAll<CardDefinition>("Cards/Defs/"))
        {
            if (!FilterCardDefinition(definition, deckType)) { continue; }
            m_Cards.Add(definition);
        }

        var n = m_Cards.Count;
        m_CardsQueue.Capacity = n;
        for (int i = 0; i < n; ++i)
        {
            m_CardsQueue.Add(i);
        }

        //Fisher-Yates shuffle
        for (int i = n - 1; i >= 0; --i)
        {
            var j = Random.Range(0, i);
            var temp = m_CardsQueue[i];
            m_CardsQueue[i] = m_CardsQueue[j];
            m_CardsQueue[j] = temp;
        }
    }

    private static bool FilterCardDefinition(CardDefinition definition, DeckType deckType)
    {
        switch (deckType)
        {
            case DeckType.All: return true;
            case DeckType.Default: return definition.inDefault;
            case DeckType.Dink: return definition.inDrink;
            case DeckType.Game: return definition.inGame;
        }

        return false;
    }

    private CardDefinition PeekCard()
    {
        if (m_CardsQueue.Count == 0) { return null; }
        return m_Cards[m_CardsQueue[0]];
    }

    private Card SpawnCard(CardDefinition definition)
    {
        var rotation = transform.rotation * Quaternion.Euler(0, 0, 180);
        var position = transform.position + Vector3.up * 2;
        var newCardObj = Instantiate(cardPrefab, position, rotation);
        var newCard = newCardObj.GetComponent<Card>() ?? newCardObj.AddComponent<Card>();
        newCard.deck = this;
        definition.ApplyDefinition(newCard);

        return newCard;
    }

    public IEnumerator DrawCard(Player forPlayer)
    {
        if (m_CardsQueue.Count == 0) { yield break; }

        FollowCamera.Push(transform);
        yield return new WaitForCamera();

        var definition = PeekCard();
        var card = SpawnCard(definition);

        var cardIdx = m_CardsQueue[0];
        m_CardsQueue.RemoveAt(0);
        m_CardsQueue.Add(cardIdx); //TODO should this card return to the queue before it has been actually activated and visually returned to the deck?

        yield return card.Show();
        yield return definition.Activate(card, forPlayer);
        Destroy(card.gameObject);

        FollowCamera.Pop();
    }
}
