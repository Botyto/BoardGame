using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board")]
    public Cell[] waypoints = null;
    public GameObject waypointPrefab = null;
    public Transform waypointsNode = null;
    public BoardDefinition boardDefinition = null;
    public Transform ground = null;

    [Header("Layout")]
    [SerializeField, GetSet("cellLength")]
    private float m_CellLength = 1.0f;
    public float cellLength { get { return m_CellLength; } set { m_CellLength = value; RebuildCells(); } }
    [SerializeField, GetSet("cellHeight")]
    private float m_CellHeight = 2.0f;
    public float cellHeight { get { return m_CellHeight; } set { m_CellHeight = value; RebuildCells(); } }
    [SerializeField, GetSet("spacing")]
    private float m_Spacing = 0.05f;
    public float spacing { get { return m_Spacing; } set { m_Spacing = value; RebuildCells(); } }
    [SerializeField, GetSet("boardSize")]
    private int m_BoardSize = 10;
    public int boardSize { get { return m_BoardSize; } set { m_BoardSize = value; RebuildCells(); } }

    public void RebuildCells()
    {
        if (boardDefinition == null)
        {
            Debug.LogFormat(this, "<color=red>[Failed]</color> Missing board definition for {0}", name);
            return;
        }

        if (waypointsNode == null)
        {
            Debug.LogFormat(this, "<color=red>[Failed]</color> Missing waypoints node for {0}", name);
            return;
        }

        if (waypointPrefab == null)
        {
            Debug.LogFormat(this, "<color=red>[Failed]</color> Missing waypoints prefab for {0}", name);
            return;
        }

        RespawnCells();

        for (int i = 0; i < waypoints.Length; ++i)
        {
            ApplyCellDefinition(waypoints[i], boardDefinition.GetCell(i));
        }
    }

    public void RespawnCells()
    {
        var totalWaypoints = m_BoardSize * 4;
        bool reuseWaypoints = (waypoints != null);

        if (waypoints != null)
        {
            if (waypoints.Length != totalWaypoints)
            {
                reuseWaypoints = false;
                foreach (var waypoint in waypoints)
                {
                    if (Application.isEditor)
                    {
                        DestroyImmediate(waypoint.gameObject);
                    }
                    else
                    {
                        Destroy(waypoint.gameObject);
                    }
                }
            }
        }
        
        var boardLength = 2 * cellHeight + cellLength * (boardSize - 1) + spacing * boardSize;
        var cornerOffset = (boardLength - cellHeight) / 2.0f;
        var startingPositions = new Vector3[]
        {
            new Vector3(+cornerOffset, 0.0f, +cornerOffset),
            new Vector3(+cornerOffset, 0.0f, -cornerOffset),
            new Vector3(-cornerOffset, 0.0f, -cornerOffset),
            new Vector3(-cornerOffset, 0.0f, +cornerOffset),
        };
        var directions = new Vector3[]
        {
            new Vector3(0, 0, -1),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0),
        };

        var waypointsList = new List<Cell>();
        int waypointIdx = 0;
        for (int side = 0; side < 4; ++side)
        {
            var startingPosition = waypointsNode.transform.TransformPoint(startingPositions[side]);
            var direction = waypointsNode.transform.TransformDirection(directions[side]);
            var positionOffset = 0.0f;

            for (int i = 0; i < boardSize; ++i)
            {
                var position = startingPosition + direction * positionOffset;
                var rotation = Quaternion.Euler(0.0f, side * 90, 0.0f);
                Cell waypointComp = null;

                if (reuseWaypoints)
                {
                    waypointComp = waypoints[waypointIdx];
                }
                
                if (waypointComp == null)
                {
                    var waypointObj = Instantiate(waypointPrefab, waypointsNode) as GameObject;
                    waypointObj.name = string.Format("Waypoint {0}", waypointIdx + 1);
                    waypointComp = waypointObj.GetComponent<Cell>() ?? waypointObj.AddComponent<Cell>();
                }

                waypointComp.gameObject.isStatic = true;
                var waypointTransform = waypointComp.transform;
                waypointTransform.position = position;
                waypointTransform.rotation = rotation;
                waypointTransform.SetParent(waypointsNode);
                waypointTransform.SetAsLastSibling();

                foreach (Transform child in waypointTransform)
                {
                    if (Application.isEditor)
                    {
                        DestroyImmediate(child.gameObject);
                    }
                    else
                    {
                        Destroy(child.gameObject);
                    }
                }

                waypointComp.index = waypointIdx;
                waypointComp.isCorner = (i == 0);
                waypointComp.SetBounds(new Bounds(waypointTransform.position, new Vector3(cellHeight, 0, cellLength)));

                if (i == 0)
                {                    
                    positionOffset += (cellHeight - cellLength) / 2.0f;
                }

                waypointsList.Add(waypointComp);
                positionOffset += cellLength + spacing;
                ++waypointIdx;
            }
        }
        
        waypoints = waypointsList.ToArray();
    }

    public Cell GetWaypoint(int index)
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            return null;
        }

        return waypoints[index % waypoints.Length];
    }

    public void ApplyCellDefinition(Cell cell, CellDefinition definition)
    {
        //TODO after creating special asset fix this step
        if (cell.isCorner)
        {
            cell.transform.localScale = new Vector3(cellHeight, 1, cellHeight) / 10.0f;
        }
        else
        {
            cell.transform.localScale = new Vector3(cellHeight, 1, cellLength) / 10.0f; 
        }

        definition.ApplyDefinition(cell);
    }

    public void OnEnable()
    {
        RebuildCells();
    }

    #region Singleton

    public static Board instance { get; private set; }

    void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
    }

    void OnDestroy()
    {
        Debug.Assert(instance == this);
        instance = null;
    }

    #endregion
}
