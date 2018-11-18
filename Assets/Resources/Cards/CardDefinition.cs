using System;
using System.Collections;
using UnityEngine;

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
    public string caption = "";
    public Sprite sprite = null;

    [Header("Functional")]
    public string[] effects = new string[0];
    public Parameter[] parameters = new Parameter[0];

    [Header("Groups")]
    public bool inDefault = true;
    public bool inDrink = true;
    public bool inGame = false;

    public void ApplyDefinition(Card card)
    {
        card.definition = this;

        if (card.caption != null) { card.caption.text = Localization.T(caption); }
        if (card.image != null) { card.image.sprite = sprite; }
        //card.hint.SetActive(true); //TODO hint text should be localized as well. Probably inherit Text UI component
    }

    public IEnumerator Activate(Player player)
    {
        var ty = typeof(CardEffects);
        var parameters = new object[] { this, player };
        foreach (var effect in effects)
        {
            var method = ty.GetMethod(effect);
            if (method == null) { continue; }

            object result = null;
            try
            {
                result = method.Invoke(null, parameters);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            yield return result;
        }
    }
}
