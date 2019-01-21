using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private int currentValue = 1;
    public bool rollComplete = true;

    public float forceAmount = 55.0f;
    public float torqueAmount = 150.0f;
    public float rotateDuration = 3f;
    public ForceMode forceMode;

    public new Rigidbody rigidbody;
    public DiceValueSelector diceValueSelector;


    private Vector3[] sideRotations = new Vector3[6];


    private void Awake()
    {
        sideRotations[0] = new Vector3(50, 45, -90); //check
        sideRotations[1] = new Vector3(-50, -135, 0); //check
        sideRotations[2] = new Vector3(40, -135, 90); // check
        sideRotations[3] = new Vector3(-40, 45, 90); // check
        sideRotations[4] = new Vector3(50, 45, 180); // check
        sideRotations[5] = new Vector3(0, 135, 140); // check
        diceValueSelector = GetComponent<DiceValueSelector>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public IEnumerator ShakeDice()
    {
        rollComplete = false;
        StartCoroutine( Rotate());
        this.transform.position = Random.onUnitSphere * 3f + new Vector3(5f, 5f, 5f);
        rigidbody.AddForce((Random.onUnitSphere + new Vector3(-4f, 3f, -4f)) * forceAmount, ForceMode.Force);
        //rigidbody.AddTorque(Random.onUnitSphere * torqueAmount, forceMode);

        while (!GetComponent<Rigidbody>().IsSleeping())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        //if(rigidbody.IsSleeping())
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
            float xRotation = Mathf.Lerp(startRotation, endRotation, t / rotateDuration) % 240.0f;
            transform.eulerAngles = new Vector3(xRotation, xRotation, transform.eulerAngles.z);

        }
        yield return null;
    }

    public IEnumerator ShowToCamera(bool polarity)
    {
        Destroy(this.GetComponent<DiceValueSelector>());
        Destroy(this.GetComponent<BoxCollider>());
        while (!GetComponent<Rigidbody>().IsSleeping())
        {
            yield return null;
        }
        Destroy(this.GetComponent<Rigidbody>());
       // transform.localScale = new Vector3(4, 4, 4);
        this.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        yield return null;
        var cam = Camera.main;

        var targetPos = cam.transform.TransformPoint(Vector3.forward * 5.0f);
        var lookRotation = Quaternion.LookRotation(cam.transform.position - targetPos, cam.transform.up);
        var targetRotation = lookRotation * Quaternion.Euler(180, 90, 90);
        if (polarity)
            targetPos += new Vector3(4, 0, -4);
        else
            targetPos += new Vector3(-4, 0, 4);


        Debug.Log("dice roll: " + currentValue);
        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", new Vector3(4,4,4), 
            "speed", 2.0f, 
            "easetype", iTween.EaseType.Linear));
        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation", sideRotations[currentValue -1],
            "easeType", iTween.EaseType.Linear,
            "time", 1.0f));
        iTween.MoveTo(gameObject, iTween.Hash(
            "position", targetPos ,
            "time", 2.0f));
        yield return new WaitForSeconds(2.0f);
    }
}
