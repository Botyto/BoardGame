using System;
using System.Collections;
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

    public IEnumerator OnEnter(Cell cell, Player player)
    {
        return ApplyEffects(enterEffects, cell, player);
    }

    public IEnumerator OnLeave(Cell cell, Player player)
    {
        return ApplyEffects(leaveEffects, cell, player);
    }

    private IEnumerator ApplyEffects(string[] effects, Cell cell, Player player)
    {
        var ty = typeof(CellEffects);
        var parameters = new object[] { this, cell, player };
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
