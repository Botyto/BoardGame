using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour //TODO - this is badly named :( shouldn't it be Cell or something like that?
{
    [Header("Waypoint")]
    public int index = -1;
    public bool isCorner = false;
    public Bounds bounds = new Bounds();

    #region Parking

    private List<Player> m_ParkedPlayers = null;

    public Vector3 TakeParkingSpace(Player player)
    {
        for (int i = 0; i < m_ParkedPlayers.Count; ++i)
        {
            if (m_ParkedPlayers[i] == null)
            {
                m_ParkedPlayers[i] = player;
                return GetParkingSpacePosition(i);
            }
        }

        m_ParkedPlayers.Add(player);
        return GetParkingSpacePosition(m_ParkedPlayers.Count - 1);
    }

    public void ReleaseParkingSpace(Player player)
    {
        int i;
        for (i = 0; i < m_ParkedPlayers.Count; ++i)
        {
            if (m_ParkedPlayers[i] == player)
            {
                m_ParkedPlayers[i] = null;
                break;
            }
        }

        //Release some memory
        var allEmpty = true;
        var removedAt = i;
        for (++i; i < m_ParkedPlayers.Count; ++i)
        {
            if (m_ParkedPlayers[i] != null)
            {
                allEmpty = false;
                break;
            }
        }

        if (allEmpty)
        {
            m_ParkedPlayers.RemoveRange(removedAt, m_ParkedPlayers.Count - removedAt);
        }
    }

    public Vector3 GetParkingSpacePosition(int index)
    {
        //TODO - can we generate a grid-like (instead of line-like) arrangement of the parking spaces
        //       This should take into account the size of the cell (see Waypoint.bounds.size) and adjust to it
        return transform.TransformPoint(Vector3.right * 2.0f * (index + 1));
    }

    #endregion

    void OnEnable()
    {
        m_ParkedPlayers = new List<Player>();
    }
}
