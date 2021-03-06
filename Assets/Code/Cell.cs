﻿using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Cell")]
    public int index = -1;
    public bool isCorner = false;
    public Bounds bounds = new Bounds();
    public CellDefinition definition = null;

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
        const float spacing = 2.0f;
        var rowSize = Mathf.FloorToInt(bounds.size.x / spacing);
        var pt = new Vector3((index / rowSize) + 1, 0, (index % rowSize) + 1 - rowSize / 2) * spacing;
        return transform.TransformPoint(pt);
    }

    #endregion

    public void SetBounds(Bounds newBounds)
    {
        bounds = newBounds;
        var boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.center = boxCollider.center;
            boxCollider.size = boxCollider.size;
        }
    }
    
    void OnEnable()
    {
        m_ParkedPlayers = new List<Player>();
    }
}
