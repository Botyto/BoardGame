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
    
    public IEnumerator MoveToNext()
    {
        var currentWp = currentWaypoint;
        return MoveToToDirect(currentWp.index + 1);
    }

    public IEnumerator MoveTo(int index)
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

        return MoveToRoutine(waypointsOrder.ToArray());
    }

    public IEnumerator MoveToRoutine(Waypoint[] order)
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

    public IEnumerator MoveToDirectRoutine(Waypoint targetWaypoint)
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
        //TODO should we wait for the camera to arrive (and how? maybe keep it's previous position and check for distance travelled)
        yield return Unpark();

        //TODO - Maybe this should be a flag (or something..) to allow other types of input?
        //(can we avoid making another WaitFor* instruction, but also avoid starting another heavy UnityCoroutine as in GameController?)
        yield return new WaitForKeyDown(KeyCode.Space);
        var dice = GameController.instance.RollDice(2);
        yield return MoveTo(currentCellIndex + dice);

        yield return Park();
        //TODO maybe wait a bit so the motion is not that rought?
    }

    public IEnumerator OnEnterCell(Waypoint waypoint)
    {
        var cell = Board.instance.boardDefinition.GetCell(waypoint.index);
        return cell.OnEnter(waypoint.gameObject, this);
    }

    public IEnumerator OnLeaveCell(Waypoint waypoint)
    {
        var cell = Board.instance.boardDefinition.GetCell(waypoint.index);
        return cell.OnLeave(waypoint.gameObject, this);
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
    
    public Waypoint FindWaypointAt(int index)
    {
        return Board.instance.GetWaypoint(index);
    }
}
