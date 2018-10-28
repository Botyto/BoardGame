using System;
using UnityEngine;

//Example yield instruction
public class WaitForPredicate<T> : CustomYieldInstruction //TODO - this might have to go away
{
    public Predicate<T> predicate;
    public T context;

    public WaitForPredicate(Predicate<T> predicate, T context)
    {
        this.predicate = predicate;
        this.context = context;
    }

    public override bool keepWaiting { get { return !predicate(context); } }
}

public class WaitForKeyDown : CustomYieldInstruction
{
    private KeyCode m_KeyCode;

    public WaitForKeyDown(KeyCode keyCode)
    {
        m_KeyCode = keyCode;
    }

    public override bool keepWaiting { get { return !Input.GetKeyDown(m_KeyCode); } }
}

public class WaitForCamera : CustomYieldInstruction
{
    private FollowCamera m_Camera;

    public WaitForCamera(FollowCamera camera)
    {
        m_Camera = camera;
    }

    public override bool keepWaiting { get { return m_Camera.isMoving; } }
}

public class WaitForObjectDestroyed : CustomYieldInstruction //TODO - this might have to go away
{
    private GameObject m_Object;

    public WaitForObjectDestroyed(GameObject obj)
    {
        m_Object = obj;
    }

    public override bool keepWaiting { get { return m_Object != null; } }
}
