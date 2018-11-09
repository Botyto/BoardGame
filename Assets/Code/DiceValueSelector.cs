using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DiceValueSelector : MonoBehaviour
{
    public enum DiceValue
    {
        Undefined = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
    }

    public DiceValue upValue;
    public DiceValue forwardValue;
    public DiceValue rightValue;

    private Rigidbody m_Rigidbody;

    public DiceValue SelectedValue()
    {
        return DiceValue.Undefined; //TODO implement me
    }

    public IEnumerator WaitToSettle()
    {
        while (m_Rigidbody.IsSleeping())
        {
            yield return null;
        }
    }

    void OnEnable()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
}
