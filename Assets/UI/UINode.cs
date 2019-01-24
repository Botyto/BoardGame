using System.Collections.Generic;
using UnityEngine;

public class UINode : MonoBehaviour
{
    [HideInInspector]
    public UINode parent = null;
    [HideInInspector]
    public Dictionary<string, UINode> nodes = new Dictionary<string, UINode>();
    
    public UINode this[string nodeName] { get { return ResolveNode(nodeName); } }

    public UINode ResolveNode(string nodeName)
    {
        if (nodes == null) { return null; }
        if (string.IsNullOrEmpty(nodeName)) { return null; }

        UINode result = null;
        if (nodes.TryGetValue(nodeName, out result))
        {
            return result;
        }

        if (parent != null)
        {
            if (parent.nodes.TryGetValue(nodeName, out result))
            {
                return result;
            }
        }

        return null;
    }

    public T ResolveNode<T>(string nodeName) where T : MonoBehaviour
    {
        var node = ResolveNode(nodeName);
        if (node == null) { return null; }
        return node.GetComponent<T>();
    }

    #region Unity internals

    protected virtual void Awake()
    {
        if (!Application.isPlaying) { return; }
        Debug.Assert(!string.IsNullOrEmpty(name), "UINodes cannot have empty names");
        AddToParent();
    }

    protected virtual void OnDestroy()
    {
        if (!Application.isPlaying) { return; }
        RemoveFromParent();
    }

    protected virtual void OnTransformParentChanged()
    {
        if (!Application.isPlaying) { return; }
        RemoveFromParent();
        AddToParent();
    }

    #endregion

    #region UINode Internals

    private void AddToParent()
    {
        parent = FindParent();
        if (parent != null && parent.nodes != null)
        {
            Debug.AssertFormat(!parent.nodes.ContainsKey(name), "Duplicate UINode IDs '{0}' in children of '{1}'", name, parent.name);
            parent.nodes[name] = this;
        }
    }

    private void RemoveFromParent()
    {
        if (parent != null && parent.nodes != null)
        {
            var removed = parent.nodes.Remove(name);
            Debug.AssertFormat(removed, "UINode '{0}' was already removed from its parent '{1}'", name, parent.name);
        }

        parent = null;
    }

    private UINode FindParent()
    {
        var parent = transform.parent;
        while (parent != null)
        {
            var parentNode = parent.GetComponent<UINode>();
            if (parentNode != null)
            {
                return parentNode;
            }

            parent = parent.parent;
        }

        return null;
    }
    
    #endregion
}
