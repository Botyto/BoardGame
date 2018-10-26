using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class BoardDefinition : ScriptableObject
{
    [Header("Cells")]
    public CellDefinition[] cells = null;
    public bool useFirstCellOnce = true;

    /// <summary>
    /// Returns the cell definition on the requested index.
    /// If `useFirstCellOnce` is true, then after looping the board, the first cell will be skipped.
    /// </summary>
    public CellDefinition GetCell(int index)
    {
        if (cells == null || cells.Length == 0)
        {
            return null;
        }

        if (useFirstCellOnce)
        {
            if (index >= cells.Length)
            {
                return cells[((index - 1) % (cells.Length - 1)) + 1];
            }
            else
            {
                return cells[index];
            }
        }
        else
        {
            return cells[index % cells.Length];
        }
    }
}
