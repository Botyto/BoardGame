using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityCoroutine
{
    #region Members

    public enum CoroutineState
    {
        Ready,
        Running,
        Paused,
        Finished,
        Failed,
    }

    [SerializeField]
    private IEnumerator m_Coroutine;
    [SerializeField]
    private CoroutineState m_State;
    [SerializeField]
    private HashSet<string> m_PauseReasons;

    public CoroutineState state { get { return m_State; } }

    #endregion

    #region Lifetime

    public UnityCoroutine(IEnumerator routine)
    {
        m_Coroutine = routine;
        m_State = routine != null ? CoroutineState.Ready : CoroutineState.Failed;
        m_PauseReasons = new HashSet<string>();
    }

    public IEnumerator Start()
    {
        if (state != CoroutineState.Ready)
        {
            throw new InvalidOperationException("Unable to start coroutine in state: " + m_State);
        }

        m_State = CoroutineState.Running;
        while (true)
        {
            try
            {
                var success = m_Coroutine.MoveNext();
                if (!success) { break; }
            }
            catch (Exception e)
            {
                m_State = CoroutineState.Failed;
                yield break;
            }

            yield return m_Coroutine.Current;

            while (state == CoroutineState.Paused)
            {
                yield return null;
            }

            if (state == CoroutineState.Finished)
            {
                yield break;
            }
        }

        m_State = CoroutineState.Finished;
    }

    public void Stop()
    {
        if (state != CoroutineState.Running && m_State != CoroutineState.Paused)
        {
            throw new InvalidOperationException("Unable to stop coroutine in state: " + m_State);
        }

        m_State = CoroutineState.Finished;
    }

    #endregion

    #region Pause/Resume

    public void Pause(string reason)
    {
        m_PauseReasons.Add(reason);
        if (state != CoroutineState.Paused) { Pause(); }
    }

    public void Pause()
    {
        if (state != CoroutineState.Running)
        {
            throw new InvalidOperationException("Unable to pause coroutine in state: " + m_State);
        }

        m_State = CoroutineState.Paused;
    }

    public void Resume(string reason)
    {
        m_PauseReasons.Remove(reason);
        if (state == CoroutineState.Paused && m_PauseReasons.Count == 0)
        {
            Resume();
        }
    }

    public void Resume()
    {
        if (state != CoroutineState.Paused)
        {
            throw new InvalidOperationException("Unable to resume coroutine in state: " + m_State);
        }

        m_State = CoroutineState.Running;
    }

    #endregion
}
