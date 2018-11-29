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
    public int numberOfTurns = 1;

    public Cell currentWaypoint { get { return Board.instance.GetWaypoint(currentCellIndex); } }
    public bool isMoving { get { return m_Moving; } }
    
    #region Movement

    private bool m_Moving = false;
    
    public IEnumerator MoveToNext()
    {
        var currentWp = currentWaypoint;
        return MoveToToDirect(currentWp.index + 1);
    }

    public IEnumerator MoveTo(int index)
    {
        var positionDelta = index - currentCellIndex;
        while (positionDelta < 0)
        {
            positionDelta += Board.instance.waypoints.Length;
        }

        return MoveBy(positionDelta);
    }

    public IEnumerator MoveBy(int positionDelta)
    {
        if (m_Moving) { return null; }

        var waypointsOrder = new List<Cell>() { };
        var wp = currentWaypoint;
        var direction = (int)Mathf.Sign(positionDelta);
        for (int i = 0; i != positionDelta; i += direction)
        {
            waypointsOrder.Add(wp);
            var nextIndex = wp.index + direction;
            while (nextIndex < 0)
            {
                nextIndex += Board.instance.waypoints.Length;
            }
            wp = FindWaypointAt(nextIndex);
        }
        waypointsOrder.Add(wp);

        if (waypointsOrder.Count <= 1)
        {
            return null;
        }

        return MoveByRoutine(waypointsOrder.ToArray());
    }

    public IEnumerator MoveByRoutine(Cell[] order)
    {
        yield return OnLeaveCell(currentWaypoint);

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

        yield return OnEnterCell(currentWaypoint);
    }

    public IEnumerator MoveToToDirect(int index)
    {
        if (m_Moving) { return null; }
        var targetWaypoint = FindWaypointAt(index);
        return MoveToDirectRoutine(targetWaypoint);
    }

    public IEnumerator MoveToDirectRoutine(Cell targetWaypoint)
    {
        yield return OnLeaveCell(currentWaypoint);

        currentCellIndex = targetWaypoint.index;

        m_Moving = true;
        var moveTween = iTween.MoveTo(gameObject, iTween.Hash(
            "position", targetWaypoint.bounds.center,
            "speed", moveSpeed,
            "easeType", iTween.EaseType.Linear));
        yield return new WaitForSeconds(moveTween.time);
        m_Moving = false;

        yield return OnEnterCell(currentWaypoint);
    }

    #endregion

    #region Parking

    public const float parkedScale = 0.75f;

    public IEnumerator Park()
    {
        if (m_Moving) { return null; }

        var spot = currentWaypoint.TakeParkingSpace(this);
        return ParkRoutine(spot);
    }

    public IEnumerator ParkRoutine(Vector3 spot)
    {
        var spotlight = GetComponentInChildren<Light>(true);
        if (spotlight != null) { spotlight.enabled = false; }

        m_Moving = true;
        var path = new Vector3[] { transform.position, spot };
        var parkTween = iTween.MoveTo(gameObject, iTween.Hash(
            "path", path,
            "speed", moveSpeed,
            "easeType", iTween.EaseType.Linear));
        var parkingTime = parkTween.time;
        iTween.ScaleTo(gameObject, Vector3.one * parkedScale, parkingTime);
        yield return new WaitForSeconds(parkingTime);
        m_Moving = false;
    }

    public IEnumerator Unpark()
    {
        if (m_Moving) { return null; }

        currentWaypoint.ReleaseParkingSpace(this);
        return UnparkRoutine();
    }

    public IEnumerator UnparkRoutine()
    {
        var spotlight = GetComponentInChildren<Light>(true);
        if (spotlight != null) { spotlight.enabled = true; }

        m_Moving = true;
        var center = currentWaypoint.bounds.center;
        var path = new Vector3[] { transform.position, center };        
        var unparkTween = iTween.MoveTo(gameObject, iTween.Hash(
            "path", path,
            "speed", moveSpeed,
            "easeType", iTween.EaseType.Linear));
        var unpackingTime = unparkTween.time;
        iTween.ScaleTo(gameObject, Vector3.one, unpackingTime);
        yield return new WaitForSeconds(unpackingTime);
        m_Moving = false;
    }

    #endregion

    public IEnumerator BeginTurn()
    {
        yield return new WaitForCamera();
        yield return Unpark();
        
        yield return DiceController.instance.RollDice();

        yield return MoveBy(DiceController.instance.diceSum);
        DiceController.instance.diceSum = 0;

        if (numberOfTurns <= 0)
        {
            yield return Park();
        }

        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator OnEnterCell(Cell cell)
    {
        return cell.definition.OnEnter(cell, this);
    }

    public IEnumerator OnLeaveCell(Cell cell)
    {
        return cell.definition.OnLeave(cell, this);
    }
    
    public void Start()
    {
        ApplyColors();
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
    
    public Cell FindWaypointAt(int index)
    {
        return Board.instance.GetWaypoint(index);
    }
}
