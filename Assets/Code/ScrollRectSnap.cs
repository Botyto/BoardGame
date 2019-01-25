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

    [HideInInspector]
    public RectTransform[] items = null;
    [HideInInspector]
    public int snappedItemIndex = -1;
    public RectTransform snappedItem
    {
        get
        {
            if (items == null || items.Length == 0 || snappedItemIndex == -1)
            {
                return null;
            }

            return items[snappedItemIndex];
        }
    }

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

    void OnEnable()
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

            var n = content.childCount;
            if (n > 0)
            {
                var minY = float.PositiveInfinity;
                var maxY = float.NegativeInfinity;
                m_Points = new float[n];
                items = new RectTransform[n];
                for (int i = 0; i < n; ++i)
                {
                    var child = content.GetChild(i) as RectTransform;
                    items[i] = child;
                    m_Points[i] = child.localPosition.y;
                    minY = Mathf.Min(minY, m_Points[i]);
                    maxY = Mathf.Max(maxY, m_Points[i]);
                }

                for (int i = 0; i < n; ++i)
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
        var scrollVPos = targetScrollRect.verticalNormalizedPosition;
        int target = FindNearest(scrollVPos, m_Points);
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
        
        if (targetScrollRect.vertical && scrollVPos > 0f && scrollVPos < 1f)
        {
            snappedItemIndex = target;
            m_TargetV = m_Points[target];
            m_LerpV = true;
        }
        else
        {
            snappedItemIndex = -1;
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
