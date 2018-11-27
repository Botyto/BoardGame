using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private int currentValue = 1;
    public bool rollComplete = true;

    public float forceAmount = 40.0f;
    public float torqueAmount = 150.0f;
    public float rotateDuration = 1f;
    public ForceMode forceMode;

    public new Rigidbody rigidbody;
    public DiceValueSelector diceValueSelector;

    private void Awake()
    {
        diceValueSelector = GetComponent<DiceValueSelector>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public IEnumerator ShakeDice()
    {
        rollComplete = false;
        StartCoroutine( Rotate());
        this.transform.position += new Vector3(0, 7, 0);
        rigidbody.AddForce(Random.onUnitSphere * forceAmount, forceMode);
        rigidbody.AddTorque(Random.onUnitSphere * torqueAmount, forceMode);

        while (!GetComponent<Rigidbody>().IsSleeping())
        {
            yield return null;
        }

        if(rigidbody.IsSleeping())
        currentValue = (int)diceValueSelector.SelectedValue();
        DiceController.instance.diceSum += currentValue;
        //Debug.Log("Die roll complete, " + currentValue);
        rollComplete = true;
    }

    IEnumerator Rotate()
    {
        float startRotation = transform.eulerAngles.x;
        float endRotation = startRotation + Random.Range(200f, 275.0f);
        float t = 0.0f;
        while (t < rotateDuration)
        {
            t += Time.deltaTime;
            float xRotation = Mathf.Lerp(startRotation, endRotation, t / rotateDuration) % 360.0f;
            transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        }
        yield return null;
    }

    //void Awake()
    //{
    //    // For the sake of this example we assume a regular cube dice if 
    //    // directions haven't been specified in the editor. Sum of opposite
    //    // sides is 7, haven't consider exact real layout though.
    //    if (directions.Count == 0)
    //    {
    //        // Object space directions
    //        directions.Add(Vector3.up);
    //        sideValues.Add(4); // up
    //        directions.Add(Vector3.down);
    //        sideValues.Add(3); // down

    //        directions.Add(Vector3.left);
    //        sideValues.Add(2); // left
    //        directions.Add(Vector3.right);
    //        sideValues.Add(5); // right

    //        directions.Add(Vector3.forward);
    //        sideValues.Add(6); // fw
    //        directions.Add(Vector3.back);
    //        sideValues.Add(1); // back
    //    }

    //    // Assert equal side of lists
    //    if (directions.Count != sideValues.Count)
    //    {
    //        Debug.LogError("Not consistent list sizes");
    //    }
    //}

    //// Gets the number of the side pointing in the same direction as the reference vector,
    //// allowing epsilon degrees error.
    //public int GetNumber(Vector3 referenceVectorUp, float epsilonDeg = 5f)
    //{
    //    // here I would assert lookup is not empty, epsilon is positive and larger than smallest possible float etc
    //    // Transform reference up to object space
    //    Vector3 referenceObjectSpace = transform.InverseTransformDirection(referenceVectorUp);

    //    // Find smallest difference to object space direction
    //    float min = float.MaxValue;
    //    int mostSimilarDirectionIndex = -1;
    //    for (int i = 0; i < directions.Count; ++i)
    //    {
    //        float a = Vector3.Angle(referenceObjectSpace, directions[i]);
    //        if (a <= epsilonDeg && a < min)
    //        {
    //            min = a;
    //            mostSimilarDirectionIndex = i;
    //        }
    //    }

    //    // -1 as error code for not within bounds
    //    return (mostSimilarDirectionIndex >= 0) ? sideValues[mostSimilarDirectionIndex] : -1;
    //}
}
