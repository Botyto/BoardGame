using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public enum State
    {
        Unseen,
        Seen,
        Dismissed,
    }

    [Header("Card")]
    public CardDefinition definition = null;
    public Deck deck = null;

    [Header("Visuals")]
    public Text caption = null;
    public Image image = null;
    public GameObject hint = null;
    
    [Header("Dragging & Interaction")]
    private Vector3 m_Velocity = Vector3.zero;
    private Vector3 m_PreviousPosition = Vector3.zero;
    private bool m_IsDragging = false;
    private Vector3 m_DragOffset = Vector3.zero;
    private State m_State = State.Unseen;

    private Dictionary<string, object> m_Memory;
    
    public IEnumerator ShowToCamera()
    {
        var cam = Camera.main;

        var targetPos = cam.transform.TransformPoint(Vector3.forward * 5.0f);
        var lookRotation = Quaternion.LookRotation(cam.transform.position - targetPos, cam.transform.up);
        var targetRotation = lookRotation * Quaternion.Euler(180, 90, 90);

        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  transform.position + Vector3.up * 2,
            "easeType", iTween.EaseType.EaseInOutBack,
            "time", 0.5f));
        yield return new WaitForSeconds(0.5f);

        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation", targetRotation.eulerAngles,
            "easeType", iTween.EaseType.Linear,
            "time", 1.0f));
        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  targetPos,
            "easeType", iTween.EaseType.EaseInOutBack,
            "time", 2.0f));
        yield return new WaitForSeconds(2.0f);

        m_State = State.Seen;
    }

    public IEnumerator ReturnToDeck()
    {
        m_State = State.Dismissed;

        var deltaPos = transform.position - deck.transform.position;
        var side = Vector3.Dot(deltaPos, deck.transform.right);
        var settle_dir = (side > 0.0f) ? deck.transform.right : -deck.transform.right;

        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation",  deck.transform.rotation * Quaternion.Euler(0, 0, 180),
            "easeType", iTween.EaseType.EaseInOutBack,
            "time", 0.75f));
        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  deck.transform.position + settle_dir * 5.0f,
            "time", 1.5f));

        yield return new WaitForSeconds(2.0f);

        iTween.MoveTo(gameObject, iTween.Hash(
            "position", deck.transform.position,
            "easeType", iTween.EaseType.EaseInOutBack,
            "time", 0.5f));
        yield return new WaitForSeconds(0.5f + 0.5f);
    }
    
    public IEnumerator Show()
    {
        yield return ShowToCamera();
        
        while (m_State == State.Unseen) { yield return null; }

        deck.SendMessage("CardShown", this, SendMessageOptions.DontRequireReceiver);

        while (true)
        {
            var viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPos.x < 0 || viewportPos.x > 1) { break; }
            if (viewportPos.y < 0 || viewportPos.y > 1) { break; }
            yield return null;
        }
        
        yield return ReturnToDeck();
    }

    private void OnEnable()
    {
        m_Memory = new Dictionary<string, object>();
    }

    private void Update()
    {
        if (m_State == State.Seen)
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
            }
        }
    }

    #region Memory

    public void WriteData(string key, object value)
    {
        m_Memory[key] = value;
    }

    public object ReadData(string key)
    {
        object value = null;
        if (m_Memory.TryGetValue(key, out value))
        {
            return value;
        }
        else
        {
            return "";
        }
    }

    #endregion

    #region Dragging

    private void OnMouseDown()
    {
        if (m_State == State.Unseen) { return; }

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
