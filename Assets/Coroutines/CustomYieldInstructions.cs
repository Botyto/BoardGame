using System;
using UnityEngine;

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
    public WaitForCamera() { }

    public WaitForCamera(Transform newTarget)
    {
        FollowCamera.Push(newTarget);
    }

    public WaitForCamera(Transform[] newTarget)
    {
        FollowCamera.Push(newTarget);
    }
    
    public override bool keepWaiting { get { return FollowCamera.instance.isMoving; } }
}

public class WaitForObjectDestroyed : CustomYieldInstruction
{
    private UnityEngine.Object m_Object;
    
    public WaitForObjectDestroyed(UnityEngine.Object obj)
    {
        m_Object = obj;
    }

    public override bool keepWaiting { get { return m_Object != null; } }
}
