using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Snap"))
        {
            SnapPlayer();
        }
    }

    private void SnapPlayer()
    {
        var player = target as Player;

        var board = Board.instance;
        if (board == null) { return; }

        var waypoint = board.GetWaypoint(player.currentCellIndex);
        if (waypoint == null) { return; }
        
        player.transform.position = waypoint.transform.position;
    }
}
