using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class iTween
{
    /// <summary>
    /// Returns the length of a curved path drawn through the provided array of Transforms.
    /// </summary>
    /// <returns>
    /// A <see cref="System.Single"/>
    /// </returns>
    /// <param name='path'>
    /// A <see cref="Transform[]"/>
    /// </param>
    public static float PathLength(Transform[] path)
    {
        Vector3[] suppliedPath = new Vector3[path.Length];
        float pathLength = 0;

        //create and store path points:
        for (int i = 0; i < path.Length; i++)
        {
            suppliedPath[i] = path[i].position;
        }

        Vector3[] vector3s = PathControlPointGenerator(suppliedPath);

        //Line Draw:
        Vector3 prevPt = Interp(vector3s, 0);
        int SmoothAmount = path.Length * 20;
        for (int i = 1; i <= SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = Interp(vector3s, pm);
            pathLength += Vector3.Distance(prevPt, currPt);
            prevPt = currPt;
        }

        return pathLength;
    }

    /// <summary>
    /// Returns the length of a curved path drawn through the provided array of Vector3s.
    /// </summary>
    /// <returns>
    /// The length.
    /// </returns>
    /// <param name='path'>
    /// A <see cref="Vector3[]"/>
    /// </param>
    public static float PathLength(Vector3[] path)
    {
        float pathLength = 0;

        Vector3[] vector3s = PathControlPointGenerator(path);

        //Line Draw:
        Vector3 prevPt = Interp(vector3s, 0);
        int SmoothAmount = path.Length * 20;
        for (int i = 1; i <= SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = Interp(vector3s, pm);
            pathLength += Vector3.Distance(prevPt, currPt);
            prevPt = currPt;
        }

        return pathLength;
    }

    /// <summary>
    /// Puts a GameObject on a path at the provided percentage 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/>
    /// </param>
    /// <param name="path">
    /// A <see cref="Vector3[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    public static void PutOnPath(GameObject target, Vector3[] path, float percent)
    {
        target.transform.position = Interp(PathControlPointGenerator(path), percent);
    }

    /// <summary>
    /// Puts a GameObject on a path at the provided percentage 
    /// </summary>
    /// <param name="target">
    /// A <see cref="Transform"/>
    /// </param>
    /// <param name="path">
    /// A <see cref="Vector3[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    public static void PutOnPath(Transform target, Vector3[] path, float percent)
    {
        target.position = Interp(PathControlPointGenerator(path), percent);
    }

    /// <summary>
    /// Puts a GameObject on a path at the provided percentage 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/>
    /// </param>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    public static void PutOnPath(GameObject target, Transform[] path, float percent)
    {
        //create and store path points:
        Vector3[] suppliedPath = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            suppliedPath[i] = path[i].position;
        }
        target.transform.position = Interp(PathControlPointGenerator(suppliedPath), percent);
    }

    /// <summary>
    /// Puts a GameObject on a path at the provided percentage 
    /// </summary>
    /// <param name="target">
    /// A <see cref="Transform"/>
    /// </param>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    public static void PutOnPath(Transform target, Transform[] path, float percent)
    {
        //create and store path points:
        Vector3[] suppliedPath = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            suppliedPath[i] = path[i].position;
        }
        target.position = Interp(PathControlPointGenerator(suppliedPath), percent);
    }

    /// <summary>
    /// Returns a Vector3 position on a path at the provided percentage  
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    /// <returns>
    /// A <see cref="Vector3"/>
    /// </returns>
    public static Vector3 PointOnPath(Transform[] path, float percent)
    {
        //create and store path points:
        Vector3[] suppliedPath = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            suppliedPath[i] = path[i].position;
        }
        return (Interp(PathControlPointGenerator(suppliedPath), percent));
    }
    
    //##################################
    //# RESUME UTILITIES AND OVERLOADS # 
    //##################################

    /// <summary>
    /// Resume all iTweens on a GameObject.
    /// </summary>
    public static void Resume(GameObject target)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            item.enabled = true;
        }
    }

    /// <summary>
    /// Resume all iTweens on a GameObject including its children.
    /// </summary>
    public static void Resume(GameObject target, bool includechildren)
    {
        Resume(target);
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Resume(child.gameObject, true);
            }
        }
    }

    /// <summary>
    /// Resume all iTweens on a GameObject of a particular type.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to resume.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Resume(GameObject target, string type)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.type + item.method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                item.enabled = true;
            }
        }
    }

    /// <summary>
    /// Resume all iTweens on a GameObject of a particular type including its children.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to resume.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Resume(GameObject target, string type, bool includechildren)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.type + item.method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                item.enabled = true;
            }
        }
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Resume(child.gameObject, type, true);
            }
        }
    }

    /// <summary>
    /// Resume all iTweens in scene.
    /// </summary>
    public static void Resume()
    {
        for (int i = 0; i < tweens.Count; i++)
        {
            Hashtable currentTween = tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            Resume(target);
        }
    }

    /// <summary>
    /// Resume all iTweens in scene of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to resume.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param> 
    public static void Resume(string type)
    {
        ArrayList resumeArray = new ArrayList();

        for (int i = 0; i < tweens.Count; i++)
        {
            Hashtable currentTween = tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            resumeArray.Insert(resumeArray.Count, target);
        }

        for (int i = 0; i < resumeArray.Count; i++)
        {
            Resume((GameObject)resumeArray[i], type);
        }
    }

    //#################################
    //# PAUSE UTILITIES AND OVERLOADS #
    //#################################

    /// <summary>
    /// Pause all iTweens on a GameObject.
    /// </summary>
    public static void Pause(GameObject target)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            if (item.delay > 0)
            {
                item.delay -= Time.time - item.m_DelayStarted;
                item.StopCoroutine("TweenDelay");
            }
            item.isPaused = true;
            item.enabled = false;
        }
    }

    /// <summary>
    /// Pause all iTweens on a GameObject including its children.
    /// </summary>
    public static void Pause(GameObject target, bool includechildren)
    {
        Pause(target);
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Pause(child.gameObject, true);
            }
        }
    }

    /// <summary>
    /// Pause all iTweens on a GameObject of a particular type.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to pause.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Pause(GameObject target, string type)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.type + item.method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                if (item.delay > 0)
                {
                    item.delay -= Time.time - item.m_DelayStarted;
                    item.StopCoroutine("TweenDelay");
                }
                item.isPaused = true;
                item.enabled = false;
            }
        }
    }

    /// <summary>
    /// Pause all iTweens on a GameObject of a particular type including its children.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to pause.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Pause(GameObject target, string type, bool includechildren)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.type + item.method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                if (item.delay > 0)
                {
                    item.delay -= Time.time - item.m_DelayStarted;
                    item.StopCoroutine("TweenDelay");
                }
                item.isPaused = true;
                item.enabled = false;
            }
        }
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Pause(child.gameObject, type, true);
            }
        }
    }

    /// <summary>
    /// Pause all iTweens in scene.
    /// </summary>
    public static void Pause()
    {
        for (int i = 0; i < tweens.Count; i++)
        {
            Hashtable currentTween = tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            Pause(target);
        }
    }

    /// <summary>
    /// Pause all iTweens in scene of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to pause.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param> 
    public static void Pause(string type)
    {
        ArrayList pauseArray = new ArrayList();

        for (int i = 0; i < tweens.Count; i++)
        {
            Hashtable currentTween = tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            pauseArray.Insert(pauseArray.Count, target);
        }

        for (int i = 0; i < pauseArray.Count; i++)
        {
            Pause((GameObject)pauseArray[i], type);
        }
    }

    //#################################
    //# COUNT UTILITIES AND OVERLOADS #
    //#################################

    /// <summary>
    /// Count all iTweens in current scene.
    /// </summary>
    public static int Count()
    {
        return (tweens.Count);
    }

    /// <summary>
    /// Count all iTweens in current scene of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to stop.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param> 
    public static int Count(string type)
    {
        int tweenCount = 0;

        for (int i = 0; i < tweens.Count; i++)
        {
            Hashtable currentTween = tweens[i];
            string targetType = (string)currentTween["type"] + (string)currentTween["method"];
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                tweenCount++;
            }
        }

        return (tweenCount);
    }

    /// <summary>
    /// Count all iTweens on a GameObject.
    /// </summary>
    public static int Count(GameObject target)
    {
        Component[] tweens = target.GetComponents<iTween>();
        return (tweens.Length);
    }

    /// <summary>
    /// Count all iTweens on a GameObject of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to count.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>  
    public static int Count(GameObject target, string type)
    {
        int tweenCount = 0;
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.type + item.method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                tweenCount++;
            }
        }
        return (tweenCount);
    }

    //################################
    //# STOP UTILITIES AND OVERLOADS #
    //################################

    /// <summary>
    /// Stop and destroy all Tweens in current scene.
    /// </summary>
    public static void Stop()
    {
        for (int i = 0; i < tweens.Count; i++)
        {
            Hashtable currentTween = tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            Stop(target);
        }
        tweens.Clear();
    }

    /// <summary>
    /// Stop and destroy all iTweens in current scene of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to stop.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param> 
    public static void Stop(string type)
    {
        ArrayList stopArray = new ArrayList();

        for (int i = 0; i < tweens.Count; i++)
        {
            Hashtable currentTween = tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            stopArray.Insert(stopArray.Count, target);
        }

        for (int i = 0; i < stopArray.Count; i++)
        {
            Stop((GameObject)stopArray[i], type);
        }
    }

    /// <summary>
    /// Stop and destroy all iTweens in current scene of a particular name.
    /// </summary>
    /// <param name="name">
    /// The <see cref="System.String"/> name of iTween you would like to stop.
    /// </param> 
    public static void StopByName(string name)
    {
        ArrayList stopArray = new ArrayList();

        for (int i = 0; i < tweens.Count; i++)
        {
            Hashtable currentTween = tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            stopArray.Insert(stopArray.Count, target);
        }

        for (int i = 0; i < stopArray.Count; i++)
        {
            StopByName((GameObject)stopArray[i], name);
        }
    }

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject.
    /// </summary>
    public static void Stop(GameObject target)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            item.Dispose();
        }
    }

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject including its children.
    /// </summary>
    public static void Stop(GameObject target, bool includechildren)
    {
        Stop(target);
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Stop(child.gameObject, true);
            }
        }
    }

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject of a particular type.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to stop.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Stop(GameObject target, string type)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.type + item.method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                item.Dispose();
            }
        }
    }

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject of a particular name.
    /// </summar
    /// <param name="name">
    /// The <see cref="System.String"/> name of iTween you would like to stop.
    /// </param>	
    public static void StopByName(GameObject target, string name)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            /*string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				item.Dispose();
			}*/
            if (item.tweenName == name)
            {
                item.Dispose();
            }
        }
    }

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject of a particular type including its children.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to stop.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Stop(GameObject target, string type, bool includechildren)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.type + item.method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                item.Dispose();
            }
        }
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Stop(child.gameObject, type, true);
            }
        }
    }

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject of a particular name including its children.
    /// </summar
    /// <param name="name">
    /// The <see cref="System.String"/> name of iTween you would like to stop.
    /// </param>	
    public static void StopByName(GameObject target, string name, bool includechildren)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            /*string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				item.Dispose();
			}*/
            if (item.tweenName == name)
            {
                item.Dispose();
            }
        }
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                //Stop(child.gameObject,type,true);
                StopByName(child.gameObject, name, true);
            }
        }
    }

    /// <summary>
    /// Universal interface to help in the creation of Hashtables.  Especially useful for C# users.
    /// </summary>
    /// <param name="args">
    /// A <see cref="System.Object[]"/> of alternating name value pairs.  For example "time",1,"delay",2...
    /// </param>
    /// <returns>
    /// A <see cref="Hashtable"/>
    /// </returns>
    public static Hashtable Hash(params object[] args)
    {
        Hashtable hashTable = new Hashtable(args.Length / 2);
        if (args.Length % 2 != 0)
        {
            Debug.LogError("Tween Error: Hash requires an even number of arguments!");
            return null;
        }
        else
        {
            int i = 0;
            while (i < args.Length - 1)
            {
                hashTable.Add(args[i], args[i + 1]);
                i += 2;
            }
            return hashTable;
        }
    }
}
