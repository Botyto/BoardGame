using System;
using System.Collections;
using UnityEngine;

public static class UnityCoroutineExtensions
{
    public static Coroutine StartCoroutineEx(this MonoBehaviour self, IEnumerator routine)
    {
        UnityCoroutine coroutine;
        return StartCoroutineEx(self, routine, out coroutine);
    }

    public static Coroutine StartCoroutineEx(this MonoBehaviour self, IEnumerator routine, out UnityCoroutine unityCoroutine)
    {
        if (routine == null)
        {
            throw new ArgumentNullException("routine");
        }

        unityCoroutine = new UnityCoroutine(routine);
        return self.StartCoroutine(unityCoroutine.Start());
    }
}
