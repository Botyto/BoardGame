using System.Collections.Generic;
using UnityEngine;

public class Dialog : UINode
{
    public static Dictionary<string, Dialog> dialogs = null;
    
    public void Close()
    {
        Destroy(gameObject);
    }

    public virtual void OnOpen() { }
    public virtual void OnClose() { }

    protected virtual void Start()
    {
        if (!CreateDialogsDict())
        {
            dialogs.Add(name, this);
        }

        OnOpen();
	}

    protected override void OnDestroy()
    {
        OnClose();

        if (dialogs != null)
        {
            Dialog result = null;
            var success = dialogs.TryGetValue(name, out result);
            if (success && result == this)
            {
                dialogs.Remove(name);
            }
        }
    }

    private static bool CreateDialogsDict()
    {
        if (dialogs == null)
        {
            dialogs = new Dictionary<string, Dialog>();
            foreach (var dlg in FindObjectsOfType<Dialog>())
            {
                dialogs.Add(dlg.name, dlg);
            }

            return true;
        }

        return false;
    }

    public static Dialog Get(string name)
    {
        CreateDialogsDict();

        Dialog result = null;
        var success = dialogs.TryGetValue(name, out result);
        return success ? result : null;
    }

    public static T Spawn<T>(Transform parent = null) where T : Dialog
    {
        if (parent == null)
        {
            var canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
            parent = (canvas != null) ? canvas.transform : null;
        }

        var prefab = Resources.Load<GameObject>("Dialogs/" + typeof(T).Name);
        if (prefab == null)
        {
            Debug.LogFormat("<color=red>[Error]</color>Cannot find dialog prefab {0}", typeof(T).Name);
            return null;
        }

        if (prefab.GetComponent<T>() == null)
        {
            Debug.LogFormat("<color=red>[Error]</color>Dialog {0} doesn't contain its component", typeof(T).Name);
            return null;
        }

        var dlg = Instantiate(prefab, parent);
        return dlg.GetComponent<T>();
    }

    public static void Close(string name)
    {
        var dlg = Get(name);
        if (dlg != null)
        {
            dlg.Close();
        }
    }

    //TODO implement modes
}
