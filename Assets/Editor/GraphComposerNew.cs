using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphComposerNew))]
[ExecuteInEditMode]
public class GraphComposerEditor : Editor
{

    private GraphComposerNew GC;

    private void OnEnable()
    {
        GC = FindObjectOfType<GraphComposerNew>();
    }
    
    [ExecuteInEditMode]
    public override void OnInspectorGUI()
    {
        
        if (GUILayout.Button("REDRAW", GUILayout.Height(30),GUILayout.Width(160)))
        {
            GC.ClearDialogueGraph();
            GC.BuildDialogueGraph();
            Repaint();
        }
        if (GUILayout.Button("CLEAR", GUILayout.Height(30),GUILayout.Width(160)))
        {
            GC.ClearDialogueGraph();
         
            Repaint();
        }

    }
}
