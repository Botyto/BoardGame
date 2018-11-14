using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance { get; private set; }

    protected virtual void Awake()
    {
        Debug.Assert(instance == null);
        instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        Debug.Assert(instance == this);
        instance = null;
    }
}
