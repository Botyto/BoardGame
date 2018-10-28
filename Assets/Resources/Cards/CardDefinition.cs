using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu]
public class CardDefinition : ScriptableObject
{
    [Serializable]
    public struct Parameter
    {
        public string key;
        public string value;
    }

    [Header("Visuals")]
    public string displayName = "";
    public Sprite sprite = null;

    [Header("Functional")]
    public string[] effects = new string[0];
    public Parameter[] parameters = new Parameter[0];

    [Header("Groups")]
    public bool inDefault = true;

    public void ApplyDefinition(Card card)
    {
        card.definition = this;

        if (sprite != null)
        {
            var imgUI = card.GetComponentInChildren<Image>();
            if (imgUI != null)
            {
                imgUI.sprite = sprite;
            }
        }
    }
}
