using System.Collections;
using System.Collections.Generic;
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

    public DiceValue upValue = DiceValue.Two;
    public DiceValue forwardValue = DiceValue.Four;
    public DiceValue rightValue = DiceValue.Six;

    private Rigidbody m_Rigidbody = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log(SelectedValue().ToString());
        }
    }

    public DiceValue SelectedValue()
    {
        var dotProducts = new float[3];
        dotProducts[0] = Vector3.Dot(transform.up, Vector3.up);
        dotProducts[1] = Vector3.Dot(transform.forward, Vector3.up);
        dotProducts[2] = Vector3.Dot(transform.right, Vector3.up);

        var maxDotAbs = float.NegativeInfinity;
        var maxDotIdx = -1;
        for (int i = 0; i < dotProducts.Length; ++i)
        {
            var abs = Mathf.Abs(dotProducts[i]);
            if (abs > maxDotAbs)
            {
                maxDotAbs = abs;
                maxDotIdx = i;
            }
        }

        Debug.AssertFormat(-1 <= maxDotIdx && maxDotIdx < dotProducts.Length, "<color=red>[Error]</color> Invalid dice side index {0}", maxDotIdx);

        if (maxDotAbs < 0.8f || maxDotIdx == -1)
        {
            return DiceValue.Undefined;
        }

        var side = DiceValue.Undefined;
        switch (maxDotIdx)
        {
            case 0: side = upValue; break;
            case 1: side = forwardValue; break;
            case 2: side = rightValue; break;
        }
        
        if (dotProducts[maxDotIdx] < 0.0f)
        {
            side = OppositeValue(side);
        }

        return side;
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
