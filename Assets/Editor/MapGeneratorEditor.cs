using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
   public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap(); 
        }

        if (GUILayout.Button("Clear"))
        {
            mapGen.ClearMap(true);
        }

        if (GUILayout.Button("Debug Noisemap"))
        {
            mapGen.DebugDrawNoiseMap();
        }
        if (GUILayout.Button("Update Colors"))
        {
            mapGen.UpdateColors();
        }
    }
}
