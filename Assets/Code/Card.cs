﻿using System.Collections;
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

    public static Vector3 StartPosition = new Vector3(0f, 2f, 0f); //TODO - these should go away. The card needs to know which deck it came from. There might be multiple decks
    public static Vector3 StartRotation = new Vector3(90f, 0f, 90f);

    public IEnumerator ShowToCamera()
    {
        var cam = Camera.main;

        var targetPos = cam.transform.TransformPoint(Vector3.forward);
        var targetRotation = Quaternion.LookRotation(cam.transform.position - targetPos, cam.transform.up);

        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  transform.position + Vector3.up * 2,
            "easeType", iTween.EaseType.Linear,
            "time", 0.5f));
        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation",  targetRotation.eulerAngles,
            "time", 2.0f,
            "delay", 0.5f));
        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  targetPos,
            "time", 2.0f,
            "delay", 0.5f));

        yield return new WaitForSeconds(2.5f);

        m_Seen = true;
    }

    public IEnumerator ReturnToDeck()
    {
        //TODO - cards should slide underneath the deck (consider invisible decks?)
        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  transform.position + Vector3.up * 2,
            "easeType", iTween.EaseType.Linear,
            "time", 0.5f));
        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation",  StartRotation,
            "time", 2.0f,
            "delay", 0.5f));
        iTween.MoveTo(gameObject, iTween.Hash(
            "position",  StartPosition,
            "time", 2.0f,
            "delay", 0.5f));

        yield return new WaitForSeconds(2.5f);
        //TODO - the card shouldn't be reused. It should be destroyed here and a new one spawned by the deck when needed.
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
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