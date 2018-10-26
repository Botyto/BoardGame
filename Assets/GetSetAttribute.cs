using UnityEngine;

public class GetSetAttribute : PropertyAttribute
{
    public readonly string name;
    public bool dirty;

    public GetSetAttribute(string name)
    {
        this.name = name;
    }
}