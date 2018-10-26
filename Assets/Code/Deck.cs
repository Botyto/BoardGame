using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour {

    public Card card = null;
    Image image;

    public static string cardPrefabPath = "Assets/Prefabs/Card.prefab";
    public static string cardsImagePath = "Assets/Resources/CardImages";
    public static int CardsInDeck = 5;

    void Start()
    {
        image = card.transform.GetChild(0).GetChild(1).GetComponentInChildren<Image>();
    }

    public void DrawCard()
    {
        int cardID = Random.Range(1, CardsInDeck);
        string selectedImagePath = cardsImagePath + "/" + cardID.ToString() + ".jpg";
        Sprite sprite = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath(
            selectedImagePath,
            typeof(Sprite));
        image.sprite = sprite;
        card.Show();

        //Based on CardID do more move other players / draw one more etc ....
    }
}
