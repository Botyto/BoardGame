using System;
using UnityEngine;

//Example yield instruction
public class WaitForPredicate<T> : CustomYieldInstruction
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
