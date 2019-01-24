using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect targetScrollRect;
    
    [Tooltip("How quickly the GUI snaps to each panel")]
    public float snapSpeed = 5.0f;
    public float inertiaCutoffMagnitude = 0.5f;
    
    private bool m_LerpV = false;
    private float m_TargetV = 0.0f;
    
    public float[] m_Points = null;
    private int m_DragStartNearest = -1;

    void Reset()
    {
        if (targetScrollRect == null)
        {
            targetScrollRect = gameObject.GetComponent<ScrollRect>();
        }
    }

    void Start()
    {
        if (targetScrollRect == null)
        {
            targetScrollRect = gameObject.GetComponent<ScrollRect>();
        }

        StartCoroutine(UpdatePoints());
    }
    
    IEnumerator UpdatePoints()
    {
        yield return null;
        Debug.Assert(targetScrollRect != null, this);

        if (targetScrollRect.viewport != null)
        {
            var content = targetScrollRect.viewport.transform as RectTransform;
            if (targetScrollRect.viewport.childCount == 1)
            {
                content = targetScrollRect.viewport.GetChild(0) as RectTransform;
            }

            if (content.childCount > 0)
            {
                var minY = float.PositiveInfinity;
                var maxY = float.NegativeInfinity;
                m_Points = new float[content.childCount];
                for (int i = 0; i < content.childCount; ++i)
                {
                    var child = content.GetChild(i) as RectTransform;
                    m_Points[i] = child.localPosition.y;
                    minY = Mathf.Min(minY, m_Points[i]);
                    maxY = Mathf.Max(maxY, m_Points[i]);
                }

                for (int i = 0; i < content.childCount; ++i)
                {
                    m_Points[i] = (m_Points[i] - minY) / (maxY - minY);
                }
            }
            else
            {
                m_Points = new float[] { 0.0f };
            }
        }
        else
        {
            m_Points = new float[] { 0.0f };
        }
    }

    void Update()
    {
        if (m_LerpV)
        {
            targetScrollRect.verticalNormalizedPosition = Mathf.Lerp(targetScrollRect.verticalNormalizedPosition, m_TargetV, snapSpeed * Time.deltaTime);
            m_LerpV = !Mathf.Approximately(targetScrollRect.verticalNormalizedPosition, m_TargetV);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int target = FindNearest(targetScrollRect.verticalNormalizedPosition, m_Points);
        if (target == m_DragStartNearest && targetScrollRect.velocity.sqrMagnitude > inertiaCutoffMagnitude * inertiaCutoffMagnitude)
        {
            if (targetScrollRect.velocity.x < 0)
            {
                target = m_DragStartNearest + 1;
            }
            else if (targetScrollRect.velocity.x > 1)
            {
                target = m_DragStartNearest - 1;
            }

            target = Mathf.Clamp(target, 0, m_Points.Length - 1);
        }
        
        if (targetScrollRect.vertical && targetScrollRect.verticalNormalizedPosition > 0f && targetScrollRect.verticalNormalizedPosition < 1f)
        {
            m_TargetV = m_Points[target];
            m_LerpV = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_DragStartNearest = FindNearest(targetScrollRect.verticalNormalizedPosition, m_Points);
        m_LerpV = false;
    }

    int FindNearest(float f, float[] array)
    {
        var distance = Mathf.Infinity;
        var output = 0;
        for (int index = 0; index < array.Length; ++index)
        {
            if (Mathf.Abs(array[index] - f) < distance)
            {
                distance = Mathf.Abs(array[index] - f);
                output = index;
            }
        }

        return output;
    }
}
