using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Card")]
    public CardDefinition definition = null;
    public Deck deck = null;

    private Vector3 m_Velocity = Vector3.zero;
    private Vector3 m_PreviousPosition = Vector3.zero;
    private bool m_IsDragging = false;
    private Vector3 m_DragOffset = Vector3.zero;
    private bool m_Seen = false;
    
    public IEnumerator ShowToCamera()
    {
        var cam = Camera.main;

        var targetPos = cam.transform.TransformPoint(Vector3.forward * 5.0f);
        var lookRotation = Quaternion.LookRotation(cam.transform.position - targetPos, cam.transform.up);
        var targetRotation = lookRotation * Quaternion.Euler(180, 90, 90);

        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  transform.position + Vector3.up * 2,
            "easeType", iTween.EaseType.Linear,
            "time", 0.5f));
        yield return new WaitForSeconds(0.5f);

        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation", targetRotation.eulerAngles,
            "time", 1.0f,
            "easeType", iTween.EaseType.Linear));
        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  targetPos,
            "time", 2.0f));
        yield return new WaitForSeconds(2.0f);

        m_Seen = true;
    }

    public IEnumerator ReturnToDeck()
    {
        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation",  deck.transform.rotation,
            "time", 2.0f));
        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  deck.transform.position,
            "time", 2.0f));

        yield return new WaitForSeconds(2.0f);
    }
    
    public void Show()
    {
        StartCoroutine(ShowToCamera());
    }

    private void Update()
    {
        if (m_IsDragging)
        {
            m_Velocity = transform.position - m_PreviousPosition;
            m_PreviousPosition = transform.position;
        }
        else
        {
            transform.position += m_Velocity;
            m_Velocity *= 0.9f;

            if (m_Seen)
            {
                var viewportPos = Camera.main.WorldToViewportPoint(transform.position);
                if (viewportPos.x < 0 || viewportPos.x > 1) { StartCoroutine(ReturnCardAndContinue()); }
                if (viewportPos.y < 0 || viewportPos.y > 1) { StartCoroutine(ReturnCardAndContinue()); }
            }
        }
    }

    public IEnumerator ReturnCardAndContinue() //TODO - should this be called like that?
    {
        yield return ReturnToDeck();
        Destroy(gameObject);
    }

    #region Dragging

    private void OnMouseDown()
    {
        if (!m_Seen) { return; }

        m_DragOffset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        m_PreviousPosition = transform.position;
        m_IsDragging = true;
    }

    private void OnMouseDrag()
    {
        if (!m_IsDragging) { return; }

        var curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        var curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + m_DragOffset;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        m_IsDragging = false;
    }

    #endregion
}
