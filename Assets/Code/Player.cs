using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Visuals")]
    public MeshRenderer[] meshes = null;
    public Color color = Color.white;
    public float moveSpeed = 15.0f;

    [Header("Board")]
    public int playerNumber = 0;
    public int currentCellIndex = 0;

    public Waypoint currentWaypoint { get { return Board.instance.GetWaypoint(currentCellIndex); } }
    public bool isMoving { get { return m_Moving; } }

    #region Movement

    private bool m_Moving = false;
    
    public void MoveToNext()
    {
        var currentWp = currentWaypoint;
        MoveToToDirect(currentWp.index + 1);
    }

    public Coroutine MoveTo(int index)
    {
        if (m_Moving) { return null; }

        var targetWaypoint = FindWaypointAt(index);
        var waypointsOrder = new List<Waypoint>() { };
        var wp = currentWaypoint;
        while (wp != targetWaypoint)
        {
            waypointsOrder.Add(wp);
            wp = FindWaypointAt(wp.index + 1);
        }
        waypointsOrder.Add(wp);

        if (waypointsOrder.Count <= 1)
        {
            return null;
        }

        return StartCoroutine(MoveToRoutine(waypointsOrder.ToArray()));
    }

    public IEnumerator MoveToRoutine(Waypoint[] order)
    {
        OnLeaveCell(currentWaypoint);

        currentCellIndex = order[order.Length - 1].index;

        var path = new Vector3[order.Length];
        for (int i = 0; i < order.Length; ++i)
        {
            path[i] = order[i].bounds.center;
        }
        
        m_Moving = true;
        var moveTween = iTween.MoveTo(gameObject, iTween.Hash(
            "path", path,
            "speed", moveSpeed,
            "easeType", iTween.EaseType.Linear));
        yield return new WaitForSeconds(moveTween.time);
        m_Moving = false;
        
        OnEnterCell(currentWaypoint);
    }

    public Coroutine MoveToToDirect(int index)
    {
        if (m_Moving) { return null; }
        var targetWaypoint = FindWaypointAt(index);
        return StartCoroutine(MoveToDirectRoutine(targetWaypoint));
    }

    public IEnumerator MoveToDirectRoutine(Waypoint targetWaypoint)
    {
        OnLeaveCell(currentWaypoint);

        currentCellIndex = targetWaypoint.index;

        m_Moving = true;
        var moveTween = iTween.MoveTo(gameObject, iTween.Hash(
            "position", targetWaypoint.bounds.center,
            "speed", moveSpeed,
            "easeType", iTween.EaseType.Linear));
        yield return new WaitForSeconds(moveTween.time);
        m_Moving = false;

        OnEnterCell(currentWaypoint);
    }

    #endregion

    #region Parking

    public const float parkedScale = 0.75f;

    public Coroutine Park()
    {
        if (m_Moving) { return null; }

        var spot = currentWaypoint.TakeParkingSpace(this);
        return StartCoroutine(ParkRoutine(spot));
    }

    public IEnumerator ParkRoutine(Vector3 spot)
    {
        var spotlight = GetComponentInChildren<Light>(true);
        if (spotlight != null) { spotlight.enabled = false; }

        var path = new Vector3[] { transform.position, spot };
        var parkTween = iTween.MoveTo(gameObject, iTween.Hash(
            "path", path,
            "speed", moveSpeed,
            "easeType", iTween.EaseType.Linear));
        var parkingTime = parkTween.time;
        iTween.ScaleTo(gameObject, Vector3.one * parkedScale, parkingTime);
        
        yield return new WaitForSeconds(parkingTime);
    }

    public Coroutine Unpark()
    {
        if (m_Moving) { return null; }

        currentWaypoint.ReleaseParkingSpace(this);
        return StartCoroutine(UnparkRoutine());
    }

    public IEnumerator UnparkRoutine()
    {
        var spotlight = GetComponentInChildren<Light>(true);
        if (spotlight != null) { spotlight.enabled = true; }

        var center = currentWaypoint.bounds.center;
        var path = new Vector3[] { transform.position, center };        
        var unparkTween = iTween.MoveTo(gameObject, iTween.Hash(
            "path", path,
            "speed", moveSpeed,
            "easeType", iTween.EaseType.Linear));
        var unpackingTime = unparkTween.time;
        iTween.ScaleTo(gameObject, Vector3.one, unpackingTime);

        yield return new WaitForSeconds(unpackingTime);
    }

    #endregion

    public void OnEnterCell(Waypoint waypoint)
    {
        var cell = Board.instance.boardDefinition.GetCell(waypoint.index);
        cell.OnEnter(waypoint.gameObject, this);
    }

    public void OnLeaveCell(Waypoint waypoint)
    {
        var cell = Board.instance.boardDefinition.GetCell(waypoint.index);
        cell.OnLeave(waypoint.gameObject, this);
    }
    
    public void Start()
    {
        ApplyColors();
        Park();
    }

    public void ApplyColors()
    {
        if (meshes != null)
        {
            foreach (var mesh in meshes)
            {
                mesh.material.color = color;
            }
        }
    }
    
    public Waypoint FindWaypointAt(int index)
    {
        return Board.instance.GetWaypoint(index);
    }
}
