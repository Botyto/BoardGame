using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class CellDefinition : ScriptableObject
{
    [Serializable]
    public struct Parameter
    {
        public string key;
        public string value;
    }

    [Header("Visuals")]
    public string displayName = "";
    public Material material;

    [Header("Functional")]
    public string[] enterEffects = new string[] { "Announce", "DrawCard" };
    public string[] leaveEffects = new string[0];
    public Parameter[] parameters = new Parameter[0];

    public string GetParameter(string key, string defaultValue = "")
    {
        foreach (var param in parameters)
        {
            if (param.key == key)
            {
                return param.value;
            }
        }

        return defaultValue;
    }

    public string finalDisplayName { get { return (displayName != "") ? displayName : name; } }

    public void ApplyDefinition(GameObject cell)
    {
        cell.name = finalDisplayName;

        var renderer = cell.GetComponent<MeshRenderer>() ?? cell.AddComponent<MeshRenderer>();
        renderer.material = material;
    }

    public void OnEnter(GameObject cell, Player player)
    {
        ApplyEffects(enterEffects, cell, player);
    }

    public void OnLeave(GameObject cell, Player player)
    {
        ApplyEffects(leaveEffects, cell, player);
    }

    private void ApplyEffects(string[] effects, GameObject cell, Player player)
    {
        var ty = typeof(CellEffects);
        var parameters = new object[] { this, cell, player };
        foreach (var effect in effects)
        {
            var method = ty.GetMethod(effect);
            if (method == null) { continue; }

            method.Invoke(null, parameters);
        }
    }
}
