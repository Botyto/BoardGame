using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DiceValueSelector : MonoBehaviour
{
    private float epsilonDeg = 30f;
    public List<Vector3> directions = new List<Vector3>{Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.back, Vector3.forward};

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log(SelectedValue().ToString());
        }
    }

    public DiceValue SelectedValue()
    {
        Vector3 referenceObjectSpace = transform.InverseTransformDirection(Vector3.up);
        // Find smallest difference to object space direction
        float min = float.MaxValue;
        int mostSimilarDirectionIndex = -1;
        for (int i = 0; i < 6; i++)
        {
            float a = Vector3.Angle(referenceObjectSpace, directions[i]);
            if (a <= epsilonDeg && a < min)
            {
                min = a;
                mostSimilarDirectionIndex = i + 1;
            }
        }

        switch (mostSimilarDirectionIndex)
        {
            case 1: return upValue;
            case 2: return OppositeValue(upValue);
            case 3: return rightValue;
            case 4: return OppositeValue(rightValue);
            case 5: return forwardValue; 
            case 6: return OppositeValue(forwardValue); ;
            default: return 0;         // 0 as error code for not within bounds
        }
    }

    public IEnumerator WaitToSettle()
    {
        while (m_Rigidbody.IsSleeping())
        {
            yield return null;
        }
    }

    private DiceValue OppositeValue(DiceValue value)
    {
        return (DiceValue)(DiceValue.Six - (value - 1));
    }

    void OnEnable()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
}
