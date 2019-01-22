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
        sideRotations[1] = new Vector3(50, 45, 0); //check
        sideRotations[2] = new Vector3(40, -135, 90); // check
        sideRotations[3] = new Vector3(-40, 45, 90); // check
        sideRotations[4] = new Vector3(50, 45, 180); // check
        sideRotations[5] = new Vector3(0, -45, 40); // check
        diceValueSelector = GetComponent<DiceValueSelector>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public IEnumerator ShakeDice()
    {
        rollComplete = false;
        StartCoroutine( Rotate());
        this.transform.position = Random.onUnitSphere * 3f + new Vector3(5f, 5f, 5f);
        rigidbody.AddForce((Random.onUnitSphere + new Vector3(-4f, 3f, -4f)) * forceAmount, ForceMode.Force);

        while (!GetComponent<Rigidbody>().IsSleeping())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

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

    public IEnumerator ShowToCamera(int n)
    {
        //Removing unnecessary components
        Destroy(this.GetComponent<DiceValueSelector>());
        Destroy(this.GetComponent<BoxCollider>());
        Destroy(this.GetComponent<Rigidbody>());
        yield return null;
        this.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        var cam = Camera.main;
        var targetPos = cam.transform.TransformPoint(Vector3.forward * 5.0f);
        targetPos += new Vector3(-4+n*8, 0, 4-n*8);

        //transform.RotateAround(cam.transform.rotation, transform.up, 15 * Time.deltaTime);
        //transform.LookAt(Camera.main.transform.forward);
        //transform.rotation.loo
        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", new Vector3(4, 4, 4),
            "speed", 1.0f,
            "easetype", iTween.EaseType.Linear));
        Vector3[] sidesVectors = new Vector3[] {
            new Vector3(90, -90, 0),
            new Vector3(0, -90, 90),
            new Vector3(0, 0, 0),
            new Vector3(180, 0, 0),
            new Vector3(90, 90, 90),
            new Vector3(0, -90, 180) };

        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation", 
            Quaternion.LookRotation(cam.transform.forward) * Quaternion.Euler(sidesVectors[currentValue-1]), 
            "easeType", iTween.EaseType.Linear,
            "time", 1.0f));

        iTween.MoveTo(gameObject, iTween.Hash(
            "position", targetPos,
            "time", 2.0f));
      //  iTween.rot
        yield return new WaitForSeconds(2.0f);
    }
}
