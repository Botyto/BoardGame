using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Board))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Rebuild"))
        {
            (target as Board).RebuildCells();
        }
    }
}
