using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private int currentValue = 1;
    public bool rollComplete = true;

    public float forceAmount = 55.0f;
    public float torqueAmount = 150.0f;
    public float rotateDuration = 0.5f;
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
        this.transform.position = Random.onUnitSphere * 3f + new Vector3(5f, 5f, 5f);
        rigidbody.AddForce((Random.onUnitSphere + new Vector3(-3f, 2f, -3f)) * forceAmount, ForceMode.Force);
        //rigidbody.AddTorque(Random.onUnitSphere * torqueAmount, forceMode);

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
        float endRotation = startRotation + Random.Range(45f, 275.0f);
        float t = 0.0f;
        while (t < rotateDuration)
        {
            t += Time.deltaTime;
            float xRotation = Mathf.Lerp(startRotation, endRotation, t / rotateDuration) % 360.0f;
            transform.eulerAngles = new Vector3(xRotation, xRotation, transform.eulerAngles.z);

        }
        yield return null;
    }
}
