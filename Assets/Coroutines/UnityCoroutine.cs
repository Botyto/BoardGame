using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityCoroutine
{
    #region Members

    /// <summary>
    /// States that a UnityCoroutine can be in.
    /// </summary>
    public enum CoroutineState
    {
        Ready,
        Running,
        Paused,
        Finished,
        Failed,
    }

    /// <summary>
    /// The actual coroutine being executed.
    /// </summary>
    [SerializeField]
    private IEnumerator m_Coroutine;
    /// <summary>
    /// The current UnityCoroutine state.
    /// </summary>
    [SerializeField]
    private CoroutineState m_State;
    /// <summary>
    /// Pause reasons for this Coroutine.
    /// </summary>
    [SerializeField]
    private HashSet<string> m_PauseReasons;

    /// <summary>
    /// The current UnityCoroutine state.
    /// </summary>
    public CoroutineState state { get { return m_State; } }

    #endregion

    #region Lifetime

    public UnityCoroutine(IEnumerator routine)
    {
        m_Coroutine = routine;
        m_State = routine != null ? CoroutineState.Ready : CoroutineState.Failed;
        m_PauseReasons = new HashSet<string>();
    }

    /// <summary>
    /// Starts the UnityCoroutine. This is unsafe and exceptions will not be caught.
    /// </summary>
    public IEnumerator Start()
    {
        if (state != CoroutineState.Ready)
        {
            throw new InvalidOperationException("Unable to start coroutine in state: " + m_State);
        }

        m_State = CoroutineState.Running;
        while (m_Coroutine.MoveNext())
        {
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

    /// <summary>
    /// Starts the UnityCoroutine in a safe manner, such that exceptions will cause it to fail.
    /// This version is slower than Start().
    /// </summary>
    public IEnumerator StartSafe()
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
            catch (Exception)
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
    
    /// <summary>
    /// Interrupts the coroutine.
    /// </summary>
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

    /// <summary>
    /// Adds a pause reason and pauses the UnityCoroutine.
    /// </summary>
    public void Pause(string reason) //TODO - test pause reasons.
    {
        m_PauseReasons.Add(reason);
        if (state != CoroutineState.Paused) { Pause(); }
    }

    /// <summary>
    /// Pauses the coroutine.
    /// </summary>
    public void Pause()
    {
        if (state != CoroutineState.Running)
        {
            throw new InvalidOperationException("Unable to pause coroutine in state: " + m_State);
        }

        m_State = CoroutineState.Paused;
    }

    /// <summary>
    /// Removes a pause reason. If no pause reasons are left, the UnityCoroutine will get resumed.
    /// </summary>
    public void Resume(string reason)
    {
        m_PauseReasons.Remove(reason);
        if (state == CoroutineState.Paused && m_PauseReasons.Count == 0)
        {
            Resume();
        }
    }

    /// <summary>
    /// Resumes the coroutine.
    /// </summary>
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
